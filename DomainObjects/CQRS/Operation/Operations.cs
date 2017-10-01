using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.CQRS.Operation
{
    public interface IOperation
    {
        IOperationResponse Execute(object parameters);
        IOperationResponse Execute(IOperationResponse resp, object parameters);
    }

    public abstract class CommandA : IOperation
    {
        public abstract IOperationResponse Payload(IOperationResponse resp, object parameters);

        [DebuggerStepThrough]
        public IOperationResponse Execute(object parameters)
        {
            return Execute(null, parameters);
        }

        [DebuggerStepThrough]
        public IOperationResponse Execute(IOperationResponse resp, object parameters)
        {
            if (resp == null)
                resp = new CommandResponse();
            try
            {
                return Payload(resp, parameters);
            }
            catch (Exception e)
            {
                resp.Errors.Add(e.Message);
                return resp;
            }
        }
    }

    public class Command : CommandA
    {
        private class CC
        {
            public CC(Command a)
            {
                
            }
        }
        protected Func<IOperationResponse, IOperationResponse> commandFunc;
        Func<IOperationResponse, object, IOperationResponse> commandFuncWithParameters;
        public Command(Func<IOperationResponse, IOperationResponse> commandFunc)
        {
            this.commandFunc = commandFunc;
        }

        public Command(Func<IOperationResponse,  object,IOperationResponse> commandFunc)
        {
            this.commandFuncWithParameters = commandFunc;
        }

        public override IOperationResponse Payload(IOperationResponse resp, object parameters)
        {
            if (commandFunc != null)
                return commandFunc(resp);
            else
                return commandFuncWithParameters(resp, parameters);
        }
    }

    //public class Query : IOperation
    //{
    //    IQueryResponse Execute() { throw new NotImplementedException(); }
    //    IOperationResponse IOperation.Execute()
    //    {
    //        return Execute();
    //    }
    //}

    //public class CreateNew : IOperation
    //{
    //    ICreateNewResponse Execute() { throw new NotImplementedException(); }
    //    IOperationResponse IOperation.Execute()
    //    {
    //        return Execute();
    //    }
    //}
}
