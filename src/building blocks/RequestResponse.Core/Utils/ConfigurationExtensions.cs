using Microsoft.Extensions.Configuration;

namespace RequestResponse.Core.Utils
{
    public static class ConfigurationExtensions
    {
        public static string GetConnectionStringMessageQueue(this IConfiguration configuration, string name)
        {
            return configuration?.GetSection("MessageQueueConnection")?[name];
        }
    }
}
