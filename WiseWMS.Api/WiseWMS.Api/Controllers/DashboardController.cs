using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WiseWMS.Application.DTOs;
using WiseWMS.Infrastructure.Data;

namespace WiseWMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _db;

        public DashboardController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            string today = DateTime.UtcNow.ToString("yyyyMMdd");

            var data = new DashboardDto
            {
                TotalProducts = await _db.Products.CountAsync(),
                TotalStock = await _db.Products.SumAsync(p => p.Stock),
                TodayInbound = await _db.InboundOrders.CountAsync(o =>
                    o.OrderNo.StartsWith($"IN-{today}")
                ),
                TodayOutbound = await _db.OutboundOrders.CountAsync(o =>
                    o.OrderNo.StartsWith($"OUT-{today}")
                ),
                LowStockCount = await _db.Products.CountAsync(p => p.Stock <= p.MinStock),
            };

            return Ok(data);
        }
    }
}
