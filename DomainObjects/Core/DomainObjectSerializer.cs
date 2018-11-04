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
            var entityMetadata = objectType.IsDomainEntity() ?
                        Metadata.DomainModelMetadataRegistry.GetEntityMetadta(objectType)
                        : null;

            var types = new[] { objectType }.Concat(objectType.GetBaseTypes())
                .Where(x => !x.GetInterfaces().Contains(typeof(IDynamicProxy)) 
                    && !x.IsFrameworkType())
                .ToList();

            var eventNames = types.SelectMany(x => x.GetEvents().Select(e => e.Name)).ToList();

            this.fields = types.SelectMany(t => t
                .GetFieldsEx(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(x => !x.FieldInfo.HasAttribute<NonSerializedAttribute>()
                    && (entityMetadata == null || string.IsNullOrEmpty(x.AutoPropertyName) || !entityMetadata.IsIgnored(x.AutoPropertyName))
                    && !eventNames.Contains(x.Name)))
                .GroupBy(x => x.FieldInfo.Name)
                .SelectMany(x => {
                    var c = x.Count();
                    return x.Select(f => new
                    {
                        Field = f,
                        Name = $"{(c == 1 ? "" : $"[{f.FieldInfo.DeclaringType}]")}{f.AutoPropertyName ?? f.Name}"
                    });
                })
                .ToDictionary(x => x.Name, x => GetFieldWithTargetType(x.Field, objectType));
        }

        private (FieldInfoEx field, Type targetType) GetField(string name)
        {
            if (fields.TryGetValue(name, out var field))
                return field;
            else
                return (null,null);
        }

        private (FieldInfoEx field, Type targetType) GetFieldWithTargetType(FieldInfoEx field, Type objectType)
        {
            if (field.AutoPropertyName == null)
                return (field, field.Type);

            return (field, 
                objectType.GetPropertiesEx(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .FirstOrDefault(x => x.Name == field.AutoPropertyName && x.PropertyInfo.DeclaringType == field.FieldInfo.DeclaringType)
                ?.PropertyInfo?.GetAttribute<DeserializeAsAttribute>()?.Type ?? field.Type);
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
            foreach(var f in fields)
                info.AddValue(f.Key, f.Value.field.Get(obj));
        }
    }
}
