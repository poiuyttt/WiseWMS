using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using WiseWMS.Infrastructure.MessageQueue;

namespace WiseWMS.Application.Publishers
{
    /// <summary>
    /// 发布消息到 RabbitMQ，用于库存变动时通知其他系统
    /// </summary>
    public class MessagePublisher
    {
        private readonly RabbitMqConnection _connection;
        private readonly ILogger<MessagePublisher> _logger;

        public MessagePublisher(RabbitMqConnection connection, ILogger<MessagePublisher> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        /// <summary>
        /// 发布库存变动消息
        /// </summary>
        public virtual async Task PublishInventoryChange(int productId, int quantityAfter, string orderNo)
        {
            try
            {
                await using var channel = _connection.CreateChannel();

                var message = JsonSerializer.Serialize(
                    new
                    {
                        ProductId = productId,
                        QuantityAfter = quantityAfter,
                        OrderNo = orderNo,
                        Timestamp = DateTime.UtcNow,
                    }
                );

                var body = Encoding.UTF8.GetBytes(message);

                await channel.BasicPublishAsync(
                    exchange: "",
                    routingKey: "inventory_sync",
                    mandatory: false,
                    body: body
                );

                _logger.LogInformation(
                    "库存消息已发布：商品={ProductId}, 单号={OrderNo}",
                    productId,
                    orderNo
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "库存消息发布失败：商品={ProductId}, 单号={OrderNo}",
                    productId,
                    orderNo
                );
            }
        }
    }
}
