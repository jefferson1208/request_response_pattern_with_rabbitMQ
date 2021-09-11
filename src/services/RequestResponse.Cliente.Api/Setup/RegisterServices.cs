using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RequestResponse.Cliente.Api.Application.Commands;
using RequestResponse.Cliente.Api.Application.Events;
using RequestResponse.Cliente.Api.Data;
using RequestResponse.Cliente.Api.Data.Repository;
using RequestResponse.Cliente.Api.Models;
using RequestResponse.Core.Mediator;

namespace RequestResponse.Cliente.Api.Setup
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<IMediatorHandler, MediatorHandler>();

            services.AddScoped<IRequestHandler<RegistrarClienteCommand, ValidationResult>, ClienteCommandHandler>();
 

            services.AddScoped<INotificationHandler<ClienteRegistradoEvent>, ClienteEventHandler>();

            services.AddScoped<IClienteRepository, ClienteRepository>();
            services.AddScoped<ClienteContext>();
        }
    }
}
