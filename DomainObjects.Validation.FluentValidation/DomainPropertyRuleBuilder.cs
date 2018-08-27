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
    public class DomainPrimitiveRuleBuilder<T, TProperty>
    {
        readonly IRuleBuilder<T, TProperty> ruleBuilder;
        readonly RuleBuilder<T, TProperty> internalRuleBuilder;
        readonly Func<T, bool> propertyNotNullPredicate;

        public DomainPrimitiveRuleBuilder(IRuleBuilder<T, TProperty> ruleBuilder)
        {
            this.ruleBuilder = ruleBuilder;
            if (ruleBuilder is RuleBuilder<T, TProperty>)
            {
                internalRuleBuilder = (RuleBuilder<T, TProperty>)ruleBuilder;

                if (internalRuleBuilder.Rule is CollectionPropertyRule<TProperty> collectionRule)
                {
                    propertyNotNullPredicate = x => x != null;
                }
                else
                {
                    var propertyExpression = (Expression<Func<T, TProperty>>)internalRuleBuilder.Rule.Expression;

                    propertyNotNullPredicate = Expression.Lambda<Func<T, bool>>(
                    Expression.NotEqual(propertyExpression.Body, ExpressionEx.Constants.Null),
                    propertyExpression.Parameters[0]).Compile();
                }
            }
        }

        private void PrepareExecution<TValidator>(CustomContext context, TProperty instance, bool buildChildContext, out DomainValidationContext<T> parentContext, out TValidator childValidator, out DomainValidationContext<TProperty> clonedChildContext)
            where TValidator : IDomainPrimitiveFluentValidator<TProperty>
        {
            if (context.ParentContext.IsChildCollectionContext && context.ParentContext is ValidationContext validationContext)
            {
                parentContext = (DomainValidationContext<T>)((IValidationContext)validationContext).ParentContext;
                childValidator = parentContext.ResolveChildValidator<TValidator>();
                clonedChildContext = buildChildContext ? new DomainValidationContext<TProperty>(instance, validationContext.PropertyChain, validationContext.Selector, ChildContextType.ChildCollection, childValidator, parentContext) : null;
            }
            else
            {
                parentContext = (DomainValidationContext<T>)context.ParentContext;
                childValidator = parentContext.ResolveChildValidator<TValidator>();
                clonedChildContext = buildChildContext ? parentContext.CloneForPrimitiveValidator(instance, childValidator) : null;
            }
        }

        public IRuleBuilderOptions<T, TProperty> ValidateUsing<TValidator>()
            where TValidator : IDomainPrimitiveFluentValidator<TProperty>
        {
            return (IRuleBuilderOptions<T, TProperty>)ruleBuilder.Custom((x, context) =>
            {
                PrepareExecution<TValidator>(context, x, true, out var _, out var childValidator, out var clonedChildContext);

                var result = childValidator.FluentValidator.Validate(clonedChildContext);
                context.Append(result);
            });
        }

        public IRuleBuilderOptions<T, TProperty> ValidateUsing<TValidator>(Action<T, TProperty, TValidator, DomainValidationContext<TProperty>> customValidator)
            where TValidator : IDomainPrimitiveFluentValidator<TProperty>
        {
            return (IRuleBuilderOptions<T, TProperty>)ruleBuilder.Custom((x, context) =>
            {
                PrepareExecution<TValidator>(context, x, true, out var parentContext, out var childValidator, out var clonedChildContext);

                customValidator(parentContext.InstanceToValidate, x, childValidator, clonedChildContext);
            });
        }

        public IRuleBuilderOptions<T, TProperty> ValidateUsing<TValidator>(Func<T, TProperty, TValidator, ValidationResult> customValidator)
            where TValidator : IDomainPrimitiveFluentValidator<TProperty>
        {
            return (IRuleBuilderOptions<T, TProperty>)ruleBuilder.Custom((x, context) =>
            {
                PrepareExecution<TValidator>(context, x, false, out var parentContext, out var childValidator, out var _);

                var result = customValidator(parentContext.InstanceToValidate, x, childValidator);
                context.Append(result);
            });
        }

        public IRuleBuilderOptions<T, TProperty> ValidateUsing<TValidator>(Func<T, TProperty, TValidator, ValidationFailure> customValidator)
            where TValidator : IDomainPrimitiveFluentValidator<TProperty>
        {
            return (IRuleBuilderOptions<T, TProperty>)ruleBuilder.Custom((x, context) =>
            {
                PrepareExecution<TValidator>(context, x, false, out var parentContext, out var childValidator, out var _);

                var result = customValidator(parentContext.InstanceToValidate, x, childValidator);
                context.AddFailure(result);
            });
        }

        public IRuleBuilderOptions<T, TProperty> ValidateUsing<TValidator>(Func<T, TProperty, TValidator, bool> customValidator)
            where TValidator : IDomainPrimitiveFluentValidator<TProperty>
        {
            return (IRuleBuilderOptions<T, TProperty>)ruleBuilder.Custom((instance, context) =>
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
            });
        }

        public IRuleBuilderOptions<T, TProperty> ValidateUsingCustom<TValidator>(Func<T, TProperty, TValidator, DomainValidationResult> customValidator)
            where TValidator : IDomainValidator<TProperty>
        {
            return (IRuleBuilderOptions<T, TProperty>)ruleBuilder.Custom((x, context) =>
            {
                var parentContext = (DomainValidationContext<T>)context.ParentContext;
                var childValidator = parentContext.ResolveChildValidator<TValidator>();

                var result = customValidator(parentContext.InstanceToValidate, x, childValidator);
                context.Append(result);
            });
        }

        public IRuleBuilderOptions<T, TProperty> ValidateUsingCustom<TValidator>()
            where TValidator : IDomainValidator<TProperty>
        {
            return (IRuleBuilderOptions<T, TProperty>)ruleBuilder.Custom((x, context) =>
            {
                var parentContext = (DomainValidationContext<T>)context.ParentContext;
                var childValidator = parentContext.ResolveChildValidator<TValidator>();
                
                var result = childValidator.Validate(x);
                context.Append(result);
            });
        }
    }
}
