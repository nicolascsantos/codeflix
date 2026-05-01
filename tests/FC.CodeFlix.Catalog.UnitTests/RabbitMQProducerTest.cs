using FC.CodeFlix.Catalog.Domain.Events;
using FC.CodeFlix.Catalog.Infra.Messaging.Configuration;
using FC.CodeFlix.Catalog.Infra.Messaging.Producer;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace FC.CodeFlix.Catalog.UnitTests
{
    public class RabbitMQProducerTest
    {
        [Fact(DisplayName = nameof(SendMessageAsync))]
        public async Task SendMessageAsync()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "adm-videos",
                Password = "123456"
            };

            var channelOptions = new CreateChannelOptions(
                publisherConfirmationsEnabled: true,
                publisherConfirmationTrackingEnabled: true
            );

            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();
            var options = Options.Create(new RabbitMQConfiguration()
            {
                Exchange = "videos.events"
            });
            var producer = new RabbitMQProducer(channel, options);
            var @event = new VideoUploadedEvent(Guid.NewGuid(), "videos/test.mp4");
            await producer.SendMessageAsync(@event, CancellationToken.None);
        }
    }
}
