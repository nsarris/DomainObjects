using DomainObjects.Metadata;
using DomainObjects.ModelBuilder.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainObjects.ModelBuilder
{
    internal class EntityModelBuilder
    {
        public EntityModelBuilder(EntityDescriptor descriptor, EntityModelBuilderConfiguration configuration)
        {
            Descriptor = descriptor;
            Configuration = configuration;
        }

        public EntityDescriptor Descriptor { get; }
        public EntityModelBuilderConfiguration Configuration { get; }

        public DomainEntityMetadata Build()
        {
            var propertyDescriptors = Descriptor.PropertyDescriptors
                .Where(x => (Configuration == null || !Configuration.IgnoredMembers.Contains(x.Property.Name)) && x.Property.Name != "Parent");

            ValidateModel(propertyDescriptors);

            var properties = new List<DomainPropertyMetadata>();

            foreach (var prop in propertyDescriptors)
            {
                DomainPropertyMetadata propertyMetadata = null;
                var configuration = Configuration?.PropertyModelConfigurations.FirstOrDefault(x => x.Property.Name == prop.Property.Name);
                var keyPosition = GetKeyPosition(prop);

                if (prop is ValuePropertyDescriptor valuePropertyDescriptor)
                {
                    propertyMetadata = new DomainValuePropertyMetadata(valuePropertyDescriptor, configuration, keyPosition >= 0 ? keyPosition : null);
                }
                else if(prop is ValueListPropertyDescriptor valueListPropertyDescriptor)
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

                properties.Add(propertyMetadata);
            }

            return new DomainEntityMetadata(Descriptor.Type, properties);
        }

        private int? GetKeyPosition(PropertyDescriptor property) => Configuration?.KeyMembers.IndexOf(property.Property.Name);
        

        private void ValidateModel(IEnumerable<PropertyDescriptor> propertyDescriptors)
        {
            var properties = propertyDescriptors
                .Select(x => new
                {
                    KeyPosition = GetKeyPosition(x),
                    Property = x
                })
                .ToList();

            var unsupportedProperty = properties.Select(x => x.Property).OfType<UnsupportedPropertyDescriptor>().FirstOrDefault();
            if (unsupportedProperty != null)
                throw new InvalidOperationException($"Unsupported property {unsupportedProperty.Property.Name} of Type {unsupportedProperty.Property.Type.Name} in Entity {Descriptor.Type.Name}");

            var ctors = Descriptor.Type.GetConstructors(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

            if (ctors.Length == 0)
                throw new InvalidOperationException($"Entity type {Descriptor.Type.Name} does not have any public constructors");

            ValidateKey(properties.Where(x => x.KeyPosition >= 0).Select(x => x.Property).ToList());
        }

        private void ValidateKey(List<PropertyDescriptor> keyProperties)
        {
            if (!keyProperties.Any())
                throw new InvalidOperationException($"Entity {Descriptor.Type.Name} has no key defined");
            else if (!keyProperties.All(x => x is ValuePropertyDescriptor))
                throw new InvalidOperationException($"Key properties can only be supported on value types");
            else if (keyProperties.Count > 1 && keyProperties.Any(x => TypeHelper.GetSupportedValueType(x.Property.Type) == DomainValueType.ValueObject)
                    || keyProperties.Count(x => TypeHelper.GetSupportedValueType(x.Property.Type) == DomainValueType.ValueObject) > 1)
                throw new InvalidOperationException($"Entity {Descriptor.Type.Name} has a complex type and at least one more property in its key configuration. This is not supported");
        }
    }
}
