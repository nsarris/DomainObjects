using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;
using FluentValidation.Validators;
using System;
using System.Linq;
using System.Linq.Expressions;


namespace FluentValidation.IoC
{
    public sealed class ResolverRuleBuilder<T, TChild> : IRuleBuilderOptions<T, TChild>
    {
        #region Private Fields

        readonly IRuleBuilder<T, TChild> ruleBuilder;
        readonly RuleBuilder<T, TChild> internalRuleBuilder;

        #endregion

        #region Ctor

        public ResolverRuleBuilder(IRuleBuilder<T, TChild> ruleBuilder)
        {
            this.ruleBuilder = ruleBuilder;
            this.internalRuleBuilder = ruleBuilder as RuleBuilder<T, TChild>;
        }

        #endregion

        #region Validation and Dependency resolition

        private IDependencyResolver GetResolver(CustomContext context)
        {
            return GetResolver(context.ParentContext);
        }
        private IDependencyResolver GetResolver(PropertyValidatorContext context)
        {
            return GetResolver(context.ParentContext);
        }

        private IDependencyResolver GetResolver(ValidationContext context)
        {
            return (IDependencyResolver)context.RootContextData[Constants.DependencyResolverKeyLiteral];
        }

        private IValidatorFactory GetFactory(CustomContext context)
        {
            return GetFactory(context.ParentContext);
        }
        private IValidatorFactory GetFactory(PropertyValidatorContext context)
        {
            return GetFactory(context.ParentContext);
        }

        private IValidatorFactory GetFactory(ValidationContext context)
        {
            return (IValidatorFactory)context.RootContextData[Constants.ValidatorFactoryKeyLiteral];
        }

        private TValidator ResolveValidator<TValidator>(CustomContext context)
            where TValidator : IValidator<TChild>
        {
            return GetFactory(context).GetValidator<TChild, TValidator>();
        }

        private TValidator ResolveValidator<TValidator>(PropertyValidatorContext context)
            where TValidator : IValidator<TChild>
        {
            return GetFactory(context).GetValidator<TChild, TValidator>();
        }

        private TDependency ResolveDependency<TDependency>(CustomContext context)
        {
            return GetResolver(context).Resolve<TDependency>();
        }

        private TDependency ResolveDependency<TDependency>(PropertyValidatorContext context)
        {
            return GetResolver(context).Resolve<TDependency>();
        }

        #endregion

        #region ExecuteChild (Internal) 

        private ValidationResult ExecuteChild(TChild instance, IValidator<TChild> childValidator, CustomContext context)
        {
            var childContext =
                context.ParentContext.IsChildCollectionContext ?
                context.ParentContext.CloneForChildCollectionValidator(instance, true) :
                context.ParentContext.CloneForChildValidator(instance, true, context.ParentContext.Selector);

            return childValidator.Validate(childContext);
        }

        private ValidationResult ExecuteChild<TValidator>(TChild instance, CustomContext context)
            where TValidator : IValidator<TChild>
        {
            var childValidator = ResolveValidator<TValidator>(context);
            return ExecuteChild(instance, childValidator, context);
        }

        #endregion

        #region SetValidator - Resolve another Validator

        public ResolverRuleBuilder<T, TChild> SetValidator<TValidator>()
            where TValidator : IValidator<TChild>
        {
            return new ResolverRuleBuilder<T, TChild>(
                ruleBuilder
                .Custom((x, context) =>
                {
                    if (x != null)
                        context.Append(ExecuteChild<TValidator>(x, context));
                }));
        }

        public ResolverRuleBuilder<T, TChild> SetValidator<TValidator>(Func<T, TChild, TValidator, bool> validatorFunction)
            where TValidator : IValidator<TChild>
        {
            return new ResolverRuleBuilder<T, TChild>(ruleBuilder
                .Must((parent, child, context) =>
                {
                    var validator = ResolveValidator<TValidator>(context);
                    return validatorFunction(parent, child, validator);
                }));
        }

        public ResolverRuleBuilder<T, TChild> SetValidator<TValidator>(Func<T, TChild, TValidator, ValidationResult> validatorFunction)
            where TValidator : IValidator<TChild>
        {
            return new ResolverRuleBuilder<T, TChild>(ruleBuilder
                .Custom((x, context) =>
                {
                    var validator = ResolveValidator<TValidator>(context);
                    var parent = (T)context.ParentContext.InstanceToValidate;
                    context.Append(validatorFunction(parent, x, validator));
                }));
        }

        #endregion

        #region Custom - Inject resolver and implement custom logic

        public ResolverRuleBuilder<T, TChild> Custom(Func<T, TChild, IDependencyResolver, bool> validatorFunction)
        {
            return new ResolverRuleBuilder<T, TChild>(ruleBuilder
                .Must((parent, child, context) =>
                {
                    var resolver = GetResolver(context);
                    return validatorFunction(parent, child, resolver);
                }));
        }

        public ResolverRuleBuilder<T, TChild> Custom(Func<T, TChild, IDependencyResolver, ValidationResult> validatorFunction)
        {
            return new ResolverRuleBuilder<T, TChild>(ruleBuilder
                .Custom((x, context) =>
                {
                    var resolver = GetResolver(context);
                    var parent = (T)context.ParentContext.InstanceToValidate;
                    context.Append(validatorFunction(parent, x, resolver));
                }));
        }

        public ResolverRuleBuilder<T, TChild> Custom(Action<T, TChild, IDependencyResolver, CustomContext> validatorAction)
        {
            return new ResolverRuleBuilder<T, TChild>(ruleBuilder
                .Custom((x, context) =>
                {
                    var resolver = GetResolver(context);
                    var parent = (T)context.ParentContext.InstanceToValidate;
                    validatorAction(parent, x, resolver, context);
                }));
        }

        #endregion

        #region Custom - Inject a dependency and implement custom logic

        public ResolverRuleBuilder<T, TChild> CustomUsing<TDependency>(Func<T, TChild, TDependency, bool> validatorFunction)
        {
            return new ResolverRuleBuilder<T, TChild>(ruleBuilder
                .Must((parent, child, context) =>
                {
                    var dependency = ResolveDependency<TDependency>(context);
                    return validatorFunction(parent, child, dependency);
                }));
        }

        public ResolverRuleBuilder<T, TChild> CustomUsing<TDependency>(Action<T, TChild, TDependency, CustomContext> validationAction)
        {
            return new ResolverRuleBuilder<T, TChild>(ruleBuilder
                .Custom((x, context) =>
                {
                    var dependency = ResolveDependency<TDependency>(context);
                    var parent = (T)context.ParentContext.InstanceToValidate;
                    validationAction(parent, x, dependency, context);
                }));
        }

        public ResolverRuleBuilder<T, TChild> CustomUsing<TDependency>(Func<T, TChild, TDependency, ValidationResult> validatorFunction)
        {
            return new ResolverRuleBuilder<T, TChild>(ruleBuilder
                .Custom((x, context) =>
                {
                    var dependency = ResolveDependency<TDependency>(context);
                    var parent = (T)context.ParentContext.InstanceToValidate;
                    context.Append(validatorFunction(parent, x, dependency));
                }));
        }

        #endregion

        #region Custom - Inject two dependencies and implement custom logic

        public ResolverRuleBuilder<T, TChild> CustomUsing<T1, T2>(Func<T, TChild, T1, T2, bool> validatorFunction)
        {
            return new ResolverRuleBuilder<T, TChild>(ruleBuilder
                .Must((parent, child, context) =>
                {
                    var dependency1 = ResolveDependency<T1>(context);
                    var dependency2 = ResolveDependency<T2>(context);
                    return validatorFunction(parent, child, dependency1, dependency2);
                }));
        }

        public ResolverRuleBuilder<T, TChild> CustomUsing<T1, T2>(Action<T, TChild, T1, T2, CustomContext> validationAction)
        {
            return new ResolverRuleBuilder<T, TChild>(ruleBuilder
                .Custom((x, context) =>
                {
                    var dependency1 = ResolveDependency<T1>(context);
                    var dependency2 = ResolveDependency<T2>(context);
                    var parent = (T)context.ParentContext.InstanceToValidate;
                    validationAction(parent, x, dependency1, dependency2, context);
                }));
        }

        public ResolverRuleBuilder<T, TChild> CustomUsing<T1, T2>(Func<T, TChild, T1, T2, ValidationResult> validatorFunction)
        {
            return new ResolverRuleBuilder<T, TChild>(ruleBuilder
                .Custom((x, context) =>
                {
                    var dependency1 = ResolveDependency<T1>(context);
                    var dependency2 = ResolveDependency<T2>(context);
                    var parent = (T)context.ParentContext.InstanceToValidate;
                    context.Append(validatorFunction(parent, x, dependency1, dependency2));
                }));
        }

        #endregion

        #region Custom - Inject three dependencies and implement custom logic

        public ResolverRuleBuilder<T, TChild> CustomUsing<T1, T2, T3>(Func<T, TChild, T1, T2, T3, bool> validatorFunction)
        {
            return new ResolverRuleBuilder<T, TChild>(ruleBuilder
                .Must((parent, child, context) =>
                {
                    var dependency1 = ResolveDependency<T1>(context);
                    var dependency2 = ResolveDependency<T2>(context);
                    var dependency3 = ResolveDependency<T3>(context);
                    return validatorFunction(parent, child, dependency1, dependency2, dependency3);
                }));
        }

        public ResolverRuleBuilder<T, TChild> CustomUsing<T1, T2, T3>(Action<T, TChild, T1, T2, T3, CustomContext> validationAction)
        {
            return new ResolverRuleBuilder<T, TChild>(ruleBuilder
                .Custom((x, context) =>
                {
                    var dependency1 = ResolveDependency<T1>(context);
                    var dependency2 = ResolveDependency<T2>(context);
                    var dependency3 = ResolveDependency<T3>(context);
                    var parent = (T)context.ParentContext.InstanceToValidate;
                    validationAction(parent, x, dependency1, dependency2, dependency3, context);
                }));
        }

        public ResolverRuleBuilder<T, TChild> CustomUsing<T1, T2, T3>(Func<T, TChild, T1, T2, T3, ValidationResult> validatorFunction)
        {
            return new ResolverRuleBuilder<T, TChild>(ruleBuilder
                .Custom((x, context) =>
                {
                    var dependency1 = ResolveDependency<T1>(context);
                    var dependency2 = ResolveDependency<T2>(context);
                    var dependency3 = ResolveDependency<T3>(context);
                    var parent = (T)context.ParentContext.InstanceToValidate;
                    context.Append(validatorFunction(parent, x, dependency1, dependency2, dependency3));
                }));
        }

        #endregion

        #region Custom - Inject four dependencies and implement custom logic

        public ResolverRuleBuilder<T, TChild> CustomUsing<T1, T2, T3, T4>(Func<T, TChild, T1, T2, T3, T4, bool> validatorFunction)
        {
            return new ResolverRuleBuilder<T, TChild>(ruleBuilder
                .Must((parent, child, context) =>
                {
                    var dependency1 = ResolveDependency<T1>(context);
                    var dependency2 = ResolveDependency<T2>(context);
                    var dependency3 = ResolveDependency<T3>(context);
                    var dependency4 = ResolveDependency<T4>(context);
                    return validatorFunction(parent, child, dependency1, dependency2, dependency3, dependency4);
                }));
        }

        public ResolverRuleBuilder<T, TChild> CustomUsing<T1, T2, T3, T4>(Action<T, TChild, T1, T2, T3, T4, CustomContext> validationAction)
        {
            return new ResolverRuleBuilder<T, TChild>(ruleBuilder
                .Custom((x, context) =>
                {
                    var dependency1 = ResolveDependency<T1>(context);
                    var dependency2 = ResolveDependency<T2>(context);
                    var dependency3 = ResolveDependency<T3>(context);
                    var dependency4 = ResolveDependency<T4>(context);
                    var parent = (T)context.ParentContext.InstanceToValidate;
                    validationAction(parent, x, dependency1, dependency2, dependency3, dependency4, context);
                }));
        }

        public ResolverRuleBuilder<T, TChild> CustomUsing<T1, T2, T3, T4>(Func<T, TChild, T1, T2, T3, T4, ValidationResult> validatorFunction)
        {
            return new ResolverRuleBuilder<T, TChild>(ruleBuilder
                .Custom((x, context) =>
                {
                    var dependency1 = ResolveDependency<T1>(context);
                    var dependency2 = ResolveDependency<T2>(context);
                    var dependency3 = ResolveDependency<T3>(context);
                    var dependency4 = ResolveDependency<T4>(context);
                    var parent = (T)context.ParentContext.InstanceToValidate;
                    context.Append(validatorFunction(parent, x, dependency1, dependency2, dependency3, dependency4));
                }));
        }

        #endregion

        #region IRuleBuilderOptions Implementation

        public PropertyRule Rule => internalRuleBuilder?.Rule;

        public ResolverRuleBuilder<T, TChild> SetValidator(IPropertyValidator validator)
        {
            return new ResolverRuleBuilder<T, TChild>(ruleBuilder.SetValidator(validator));
        }

        public ResolverRuleBuilder<T, TChild> SetValidator(IValidator<TChild> validator, params string[] ruleSets)
        {
            return new ResolverRuleBuilder<T, TChild>(ruleBuilder.SetValidator(validator, ruleSets));
        }

        public ResolverRuleBuilder<T, TChild> SetValidator<TValidator>(Func<T, TValidator> validatorProvider, params string[] ruleSets) where TValidator : IValidator<TChild>
        {
            return new ResolverRuleBuilder<T, TChild>(ruleBuilder.SetValidator(validatorProvider, ruleSets));
        }

        public ResolverRuleBuilder<T, TChild> Configure(Action<PropertyRule> configurator)
        {
            if (this.Rule != null) configurator(this.Rule);
            return this;
        }

        IRuleBuilderOptions<T, TChild> IRuleBuilder<T, TChild>.SetValidator(IPropertyValidator validator)
            => SetValidator(validator);


        IRuleBuilderOptions<T, TChild> IRuleBuilder<T, TChild>.SetValidator(IValidator<TChild> validator, params string[] ruleSets)
            => SetValidator(validator, ruleSets);

        IRuleBuilderOptions<T, TChild> IRuleBuilder<T, TChild>.SetValidator<TValidator>(Func<T, TValidator> validatorProvider, params string[] ruleSets)
            => SetValidator(validatorProvider, ruleSets);

        IRuleBuilderOptions<T, TChild> IConfigurable<PropertyRule, IRuleBuilderOptions<T, TChild>>.Configure(Action<PropertyRule> configurator)
            => Configure(configurator);

        #endregion
    }
}
