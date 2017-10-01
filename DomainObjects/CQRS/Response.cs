using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.CQRS
{
    public class ServiceResponse : IResponse
    //where T : new()
    {
        #region Fields and Properties

        public CallerInfo CallerInfo { get; protected set; }
        public List<ResponseError> Errors { get; protected set; }
        public ExecutionTimer ExecutionTimer { get; private set; }
        public Dictionary<string, ExecutionTimer.Elapsed> ExecutionTimes { get; private set; }
        public object ResponseData { get; set; }
        public object CustomData { get; set; }

        #endregion Fields and Properties

        #region Ctor
        public ServiceResponse(CallerInfo CallerInfo = null)
        {
            this.CallerInfo = CallerInfo;
            if (this.CallerInfo == null)
            {
                var frame = new StackFrame(1, true);
                if (frame != null)
                {
                    var callerMethod = frame.GetMethod();
                    this.CallerInfo = new CallerInfo(callerMethod.ReflectedType.Name, callerMethod.Name, frame.GetFileName(), frame.GetFileLineNumber());
                }
                else
                    this.CallerInfo = new CallerInfo();
            }
            else if (string.IsNullOrEmpty(this.CallerInfo.Caller))
            {
                var i = 1;
                while ((string.IsNullOrEmpty(this.CallerInfo.Caller)
                    || this.CallerInfo.Caller == this.GetType().Name
                    || this.CallerInfo.Caller == typeof(ServiceResponse).Name
                    //|| this.CallerInfo.Caller == typeof(ServiceResponse<>).Name
                    //|| this.CallerInfo.Caller == typeof(NonQueryServiceResponse).Name
                    )
                    && i < 5)
                {
                    var frame = new StackFrame(i++, true);
                    if (frame != null)
                    {
                        this.CallerInfo.Caller = frame.GetMethod().ReflectedType.Name;
                    }
                    else
                        break;
                }
            }

            Errors = new List<ResponseError>();
            ExecutionTimes = new Dictionary<string, ExecutionTimer.Elapsed>();
            ExecutionTimer = new ExecutionTimer();
            ExecutionTimer.TimerStarted += ExecutionTimer_TimerStarted;
            ExecutionTimer.TimerStopped += ExecutionTimer_TimerStopped;
        }

        //public ServiceResponse(T resp, CallerInfo CallerInfo = null)
        //    : this(CallerInfo)
        //{
        //    ResponseData = resp;
        //}

        #endregion Ctor

        #region ExecutionTimer events

        void ExecutionTimer_TimerStopped(object sender, ExecutionTimer.TimerStateEventArgs e)
        {
            ExecutionTimes[e.Tag] = e.Elapsed;
        }

        void ExecutionTimer_TimerStarted(object sender, ExecutionTimer.TimerStateEventArgs e)
        {
            //TODO: scrap this, use reset event to reset results
            //ExecutionTimes[e.Tag] = e.Elapsed;
        }

        #endregion ExecutionTimer events
        
        #region Add Errors

        public void AddExceptionError(Exception e, ResponseErrorSeverity severity = ResponseErrorSeverity.Error, object traceData = null)
        {
            Errors.Add(new ResponseError()
            {
                Type = ResponseErrorType.Exception,
                Exception = e,
                Source = e.Source,
                Trace = e.StackTrace,
                Message = e.Message,
                Code = e.GetType().Name,
                Severity = severity,
                TraceData = traceData,
                OriginatingResponse = this
            });

        }

        public void AddBusinessError(string code, string message, string source = null, ResponseErrorSeverity severity = ResponseErrorSeverity.Error, object traceData = null)
        {
            Errors.Add(new ResponseError()
            {
                Type = ResponseErrorType.Business,
                Source = source ?? GetCurrentMethod(),
                Trace = (new StackTrace()).ToString().Split(new string[] { "\r\n" }, StringSplitOptions.None)[1],
                Code = code,
                Message = message,
                Severity = severity,
                TraceData = traceData,
                OriginatingResponse = this
            });
        }

        public void AddBusinessError(object error, string source = null, ResponseErrorSeverity severity = ResponseErrorSeverity.Error, object[] descriptionParameters = null, object traceData = null)
        {
            if (error != null && error.GetType().IsEnum)
            {
                //var d = ADAPT3.Extensions.EnumHelper.GetItem(error, descriptionParameters);
                //AddBusinessError(d.FullName, d.Description, source, severity, traceData);
            }
            else
                AddBusinessError("", (error ?? "").ToString(), source, severity, traceData);

        }

        public void AddValidationError(string code, string message, string source = null, ResponseErrorSeverity severity = ResponseErrorSeverity.Error, object traceData = null)
        {
            Errors.Add(new ResponseError()
            {
                Type = ResponseErrorType.Validation,
                Source = source ?? GetCurrentMethod(),
                Trace = (new StackTrace()).ToString().Split(new string[] { "\r\n" }, StringSplitOptions.None)[1],
                Code = code,
                Message = message,
                Severity = severity,
                TraceData = traceData,
                OriginatingResponse = this
            });
        }

        public void AddValidationError(object error, string source = null, ResponseErrorSeverity severity = ResponseErrorSeverity.Error, object[] descriptionParameters = null, object traceData = null)
        {
            if (error != null && error.GetType().IsEnum)
            {
                //var d = ADAPT3.Extensions.EnumHelper.GetItem(error, descriptionParameters);
                //AddValidationError(d.FullName, d.Description, source, severity, traceData);
            }
            else
                AddValidationError("", (error ?? "").ToString(), source, severity, traceData);

        }

        public void AddSecurityError(string code, string message, string source = null, ResponseErrorSeverity severity = ResponseErrorSeverity.Error, object traceData = null)
        {
            Errors.Add(new ResponseError()
            {
                Type = ResponseErrorType.Security,
                Source = source ?? GetCurrentMethod(),
                Trace = (new StackTrace()).ToString().Split(new string[] { "\r\n" }, StringSplitOptions.None)[1],
                Code = code,
                Message = message,
                Severity = severity,
                TraceData = traceData,
                OriginatingResponse = this
            });
        }

        #endregion Add Errors


        //object IResponse.ResponseData
        //{
        //    get
        //    {
        //        return ResponseData;
        //    }
        //    set
        //    {
        //        if (typeof(T).IsAssignableFrom(value.GetType()))
        //            ResponseData = (T)value;
        //    }
        //}

        #region Response strings

        public string GetResponseString(ResponseErrorSeverity severity = ResponseErrorSeverity.Error)
        {
            return string.Join(Environment.NewLine + Environment.NewLine,
                           Errors
                           .Where(x => x.Severity == severity)
                           .Select(x =>
                           {
                               var r = (string.IsNullOrEmpty(x.Code) ? "" : x.Code + " - ") + (x.Message ?? "");
                               if (x.Exception != null)
                               {
                                   var exc = x.Exception.InnerException;
                                   while (exc != null)
                                   {
                                       r += Environment.NewLine + Environment.NewLine + "Inner Exception : " + exc.Message;
                                       exc = exc.InnerException;
                                   }

                               }
                               return r;
                           }));
        }

        public string GetErrorString()
        {
            return GetResponseString(ResponseErrorSeverity.Error);
        }

        public string GetWarningString()
        {
            return GetResponseString(ResponseErrorSeverity.Warning);
        }

        #endregion Response strings

        #region Append Responses
        public IResponse Append(IResponse resp)
        {
            if (resp == null || resp.Errors == null) return this;
            this.Errors.AddRange(resp.Errors);
            return this;
            //TODO: Append Times, need to share stopwatches?
        }

        //public ServiceResponse<T> AppendResponse(ServiceResponse<T> resp)
        //{
        //    Append(resp);
        //    this.ResponseData = resp.ResponseData;
        //    this.CustomData = resp.CustomData;
        //    return this;
        //}

        //public ServiceResponse<T> AppendSingle(IServiceResponse resp, ServiceResponseError error)
        //{
        //    if (resp.Errors.Count > 0)
        //    {
        //        error.Message = error.Message + Environment.NewLine + Environment.NewLine + resp.GetErrorString();
        //        this.Errors.Add(error);
        //    }
        //    return this;
        //}

        public IResponse AppendWithMaxSeverity(IResponse resp, ResponseErrorSeverity severity)
        {
            foreach (var err in resp.Errors)
            {
                if (err.Severity > severity) err.Severity = severity;
                this.Errors.Add(err);
            }
            return this;
        }

        #endregion Append Responses


        public bool HasErrors
        {
            get
            {
                return this.Errors.Any(x => x.Severity == ResponseErrorSeverity.Error);
            }
        }

        public bool HasWarnings
        {
            get
            {
                return this.Errors.Any(x => x.Severity == ResponseErrorSeverity.Warning);
            }
        }

        ICollection<ResponseError> IResponse.Errors => throw new NotImplementedException();

        //public ServiceResponse<TResult> Transform<TResult>(Func<T, TResult> transformFunc = null)
        //{
        //    var r = new ServiceResponse<TResult>(this.CallerInfo)
        //    {
        //        CustomData = this.CustomData,
        //    };

        //    if (transformFunc != null && this.ResponseData != null)
        //        r.ResponseData = transformFunc(this.ResponseData);

        //    r.Append(this);
        //    return r;
        //}


        [MethodImpl(MethodImplOptions.NoInlining)]
        private string GetCurrentMethod()
        {
            //var res = string.Empty;
            //System.Reflection.MethodBase method = null;

            //try
            //{
            //    method = System.Reflection.MethodBase.GetCurrentMethod();
            //}
            //catch
            //{
            //    try
            //    {
            //        var st = new StackTrace();
            //        StackFrame sf;
            //        if (st.FrameCount > 1)
            //            sf = st.GetFrame(1);
            //        else
            //            sf = st.GetFrame(0);

            //        method = sf.GetMethod();
            //    }
            //    catch (Exception e)
            //    {
            //        res = "Failed to get method info: " + e.Message;
            //    }
            //}

            //if (method != null && string.IsNullOrEmpty(res))
            //{
            //    res = method.DeclaringType.Assembly.GetName().Name + "." + method.Name;
            //}

            //return res;
            //return callerName;
            return CallerInfo.Caller;
        }



    }
}
