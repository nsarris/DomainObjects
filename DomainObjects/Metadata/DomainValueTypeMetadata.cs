using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Metadata
{
    public class DomainValueTypeMetadata
    {
        public DomainValueTypeMetadata(Type type)
        {
            Type = type;
        }

        public Type Type { get; }
    }
}
