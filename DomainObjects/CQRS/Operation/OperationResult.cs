using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.CQRS.Operation
{
    public interface IOperationResponse
    {
        List<string> Errors { get; }
        object Data { get; set; }
    }

    public interface ICommandResponse : IOperationResponse
    {
    }

    public interface IQueryResponse : IOperationResponse
    {
    }

    public interface ICreateNewResponse : IOperationResponse
    {

    }

    public class CommandResponse : ICommandResponse
    {
        public List<string> Errors { get; private set; } = new List<string>();
        public object Data { get; set; }
        public CommandResponse()
        {

        }
    }
}
