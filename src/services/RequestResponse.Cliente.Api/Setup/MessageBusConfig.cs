using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RequestResponse.Cliente.Api.Services;
using RequestResponse.Core.Utils;
using RequestResponse.MessageBus;

namespace RequestResponse.Cliente.Api.Setup
{
    public static class MessageBusConfig
    {
        public static void AdicionarConfiguracaoMessageBus(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionStringMessageQueue("MessageBus");
            services.AdicionarMessageBus(connectionString)
                        .AddHostedService<RegistroClienteIntegrationHandler>();
        }
    }
}
