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
            var properties = new List<DomainPropertyMetadata>();

            foreach (var prop in Descriptor.PropertyDescriptors
                .Where(x => Configuration == null || !Configuration.IgnoredMembers.Contains(x.Property.Name)))
            {
                var keyPosition = Configuration?.KeyMembers.IndexOf(prop.Property.Name);

                if (keyPosition > 0 && !(prop is ValuePropertyDescriptor))
                    throw new InvalidOperationException($"Key properties can only be supported value types");

                DomainPropertyMetadata propertyMetadata = null;
                var configuration = Configuration?.PropertyModelConfigurations.FirstOrDefault(x => x.Property.Name == prop.Property.Name);

                if (prop is ValuePropertyDescriptor valuePropertyDescriptor)
                {
                    propertyMetadata = new DomainValuePropertyMetadata(valuePropertyDescriptor, configuration, keyPosition >= 0 ? keyPosition : null);
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

            var metadata = new DomainValueObjectMetadata(Descriptor.Type, properties);

            return metadata;
        }

        //TODO:Validate
        //Validate all properties are supported
        //Validate constructor has all properties in sequence
    }
}
