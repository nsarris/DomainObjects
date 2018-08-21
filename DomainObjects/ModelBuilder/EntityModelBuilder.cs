using DomainObjects.Core;
using DomainObjects.Metadata;
using Dynamix.Reflection;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace DomainObjects.ModelBuilder
{
    public class EntityModelBuilder
    {
        internal List<string> IgnoredMembers { get; } = new List<string>();
        internal List<PropertyModelConfiguration> PropertyModelConfigurations { get; } = new List<PropertyModelConfiguration>();
        internal Expression<Func<object, object>> keySelector;
    }

    public class EntityModelBuilder<T> : EntityModelBuilder where T : DomainEntity
    {
        
        public virtual EntityModelBuilder<T> HasKey<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            //Dynamix.Expressions.InitExpressionParser.ParseNewExpression(keySelector);
            //this.keySelector = (object x) => keySelector(x);
            return this;
        }

        public EntityModelBuilder<T> IgnoreMember<TMember>(Expression<Func<T, TMember>> memberSelector)
        {
            IgnoredMembers.Add(ReflectionHelper.GetMemberName(memberSelector));
            return this;
        }

        public EntityModelBuilder<T> IgnoreMember(string memberName)
        {
            IgnoredMembers.Add(memberName);
            return this;
        }

        public StringPropertyModelConfiguration Property(Expression<Func<T, string>> memberSelector)
        {
            var configuration = new StringPropertyModelConfiguration(ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public DateTimePropertyModelConfiguration Property(Expression<Func<T, DateTime>> memberSelector)
        {
            var configuration = new DateTimePropertyModelConfiguration(ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        //public IPropertyModelConfiguration<TimeSpan> Property(Expression<Func<T, TimeSpan>> memberSelector)
        //{
        //    return new StringPropertyModelConfiguration(ReflectionHelper.GetProperty(memberSelector));
        //}

        public BooleanPropertyModelConfiguration Property(Expression<Func<T, bool>> memberSelector)
        {
            var configuration = new BooleanPropertyModelConfiguration(ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public NumericPropertyModelConfiguration Property(Expression<Func<T, sbyte>> memberSelector)
        {
            var configuration = new NumericPropertyModelConfiguration(ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public NumericPropertyModelConfiguration Property(Expression<Func<T, byte>> memberSelector)
        {
            var configuration = new NumericPropertyModelConfiguration(ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public NumericPropertyModelConfiguration Property(Expression<Func<T, short>> memberSelector)
        {
            var configuration = new NumericPropertyModelConfiguration(ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public NumericPropertyModelConfiguration Property(Expression<Func<T, ushort>> memberSelector)
        {
            var configuration = new NumericPropertyModelConfiguration(ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public NumericPropertyModelConfiguration Property(Expression<Func<T, int>> memberSelector)
        {
            var configuration = new NumericPropertyModelConfiguration(ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public NumericPropertyModelConfiguration Property(Expression<Func<T, uint>> memberSelector)
        {
            var configuration = new NumericPropertyModelConfiguration(ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public NumericPropertyModelConfiguration Property(Expression<Func<T, long>> memberSelector)
        {
            var configuration = new NumericPropertyModelConfiguration(ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public NumericPropertyModelConfiguration Property(Expression<Func<T, ulong>> memberSelector)
        {
            var configuration = new NumericPropertyModelConfiguration(ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public NumericPropertyModelConfiguration Property(Expression<Func<T, float>> memberSelector)
        {
            var configuration = new NumericPropertyModelConfiguration(ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public NumericPropertyModelConfiguration Property(Expression<Func<T, double>> memberSelector)
        {
            var configuration = new NumericPropertyModelConfiguration(ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public NumericPropertyModelConfiguration Property(Expression<Func<T, decimal>> memberSelector)
        {
            var configuration = new NumericPropertyModelConfiguration(ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        internal PropertyModelConfiguration Property(PropertyInfo property)
        {
            if (property.PropertyType.IsEnum)
                return new EnumPropertyModelConfiguration(property);

            var domainPropertyType = property.PropertyType.GetSupportedValueType();

            if (domainPropertyType.HasValue)
                switch (domainPropertyType.Value)
                {
                    case DomainValueType.String:
                        return new StringPropertyModelConfiguration(property);
                    case DomainValueType.Boolean:
                        return new BooleanPropertyModelConfiguration(property);
                    case DomainValueType.Number:
                        return new NumericPropertyModelConfiguration(property);
                    case DomainValueType.DateTime:
                        return new DateTimePropertyModelConfiguration(property);
                    case DomainValueType.TimeSpan:
                        break;
                    case DomainValueType.Complex:
                        return new ValueTypePropertyModelConfiguration(property);
                    default:
                        break;
                }

            return new UnsupportedTypePropertyModelConfiguration(property);
        }

        public PropertyModelConfiguration Property<TMember>(Expression<Func<T, TMember>> memberSelector)
        {
            return Property(ReflectionHelper.GetProperty(memberSelector));
        }

        public PropertyModelConfiguration Property<TMember>(Expression<Func<T, ValueList<TMember>>> memberSelector)
        {
            if (typeof(TMember).IsEnum)
                return new EnumPropertyModelConfiguration(ReflectionHelper.GetProperty(memberSelector));
            else if (typeof(TMember).IsSubclassOfDeep(typeof(DomainValue)))
                return new ValueTypePropertyModelConfiguration(ReflectionHelper.GetProperty(memberSelector));
            else
                return new UnsupportedTypePropertyModelConfiguration(ReflectionHelper.GetProperty(memberSelector));
        }


        public PropertyModelConfiguration Aggregate<TAggregate>(Expression<Func<T, TAggregate>> memberSelector)
            where TAggregate : Aggregate
        {
            return null;
        }

        public PropertyModelConfiguration Aggregate<TAggregate>(Expression<Func<T, AggregateList<TAggregate>>> memberSelector)
            where TAggregate : Aggregate
        {
            return null;
        }

        public PropertyModelConfiguration Aggregate<TAggregate>(Expression<Func<T, AggregateReadOnlyList<TAggregate>>> memberSelector)
            where TAggregate : Aggregate
        {
            return null;
        }

        internal DomainEntityMetadata Build()
        {
            return null;
        }
    }

    public class EntityModelBuilder<T, TKey> : EntityModelBuilder<T> where T : DomainEntity<TKey>
    {
        new Expression<Func<T, TKey>> keySelector;
        public override EntityModelBuilder<T> HasKey<TKey1>(Expression<Func<T, TKey1>> keySelector)
        {
            return base.HasKey(keySelector);
        }

        public EntityModelBuilder<T> HasKey(Expression<Func<T, TKey>> keySelector)
        {
            //Dynamix.Expressions.InitExpressionParser.ParseNewExpression(keySelector);
            //this.keySelector = (object x) => keySelector(x);
            return this;
        }
    }
}
