using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.CQRS
{
    public interface IResponse
    {
        ICollection<ResponseError> Errors { get; }
        object ResponseData { get; set; }
        object CustomData { get; set; }
        void AddExceptionError(Exception e, ResponseErrorSeverity severity = ResponseErrorSeverity.Error, object traceData = null);
        void AddBusinessError(string code, string message, string source = null, ResponseErrorSeverity severity = ResponseErrorSeverity.Error, object traceData = null);
        void AddValidationError(string code, string message, string source = null, ResponseErrorSeverity severity = ResponseErrorSeverity.Error, object traceData = null);
        void AddSecurityError(string code, string message, string source = null, ResponseErrorSeverity severity = ResponseErrorSeverity.Error, object traceData = null);
        string GetErrorString();
        CallerInfo CallerInfo { get; }
    }
}
