using Dynamix.Reflection;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace DomainObjects.Core
{
    public static class Extensions
    {
        public static T GetValue<T>(this SerializationInfo info, string name)
        {
            return (T)info.GetValue(name, typeof(T));
        }

        public static SerializationInfoWrapper<T> For<T>(this SerializationInfo info, T instance)
        {
            return new SerializationInfoWrapper<T>(instance, info);
        }
    }

    public class SerializationInfoWrapper<T>
    {
        private readonly T instance;
        private readonly SerializationInfo info;

        internal SerializationInfoWrapper(T instance, SerializationInfo info)
        {
            this.instance = instance;
            this.info = info;
        }

        public SerializationInfoWrapper<T> AddValue(Expression<Func<T, sbyte>> memberExpression)
        {
            info.AddValue(
                ReflectionHelper.GetMemberName(memberExpression),
                ReflectionHelper.GetValue(instance, memberExpression)
                );

            return this;
        }

        public SerializationInfoWrapper<T> AddValue(string memberName)
        {
            var fields = instance.GetType()
                    .GetFieldsEx(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic)
                    .ToList();

            info.AddValue(
                memberName,
                instance
                .GetType()
                    .GetFieldEx(memberName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.FlattenHierarchy)
                    .Get(instance), 
                instance.GetType()
                );

            return this;
        }

        //public void AddValue(string name, sbyte value);
       
        //public void AddValue(string name, object value, Type type);
        
        //public void AddValue(string name, bool value);
       
        //public void AddValue(string name, DateTime value);
       
        //public void AddValue(string name, decimal value);
       
        //public void AddValue(string name, double value);
        
        //public void AddValue(string name, object value);
      
        //public void AddValue(string name, float value);
        
        //public void AddValue(string name, long value);
      
   
        //public void AddValue(string name, uint value);
       
        //public void AddValue(string name, int value);
       
        //public void AddValue(string name, ushort value);
       
        //public void AddValue(string name, short value);
     
        //public void AddValue(string name, byte value);
       
        //public void AddValue(string name, ulong value);
        //public void AddValue(string name, char value);
    }
}
