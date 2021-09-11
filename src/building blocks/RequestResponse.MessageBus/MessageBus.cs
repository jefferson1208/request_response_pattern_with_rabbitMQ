using EasyNetQ;
using Polly;
using RabbitMQ.Client.Exceptions;
using RequestResponse.Core.Messages.Integration;
using System;
using System.Threading.Tasks;

namespace RequestResponse.MessageBus
{
    public class MessageBus : IMessageBus
    {
        private IBus _bus;
        private readonly string _connectionString;

        public MessageBus(string connectionString)
        {
            _connectionString = connectionString;
            TentarConectar();
        }
        public bool IsConnected => _bus?.IsConnected ?? false;

        

        public void Publicar<T>(T message) where T : IntegrationEvent
        {
            TentarConectar();
            _bus.Publish(message);
        }

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
        public void Dispose()
        {
            _bus?.Dispose();
        }

        public IDisposable RespondAsync<TRequest, TResponse>(Func<TRequest, Task<TResponse>> responder)
            where TRequest : IntegrationEvent
            where TResponse : ResponseMessage
        {
            throw new NotImplementedException();
        }

        public async Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest request)
            where TRequest : IntegrationEvent
            where TResponse : ResponseMessage
        {
            TentarConectar();
            return await _bus.RequestAsync<TRequest, TResponse>(request);
        }
    }
}
