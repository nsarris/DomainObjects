using DomainObjects.Core;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Metadata
{
    public enum DomainValueType
    {
        String = 1,
        Boolean,
        Number,
        DateTime,
        TimeSpan,
        Enum,
        Complex,
    }

    public enum DomainPropertyType
    {
        Value = 1,
        ValueList,
        Aggregate,
        AggregateList
    }
}
