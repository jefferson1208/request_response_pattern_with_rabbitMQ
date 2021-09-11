using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace RequestResponse.Cliente.Api.Application.Events
{
    public class ClienteEventHandler : INotificationHandler<ClienteRegistradoEvent>
    {
        public Task Handle(ClienteRegistradoEvent notification, CancellationToken cancellationToken)
        {
            // Enviar evento de confirmação (email, sms, etc);
            return Task.CompletedTask;
        }
    }
}
