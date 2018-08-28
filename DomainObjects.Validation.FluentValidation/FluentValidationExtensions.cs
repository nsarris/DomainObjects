using FluentValidation;
using System.Collections.Generic;
using System.Linq;

namespace DomainObjects.Validation.FluentValidation
{
    public static class FluentValidationExtensions
    {
        public static IRuleBuilderOptions<T, IEnumerable<TElement>> MaxCount<T, TElement>(this IRuleBuilder<T, IEnumerable<TElement>> ruleBuilder, int limit)
        {

            return ruleBuilder.Must((rootObject, list, context) =>
            {
                context.MessageFormatter.AppendArgument("MaxCount", limit);
                return list.Count() <= limit;
            })
            .WithMessage("{PropertyName} must contain fewer than {MaxCount} items.");
        }

        public static IRuleBuilderOptions<T, IEnumerable<TElement>> MinCount<T, TElement>(this IRuleBuilder<T, IEnumerable<TElement>> ruleBuilder, int limit)
        {

            return ruleBuilder.Must((rootObject, list, context) =>
            {
                context.MessageFormatter.AppendArgument("MinCount", limit);
                return list.Count() <= limit;
            })
            .WithMessage("{PropertyName} must contain more than {MinCount} items.");
        }

        public static IRuleBuilderOptions<T, IEnumerable<TElement>> Count<T, TElement>(this IRuleBuilder<T, IEnumerable<TElement>> ruleBuilder, int min, int max)
        {

            return ruleBuilder.Must((rootObject, list, context) =>
            {
                context.MessageFormatter
                    .AppendArgument("MinCount", min)
                    .AppendArgument("MaxCount", max);

                return list.Count() >= min && list.Count() <= max;
            })
            .WithMessage("{PropertyName} must contain fewer than {MaxCount} items and more than {MinCount}.");
        }
    }
}
