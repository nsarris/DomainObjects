using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Metadata
{
    public class DomainValueObjectMetadata : DomainObjectMetadata
    {
        public DomainValueObjectMetadata(Type type, IEnumerable<DomainPropertyMetadata> propertyMetadata)
            :base(type, propertyMetadata)
        {
            
        }
    }
}
