using DomainObjects.Core;

namespace DomainObjects.ModelBuilder
{
    public class DomainModelBuilder
    {
        public EntityModelBuilder<T> ForEntity<T>() where T : DomainEntity
        {
            return new EntityModelBuilder<T>();
        }
    }
}
