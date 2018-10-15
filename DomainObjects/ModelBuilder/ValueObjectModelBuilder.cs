using DomainObjects.Metadata;
using DomainObjects.ModelBuilder.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainObjects.ModelBuilder
{
    internal class ValueObjectModelBuilder
    {
        public ValueObjectModelBuilder(ValueTypeDescriptor descriptor, EntityModelBuilderConfiguration configuration)
        {
            Descriptor = descriptor;
            Configuration = configuration;
        }

        public ValueTypeDescriptor Descriptor { get; }
        public EntityModelBuilderConfiguration Configuration { get; }

        public DomainValueObjectMetadata Build()
        {
            var propertyDescriptors = Descriptor.PropertyDescriptors
                .Where(x => (Configuration == null || !Configuration.IgnoredMembers.Contains(x.Property.Name)) && x.Property.Name != "Parent");

            ValidateModel(propertyDescriptors);

            var properties = new List<DomainPropertyMetadata>();

            foreach (var prop in propertyDescriptors)
            {
                DomainPropertyMetadata propertyMetadata = null;
                var configuration = Configuration?.PropertyModelConfigurations.FirstOrDefault(x => x.Property.Name == prop.Property.Name);

                if (prop is ValuePropertyDescriptor valuePropertyDescriptor)
                {
                    propertyMetadata = new DomainValuePropertyMetadata(valuePropertyDescriptor, configuration, null);
                }
                else if (prop is ValueListPropertyDescriptor valueListPropertyDescriptor)
                {
                    propertyMetadata = new DomainValueListPropertyMetadata(valueListPropertyDescriptor, configuration);
                }
                else if (prop is AggregatePropertyDescriptor aggregatePropertyDescriptor)
                {
                    propertyMetadata = new DomainAggregatePropertyMetadata(aggregatePropertyDescriptor, configuration);
                }
                else if (prop is AggregateListPropertyDescriptor aggregateListPropertyDescriptor)
                {
                    propertyMetadata = new DomainAggregateListPropertyMetadata(aggregateListPropertyDescriptor, configuration);
                }
                else if (prop is UnsupportedPropertyDescriptor unsupportedPropertyDescriptor)
                {
                    throw new InvalidOperationException($"Unsupported property {prop.Property.Name} of Type {prop.Property.Type.Name} in Entity {Descriptor.Type.Name}");
                }

                properties.Add(propertyMetadata);
            }

            return new DomainValueObjectMetadata(Descriptor.Type, properties);
        }

        private void ValidateModel(IEnumerable<PropertyDescriptor> propertyDescriptors)
        {
            var unsupportedProperty = propertyDescriptors.OfType<UnsupportedPropertyDescriptor>().FirstOrDefault();
            if (unsupportedProperty != null)
                throw new InvalidOperationException($"Unsupported property {unsupportedProperty.Property.Name} of Type {unsupportedProperty.Property.Type.Name} in Entity {Descriptor.Type.Name}");

            var ctors = Descriptor.Type.GetConstructors(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

            if (ctors.Length == 0)
                throw new InvalidOperationException($"ValueObject type {Descriptor.Type.Name} does not have public constructors");

            //var ctor = ctors.FirstOrDefault();

            //var ctorParameters = ctor.GetParameters().Select(x => (Name: x.Name.ToLower(), x.ParameterType)).OrderBy(x => x.Name).ToList();
            //var propertiesToCompare = propertyDescriptors.Select(x => (Name: x.Property.Name.ToLower(), x.Property.Type)).OrderBy(x => x.Name).ToList();

            //if (!ctorParameters.SequenceEqual(propertiesToCompare))
            //    throw new InvalidOperationException($"ValueObject type {Descriptor.Type.Name} does not have a single constructor that includes all supported properties");
        }
    }
}
