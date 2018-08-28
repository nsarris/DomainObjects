using FluentValidation;
using FluentValidation.Results;
using FluentValidation.Validators;
using System;

namespace FluentValidation.IoC
{
    public static class FluentValidationExtensions
    {
        public static CustomContext Append(this CustomContext validationContext, ValidationResult result)
        {
            foreach (var failure in result.Errors)
                validationContext.AddFailure(failure);
            return validationContext;
        }

        public static ResolverRuleBuilder<T, TProperty> WithIoC<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return new ResolverRuleBuilder<T, TProperty>(ruleBuilder);
        }
    }
}
