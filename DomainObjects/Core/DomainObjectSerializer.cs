using Dynamix.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace DomainObjects.Core
{
    internal class DomainObjectSerializer
    {
        static readonly Dictionary<Type, DomainObjectSerializer> cache = new Dictionary<Type, DomainObjectSerializer>();
        //readonly Type objectType;
        readonly Dictionary<string, FieldInfoEx> autoPropertyFields;
        readonly Dictionary<string, FieldInfoEx> fields;

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
            //this.objectType = objectType;
            var eventNames = objectType.GetEvents().Select(x => x.Name).ToList();

            this.fields = objectType
                .GetFieldsEx(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(x => !eventNames.Contains(x.Name))
                .ToDictionary(x => x.Name);

            autoPropertyFields = fields.Values
                .Where(x => x.AutoPropertyName != null)
                .ToDictionary(x => x.AutoPropertyName);
        }

        private FieldInfoEx GetField(string name)
        {
            if (autoPropertyFields.TryGetValue(name, out var field)
               || fields.TryGetValue(name, out field))
                return field;
            else
                return null;
        }

        public void Deserialize(SerializationInfo info, object obj)
        {
            var converter = info.GetFormatterConverter();

            foreach (var item in info)
            {
                var field = GetField(item.Name);
                if (field != null)
                    field.Set(obj, converter.Convert(item.Value, field.Type));
            }
        }

        public void Serialize(SerializationInfo info, object obj)
        {
            foreach(var field in fields.Values)
                info.AddValue(field.AutoPropertyName ?? field.Name, field.Get(obj));
        }
    }
}
