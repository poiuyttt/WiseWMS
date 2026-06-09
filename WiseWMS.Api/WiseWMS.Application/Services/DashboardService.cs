using Microsoft.EntityFrameworkCore;
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

        public DashboardService(AppDbContext db, ILogger<DashboardService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<DashboardDto> GetDashboard()
        {
            var today = DateTime.UtcNow.Date;

            int totalProducts = await _db.Products.CountAsync();
            int totalStock = await _db.Products.SumAsync(p => p.Stock);

            int todayInbound = await _db.InboundOrders.CountAsync(o =>
                o.CreatedAt >= today
            );

            int todayOutbound = await _db.OutboundOrders.CountAsync(o =>
                o.CreatedAt >= today
            );

            int lowStockCount = await _db.Products.CountAsync(p =>
                p.Stock <= p.MinStock
            );

            _logger.LogInformation(
                "仪表盘统计：商品总数={TotalProducts}, 库存总数={TotalStock}, 今日入库={TodayInbound}, 今日出库={TodayOutbound}, 低库存预警={LowStockCount}",
                totalProducts,
                totalStock,
                todayInbound,
                todayOutbound,
                lowStockCount
            );

            return new DashboardDto
            {
                TotalProducts = totalProducts,
                TotalStock = totalStock,
                TodayInbound = todayInbound,
                TodayOutbound = todayOutbound,
                LowStockCount = lowStockCount,
            };
        }
    }
}
