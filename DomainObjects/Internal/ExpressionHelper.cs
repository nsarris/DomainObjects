using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Internal
{
    internal static class ExpressionHelper
    {
        public static List<PropertyInfo> GetSelectedProperties<Tin, Tout>(Expression<Func<Tin, Tout>> expression)
           where Tin : class
        {
            var properties = new List<PropertyInfo>();

            if (expression.Body is MemberExpression memberExpression)
            {
                if (memberExpression.Member is PropertyInfo propertyInfo)
                    properties.Add(propertyInfo);
                else
                    throw new InvalidOperationException($"Member {memberExpression.Member.Name} is not a property");

                return properties;
            }
            else if ((expression.Body is NewExpression body))
            {
                foreach (var argument in body.Arguments)
                {
                    if (argument is MemberExpression argumentExpression)
                    {
                        if (argumentExpression.Member is PropertyInfo propertyInfo)
                            properties.Add(propertyInfo);
                        else
                            throw new InvalidOperationException($"Member {argumentExpression.Member.Name} is not a property");
                    }
                    else
                        throw new InvalidOperationException($"Invalid key selector expression. Please only selected the required properties");
                }

                return properties.Distinct().ToList();
            }
            else
                throw new ArgumentException("Expression must be property selector or a new anonymous initialiazer");
        }
    }
}
