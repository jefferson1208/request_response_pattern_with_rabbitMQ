using FluentValidation.Results;
using RequestResponse.Core.Messages;
using System;
using System.Threading.Tasks;

namespace RequestResponse.Core.Mediator
{
    public class MediatorHandler : IMediatorHandler
    {
        public Task<ValidationResult> EnviarComando<T>(T comando) where T : Command
        {
            throw new NotImplementedException();
        }

        public Task PublicarEvento<T>(T evento) where T : Event
        {
            throw new NotImplementedException();
        }
    }
}
