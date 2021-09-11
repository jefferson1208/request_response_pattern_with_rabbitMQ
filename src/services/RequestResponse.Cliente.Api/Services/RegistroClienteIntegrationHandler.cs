using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RequestResponse.Cliente.Api.Application.Commands;
using RequestResponse.Core.Mediator;
using RequestResponse.Core.Messages.Integration;
using RequestResponse.MessageBus;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RequestResponse.Cliente.Api.Services
{
    public class RegistroClienteIntegrationHandler : BackgroundService
    {
        private readonly IMessageBus _bus;
        private readonly IServiceProvider _serviceProvider;
        public RegistroClienteIntegrationHandler(
                            IServiceProvider serviceProvider,
                            IMessageBus bus)
        {
            _serviceProvider = serviceProvider;
            _bus = bus;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _bus.RespondAsync<UsuarioRegistradoIntegrationEvent, ResponseMessage>(responder: async request =>
                                new ResponseMessage(await RegistrarCliente(request)));

            return Task.CompletedTask;

        }

        private async Task<ValidationResult> RegistrarCliente(UsuarioRegistradoIntegrationEvent request)
        {
            var clienteCommand = new RegistrarClienteCommand(request.Id, request.Nome, request.Email, request.Cpf);

            ValidationResult result;

            using (var scopo = _serviceProvider.CreateScope())
            {
                var mediator = scopo.ServiceProvider.GetRequiredService<IMediatorHandler>();

                result = await mediator.EnviarComando(clienteCommand);
            }

            return result;
        }
    }
}
