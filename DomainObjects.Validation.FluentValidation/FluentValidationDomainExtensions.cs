using DomainObjects.Core;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.Validators;
using System;
using System.Collections;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Validation.FluentValidation
{

    public static class FluentValidationDomainExtensions
    {
        public static DomainChildRuleBuilder<T, TChild> AsDomainChild<T, TChild>(this IRuleBuilder<T, TChild> ruleBuilder)
            where T : DomainObject
        {
            return new DomainChildRuleBuilder<T, TChild>(ruleBuilder);
        }

        public static CustomContext Append(this CustomContext validationContext, ValidationResult result)
        {
            foreach (var failure in result.Errors)
                validationContext.AddFailure(failure);
            return validationContext;
        }

        public static CustomContext Append(this CustomContext validationContext, DomainValidationResult result)
        {
            foreach (var error in result.Errors)
                validationContext.AddFailure(new ValidationFailure(error.ExpressionName, error.Message, error.FailedValue));
            return validationContext;
        }

        public static DomainValidationResult ToDomainValidationResult(this ValidationResult result)
        {
            var r = new DomainValidationResult();
            foreach (var item in result.Errors)
            {
                var error = new DomainValidationError
                    (
                        expressionName: item.PropertyName,
                        //expressionTitle: item.
                        message: item.ErrorMessage,
                        failedValue: item.AttemptedValue,
                        code: item.ErrorCode
                    );

                if (item.Severity == Severity.Error)
                    r.AddFailure(error);
                else
                    r.AddNonFailure(error);
            }
            return r;
        }

        internal static IRuleBuilderOptions<T, TProperty> WhenNotNull<T, TProperty>(this IRuleBuilderOptions<T, TProperty> ruleBuilderOptions, Func<T, bool> predicate)
        {
            if (ruleBuilderOptions == null)
                return ruleBuilderOptions;
            else
                return ruleBuilderOptions.When(predicate, ApplyConditionTo.CurrentValidator);
        }
    }
}
