using Dynamix.Helpers;
using Dynamix.Reflection;
using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Core
{

    [Serializable]
    public abstract class DomainObject : ISerializable//<T> where T : DomainObject<T>
    {
        protected DomainObject()
        {
  
        }

        protected DomainObject(SerializationInfo info, StreamingContext context)
            :this()
        {
            Deserialize(info, context);
        }

        protected void Deserialize(SerializationInfo info, StreamingContext context)
        {
            OnDeserializing();
            var serializer = DomainObjectSerializer.GetSerializer(this.GetType());
            serializer.Deserialize(info, this);
            OnDeserialized();
        }

        protected virtual void Serialize(SerializationInfo info, StreamingContext context)
        {
            OnSerializing();
            var serializer = DomainObjectSerializer.GetSerializer(this.GetType());
            serializer.Serialize(info, this);
            OnSerialized();
        }

        protected virtual void OnDeserialized()
        {

        }

        protected virtual void OnDeserializing()
        {

        }

        protected virtual void OnSerialized()
        {

        }

        protected virtual void OnSerializing()
        {
            
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) => Serialize(info, context);

        public object this[string property]
        {
            get
            {
                return this.GetType().GetPropertyEx(property).Get(this);
            }
            set
            {
                var prop = this.GetType().GetPropertyEx(property, BindingFlagsEx.AllInstance);
                if (prop.PublicSet)
                    prop.Set(value);
                else
                    throw new InvalidOperationException($"Property {prop.Name} of {this.GetType().Name} does not have a public set method");
            }
        }
    }
}
