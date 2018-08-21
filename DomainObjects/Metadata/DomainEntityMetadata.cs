using DomainObjects.Core;
using DomainObjects.ModelBuilder;
using Dynamix.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainObjects.Metadata
{
    public class DomainEntityMetadata
    {
        private readonly Dictionary<string, DomainEntityPropertyMetadata> propertyMetadata;
        private readonly Dictionary<Type, DomainEntityMetadata> aggregateTypes;

        public IReadOnlyDictionary<Type, DomainEntityMetadata> AggregateTypes => aggregateTypes;

        public DomainEntityMetadata(Type entityType)
        {
            propertyMetadata = new Dictionary<string, DomainEntityPropertyMetadata>();
            aggregateTypes = new Dictionary<Type, DomainEntityMetadata>();

            EntityType = entityType;
            IsRoot = entityType.IsSubclassOfDeep(typeof(AggregateRoot));
            //CheckType
        }

        public void Build(EntityModelBuilder modelBuilder)
        {
            GetProperties(modelBuilder);
            ValidateModel();
        }

        public DomainEntityPropertyMetadata GetPropertyMetadata(string propertyName)
        {
            return propertyMetadata[propertyName];
        }

        public IEnumerable<DomainEntityPropertyMetadata> GetPropertiesMetdata()
        {
            return propertyMetadata.Values;
        }

        public Type EntityType { get; }
        public bool IsRoot { get; }

        private void GetProperties(EntityModelBuilder modelBuilder)
        {
            foreach (var property in EntityType.GetPropertiesEx()
                .Where(x => x.PropertyInfo.GetIndexParameters().Length == 0)
                .Where(x => !modelBuilder.IgnoredMembers.Contains(x.Name)))
            {
                var propertyType = property.PropertyInfo.PropertyType;

                if (propertyType.IsSubclassOfDeep(typeof(DomainValue)))
                {
                    //var keyAttribute = propertyType.GetAttribute<DomainKeyAttribute>();
                    //ValueTypeProperties
                }
                if (propertyType.IsSubclassOfDeep(typeof(Aggregate)))
                {
                    //AggregateProperty
                    if (!aggregateTypes.ContainsKey(propertyType))
                        aggregateTypes.Add(propertyType, new DomainEntityMetadata(propertyType));
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
                        aggregateTypes.Add(elementType, new DomainEntityMetadata(elementType));
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
