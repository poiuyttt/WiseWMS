using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace WiseWMS.Api.BackgroundServices;

/// <summary>
/// 后台消费者：监听 inventory_sync 队列，收到库存变动后清理缓存
/// </summary>
public class InventorySyncConsumer : BackgroundService
{
    private readonly ILogger<InventorySyncConsumer> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public InventorySyncConsumer(ILogger<InventorySyncConsumer> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("库存同步消费者已启动");

        var factory = new ConnectionFactory { HostName = "localhost" };
        await using var connection = await factory.CreateConnectionAsync(stoppingToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.QueueDeclareAsync(
            queue: "inventory_sync",
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: stoppingToken
        );

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);

                _logger.LogInformation("收到库存消息：{Message}", json);

                // 解析消息
                var msg = JsonSerializer.Deserialize<InventoryMessage>(json);
                if (msg == null) return;

                // 1. 记录操作日志（通过 ScopeFactory 获取 Scoped 服务）
                using var scope = _scopeFactory.CreateScope();
                var cache = scope.ServiceProvider.GetRequiredService<IDistributedCache>();

                _logger.LogInformation(
                    "库存变动：商品ID={ProductId}, 变动后库存={Stock}, 单号={OrderNo}",
                    msg.ProductId, msg.QuantityAfter, msg.OrderNo
                );

                // 2. 清除相关缓存
                await cache.RemoveAsync("products_all");
                await cache.RemoveAsync("dashboard_stats");
                _logger.LogInformation("已清除相关缓存");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理库存消息时出错");
            }
        };

        await channel.BasicConsumeAsync(
            queue: "inventory_sync",
            autoAck: true,
            consumer: consumer,
            cancellationToken: stoppingToken
        );

        // 保持服务运行直到程序关闭
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    /// <summary>
    /// 库存变动消息结构
    /// </summary>
    private class InventoryMessage
    {
        public int ProductId { get; set; }
        public int QuantityAfter { get; set; }
        public string OrderNo { get; set; } = "";
    }
}
