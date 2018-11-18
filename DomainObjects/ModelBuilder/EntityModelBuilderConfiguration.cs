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
            IsRoot = EntityType.IsAggregateRoot();
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

        public DateTimeEntityPropertyModelConfiguration<T> Property(Expression<Func<T, DateTime>> memberSelector)
        {
            var configuration = new DateTimeEntityPropertyModelConfiguration<T>(this, ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public TimeSpanEntityPropertyModelConfiguration<T> Property(Expression<Func<T, TimeSpan>> memberSelector)
        {
            var configuration = new TimeSpanEntityPropertyModelConfiguration<T>(this, ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public BooleanEntityPropertyModelConfiguration<T> Property(Expression<Func<T, bool>> memberSelector)
        {
            var configuration = new BooleanEntityPropertyModelConfiguration<T>(this, ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public NumericEntityPropertyModelConfiguration<T> Property(Expression<Func<T, sbyte>> memberSelector)
        {
            var configuration = new NumericEntityPropertyModelConfiguration<T>(this, ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public NumericEntityPropertyModelConfiguration<T> Property(Expression<Func<T, byte>> memberSelector)
        {
            var configuration = new NumericEntityPropertyModelConfiguration<T>(this, ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public NumericEntityPropertyModelConfiguration<T> Property(Expression<Func<T, short>> memberSelector)
        {
            var configuration = new NumericEntityPropertyModelConfiguration<T>(this, ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public NumericEntityPropertyModelConfiguration<T> Property(Expression<Func<T, ushort>> memberSelector)
        {
            var configuration = new NumericEntityPropertyModelConfiguration<T>(this, ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public NumericEntityPropertyModelConfiguration<T> Property(Expression<Func<T, int>> memberSelector)
        {
            var configuration = new NumericEntityPropertyModelConfiguration<T>(this, ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public NumericEntityPropertyModelConfiguration<T> Property(Expression<Func<T, uint>> memberSelector)
        {
            var configuration = new NumericEntityPropertyModelConfiguration<T>(this, ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public NumericEntityPropertyModelConfiguration<T> Property(Expression<Func<T, long>> memberSelector)
        {
            var configuration = new NumericEntityPropertyModelConfiguration<T>(this, ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public NumericEntityPropertyModelConfiguration<T> Property(Expression<Func<T, ulong>> memberSelector)
        {
            var configuration = new NumericEntityPropertyModelConfiguration<T>(this, ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public NumericEntityPropertyModelConfiguration<T> Property(Expression<Func<T, float>> memberSelector)
        {
            var configuration = new NumericEntityPropertyModelConfiguration<T>(this, ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public NumericEntityPropertyModelConfiguration<T> Property(Expression<Func<T, double>> memberSelector)
        {
            var configuration = new NumericEntityPropertyModelConfiguration<T>(this, ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public NumericEntityPropertyModelConfiguration<T> Property(Expression<Func<T, decimal>> memberSelector)
        {
            var configuration = new NumericEntityPropertyModelConfiguration<T>(this, ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        //public ValueObjectEntityPropertyModelConfiguration<T> Property<TValueObject>(Expression<Func<T, TValueObject>> memberSelector)
        //    where TValueObject : DomainValueObject<TValueObject>
        //{
        //    var configuration = new ValueObjectEntityPropertyModelConfiguration<T>(this, ReflectionHelper.GetProperty(memberSelector));
        //    PropertyModelConfigurations.Add(configuration);
        //    return configuration;
        //}

        internal IEntityPropertyModelConfiguration<T> Property(PropertyInfo property)
        {
            var domainPropertyType = property.PropertyType.GetSupportedValueType();

            if (domainPropertyType.HasValue)
                switch (domainPropertyType.Value)
                {
                    case DomainValueType.String:
                        return new StringEntityPropertyModelConfiguration<T>(this, property);
                    case DomainValueType.Boolean:
                        return new BooleanEntityPropertyModelConfiguration<T>(this, property);
                    case DomainValueType.Number:
                        return new NumericEntityPropertyModelConfiguration<T>(this, property);
                    case DomainValueType.DateTime:
                        return new DateTimeEntityPropertyModelConfiguration<T>(this, property);
                    case DomainValueType.TimeSpan:
                        return new TimeSpanEntityPropertyModelConfiguration<T>(this, property);
                    case DomainValueType.Enum:
                        return new EnumEntityPropertyModelConfiguration<T>(this, property);
                    case DomainValueType.ValueObject:
                        return new ValueObjectEntityPropertyModelConfiguration<T>(this, property);
                    default:
                        break;
                }

            return new UnsupportedTypeEntityPropertyModelConfiguration<T>(this, property);
        }

        public IEntityPropertyModelConfiguration<T> Property<TMember>(Expression<Func<T, TMember>> memberSelector)
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

        public ValueListEntityPropertyModelConfiguration<T> ValueObjectList<TValue>(Expression<Func<T, ValueObjectList<TValue>>> memberSelector)
            where TValue : DomainValueObject<TValue>
        {
            var configuration = new ValueListEntityPropertyModelConfiguration<T>(this, ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public ValueReadOnlyListEntityPropertyModelConfiguration<T> ValueObjectList<TValue>(Expression<Func<T, ValueObjectReadOnlyList<TValue>>> memberSelector)
            where TValue : DomainValueObject<TValue>
        {
            var configuration = new ValueReadOnlyListEntityPropertyModelConfiguration<T>(this, ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public AggregateEntityPropertyModelConfiguration<T> Aggregate<TAggregate>(Expression<Func<T, TAggregate>> memberSelector)
            where TAggregate : Aggregate<TAggregate>
        {
            var configuration = new AggregateEntityPropertyModelConfiguration<T>(this, ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public AggregateListEntityPropertyModelConfiguration<T> Aggregate<TAggregate>(Expression<Func<T, IAggregateList<TAggregate>>> memberSelector)
            where TAggregate : Aggregate<TAggregate>
        {
            var configuration = new AggregateListEntityPropertyModelConfiguration<T>(this, ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }

        public AggregateReadOnlyListEntityPropertyModelConfiguration<T> Aggregate<TAggregate>(Expression<Func<T, IAggregateReadOnlyList<TAggregate>>> memberSelector)
            where TAggregate : Aggregate<TAggregate>
        {
            var configuration = new AggregateReadOnlyListEntityPropertyModelConfiguration<T>(this, ReflectionHelper.GetProperty(memberSelector));
            PropertyModelConfigurations.Add(configuration);
            return configuration;
        }
    }
}
