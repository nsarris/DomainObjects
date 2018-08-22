using DomainObjects.Metadata;
using DomainObjects.ModelBuilder.Descriptors;

namespace DomainObjects.ModelBuilder
{
    internal class ValueTypeModelBuilder
    {
        public ValueTypeModelBuilder(ValueTypeDescriptor descriptor, EntityModelBuilderConfiguration configuration)
        {
            Descriptor = descriptor;
            Configuration = configuration;
        }

        public ValueTypeDescriptor Descriptor { get; }
        public EntityModelBuilderConfiguration Configuration { get; }

        public DomainValueTypeMetadata Build()
        {
            var metadata = new DomainValueTypeMetadata(Descriptor.Type);

            return metadata;
        }
    }
}
