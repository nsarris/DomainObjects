using DomainObjects.Core;
using DomainObjects.Internal;
using DomainObjects.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Validation
{
    public interface IDomainObjectValidationContext { }
    public class DomainObjectValidationContext<T> 
    {
        private readonly Dictionary<(Type Type, string Name), object> dependencies = new Dictionary<(Type, string), object>();

        public DomainObjectValidationContext()
        {

        }

        public AggregateRoot Root { get; }
        public T Instance { get; }

        public TDependency GetDependency<TDependency>()
        {
            return (TDependency)dependencies[(typeof(TDependency), null)];
        }

        public object GetDependency(Type dependencyType)
        {
            return dependencies[(dependencyType, null)];
        }
        public TDependency GetDependency<TDependency>(string name)
        {
            return (TDependency)dependencies[(typeof(TDependency), name)];
        }

        public object GetDependency(Type dependencyType, string name)
        {
            return dependencies[(dependencyType, name)];
        }

        //Chain/Path/ParentContext
    }

    public abstract class DomainObjectValidatorBase<T> where T : DomainObject
    {
        private readonly Dictionary<Type, object> childValidators = new Dictionary<Type, object>();
        private readonly DomainEntityMetadata entityMetadata;
        protected DomainObjectValidatorBase()
        {
            entityMetadata = DomainModelMetadataRegistry.GetEntityDescriptor<T>();
        }
        public void RegisterChildValidator(Type childType, object validator)
        {
            //check type compatibility
            childValidators[childType] = validator;
        }
        public void RegisterChildValidator<TChild>(DomainObjectValidatorBase<TChild> validator) where TChild: DomainObject
        {
            //check type compatibility
            childValidators[typeof(TChild)] = validator;
        }

        protected abstract void ValidateImpl(T instance, DomainObjectValidationContext<T> context);

        internal void Validate(T instance, DomainObjectValidationContext<T> context)
        {
            ValidateImpl(instance, context);
            //For all agg/valuetpypes agg -> Get validator?? how?
            //get validtor from childValidators, if not try construct from domain
            //if from domain has dependcies fail and notify to use constrcutor and register
            foreach(var aggregateMetadata in entityMetadata.GetAggregateProperties())
            {
                
                //need property to point to domainobject metadata

                //Check for validator in childValidators
                //else check in metadata/registry -> try create
                //Append result
                
            }
            //etc
        }
    }
}
