using DomainObjects.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.ModelBuilder
{



    public class ValueListModelBuilder<T>
    {

    }


    public class AggregateListModelBuilder<T>
        where T : Aggregate
    {

    }

    public class AggregateReadOnlyListModelBuilder<T>
        where T : Aggregate
    {

    }
}
