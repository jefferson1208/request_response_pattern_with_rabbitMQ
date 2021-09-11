using Microsoft.Extensions.DependencyInjection;
using System;

namespace RequestResponse.MessageBus
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AdicionarMessageBus(this IServiceCollection services, string connection)
        {
            if (string.IsNullOrEmpty(connection)) throw new ArgumentNullException();

            services.AddSingleton<IMessageBus>(new MessageBus(connection));

            return services;
        }
    }
}
