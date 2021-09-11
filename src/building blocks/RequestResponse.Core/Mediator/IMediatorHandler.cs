using FluentValidation.Results;
using RequestResponse.Core.Messages;
using System.Threading.Tasks;

namespace RequestResponse.Core.Mediator
{
    public interface IMediatorHandler
    {
        Task PublicarEvento<T>(T evento) where T : Event;
        Task<ValidationResult> EnviarComando<T>(T comando) where T : Command;
    }
}
