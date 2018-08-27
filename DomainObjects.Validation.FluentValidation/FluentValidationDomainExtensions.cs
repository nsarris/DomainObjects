using DomainObjects.Core;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.Validators;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Validation.FluentValidation
{

    public static class FluentValidationDomainExtensions
    {
        public static DomainObjectRuleBuilder<T, TChild> AsDomainChild<T, TChild>(this IRuleBuilder<T, TChild> ruleBuilder)
            where T : DomainObject
            where TChild : DomainObject
        {
            return new DomainObjectRuleBuilder<T, TChild>(ruleBuilder);
        }

        public static DomainPrimitiveRuleBuilder<T, TProperty> AsDomainProperty<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
            where T : DomainObject
        {
            //Assert supported type
            return new DomainPrimitiveRuleBuilder<T, TProperty>(ruleBuilder);
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

        internal static IRuleBuilderOptions<T, TProperty> WhenNotNull<T,TProperty>(this IRuleBuilderOptions<T, TProperty> ruleBuilderOptions, Func<T,bool> predicate)
        {
            if (ruleBuilderOptions == null)
                return ruleBuilderOptions;
            else
                return ruleBuilderOptions.When(predicate, ApplyConditionTo.CurrentValidator);
        }
    }
}
