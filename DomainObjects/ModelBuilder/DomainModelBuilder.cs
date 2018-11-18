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
            if (!entityModelBuilderConfigurations.TryGetValue(typeof(T), out var entityModelBuilderConfiguration))
            {
                entityModelBuilderConfiguration = new EntityModelBuilderConfiguration<T>();
                entityModelBuilderConfigurations.Add(typeof(T), entityModelBuilderConfiguration);
            }

            return (EntityModelBuilderConfiguration<T>)entityModelBuilderConfiguration;
        }

        public DomainModelMetadata Build()
        {
            var scannedTypes = new HashSet<Type>();

            var rootDescriptors = entityModelBuilderConfigurations.Values
                .Where(x => x.IsRoot)
                .Select(x => ScanEntityTypeRecursive(x.EntityType, scannedTypes))
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
                .Select(x => new ValueObjectModelBuilder(
                    descriptor: x,
                    configuration: entityModelBuilderConfigurations.TryGetValue(x.Type, out var entityModelBuilder) ? entityModelBuilder : null
                ))
                .ToList();

            var domainModel = new DomainModelMetadata(
                modelName,
                entityModelBuilders.Select(x => x.Build()).ToList(),
                valueTypeModelBuilders.Select(x => x.Build()).ToList()
                );

            DomainModelMetadataRegistry.RegisterModel(domainModel);

            return domainModel;
        }



        private ValueTypeDescriptor ScanValueObjectTypeRecursive(Type type, HashSet<Type> visitedTypes)
        {
            if (visitedTypes.Contains(type))
                return null;

            var propertyDescriptors = new List<PropertyDescriptor>();
            var valueTypeDescriptors = new List<ValueTypeDescriptor>();

            foreach (var property in type.GetPropertiesEx()
                .Where(x => x.PropertyInfo.GetIndexParameters().Length == 0
                    && !x.PropertyInfo.DeclaringType.IsFrameworkType()))
            {
                var propertyType = property.PropertyInfo.PropertyType;

                if (TypeHelper.IsSupportedValueType(propertyType, out var valueType))
                {
                    propertyDescriptors.Add(new ValuePropertyDescriptor(property, valueType.Value));
                    if (valueType == DomainValueType.ValueObject)
                        valueTypeDescriptors.AddIfNotNull(ScanValueObjectTypeRecursive(propertyType, visitedTypes));

                }
                else if (propertyType.IsDomainValueObjectList(out var elementType, out var _))
                {
                    var elementValueType = elementType.GetSupportedValueType();

                    if (!elementValueType.HasValue)
                        propertyDescriptors.Add(new UnsupportedPropertyDescriptor(property));
                    else
                    {
                        propertyDescriptors.Add(new ValueListPropertyDescriptor(property, elementType, elementValueType.Value, true));
                        if (valueType == DomainValueType.ValueObject)
                            valueTypeDescriptors.AddIfNotNull(ScanValueObjectTypeRecursive(elementType, visitedTypes));
                    }
                }
                else
                    propertyDescriptors.Add(new UnsupportedPropertyDescriptor(property));
            }

            return new ValueTypeDescriptor(type, propertyDescriptors, valueTypeDescriptors);
        }

        private EntityDescriptor ScanEntityTypeRecursive(Type type, HashSet<Type> visitedTypes)
        {
            if (visitedTypes.Contains(type))
                return null;

            var propertyDescriptors = new List<PropertyDescriptor>();
            var aggregateDescriptors = new List<EntityDescriptor>();
            var valueTypeDescriptors = new List<ValueTypeDescriptor>();

            foreach (var property in type.GetPropertiesEx()
                .Where(x => x.PropertyInfo.GetIndexParameters().Length == 0
                    && !x.PropertyInfo.DeclaringType.IsFrameworkType()))
            {
                var propertyType = property.PropertyInfo.PropertyType;
                
                if (TypeHelper.IsSupportedValueType(propertyType, out var valueType))
                {
                    propertyDescriptors.Add(new ValuePropertyDescriptor(property, valueType.Value));
                    if (valueType == DomainValueType.ValueObject)
                        valueTypeDescriptors.AddIfNotNull(ScanValueObjectTypeRecursive(propertyType, visitedTypes));
                }
                else if (propertyType.IsAggregate())
                {
                    propertyDescriptors.Add(new AggregatePropertyDescriptor(property));
                    aggregateDescriptors.AddIfNotNull(ScanEntityTypeRecursive(propertyType, visitedTypes));
                }
                else if (propertyType.IsAggregateList(out var elementType, out var readOnly))
                {
                    if (!elementType.IsAggregate())
                        propertyDescriptors.Add(new UnsupportedPropertyDescriptor(property));
                    else
                    {
                        propertyDescriptors.Add(new AggregateListPropertyDescriptor(property, elementType, readOnly));
                        aggregateDescriptors.AddIfNotNull(ScanEntityTypeRecursive(elementType, visitedTypes));
                    }
                }
                else if (propertyType.IsDomainValueObjectList(out elementType, out readOnly))
                {
                    var elementValueType = elementType.GetSupportedValueType();

                    if (!elementValueType.HasValue)
                        propertyDescriptors.Add(new UnsupportedPropertyDescriptor(property));
                    else
                    {
                        propertyDescriptors.Add(new ValueListPropertyDescriptor(property, elementType, elementValueType.Value, readOnly));
                        if (valueType == DomainValueType.ValueObject)
                            valueTypeDescriptors.AddIfNotNull(ScanValueObjectTypeRecursive(elementType, visitedTypes));
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
