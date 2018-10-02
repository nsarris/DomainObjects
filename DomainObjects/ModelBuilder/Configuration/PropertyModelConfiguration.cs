using DomainObjects.Core;
using Dynamix.Reflection;
using System;
using System.Reflection;

namespace DomainObjects.ModelBuilder.Configuration
{
    public abstract class PropertyModelConfiguration
    {
        internal bool IsOptional { get; set; }
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

        //internal PropertyModelConfiguration IsRequired()
        //{
        //    IsOptional = false;
        //    return this;
        //}
    }

    public interface IEntityPropertyModelConfiguration<T>
        where T : DomainEntity
    {
        EntityModelBuilderConfiguration<T> End();
    }

    public static class PropertyModelConfigurationExtensions
    {
        public static T IsRequired<T>(this T c)
            where T : PropertyModelConfiguration
        {
            c.IsOptional = false;
            return c;
        }
    }
}
