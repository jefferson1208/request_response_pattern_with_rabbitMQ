# Padrão Request Response com RabbitMQ (EasyNetQ)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

Neste projeto demonstro como trabalhar de forma sincrona com uma fila no RabbitMQ. A intenção é criar um usuário na API de IDENTIDADE e uma vez que o processo obtiver sucesso, um evento de INTEGRAÇÃO será disparado, salvando determinada informação numa fila.
Esta informação será processada pela API de CLIENTE, cadastrando um novo cliente com o MESMO ID de usuário gerado pela API de IDENTIDADE.

Tudo isso ocorrerá no mesmo REQUEST, devolvendo o resultado completo para o usuário da aplicação.

### Pacotes
```bash
Install-Package EasyNetQ
Install-Package Polly
Install-Package FluentValidation
Install-Package MediatR
```
### Registro usuário
```csharp
[HttpPost("cadastrar")]
public async Task <ActionResult> Registrar(RegistroUsuarioViewModel registro) 
{
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

}
```
### Evento integração usuário registrado com sucesso
```csharp
private async Task <ResponseMessage> RegistrarCliente(RegistroUsuarioViewModel registro) 
{

  var usuarioId = Guid.NewGuid(); //mesmo id que foi gerado no cadastro do usuário;
  
   var usuarioRegistrado = new UsuarioRegistradoIntegrationEvent(
      usuarioId, registro.Nome, registro.Email, registro.Cpf);

   try 
   {
      return await _bus.RequestAsync<UsuarioRegistradoIntegrationEvent, ResponseMessage>(usuarioRegistrado);
      
   } catch 
   {
      //Remover Usuário pois houve erro no cadastro do cliente
      throw;
   }
}
```
### Handler
```csharp
public class RegistroClienteIntegrationHandler: BackgroundService 
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
      _bus.RespondAsync <UsuarioRegistradoIntegrationEvent, ResponseMessage> (responder: async request =>
         new ResponseMessage(await RegistrarCliente(request)));

      return Task.CompletedTask;

   }

   private async Task <ValidationResult> RegistrarCliente(UsuarioRegistradoIntegrationEvent request) 
   {
      var clienteCommand = new RegistrarClienteCommand(request.Id, request.Nome, request.Email, request.Cpf);

      ValidationResult result;

      using(var scopo = _serviceProvider.CreateScope()) 
      {
         var mediator = scopo.ServiceProvider.GetRequiredService<IMediatorHandler>();

         result = await mediator.EnviarComando(clienteCommand);
      }

      return result;
   }
}
```
### Registrar Cliente CommandHandler
```csharp
public class ClienteCommandHandler: CommandHandler, IRequestHandler<RegistrarClienteCommand, ValidationResult>  
{
   private readonly IClienteRepository _clienteRepository;

   public ClienteCommandHandler(IClienteRepository clienteRepository) 
   {
      _clienteRepository = clienteRepository;
   }

   public async Task <ValidationResult> Handle(RegistrarClienteCommand message, CancellationToken cancellationToken) 
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
      
      //evento opcional
      //caso deseje disparar um email, sms para o cliente
      cliente.AdicionarEvento(new ClienteRegistradoEvent(message.Id, message.Nome, message.Email, message.Cpf));

      return await SalvarDados(_clienteRepository.UnitOfWork);
   }

}
```
### Conexão com a fila de forma resiliente utilizando Polly
```csharp

private IBus _bus;
private readonly string _connectionString;

public MessageBus(string connectionString)
{
    _connectionString = connectionString;
    TentarConectar();
}
        
public bool IsConnected => _bus?.IsConnected ?? false;

private void TentarConectar()
{
    if (IsConnected) return;

    var politica = Policy.Handle<EasyNetQException>()
                        .Or<BrokerUnreachableException>()
                        .WaitAndRetry(retryCount: 3, sleepDurationProvider: retryAttempt =>
                         TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

    politica.Execute(action: () =>
    {
        _bus = RabbitHutch.CreateBus(_connectionString);
    });
}
```

## Tecnologias
<div style="display: inline_block"><br>
  <img align="center" alt="Jeferson-Netcore" height="30" width="40" src="https://github.com/devicons/devicon/blob/master/icons/dotnetcore/dotnetcore-original.svg">
  <img align="center" alt="Jeferson-Csharp" height="30" width="40" src="https://raw.githubusercontent.com/devicons/devicon/master/icons/csharp/csharp-original.svg">
</div>
