using Dynamix.Reflection;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace DomainObjects.Serialization
{
    public static class SerializationExtensions
    {
        private readonly static Lazy<FieldInfoEx> serializationInfoConvertedField = new Lazy<FieldInfoEx>(() =>
            typeof(SerializationInfo).GetFieldEx("m_converter", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));

        
        public static IFormatterConverter GetFormatterConverter(this SerializationInfo info)
        {
            return (IFormatterConverter)serializationInfoConvertedField.Value.Get(info);
        }

        public static T GetValue<T>(this SerializationInfo info, string name)
        {
            return (T)info.GetValue(name, typeof(T));
        }

        public static SerializationInfoWrapper<T> For<T>(this SerializationInfo info, T instance)
        {
            return new SerializationInfoWrapper<T>(instance, info);
        }
    }
}
