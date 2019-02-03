
using DomainObjects.Core;
using DomainObjects.Internal;
using DomainObjects.Metadata;
using DomainObjects.ModelBuilder.Configuration;
using Dynamix.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DomainObjects.ModelBuilder
{
    public abstract class ValueObjectModelBuilderConfiguration
    {
        protected ValueObjectModelBuilderConfiguration(Type type)
        {
            ValueObjectType = type;
        }
        internal List<string> IgnoredMembers { get; } = new List<string>();
        internal List<PropertyModelConfiguration> PropertyModelConfigurations { get; } = new List<PropertyModelConfiguration>();
        internal Type ValueObjectType { get; }
    }

    public class ValueTypeModelBuilderConfiguration<T> : ValueObjectModelBuilderConfiguration where T : DomainValueObject<T>
    {
        public ValueTypeModelBuilderConfiguration():base(typeof(T))
        {
            
        }

        public ValueTypeModelBuilderConfiguration<T> IgnoreMember<TMember>(Expression<Func<T, TMember>> memberSelector)
        {
            IgnoredMembers.Add(ReflectionHelper.GetMemberName(memberSelector));
            return this;
        }

        public ValueTypeModelBuilderConfiguration<T> IgnoreMember(string memberName)
        {
            IgnoredMembers.Add(memberName);
            return this;
        }

        public StringValueObjectPropertyModelConfiguration<T> Property(Expression<Func<T, string>> memberSelector)
        {
            var configuration = new StringValueObjectPropertyModelConfiguration<T>(this, ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public DateTimeValueObjectPropertyModelConfiguration<T> Property(Expression<Func<T, DateTime>> memberSelector)
        {
            var configuration = new DateTimeValueObjectPropertyModelConfiguration<T>(this, ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public TimeSpanValueObjectPropertyModelConfiguration<T> Property(Expression<Func<T, TimeSpan>> memberSelector)
        {
            var configuration = new TimeSpanValueObjectPropertyModelConfiguration<T>(this, ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public BooleanValueObjectPropertyModelConfiguration<T> Property(Expression<Func<T, bool>> memberSelector)
        {
            var configuration = new BooleanValueObjectPropertyModelConfiguration<T>(this, ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public NumericValueObjectPropertyModelConfiguration<T> Property(Expression<Func<T, sbyte>> memberSelector)
        {
            var configuration = new NumericValueObjectPropertyModelConfiguration<T>(this, ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public NumericValueObjectPropertyModelConfiguration<T> Property(Expression<Func<T, byte>> memberSelector)
        {
            var configuration = new NumericValueObjectPropertyModelConfiguration<T>(this, ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public NumericValueObjectPropertyModelConfiguration<T> Property(Expression<Func<T, short>> memberSelector)
        {
            var configuration = new NumericValueObjectPropertyModelConfiguration<T>(this, ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public NumericValueObjectPropertyModelConfiguration<T> Property(Expression<Func<T, ushort>> memberSelector)
        {
            var configuration = new NumericValueObjectPropertyModelConfiguration<T>(this, ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public NumericValueObjectPropertyModelConfiguration<T> Property(Expression<Func<T, int>> memberSelector)
        {
            var configuration = new NumericValueObjectPropertyModelConfiguration<T>(this, ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public NumericValueObjectPropertyModelConfiguration<T> Property(Expression<Func<T, uint>> memberSelector)
        {
            var configuration = new NumericValueObjectPropertyModelConfiguration<T>(this, ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public NumericValueObjectPropertyModelConfiguration<T> Property(Expression<Func<T, long>> memberSelector)
        {
            var configuration = new NumericValueObjectPropertyModelConfiguration<T>(this, ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public NumericValueObjectPropertyModelConfiguration<T> Property(Expression<Func<T, ulong>> memberSelector)
        {
            var configuration = new NumericValueObjectPropertyModelConfiguration<T>(this, ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public NumericValueObjectPropertyModelConfiguration<T> Property(Expression<Func<T, float>> memberSelector)
        {
            var configuration = new NumericValueObjectPropertyModelConfiguration<T>(this, ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public NumericValueObjectPropertyModelConfiguration<T> Property(Expression<Func<T, double>> memberSelector)
        {
            var configuration = new NumericValueObjectPropertyModelConfiguration<T>(this, ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public NumericValueObjectPropertyModelConfiguration<T> Property(Expression<Func<T, decimal>> memberSelector)
        {
            var configuration = new NumericValueObjectPropertyModelConfiguration<T>(this, ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        //public ValueObjectValueObjectPropertyModelConfiguration<T> Property<TValueObject>(Expression<Func<T, TValueObject>> memberSelector)
        //    where TValueObject : DomainValueObject<TValueObject>
        //{
        //    var configuration = new ValueObjectValueObjectPropertyModelConfiguration<T>(this, ReflectionHelper.GetProperty(memberSelector));
        //    PropertyModelConfigurations.Add(configuration);
        //    return configuration;
        //}

        internal IValueObjectPropertyModelConfiguration<T> Property(PropertyInfo property)
        {
            var domainPropertyType = property.PropertyType.GetSupportedValueType();

            if (domainPropertyType.HasValue)
                switch (domainPropertyType.Value)
                {
                    case DomainValueType.String:
                        return new StringValueObjectPropertyModelConfiguration<T>(this, property);
                    case DomainValueType.Boolean:
                        return new BooleanValueObjectPropertyModelConfiguration<T>(this, property);
                    case DomainValueType.Number:
                        return new NumericValueObjectPropertyModelConfiguration<T>(this, property);
                    case DomainValueType.DateTime:
                        return new DateTimeValueObjectPropertyModelConfiguration<T>(this, property);
                    case DomainValueType.TimeSpan:
                        return new TimeSpanValueObjectPropertyModelConfiguration<T>(this, property);
                    case DomainValueType.Enum:
                        return new EnumValueObjectPropertyModelConfiguration<T>(this, property);
                    case DomainValueType.ValueObject:
                        return new ValueObjectValueObjectPropertyModelConfiguration<T>(this, property);
                    default:
                        break;
                }

            return new UnsupportedTypeValueObjectPropertyModelConfiguration<T>(this, property);
        }

        public IValueObjectPropertyModelConfiguration<T> Property<TMember>(Expression<Func<T, TMember>> memberSelector)
        {
            return Property(ReflectionHelper.GetProperty(memberSelector));
        }

        //public PropertyModelConfiguration Property<TMember>(Expression<Func<T, ValueList<TMember>>> memberSelector)
        //{
        //    if (typeof(TMember).IsEnum)
        //        return new EnumPropertyModelConfiguration(ReflectionHelper.GetProperty(memberSelector));
        //    else if (typeof(TMember).IsSubclassOfDeep(typeof(DomainValue)))
        //        return new ValueObjectPropertyModelConfiguration(ReflectionHelper.GetProperty(memberSelector));
        //    else
        //        return new UnsupportedTypePropertyModelConfiguration(ReflectionHelper.GetProperty(memberSelector));
        //}

        public ValueListValueObjectPropertyModelConfiguration<T> ValueObjectList<TValue>(Expression<Func<T, IValueObjectList<TValue>>> memberSelector)
            where TValue : DomainValueObject<TValue>
        {
            var configuration = new ValueListValueObjectPropertyModelConfiguration<T>(this, ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public ValueReadOnlyListValueObjectPropertyModelConfiguration<T> ValueObjectList<TValue>(Expression<Func<T, IValueObjectReadOnlyList<TValue>>> memberSelector)
            where TValue : DomainValueObject<TValue>
        {
            var configuration = new ValueReadOnlyListValueObjectPropertyModelConfiguration<T>(this, ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }
    }
}
