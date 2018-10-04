using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Metadata
{
    public class DomainValueTypeMetadata
    {
        private readonly Dictionary<string, DomainPropertyMetadata> propertyMetadata;

        public DomainValueTypeMetadata(Type type, IEnumerable<DomainPropertyMetadata> propertyMetadata)
        {
            Type = type;
            this.propertyMetadata = propertyMetadata.ToDictionary(x => x.Property.Name);
        }

        public Type Type { get; }
    }
}
