using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.CQRS.Operation
{

    [DebuggerNonUserCode]
    public static class OperationResponseExtesions
    {
        
        public static IOperationResponse Append(this IOperationResponse resp,
            Func<IOperationResponse,object> func)
        {
            //var resp = new Copy of respnse
            var newResp = new CommandResponse();
            var data = func(newResp);
            newResp.Data = data;
            return newResp;
        }

        public static IOperationResponse Append(this IOperationResponse resp, 
            Func<IOperationResponse,object, object> func)
        {
            //var resp = new Copy of respnse
            var newResp = new CommandResponse();
            var data = func(newResp,resp.Data);
            newResp.Data = data;
            return newResp;
        }

        public static IOperationResponse AppendParallel(this IOperationResponse resp,
            Func<IOperationResponse, object, object> func1,
            Func<IOperationResponse, object, object> func2,
            Func<object,object,object> fCombine)
        {
            //var resp = new Copy of respnse
            var newResp1 = resp;
            var data1= func1(newResp1, resp.Data);
            newResp1.Data = data1;

            var newResp2 = resp;
            var data2 = func2(newResp2, resp.Data);
            newResp2.Data = data2;

            //combine r1+r2
            resp.Errors.AddRange(newResp1.Errors);
            resp.Errors.AddRange(newResp2.Errors);

            resp.Data = fCombine(data1, data2);

            return resp;
        }
    }

    public class OperationBuilder
    {

        public IOperationResponse BuildCommand(Func<IOperationResponse,IOperationResponse> func)
        {
            var cmd = new Command(func);
            //var r= cmd.Execute(null)
            IOperationResponse r = new CommandResponse();
            r.Data = 1;
            var ii = 2;
            r = r.Append((resp) =>
                {
                    return 1;
                })

                .Append((resp, data) =>
                {
                    return ii;
                })

                .AppendParallel(
                (resp, data) =>
                {
                    return (int)data + 2;
                },
                (resp, data) =>
                {
                    return (int)data + 5;
                },
                (r1,r2) =>
                {
                    return (int)r1 + (int)r2;
                });

            return r;
        }
    }
}
