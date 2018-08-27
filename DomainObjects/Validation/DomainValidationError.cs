using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Validation
{
    public class DomainValidationError
    {
        public DomainValidationError(string expressionName, string message, object failedValue, string expressionTitle = null, string userMessage = null, string code = null)
        {
            ExpressionName = expressionName;
            ExpressionTitle = expressionTitle ?? expressionName;
            Message = message;
            UserMessage = userMessage ?? message;
            FailedValue = failedValue;
            Code = code;
        }

        //TODO: Code
        //Entity?
        public string ExpressionName { get; }

        public string ExpressionTitle { get; }

        public string Code { get; }
        public string Message { get; }

        public string UserMessage { get; }

        public object FailedValue { get; }
    }
}
