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
    public abstract class EntityModelBuilderConfiguration
    {
        protected EntityModelBuilderConfiguration(Type type)
        {
            EntityType = type;
            IsRoot = EntityType.IsOrSubclassOfGenericDeep(typeof(AggregateRoot<,>));
        }
        internal List<string> IgnoredMembers { get; } = new List<string>();
        internal List<string> KeyMembers { get; } = new List<string>();
        internal List<PropertyModelConfiguration> PropertyModelConfigurations { get; } = new List<PropertyModelConfiguration>();
        internal Type EntityType { get; }
        internal bool IsRoot { get; }
    }

    public class EntityModelBuilderConfiguration<T> : EntityModelBuilderConfiguration where T : DomainEntity
    {
        public EntityModelBuilderConfiguration():base(typeof(T))
        {
            
        }

        public virtual EntityModelBuilderConfiguration<T> HasKey(params string[] propertyNames)
        {
            KeyMembers.Clear();
            KeyMembers.AddRange(propertyNames);
            return this;
        }

        public virtual EntityModelBuilderConfiguration<T> HasKey<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            var keyProps = ExpressionHelper.GetSelectedProperties(keySelector);
            KeyMembers.Clear();
            KeyMembers.AddRange(keyProps.Select(x => x.Name));
            return this;
        }

        public EntityModelBuilderConfiguration<T> IgnoreMember<TMember>(Expression<Func<T, TMember>> memberSelector)
        {
            IgnoredMembers.Add(ReflectionHelper.GetMemberName(memberSelector));
            return this;
        }

        public EntityModelBuilderConfiguration<T> IgnoreMember(string memberName)
        {
            IgnoredMembers.Add(memberName);
            return this;
        }

        public StringEntityPropertyModelConfiguration<T> Property(Expression<Func<T, string>> memberSelector)
        {
            var configuration = new StringEntityPropertyModelConfiguration<T>(this, ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public DateTimePropertyModelConfiguration Property(Expression<Func<T, DateTime>> memberSelector)
        {
            var configuration = new DateTimePropertyModelConfiguration(ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public TimeSpanPropertyModelConfiguration Property(Expression<Func<T, TimeSpan>> memberSelector)
        {
            var configuration = new TimeSpanPropertyModelConfiguration(ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

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
            var domainPropertyType = property.PropertyType.GetSupportedValueType();

            if (domainPropertyType.HasValue)
                switch (domainPropertyType.Value)
                {
                    case DomainValueType.String:
                        return new StringEntityPropertyModelConfiguration<T>(this, property);
                    case DomainValueType.Boolean:
                        return new BooleanPropertyModelConfiguration(property);
                    case DomainValueType.Number:
                        return new NumericPropertyModelConfiguration(property);
                    case DomainValueType.DateTime:
                        return new DateTimePropertyModelConfiguration(property);
                    case DomainValueType.TimeSpan:
                        return new TimeSpanPropertyModelConfiguration(property);
                    case DomainValueType.Enum:
                        return new EnumPropertyModelConfiguration(property);
                    case DomainValueType.ValueObject:
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

        //public PropertyModelConfiguration Property<TMember>(Expression<Func<T, ValueList<TMember>>> memberSelector)
        //{
        //    if (typeof(TMember).IsEnum)
        //        return new EnumPropertyModelConfiguration(ReflectionHelper.GetProperty(memberSelector));
        //    else if (typeof(TMember).IsSubclassOfDeep(typeof(DomainValue)))
        //        return new ValueTypePropertyModelConfiguration(ReflectionHelper.GetProperty(memberSelector));
        //    else
        //        return new UnsupportedTypePropertyModelConfiguration(ReflectionHelper.GetProperty(memberSelector));
        //}

        public ValueListModelConfiguration ValueObjectList<TValue>(Expression<Func<T, ValueObjectList<TValue>>> memberSelector)
            where TValue : DomainValueObject<TValue>
        {
            var configuration = new ValueListModelConfiguration(ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public ValueReadOnlyListModelConfiguration ValueObjectList<TValue>(Expression<Func<T, ValueObjectReadOnlyList<TValue>>> memberSelector)
            where TValue : DomainValueObject<TValue>
        {
            var configuration = new ValueReadOnlyListModelConfiguration(ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public AggregatePropertyModelConfiguration Aggregate<TAggregate>(Expression<Func<T, TAggregate>> memberSelector)
            where TAggregate : Aggregate<TAggregate>
        {
            var configuration = new AggregatePropertyModelConfiguration(ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public AggregateListModelConfiguration Aggregate<TAggregate>(Expression<Func<T, AggregateList<TAggregate>>> memberSelector)
            where TAggregate : Aggregate<TAggregate>
        {
            var configuration = new AggregateListModelConfiguration(ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public AggregateReadOnlyListModelConfiguration Aggregate<TAggregate>(Expression<Func<T, AggregateReadOnlyList<TAggregate>>> memberSelector)
            where TAggregate : Aggregate<TAggregate>
        {
            var configuration = new AggregateReadOnlyListModelConfiguration(ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }
    }
}
