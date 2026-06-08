using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WiseWMS.Application.DTOs;
using WiseWMS.Application.Services.Interfaces;
using WiseWMS.Infrastructure.Data;
using WiseWMS.Infrastructure.Entities;

namespace WiseWMS.Application.Services
{
    public class InboundService : IInboundService
    {
        private readonly ILogger<InboundService> _logger;
        private readonly AppDbContext _db;

        public InboundService(ILogger<InboundService> logger, AppDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task<InboundOrderDto> Create(CreateInboundDto dto, int operatorId)
        {
            string today = DateTime.UtcNow.ToString("yyyyMMdd");

            int count = await _db.InboundOrders.CountAsync(o =>
                o.OrderNo.StartsWith($"IN-{today}")
            );
            string orderNo = $"IN-{today}-{count + 1:D4}";

            decimal totalAmount = dto.Items.Sum(i => i.Quantity * i.UnitPrice);

            var order = new InboundOrder
            {
                OrderNo = orderNo,
                SupplierId = dto.SupplierId,
                OperatorId = operatorId,
                TotalAmount = totalAmount,
                Remark = dto.Remark,
                CreatedAt = DateTime.UtcNow,
            };

            _db.InboundOrders.Add(order);

            foreach (var item in dto.Items)
            {
                var product = await _db.Products.FindAsync(item.ProductId);
                if (product == null)
                {
                    _logger.LogWarning("入库失败：商品不存在，ID={ProductId}", item.ProductId);
                    throw new InvalidOperationException($"商品不存在：ID={item.ProductId}");
                }

                order.Items.Add(
                    new InboundItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                    }
                );

                int stockBefore = product.Stock;
                product.Stock += item.Quantity;

                _db.InventoryTransactions.Add(
                    new InventoryTransaction
                    {
                        ProductId = item.ProductId,
                        Type = "In",
                        Quantity = item.Quantity,
                        StockBefore = stockBefore,
                        StockAfter = product.Stock,
                        OrderNo = orderNo,
                        Remark = $"入库单 {orderNo}",
                        CreatedAt = DateTime.UtcNow,
                        OperatorId = operatorId,
                    }
                );
            }
            await _db.SaveChangesAsync();

            _logger.LogInformation(
                "入库单创建成功：单号={OrderNo}, 金额={TotalAmount}",
                orderNo,
                totalAmount
            );

            // 重新查询，一次性加载所有关联数据
            // AsNoTracking 避免跟已追踪的实体冲突
            order = await _db
                .InboundOrders.AsNoTracking()
                .Include(o => o.Supplier)
                .Include(o => o.Operator)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .FirstAsync(o => o.Id == order.Id);

            return new InboundOrderDto
            {
                Id = order.Id,
                OrderNo = order.OrderNo,
                SupplierId = order.SupplierId,
                SupplierName = order.Supplier?.Name ?? "",
                OperatorId = order.OperatorId,
                OperatorName = order.Operator?.DisplayName ?? "",
                TotalAmount = order.TotalAmount,
                Remark = order.Remark,
                CreatedAt = order.CreatedAt,
                Items = order
                    .Items.Select(i => new InboundItemDto
                    {
                        Id = i.Id,
                        ProductId = i.ProductId,
                        ProductName = i.Product?.Name ?? "",
                        ProductSpec = i.Product?.Spec ?? "",
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                    })
                    .ToList(),
            };
        }

        public async Task<PagedResult<InboundOrderDto>> GetAll(
            string? keyword,
            int page,
            int pageSize
        )
        {
            var query = _db
                .InboundOrders.Include(o => o.Operator)
                .Include(o => o.Supplier)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(o => o.OrderNo.Contains(keyword));

            int total = await query.CountAsync();

            var item = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new InboundOrderDto
                {
                    Id = o.Id,
                    OrderNo = o.OrderNo,
                    SupplierId = o.SupplierId,
                    SupplierName = o.Supplier.Name,
                    OperatorId = o.OperatorId,
                    OperatorName = o.Operator.DisplayName,
                    TotalAmount = o.TotalAmount,
                    Remark = o.Remark,
                    CreatedAt = o.CreatedAt,
                })
                .ToListAsync();

            return new PagedResult<InboundOrderDto>
            {
                Items = item,
                Total = total,
                Page = page,
                PageSize = pageSize,
            };
        }

        public async Task<InboundOrderDto?> GetById(int id)
        {
            var order = await _db
                .InboundOrders.Include(o => o.Supplier)
                .Include(o => o.Operator)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return null;

            return new InboundOrderDto
            {
                Id = order.Id,
                OrderNo = order.OrderNo,
                SupplierId = order.SupplierId,
                SupplierName = order.Supplier?.Name ?? "",
                OperatorId = order.OperatorId,
                OperatorName = order.Operator?.DisplayName ?? "",
                TotalAmount = order.TotalAmount,
                Remark = order.Remark,
                CreatedAt = order.CreatedAt,
                Items = order
                    .Items.Select(i => new InboundItemDto
                    {
                        Id = i.Id,
                        ProductId = i.ProductId,
                        ProductName = i.Product?.Name ?? "",
                        ProductSpec = i.Product?.Spec ?? "",
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                    })
                    .ToList(),
            };
        }
    }
}
