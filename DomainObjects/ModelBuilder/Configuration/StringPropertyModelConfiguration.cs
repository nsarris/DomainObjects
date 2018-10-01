﻿using DomainObjects.Core;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DomainObjects.ModelBuilder.Configuration
{
    public class StringPropertyModelConfiguration : PropertyModelConfiguration
    {
        internal uint? MaxLength { get; set; }
        internal StringPropertyModelConfiguration(PropertyInfo property) : base(property)
        {
        }
    }

    public static class StringPropertyModelConfigurationExtensions
    {
        public static T HasMaxLength<T>(this T c, uint maxLength)
            where T : StringPropertyModelConfiguration
        {
            c.MaxLength = maxLength;
            return c;
        }
    }

    public class StringEntityPropertyModelConfiguration<T> : StringPropertyModelConfiguration
        where T : DomainEntity
    {
        private readonly EntityModelBuilderConfiguration<T> propertyConfiguration;

        public StringEntityPropertyModelConfiguration(EntityModelBuilderConfiguration<T> propertyConfiguration, PropertyInfo property) : base(property)
        {
            this.propertyConfiguration = propertyConfiguration;
        }
        public EntityModelBuilderConfiguration<T> End()
        {
            return propertyConfiguration;
        }
    }

    //public class StringValueObjectPropertyModelConfiguration<T> : StringPropertyModelConfiguration
    //    where T : DomainEntity
    //{
    //    private readonly EntityModelBuilderConfiguration<T> propertyConfiguration;

    //    public StringValueObjectPropertyModelConfiguration(EntityModelBuilderConfiguration<T> propertyConfiguration, PropertyInfo property) : base(property)
    //    {
    //        this.propertyConfiguration = propertyConfiguration;
    //    }
    //    public EntityModelBuilderConfiguration<T> End()
    //    {
    //        return propertyConfiguration;
    //    }
    //}
}
