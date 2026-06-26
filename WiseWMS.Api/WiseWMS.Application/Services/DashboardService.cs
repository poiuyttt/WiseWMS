using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using WiseWMS.Application.DTOs;
using WiseWMS.Application.Services.Interfaces;
using WiseWMS.Infrastructure.Data;

namespace WiseWMS.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _db;
        private readonly ILogger<DashboardService> _logger;
        private readonly IDistributedCache _cache;

        public DashboardService(
            AppDbContext db,
            ILogger<DashboardService> logger,
            IDistributedCache cache
        )
        {
            _db = db;
            _logger = logger;
            _cache = cache;
        }

        public async Task<DashboardDto> GetDashboard()
        {
            const string cacheKey = "dashboard_stats";

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
            {
                _logger.LogInformation("仪表盘命中缓存");
                return JsonSerializer.Deserialize<DashboardDto>(cached) ?? new DashboardDto();
            }

            var today = DateTime.UtcNow.Date;

            int totalProducts = await _db.Products.CountAsync();
            int totalStock = await _db.Products.SumAsync(p => p.Stock);

            int todayInbound = await _db.InboundOrders.CountAsync(o => o.CreatedAt >= today);

            int todayOutbound = await _db.OutboundOrders.CountAsync(o => o.CreatedAt >= today);

            int lowStockCount = await _db.Products.CountAsync(p => p.Stock <= p.MinStock);

            var result = new DashboardDto
            {
                TotalProducts = totalProducts,
                TotalStock = totalStock,
                TodayInbound = todayInbound,
                TodayOutbound = todayOutbound,
                LowStockCount = lowStockCount,
            };

            _logger.LogInformation(
                "仪表盘统计：商品总数={TotalProducts}, 库存总数={TotalStock}, 今日入库={TodayInbound}, 今日出库={TodayOutbound}, 低库存预警={LowStockCount}",
                totalProducts,
                totalStock,
                todayInbound,
                todayOutbound,
                lowStockCount
            );

            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(result),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30),
                }
            );

            return result;
        }
    }
}
