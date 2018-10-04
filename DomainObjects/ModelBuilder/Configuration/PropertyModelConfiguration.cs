using DomainObjects.Core;
using Dynamix.Reflection;
using System;
using System.Reflection;

namespace DomainObjects.ModelBuilder.Configuration
{
    public abstract class PropertyModelConfiguration
    {
        internal bool IsOptional { get; set; }
        
        internal PropertyInfo Property { get; }

        protected PropertyModelConfiguration(PropertyInfo property)
        {
            this.Property = property;
            IsOptional = property.PropertyType.IsClass || property.PropertyType.IsNullable();
        }
    }

    public interface IEntityPropertyModelConfiguration<T>
        where T : DomainEntity
    {
        EntityModelBuilderConfiguration<T> End();
    }

    public interface IValueObjectPropertyModelConfiguration<T>
    where T : DomainValueObject<T>
    {
        ValueTypeModelBuilderConfiguration<T> End();
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
