using DomainObjects.Core;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DomainObjects.ModelBuilder.Configuration
{
    public class StringPropertyModelConfiguration : PropertyModelConfiguration
    {
        int? maxLength = 0;
        internal StringPropertyModelConfiguration(PropertyInfo property) : base(property)
        {
        }

        public StringPropertyModelConfiguration HasMaxLength(int maxLength)
        {
            this.maxLength = maxLength;
            return this;
        }

        public new StringPropertyModelConfiguration IsRequired()
        {
            base.IsRequired();
            return this;
        }
    }

    //public class StringPropertyModelConfiguration<T> : PropertyModelConfiguration
    //    where T : DomainObject
    //{
    //    readonly List<Func<string, bool>> validators= new List<Func<string, bool>>();
        
    //    public StringPropertyModelConfiguration(PropertyInfo property) : base(property)
    //    {
    //    }

    //    public StringPropertyModelConfiguration<T> HasMaxLength(int maxLength)
    //    {
    //        validators.Add((string x) => string.IsNullOrWhiteSpace(x) || x.Trim().Length <= maxLength);
    //        return this;
    //    }

    //    public StringPropertyModelConfiguration<T> Requires(Func<string,bool> validator)
    //    {
    //        validators.Add(validator);
    //        return this;
    //    }

    //    public new StringPropertyModelConfiguration<T> IsRequired()
    //    {
    //        base.IsRequired();
    //        return this;
    //    }

    //    public EntityModelBuilderConfiguration<T> End()
    //    {
    //        return null;
    //    }
    //}
}
