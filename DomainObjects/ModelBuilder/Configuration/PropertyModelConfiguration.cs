using Dynamix.Reflection;
using System;
using System.Reflection;

namespace DomainObjects.ModelBuilder
{
    public abstract class PropertyModelConfiguration
    {
        internal bool IsOptional { get; private set; }
        internal bool IsNullable { get; }
        internal Type EffectiveType { get; }

        readonly PropertyInfo property;

        protected PropertyModelConfiguration(PropertyInfo property)
        {
            this.property = property;
            IsNullable = (property.PropertyType.IsClass || property.PropertyType.IsNullable());
            EffectiveType = IsNullable ? Nullable.GetUnderlyingType(property.PropertyType) : property.PropertyType;
            IsOptional = IsNullable;
        }

        public PropertyModelConfiguration IsRequired()
        {
            IsOptional = false;
            return this;
        }
    }
}
