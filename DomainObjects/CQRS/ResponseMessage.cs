using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.CQRS
{
    public enum ResponseErrorType
    {
        Exception,
        Business,
        Security,
        Validation
    }

    public enum ResponseErrorSeverity
    {
        Error,
        Warning,
        Message,
        TraceInfo
    }

    public class ResponseError
    {
        public ResponseErrorType Type { get; set; }
        public ResponseErrorSeverity Severity { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public Exception Exception { get; set; }
        public string Trace { get; set; }
        public DateTime TimeStamp { get; set; }
        //public bool IsEnumError(object error) { return ADAPT3.Extensions.EnumHelper.GetItem(error).FullName == Code; }
        public object TraceData { get; set; }
        public IResponse OriginatingResponse { get; set; }
        public ResponseError()
        {
            TimeStamp = DateTime.Now;
        }
    }
}
