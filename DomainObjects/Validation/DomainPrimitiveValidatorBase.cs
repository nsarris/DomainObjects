using DomainObjects.Internal;
using DomainObjects.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Validation
{
    public abstract class DomainPrimitiveValidatorBase<T> : DomainValidatorBase<T>, IDomainPrimitiveValidator<T>
    {

    }
}
