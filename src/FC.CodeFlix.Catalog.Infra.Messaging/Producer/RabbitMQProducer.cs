using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Infra.Messaging.Configuration;
using FC.CodeFlix.Catalog.Infra.Messaging.JsonPolicies;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text.Json;

namespace FC.CodeFlix.Catalog.Infra.Messaging.Producer
{
    public class RabbitMQProducer : IMessageProducer
    {
        private readonly IChannel _channel;
        private readonly string _exchange;

        public RabbitMQProducer(IChannel channel, IOptions<RabbitMQConfiguration> options)
        {
            _channel = channel;
            _exchange = options.Value.Exchange!;
        }

        public async Task SendMessageAsync<T>(T message, CancellationToken cancellationToken)
        {
            var routingKey = EventsMapping.GetRoutingKey<T>();
            var jsonOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = new JsonSnakeCasePolicy()
            };
            var @event = JsonSerializer.SerializeToUtf8Bytes(message, jsonOptions);
            await _channel.BasicPublishAsync(
                exchange: _exchange,
                routingKey: routingKey,
                body: @event,
                cancellationToken: cancellationToken
            );
        }
    }
}
