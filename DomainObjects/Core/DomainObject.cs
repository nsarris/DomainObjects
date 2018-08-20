using Dynamix.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Core
{
    public abstract class DomainObject //: DynamicType
    {
    }

    public abstract class AggregateRoot : DomainEntity
    {

    }

    public abstract class Aggregate : DomainEntity
    {

    }

    public class AggregateReadOnlyList<T> : TrackableReadOnlyList<T>
        where T : Aggregate
    {

    }

    public class AggregateList<T> : TrackableList<T>
        where T : Aggregate
    {

    }

    public class ValueReadOnlyList<T> : TrackableReadOnlyList<T>
    {

    }

    public class ValueList<T> : TrackableList<T>
    {

    }

    public class DomainEntityPropertyDescriptor
    {
        public PropertyInfoEx Property { get; }
        public bool IsKeyMember { get; }
        public int KeyPosition { get; }
        public Type Type => Property.Type;
        public string Name => Property.Name;
        public bool IsNullableType { get; }
        //public bool IsImmutable { get; }
        public DomainPropertyType DomainPropertyType { get; } = 0;
        public DomainValueType DomainValueType { get; } = 0;

        public DomainEntityPropertyDescriptor(PropertyInfoEx property, int? keyPosition)
        {
            Property = property;
            IsKeyMember = keyPosition.HasValue;
            KeyPosition = keyPosition ?? -1;
            IsNullableType = property.Type.IsNullable();

            var valueType = TypeHelper.GetSupportedValueType(property.Type);
            if (valueType != null)
                DomainValueType = valueType.Value;
        }
    }

    //public class DomainEntityValuePropertyDescriptor : DomainEntityPropertyDescriptor
    //{

    //}

    //public class DomainEntityAggregatePropertyDescriptor : DomainEntityPropertyDescriptor
    //{

    //}

    //public class DomainEntityAggregatePropertyDescriptor : DomainEntityPropertyDescriptor
    //{

    //}

    public enum DomainValueType
    {
        String = 1,
        Boolean,
        Number,
        DateTime,
        TimeSpan,
        Complex,
    }

    public enum DomainPropertyType
    {
        Value = 1,
        Aggregate,
        AggregateList
    }

    internal static class IEnumerableExtensions
    {
        public static IEnumerable<T> SelectManyRecursive<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> selector)
        {
            return !source.Any() ? source :
                source.Concat(
                    source
                    .SelectMany(i => selector(i) ?? Enumerable.Empty<T>())
                    .SelectManyRecursive(selector)
                );
        }
    }

    internal static class DomainModelRegistry
    {
        static readonly Dictionary<Type, DomainModelDescriptor> entityModelMap = new Dictionary<Type, DomainModelDescriptor>();
        static readonly Dictionary<Type, DomainEntityDescriptor> entityDescriptors = new Dictionary<Type, DomainEntityDescriptor>();

        public static void RegisterModel(DomainModelDescriptor domainModel)
        {
            foreach(var descriptor in domainModel.GetEntityDescriptors())
            {
                if (entityModelMap.TryGetValue(descriptor.EntityType, out var modelMap))
                {
                    if (modelMap != domainModel)
                        throw new InvalidOperationException("Cannot register an entity with two different domain models");
                }
                else
                    entityModelMap.Add(descriptor.EntityType, domainModel);

                if (!entityDescriptors.ContainsKey(descriptor.EntityType))
                    entityDescriptors.Add(descriptor.EntityType, descriptor);
            }
        }

        public static DomainEntityDescriptor GetEntityDescriptor(Type type)
        {
            //Assert Entity

            if (entityDescriptors.TryGetValue(type, out var descriptor))
                return descriptor;

            throw new KeyNotFoundException($"Domain entity type {type.Name} not found in any built models or model has not been built");
        }
    }

    public class DomainModelDescriptor
    {
        readonly Dictionary<Type, DomainEntityDescriptor> entityDescriptors = new Dictionary<Type, DomainEntityDescriptor>();
        bool isBuilt;

        public IEnumerable<DomainEntityDescriptor> GetAggregateRootDescriptors()
        {
            return entityDescriptors.Values.Where(x => x.IsRoot);
        }

        public IEnumerable<DomainEntityDescriptor> GetAggregateDescriptors()
        {
            return entityDescriptors.Values.Where(x => !x.IsRoot);
        }

        public IEnumerable<DomainEntityDescriptor> GetEntityDescriptors()
        {
            return entityDescriptors.Values;
        }

        public bool TryGet(Type entityType, out DomainEntityDescriptor domainEntityDescriptor)
        {
            return entityDescriptors.TryGetValue(entityType, out domainEntityDescriptor);
        }

        public bool TryGet<T>(out DomainEntityDescriptor domainEntityDescriptor)
            where T : DomainEntity
        {
            return entityDescriptors.TryGetValue(typeof(T), out domainEntityDescriptor);
        }

        public DomainEntityDescriptor GetType(Type entityType)
        {
            return entityDescriptors[entityType];
        }

        public DomainEntityDescriptor GetType<T>()
            where T : DomainEntity
        {
            return entityDescriptors[typeof(T)];
        }

        public void RegisterType(Type entityType)
        {
            if (isBuilt)
                throw new InvalidOperationException("No new entities can be added after the model has been built");

            var descriptor = new DomainEntityDescriptor(entityType);

            if (!descriptor.IsRoot)
                throw new InvalidOperationException("Only root entities can be registered in mode descriptor");

            entityDescriptors.Add(entityType, descriptor);
        }

        public void BuildModel()
        {
            if (isBuilt)
                return;

            foreach (var descriptor in entityDescriptors.Values)
            {
                descriptor.Build();
                foreach(var aggregateDescriptor in descriptor.AggregateTypes.Values.SelectManyRecursive(x => x.AggregateTypes.Values))
                {
                    aggregateDescriptor.Build();
                    entityDescriptors.Add(aggregateDescriptor.EntityType, aggregateDescriptor);
                }
            }

            DomainModelRegistry.RegisterModel(this);

            isBuilt = true;
        }
    }

    public class DomainEntityDescriptor
    {
        private readonly Dictionary<string, DomainEntityPropertyDescriptor> propertyDescriptors;
        private readonly Dictionary<Type, DomainEntityDescriptor> aggregateTypes;

        public IReadOnlyDictionary<Type, DomainEntityDescriptor> AggregateTypes => aggregateTypes;

        public DomainEntityDescriptor(Type entityType)
        {
            propertyDescriptors = new Dictionary<string, DomainEntityPropertyDescriptor>();
            aggregateTypes = new Dictionary<Type, DomainEntityDescriptor>();

            EntityType = entityType;
            IsRoot = entityType.IsSubclassOfDeep(typeof(AggregateRoot));
            //CheckType
        }

        public void Build()
        {
            GetProperties();
            ValidateModel();
        }

        public DomainEntityPropertyDescriptor GetPropertyDescriptor(string propertyName)
        {
            return propertyDescriptors[propertyName];
        }

        public IEnumerable<DomainEntityPropertyDescriptor> GetPropertyDescriptors()
        {
            return propertyDescriptors.Values;
        }

        public Type EntityType { get; }
        public bool IsRoot { get; }

        private void GetProperties()
        {
            foreach (var property in EntityType.GetPropertiesEx())
            {
                //if not marked ignore;
                var propertyType = property.PropertyInfo.PropertyType;


                if (propertyType.IsSubclassOfDeep(typeof(DomainKey)))
                {
                    //Key value type
                    //propertyDescriptors.Add(property.Name, new DomainEntityPropertyDescriptor(property));
                }
                if (propertyType.IsSubclassOfDeep(typeof(DomainValue)))
                {
                    var keyAttribute = propertyType.GetAttribute<DomainKeyAttribute>();
                    //ValueTypeProperties
                }
                if (propertyType.IsSubclassOfDeep(typeof(Aggregate)))
                {
                    //AggregateProperty
                    if (!aggregateTypes.ContainsKey(propertyType))
                        aggregateTypes.Add(propertyType, new DomainEntityDescriptor(propertyType));
                }
                else if (propertyType.IsOrSubclassOfGenericDeep(typeof(AggregateList<>), out var aggregateListType)
                    || propertyType.IsOrSubclassOfGenericDeep(typeof(AggregateReadOnlyList<>), out aggregateListType))
                {
                    //AggregateListProperty
                    //Visit Type
                    var elementType = aggregateListType.GetGenericArguments().First();
                    if (!elementType.IsSubclassOfDeep(typeof(Aggregate)))
                    {
                        //Error
                    }
                    if (!aggregateTypes.ContainsKey(elementType))
                        aggregateTypes.Add(elementType, new DomainEntityDescriptor(elementType));
                }
                else if (propertyType.IsOrSubclassOfGenericDeep(typeof(ValueList<>), out var valueListType)
                    || propertyType.IsOrSubclassOfGenericDeep(typeof(ValueReadOnlyList<>), out valueListType))
                {
                    //check if T is supportedType
                    //if complexType visit inner
                    var elementType = valueListType.GetGenericArguments().First();
                    var elementValueType = elementType.GetSupportedValueType();
                    
                }
                //Check supported primitive types
                else if (TypeHelper.IsSupportedValueType(propertyType, out var valueType))
                {
                    //Test
                }

                //else throw not supported / dont throw set error for validation
            }
        }

        

        private void ValidateModel() //SanityCheck
        {
            //At least one key member
            //Only one complex key member
            //No unsupported types
        }
    }
}
