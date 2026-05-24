using RabbitMQ.Client;

namespace FC.CodeFlix.Catalog.Infra.Messaging.Configuration
{
    public class ChannelManager
    {
        private readonly IConnection _connection;
        private IChannel? _channel = null;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public ChannelManager(IConnection connection)
        {
            _connection = connection;
        }

        public async Task<IChannel> GetChannel()
        {
            await _semaphore.WaitAsync();
            try
            {
                if (_channel is null || _channel.IsClosed)
                {
                    _channel = await _connection.CreateChannelAsync();
                }
                return _channel;
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
