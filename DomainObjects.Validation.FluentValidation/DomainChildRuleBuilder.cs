using DomainObjects.Core;
using Dynamix.Expressions;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;
using FluentValidation.Validators;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace DomainObjects.Validation.FluentValidation
{
    public class DomainChildRuleBuilder<T, TChild>
    {
        readonly IRuleBuilder<T, TChild> ruleBuilder;
        readonly RuleBuilder<T, TChild> internalRuleBuilder;
        readonly Func<T, bool> propertyNotNullPredicate;

        public DomainChildRuleBuilder(IRuleBuilder<T, TChild> ruleBuilder)
        {
            this.ruleBuilder = ruleBuilder;

            internalRuleBuilder = ruleBuilder as RuleBuilder<T, TChild>;

            if (internalRuleBuilder != null)
            { 
                if (internalRuleBuilder.Rule is CollectionPropertyRule<TChild> collectionRule)
                {
                    propertyNotNullPredicate = x => x != null;
                }
                else
                {
                    var propertyExpression = (Expression<Func<T, TChild>>)internalRuleBuilder.Rule.Expression;

                    propertyNotNullPredicate = Expression.Lambda<Func<T, bool>>(
                    Expression.NotEqual(propertyExpression.Body, ExpressionEx.Constants.Null),
                    propertyExpression.Parameters[0]).Compile();
                }
            }
        }

        private void PrepareExecution<TValidator>(CustomContext context, TChild instance, bool buildChildContext, out DomainValidationContext<T> parentContext, out TValidator childValidator, out DomainValidationContext<TChild> clonedChildContext)
            where TValidator : IDomainFluentValidator<TChild>
        {
            if (context.ParentContext.IsChildCollectionContext)
            {
                parentContext = (DomainValidationContext<T>)((IValidationContext)context.ParentContext).ParentContext;
                childValidator = parentContext.ResolveChildValidator<TValidator>();
                clonedChildContext = buildChildContext ? new DomainValidationContext<TChild>(instance, context.ParentContext.PropertyChain, context.ParentContext.Selector, ChildContextType.ChildCollection, childValidator) : null;
            }
            else
            {
                parentContext = (DomainValidationContext<T>)context.ParentContext;
                childValidator = parentContext.ResolveChildValidator<TValidator>();
                clonedChildContext = buildChildContext ? parentContext.CloneForChildValidator(instance, childValidator) : null;
            }
        }

        public IRuleBuilderOptions<T, TChild> ValidateUsing<TValidator>()
            where TValidator : IDomainFluentValidator<TChild>
        {
            return ((IRuleBuilderOptions<T, TChild>)ruleBuilder.Custom((x, context) =>
            {
                PrepareExecution<TValidator>(context, x, true, out var _, out var childValidator, out var clonedChildContext);

                var result = childValidator.FluentValidator.Validate(clonedChildContext);
                context.Append(result);
            })).WhenNotNull(propertyNotNullPredicate);
        }

        public IRuleBuilderOptions<T, TChild> ValidateUsing<TValidator>(Action<T, TChild, TValidator, DomainValidationContext<TChild>> customValidator)
            where TValidator : IDomainFluentValidator<TChild>
        {
            return ((IRuleBuilderOptions<T, TChild>)ruleBuilder.Custom((x, context) =>
            {
                PrepareExecution<TValidator>(context, x, true, out var parentContext, out var childValidator, out var clonedChildContext);

                customValidator(parentContext.InstanceToValidate, x, childValidator, clonedChildContext);
            })).WhenNotNull(propertyNotNullPredicate);
        }

        public IRuleBuilderOptions<T, TChild> ValidateUsing<TValidator>(Func<T, TChild, TValidator, ValidationResult> customValidator)
            where TValidator : IDomainFluentValidator<TChild>
        {
            return ((IRuleBuilderOptions<T, TChild>)ruleBuilder.Custom((x, context) =>
            {
                PrepareExecution<TValidator>(context, x, false, out var parentContext, out var childValidator, out var _);

                var result = customValidator(parentContext.InstanceToValidate, x, childValidator);
                context.Append(result);
            })).WhenNotNull(propertyNotNullPredicate);
        }

        public IRuleBuilderOptions<T, TChild> ValidateUsing<TValidator>(Func<T, TChild, TValidator, ValidationFailure> customValidator)
            where TValidator : IDomainFluentValidator<TChild>
        {
            return ((IRuleBuilderOptions<T, TChild>)ruleBuilder.Custom((x, context) =>
            {
                PrepareExecution<TValidator>(context, x, false, out var parentContext, out var childValidator, out var _);

                var result = customValidator(parentContext.InstanceToValidate, x, childValidator);
                context.AddFailure(result);
            })).WhenNotNull(propertyNotNullPredicate);
        }

        public IRuleBuilderOptions<T, TChild> ValidateUsing<TValidator>(Func<T, TChild, TValidator, bool> customValidator)
            where TValidator : IDomainFluentValidator<TChild>
        {
            return ((IRuleBuilderOptions<T, TChild>)ruleBuilder.Custom((instance, context) =>
            {
                PrepareExecution<TValidator>(context, instance, false, out var parentContext, out var childValidator, out var _);

                var result = customValidator(parentContext.InstanceToValidate, instance, childValidator);
                var errorMessageSource = context.Rule.Validators.Select(x => x?.Options?.ErrorMessageSource).FirstOrDefault(x => x != null);
                if (!result)
                {
                    var message = (errorMessageSource != null ?
                    context.MessageFormatter.BuildMessage(errorMessageSource.GetString(context)) :
                    "");
                    context.AddFailure(new ValidationFailure(context.PropertyName, message, instance));
                }
            })).WhenNotNull(propertyNotNullPredicate);
        }

        public IRuleBuilderOptions<T, TChild> ValidateUsingCustom<TValidator>(Func<T, TChild, TValidator, DomainValidationResult> customValidator)
            where TValidator : IDomainValidator<TChild>
        {
            return (IRuleBuilderOptions<T, TChild>)ruleBuilder.Custom((x, context) =>
            {
                var parentContext = (DomainValidationContext<T>)context.ParentContext;
                var childValidator = parentContext.ResolveChildValidator<TValidator>();

                var result = customValidator(parentContext.InstanceToValidate, x, childValidator);
                context.Append(result);
            });
        }

        public IRuleBuilderOptions<T, TChild> ValidateUsingCustom<TValidator>()
            where TValidator : IDomainValidator<TChild>
        {
            return (IRuleBuilderOptions<T, TChild>)ruleBuilder.Custom((x, context) =>
            {
                var parentContext = (DomainValidationContext<T>)context.ParentContext;
                var childValidator = parentContext.ResolveChildValidator<TValidator>();

                var result = childValidator.Validate(x);
                context.Append(result);
            });
        }
    }
}
