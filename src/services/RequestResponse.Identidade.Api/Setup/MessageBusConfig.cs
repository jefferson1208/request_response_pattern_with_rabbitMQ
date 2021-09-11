using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RequestResponse.Core.Utils;
using RequestResponse.MessageBus;

namespace RequestResponse.Identidade.Api.Setup
{
    public static class MessageBusConfig
    {
        public static void AdicionarConfiguracaoMessageBus(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionStringMessageQueue("MessageBus");
            services.AdicionarMessageBus(connectionString);
        }
    }
}
