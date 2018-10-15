using Dynamix.Reflection;
using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace DomainObjects.Serialization
{
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

        public void AddValue(string name, object value, Type type) => info.AddValue(name, value, type);
        public void AddValue(string name, object value) => info.AddValue(name, value);
        public void AddValue(string name, sbyte value) => info.AddValue(name, value);
        public void AddValue(string name, bool value) => info.AddValue(name, value);
        public void AddValue(string name, DateTime value) => info.AddValue(name, value);
        public void AddValue(string name, TimeSpan value) => info.AddValue(name, value, typeof(TimeSpan));
        public void AddValue(string name, decimal value) => info.AddValue(name, value);
        public void AddValue(string name, double value) => info.AddValue(name, value);
        public void AddValue(string name, float value) => info.AddValue(name, value);
        public void AddValue(string name, long value) => info.AddValue(name, value);
        public void AddValue(string name, uint value) => info.AddValue(name, value);
        public void AddValue(string name, int value) => info.AddValue(name, value);
        public void AddValue(string name, ushort value) => info.AddValue(name, value);
        public void AddValue(string name, short value) => info.AddValue(name, value);
        public void AddValue(string name, byte value) => info.AddValue(name, value);
        public void AddValue(string name, ulong value) => info.AddValue(name, value);
        public void AddValue(string name, char value) => info.AddValue(name, value);
    }
}
