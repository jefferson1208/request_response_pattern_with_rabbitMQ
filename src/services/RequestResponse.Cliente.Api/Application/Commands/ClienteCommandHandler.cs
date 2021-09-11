using FluentValidation.Results;
using MediatR;
using RequestResponse.Cliente.Api.Application.Events;
using RequestResponse.Cliente.Api.Models;
using RequestResponse.Core.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace RequestResponse.Cliente.Api.Application.Commands
{
    public class ClienteCommandHandler : CommandHandler, IRequestHandler<RegistrarClienteCommand, ValidationResult>,
    {
        private readonly IClienteRepository _clienteRepository;

        public ClienteCommandHandler(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public async Task<ValidationResult> Handle(RegistrarClienteCommand message, CancellationToken cancellationToken)
        {
            if (!message.Validar()) return message.ValidationResult;

            var cliente = new Clientes(message.Id, message.Nome, message.Email, message.Cpf);

            var clienteExistente = await _clienteRepository.BuscarPorCpf(cliente.Cpf.Numero);

            if (clienteExistente != null)
            {
                AdicionarErro("Este CPF já está em uso.");
                return ValidationResult;
            }

            _clienteRepository.Adicionar(cliente);

            cliente.AdicionarEvento(new ClienteRegistradoEvent(message.Id, message.Nome, message.Email, message.Cpf));

            return await SalvarDados(_clienteRepository.UnitOfWork);
        }

    }
}
