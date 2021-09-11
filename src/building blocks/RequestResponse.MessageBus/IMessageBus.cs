using RequestResponse.Core.Messages.Integration;
using System;
using System.Threading.Tasks;

namespace RequestResponse.MessageBus
{
    public interface IMessageBus : IDisposable
    {
        bool IsConnected { get; }
        void Publicar<T>(T message) where T : IntegrationEvent;

        IDisposable RespondAsync<TRequest, TResponse>(Func<TRequest, Task<TResponse>> responder)
            where TRequest : IntegrationEvent
            where TResponse : ResponseMessage;

        Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest request)
            where TRequest : IntegrationEvent
            where TResponse : ResponseMessage;
    }
}
