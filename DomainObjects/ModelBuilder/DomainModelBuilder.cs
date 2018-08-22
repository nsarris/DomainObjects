using DomainObjects.Core;
using DomainObjects.Metadata;
using DomainObjects.ModelBuilder.Descriptors;
using Dynamix.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainObjects.ModelBuilder
{

    public class DomainModelBuilder
    {
        string modelName;
        readonly Dictionary<Type, EntityModelBuilderConfiguration> entityModelBuilderConfigurations = new Dictionary<Type, EntityModelBuilderConfiguration>();

        public DomainModelBuilder HasModelName(string name)
        {
            modelName = name;
            return this;
        }

        public EntityModelBuilderConfiguration<T> Entity<T>() where T : DomainEntity
        {
            if (!entityModelBuilderConfigurations.TryGetValue(typeof(T), out var entityModelBuilder))
            {
                entityModelBuilder = new EntityModelBuilderConfiguration<T>();
                entityModelBuilderConfigurations.Add(typeof(T), entityModelBuilder);
            }

            return (EntityModelBuilderConfiguration<T>)entityModelBuilder;
        }

        public DomainModelMetadata Build()
        {
            var scannedTypes = new HashSet<Type>();

            var rootDescriptors = entityModelBuilderConfigurations.Values
                .Where(x => x.IsRoot)
                .Select(x => ScanTypeRecursive(x.EntityType, scannedTypes))
                .ToList();

            var entityModelBuilders = rootDescriptors
                .SelectManyRecursive(x => x.AggregateDescriptors)
                .Select(x => new EntityModelBuilder(
                    descriptor: x,
                    configuration: entityModelBuilderConfigurations.TryGetValue(x.Type, out var entityModelBuilder) ? entityModelBuilder : null
                ))
                .ToList();
                    
            var valueTypeModelBuilders = rootDescriptors
                .SelectMany(x => x.ValueTypeDescriptors)
                .SelectManyRecursive(x => x.ValueTypeDescriptors)
                .Select(x => new ValueTypeModelBuilder(
                    descriptor: x,
                    configuration: entityModelBuilderConfigurations.TryGetValue(x.Type, out var entityModelBuilder) ? entityModelBuilder : null
                ))
                .ToList();

            //Validate

            var domainModel = new DomainModelMetadata(
                modelName,
                entityModelBuilders.Select(x => x.Build()).ToList(),
                valueTypeModelBuilders.Select(x => x.Build()).ToList()
                );

            DomainModelMetadataRegistry.RegisterModel(domainModel);

            return domainModel;
        }



        private ValueTypeDescriptor ScanValueTypeRecursive(Type type, HashSet<Type> visitedTypes)
        {
            if (visitedTypes.Contains(type))
                return null;

            var propertyDescriptors = new List<PropertyDescriptor>();
            var valueTypeDescriptors = new List<ValueTypeDescriptor>();

            foreach (var property in type.GetPropertiesEx()
                .Where(x => x.PropertyInfo.GetIndexParameters().Length == 0))
            {
                var propertyType = property.PropertyInfo.PropertyType;

                if (TypeHelper.IsSupportedValueType(propertyType, out var valueType))
                {
                    propertyDescriptors.Add(new ValuePropertyDescriptor(property, valueType.Value));
                    if (valueType == DomainValueType.Complex)
                        valueTypeDescriptors.AddIfNotNull(ScanValueTypeRecursive(propertyType, visitedTypes));

                }
                else if (propertyType.IsOrSubclassOfGenericDeep(typeof(ValueReadOnlyList<>), out var valueListType))
                {
                    var elementType = valueListType.GetGenericArguments().First();
                    var elementValueType = elementType.GetSupportedValueType();

                    if (!elementValueType.HasValue)
                        propertyDescriptors.Add(new UnsupportedPropertyDescriptor(property));
                    else
                    {
                        propertyDescriptors.Add(new ValueListPropertyDescriptor(property, elementType, elementValueType.Value, true));
                        if (valueType == DomainValueType.Complex)
                            valueTypeDescriptors.AddIfNotNull(ScanValueTypeRecursive(elementType, visitedTypes));
                    }
                }
                else
                    propertyDescriptors.Add(new UnsupportedPropertyDescriptor(property));
            }

            return new ValueTypeDescriptor(type, propertyDescriptors, valueTypeDescriptors);
        }

        private EntityDescriptor ScanTypeRecursive(Type type, HashSet<Type> visitedTypes)
        {
            if (visitedTypes.Contains(type))
                return null;

            var propertyDescriptors = new List<PropertyDescriptor>();
            var aggregateDescriptors = new List<EntityDescriptor>();
            var valueTypeDescriptors = new List<ValueTypeDescriptor>();

            foreach (var property in type.GetPropertiesEx()
                .Where(x => x.PropertyInfo.GetIndexParameters().Length == 0))
            {
                var readOnly = false;
                var propertyType = property.PropertyInfo.PropertyType;
                
                if (TypeHelper.IsSupportedValueType(propertyType, out var valueType))
                {
                    propertyDescriptors.Add(new ValuePropertyDescriptor(property, valueType.Value));
                    if (valueType == DomainValueType.Complex)
                        valueTypeDescriptors.AddIfNotNull(ScanValueTypeRecursive(propertyType, visitedTypes));
                    
                }
                else if (propertyType.IsSubclassOfDeep(typeof(Aggregate)))
                {
                    propertyDescriptors.Add(new AggregatePropertyDescriptor(property));
                    aggregateDescriptors.AddIfNotNull(ScanTypeRecursive(propertyType, visitedTypes));
                }
                else if ((propertyType.IsOrSubclassOfGenericDeep(typeof(AggregateList<>), out var aggregateListType)
                    || (readOnly = propertyType.IsOrSubclassOfGenericDeep(typeof(AggregateReadOnlyList<>), out aggregateListType))))
                {
                    var elementType = aggregateListType.GetGenericArguments().Single();
                    if (!elementType.IsSubclassOfDeep(typeof(Aggregate)))
                        propertyDescriptors.Add(new UnsupportedPropertyDescriptor(property));
                    else
                    {
                        propertyDescriptors.Add(new AggregateListPropertyDescriptor(property, elementType, readOnly));
                        aggregateDescriptors.AddIfNotNull(ScanTypeRecursive(elementType, visitedTypes));
                    }
                }
                else if (propertyType.IsOrSubclassOfGenericDeep(typeof(ValueList<>), out var valueListType)
                    || (readOnly = propertyType.IsOrSubclassOfGenericDeep(typeof(ValueReadOnlyList<>), out valueListType)))
                {
                    var elementType = valueListType.GetGenericArguments().First();
                    var elementValueType = elementType.GetSupportedValueType();

                    if (!elementValueType.HasValue)
                        propertyDescriptors.Add(new UnsupportedPropertyDescriptor(property));
                    else
                    {
                        propertyDescriptors.Add(new ValueListPropertyDescriptor(property, elementType, elementValueType.Value, readOnly));
                        if (valueType == DomainValueType.Complex)
                            valueTypeDescriptors.AddIfNotNull(ScanValueTypeRecursive(elementType, visitedTypes));
                    }
                }
                else
                    propertyDescriptors.Add(new UnsupportedPropertyDescriptor(property));
            }

            visitedTypes.Add(type);

            return new EntityDescriptor(type, propertyDescriptors, aggregateDescriptors, valueTypeDescriptors);
        }
    }
}
