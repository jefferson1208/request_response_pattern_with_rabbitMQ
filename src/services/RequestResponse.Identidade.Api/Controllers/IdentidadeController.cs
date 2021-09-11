using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RequestResponse.Core.Messages.Integration;
using RequestResponse.Identidade.Api.ViewModels;
using RequestResponse.MessageBus;
using System;
using System.Threading.Tasks;

namespace RequestResponse.Identidade.Api.Controllers
{
    [Route("api/identidade")]
    public class IdentidadeController : MainController
    {
        private readonly IMessageBus _bus;
        public IdentidadeController(IMessageBus bus)
        {
            _bus = bus;
        }

        [HttpPost("cadastrar")]
        public async Task<ActionResult> Registrar(RegistroUsuarioViewModel registro)
        {
            if (!ModelState.IsValid) return CustomizarResposta(ModelState);

            var result = RegistrarUsuario();

            if (result)
            {
                var clienteResult = await RegistrarCliente(registro);

                if (!clienteResult.ValidationResult.IsValid)
                {
                    //Remover Usuário
                    return CustomizarResposta(clienteResult.ValidationResult);
                }

                return CustomizarResposta(registro);
            }

            AdicionarErroProcessamento("Usuário não cadastrado!");

            return CustomizarResposta();
        }

        private async Task<ResponseMessage> RegistrarCliente(RegistroUsuarioViewModel registro)
        {

            var usuarioRegistrado = new UsuarioRegistradoIntegrationEvent(
                Guid.NewGuid(), registro.Nome, registro.Email, registro.Cpf);

            try
            {
                return await _bus.RequestAsync<UsuarioRegistradoIntegrationEvent, ResponseMessage>(usuarioRegistrado);
            }
            catch
            {
                //Remover Usuário
                throw;
            }
        }

        private bool RegistrarUsuario()
        {
            var random = new Random().Next(0, 10);

            var div = random % 2 == 0 ? true : false;

            return div;
        }

    }
}
