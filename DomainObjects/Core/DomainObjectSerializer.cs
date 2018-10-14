using DomainObjects.Internal;
using DomainObjects.Serialization;
using Dynamix.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace DomainObjects.Core
{
    public class DeserializeAsAttribute : Attribute
    {
        public DeserializeAsAttribute(Type type)
        {
            Type = type;
        }

        public Type Type { get; }
    }

    internal class DomainObjectSerializer
    {
        static readonly Dictionary<Type, DomainObjectSerializer> cache = new Dictionary<Type, DomainObjectSerializer>();
        readonly Dictionary<string, (FieldInfoEx field, Type targetType)> autoPropertyFields;
        readonly Dictionary<string, (FieldInfoEx field, Type targetType)> fields;

        public static DomainObjectSerializer GetSerializer<T>()
        {
            return GetSerializer(typeof(T));
        }

        public static DomainObjectSerializer GetSerializer(Type objectType)
        {
            if (!cache.TryGetValue(objectType, out var serializer))
            {
                serializer = new DomainObjectSerializer(objectType);
                cache[objectType] = serializer;
            }

            return serializer;
        }

        public DomainObjectSerializer(Type objectType)
        {
            var eventNames = objectType.GetEvents().Select(x => x.Name).ToList();

            if (objectType.GetInterfaces().Contains(typeof(IDynamicProxy)))
                objectType = objectType.BaseType;

            this.fields = objectType
                .GetFieldsEx(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(x => !x.FieldInfo.HasAttribute<NonSerializedAttribute>() 
                    && !eventNames.Contains(x.Name))
                .ToDictionary(x => x.Name,x => GetFieldWithTargetType(x, objectType));

            autoPropertyFields = fields.Values
                .Where(x => x.field.AutoPropertyName != null)
                .ToDictionary(x => x.field.AutoPropertyName);
        }

        private (FieldInfoEx field, Type targetType) GetField(string name)
        {
            if (autoPropertyFields.TryGetValue(name, out var field)
               || fields.TryGetValue(name, out field))
                return field;
            else
                return (null,null);
        }

        private (FieldInfoEx field, Type targetType) GetFieldWithTargetType(FieldInfoEx field, Type objectType)
        {
            if (field.AutoPropertyName == null)
                return (field, field.Type);

            return (field, 
                objectType.GetPropertyEx(field.AutoPropertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)?.PropertyInfo?.GetAttribute<DeserializeAsAttribute>()?.Type
                ?? field.Type);
        }

        public void Deserialize(SerializationInfo info, object obj)
        {
            var converter = info.GetFormatterConverter();

            foreach (var item in info)
            {
                var (field, tatgetType) = GetField(item.Name);
                if (field != null)
                    field.Set(obj, converter.Convert(item.Value, tatgetType));
                        
            }
        }

        public void Serialize(SerializationInfo info, object obj)
        {
            foreach(var (field, _) in fields.Values)
                info.AddValue(field.AutoPropertyName ?? field.Name, field.Get(obj));
        }
    }
}
