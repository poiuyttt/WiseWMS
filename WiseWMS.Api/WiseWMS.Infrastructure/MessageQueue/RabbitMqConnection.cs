using RabbitMQ.Client;

namespace WiseWMS.Infrastructure.MessageQueue
{
    /// <summary>
    /// 管理 RabbitMQ 连接，整个应用只维护一个连接
    /// </summary>
    public class RabbitMqConnection : IAsyncDisposable
    {
        private readonly IConnection _connection;

        public RabbitMqConnection(string hostName)
        {
            var factory = new ConnectionFactory() { HostName = hostName };
            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// 创建信道（每次发消息创建一个，用完关闭）
        /// </summary>
        public IChannel CreateChannel()
        {
            var channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
            channel
                .QueueDeclareAsync(
                    queue: "inventory_sync",
                    durable: true,
                    exclusive: false,
                    autoDelete: false
                )
                .GetAwaiter()
                .GetResult();
            return channel;
        }

        public async ValueTask DisposeAsync()
        {
            await _connection.DisposeAsync();
        }
    }
}
