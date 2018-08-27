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
    public class DomainObjectRuleBuilder<T, TChild>
        where T : DomainObject
        where TChild : DomainObject
    {
        readonly IRuleBuilder<T, TChild> ruleBuilder;
        readonly RuleBuilder<T, TChild> internalRuleBuilder;
        readonly Func<T, bool> propertyNotNullPredicate;

        public DomainObjectRuleBuilder(IRuleBuilder<T, TChild> ruleBuilder)
        {
            this.ruleBuilder = ruleBuilder;
            if (ruleBuilder is RuleBuilder<T, TChild>)
            {
                internalRuleBuilder = (RuleBuilder<T, TChild>)ruleBuilder;

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
            where TValidator : IDomainObjectFluentValidator<TChild>
        {
            if (context.ParentContext.IsChildCollectionContext && context.ParentContext is ValidationContext validationContext)
            {
                parentContext = (DomainValidationContext<T>)((IValidationContext)validationContext).ParentContext;
                childValidator = parentContext.ResolveChildValidator<TValidator>();
                clonedChildContext = buildChildContext ? new DomainValidationContext<TChild>(instance, validationContext.PropertyChain, validationContext.Selector, ChildContextType.ChildCollection, childValidator, parentContext) : null;
            }
            else
            {
                parentContext = (DomainValidationContext<T>)context.ParentContext;
                childValidator = parentContext.ResolveChildValidator<TValidator>();
                clonedChildContext = buildChildContext ? parentContext.CloneForChildValidator(instance, childValidator) : null;
            }
        }

        public IRuleBuilderOptions<T, TChild> ValidateUsing<TValidator>()
            where TValidator : IDomainObjectFluentValidator<TChild>
        {
            return ((IRuleBuilderOptions<T, TChild>)ruleBuilder.Custom((x, context) =>
            {
                PrepareExecution<TValidator>(context, x, true, out var _, out var childValidator, out var clonedChildContext);

                var result = childValidator.FluentValidator.Validate(clonedChildContext);
                context.Append(result);
            })).WhenNotNull(propertyNotNullPredicate);
        }

        public IRuleBuilderOptions<T, TChild> ValidateUsing<TValidator>(Action<T, TChild, TValidator, DomainValidationContext<TChild>> customValidator)
            where TValidator : IDomainObjectFluentValidator<TChild>
        {
            return ((IRuleBuilderOptions<T, TChild>)ruleBuilder.Custom((x, context) =>
            {
                PrepareExecution<TValidator>(context, x, true, out var parentContext, out var childValidator, out var clonedChildContext);

                customValidator(parentContext.InstanceToValidate, x, childValidator, clonedChildContext);
            })).WhenNotNull(propertyNotNullPredicate);
        }

        public IRuleBuilderOptions<T, TChild> ValidateUsing<TValidator>(Func<T, TChild, TValidator, ValidationResult> customValidator)
            where TValidator : IDomainObjectFluentValidator<TChild>
        {
            return ((IRuleBuilderOptions<T, TChild>)ruleBuilder.Custom((x, context) =>
            {
                PrepareExecution<TValidator>(context, x, false, out var parentContext, out var childValidator, out var _);

                var result = customValidator(parentContext.InstanceToValidate, x, childValidator);
                context.Append(result);
            })).WhenNotNull(propertyNotNullPredicate);
        }

        public IRuleBuilderOptions<T, TChild> ValidateUsing<TValidator>(Func<T, TChild, TValidator, ValidationFailure> customValidator)
            where TValidator : IDomainObjectFluentValidator<TChild>
        {
            return ((IRuleBuilderOptions<T, TChild>)ruleBuilder.Custom((x, context) =>
            {
                PrepareExecution<TValidator>(context, x, false, out var parentContext, out var childValidator, out var _);

                var result = customValidator(parentContext.InstanceToValidate, x, childValidator);
                context.AddFailure(result);
            })).WhenNotNull(propertyNotNullPredicate);
        }

        public IRuleBuilderOptions<T, TChild> ValidateUsing<TValidator>(Func<T, TChild, TValidator, bool> customValidator)
            where TValidator : IDomainObjectFluentValidator<TChild>
        {
            return ((IRuleBuilderOptions<T, TChild>)ruleBuilder.Custom((instance, context) =>
            {
                PrepareExecution<TValidator>(context, instance, false, out var parentContext, out var childValidator, out var _);

                var result = customValidator(parentContext.InstanceToValidate, instance, childValidator);
                var errorMessageSource = context.Rule.Validators.Select(x => x?.Options?.ErrorMessageSource).FirstOrDefault(x => x != null);
                //var errorMessageCode = context.Rule.Validators.Select(x => x?.Options?.ErrorCodeSource).FirstOrDefault(x => x != null);
                if (!result)
                {
                    var message = (errorMessageSource != null ?
                    context.MessageFormatter.BuildMessage(errorMessageSource.GetString(context)) :
                    "");
                    context.AddFailure(new ValidationFailure(context.PropertyName, message, instance));
                }
            })).WhenNotNull(propertyNotNullPredicate);
        }
    }
}
