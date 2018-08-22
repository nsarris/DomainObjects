using Dynamix.Reflection;
using System;
using System.Reflection;

namespace DomainObjects.ModelBuilder.Configuration
{
    public abstract class PropertyModelConfiguration
    {
        internal bool IsOptional { get; private set; }
        //internal bool IsNullable { get; }
        //internal Type EffectiveType { get; }

        internal PropertyInfo Property { get; }

        protected PropertyModelConfiguration(PropertyInfo property)
        {
            this.Property = property;
            //IsNullable = (property.PropertyType.IsClass || property.PropertyType.IsNullable());
            //EffectiveType = IsNullable ? Nullable.GetUnderlyingType(property.PropertyType) : property.PropertyType;
            IsOptional = property.PropertyType.IsClass || property.PropertyType.IsNullable();
        }

        public PropertyModelConfiguration IsRequired()
        {
            IsOptional = false;
            return this;
        }
    }
}
