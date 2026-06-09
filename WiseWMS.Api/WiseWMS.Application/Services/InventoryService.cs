using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WiseWMS.Application.DTOs;
using WiseWMS.Application.Services.Interfaces;
using WiseWMS.Infrastructure.Data;

namespace WiseWMS.Application.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly AppDbContext _db;
        private readonly ILogger<InventoryService> _logger;

        public InventoryService(AppDbContext db, ILogger<InventoryService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<PagedResult<InventoryTransactionDto>> GetTransactions(
            int productId,
            int page,
            int pageSize
        )
        {
            var query = _db
                .InventoryTransactions.Include(q => q.Product)
                .Include(q => q.Operator)
                .Where(q => q.ProductId == productId)
                .AsQueryable();

            int total = await query.CountAsync();

            var items = await query
                .OrderByDescending(q => q.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(q => new InventoryTransactionDto
                {
                    Id = q.Id,
                    ProductId = q.ProductId,
                    ProductName = q.Product != null ? q.Product.Name : "",
                    ProductSpec = q.Product != null ? q.Product.Spec : "",
                    Type = q.Type,
                    Quantity = q.Quantity,
                    StockBefore = q.StockBefore,
                    StockAfter = q.StockAfter,
                    OrderNo = q.OrderNo,
                    OperatorName = q.Operator != null ? q.Operator.DisplayName : "",
                    CreatedAt = q.CreatedAt,
                })
                .ToListAsync();

            return new PagedResult<InventoryTransactionDto>
            {
                Items = items,
                Total = total,
                Page = page,
                PageSize = pageSize,
            };
        }

        public async Task<List<ProductDto>> GetLowStockProducts()
        {
            return await _db
                .Products.Include(p => p.Category)
                .Where(p => p.Stock <= p.MinStock)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Spec = p.Spec,
                    Unit = p.Unit,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category != null ? p.Category.Name : "",
                    Price = p.Price,
                    Stock = p.Stock,
                    MinStock = p.MinStock,
                    Description = p.Description,
                    CreatedAt = p.CreatedAt,
                })
                .ToListAsync();
        }
    }
}
