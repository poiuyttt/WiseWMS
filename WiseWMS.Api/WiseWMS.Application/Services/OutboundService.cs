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
using WiseWMS.Infrastructure.Entities;

namespace WiseWMS.Application.Services
{
    public class OutboundService : IOutboundService
    {
        private readonly AppDbContext _db;
        private readonly ILogger<OutboundService> _logger;

        public OutboundService(AppDbContext db, ILogger<OutboundService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<OutboundOrderDto> Create(CreateOutboundDto dto, int operatorId)
        {
            using var tx = await _db.Database.BeginTransactionAsync();

            try
            {
                string today = DateTime.UtcNow.ToString("yyyyMMdd");

                int count = await _db.OutboundOrders.CountAsync(o =>
                    o.CreatedAt >= DateTime.UtcNow.Date
                );
                string orderNo = $"OUT-{today}-{count + 1:D4}";

                decimal totalAmount = dto.Items.Sum(o => o.Quantity * o.SalePrice);

                var order = new OutboundOrder
                {
                    OrderNo = orderNo,
                    CustomerId = dto.CustomerId,
                    OperatorId = operatorId,
                    TotalAmount = totalAmount,
                    Remark = dto.Remark,
                    CreatedAt = DateTime.UtcNow,
                };

                _db.OutboundOrders.Add(order);

                foreach (var item in dto.Items)
                {
                    var product = await _db.Products.FirstOrDefaultAsync(p =>
                        p.Id == item.ProductId
                    );

                    if (product == null)
                    {
                        _logger.LogWarning("出库失败：商品不存在，ID={ProductId}", item.ProductId);
                        throw new InvalidOperationException($"商品不存在，ID={item.ProductId}");
                    }

                    if (product.Stock < item.Quantity)
                    {
                        _logger.LogWarning(
                            "出库失败：商品库存不足，ID={ProductId}",
                            item.ProductId
                        );
                        throw new InvalidOperationException($"商品库存不足，ID={item.ProductId}");
                    }

                    order.Items.Add(
                        new OutboundItem
                        {
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            SalePrice = item.SalePrice,
                        }
                    );

                    int stockBefore = product.Stock;
                    product.Stock -= item.Quantity;
                    _db.InventoryTransactions.Add(
                        new InventoryTransaction
                        {
                            ProductId = item.ProductId,
                            Type = "Out",
                            Quantity = item.Quantity,
                            StockBefore = stockBefore,
                            StockAfter = product.Stock,
                            OrderNo = orderNo,
                            Remark = $"出库单 {orderNo}",
                            CreatedAt = DateTime.UtcNow,
                            OperatorId = operatorId,
                        }
                    );
                }

                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                _logger.LogInformation(
                    "出库单创建成功：单号={OrderNo}, 金额={TotalAmount}",
                    orderNo,
                    totalAmount
                );

                order = await _db
                    .OutboundOrders.AsNoTracking()
                    .Include(o => o.Customer)
                    .Include(o => o.Operator)
                    .Include(o => o.Items)
                        .ThenInclude(i => i.Product)
                    .FirstAsync(o => o.Id == order.Id);

                return new OutboundOrderDto
                {
                    Id = order.Id,
                    OrderNo = order.OrderNo,
                    CustomerId = order.CustomerId,
                    CustomerName = order.Customer != null ? order.Customer.Name : "",
                    OperatorId = order.OperatorId,
                    OperatorName = order.Operator != null ? order.Operator.DisplayName : "",
                    TotalAmount = order.TotalAmount,
                    Remark = order.Remark,
                    CreatedAt = order.CreatedAt,
                    Items = order
                        .Items.Select(i => new OutboundItemDto
                        {
                            Id = i.Id,
                            ProductId = i.ProductId,
                            Quantity = i.Quantity,
                            SalePrice = i.SalePrice,
                            ProductName = i.Product != null ? i.Product.Name : "",
                        })
                        .ToList(),
                };
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<PagedResult<OutboundOrderDto>> GetAll(
            string? keyword,
            int page,
            int pageSize
        )
        {
            var query = _db
                .OutboundOrders.Include(o => o.Customer)
                .Include(o => o.Operator)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(o => o.OrderNo.Contains(keyword));

            int total = await query.CountAsync();

            var item = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OutboundOrderDto
                {
                    Id = o.Id,
                    OrderNo = o.OrderNo,
                    CustomerId = o.CustomerId,
                    CustomerName = o.Customer != null ? o.Customer.Name : "",
                    OperatorId = o.OperatorId,
                    OperatorName = o.Operator != null ? o.Operator.DisplayName : "",
                    TotalAmount = o.TotalAmount,
                    Remark = o.Remark,
                    CreatedAt = o.CreatedAt,
                })
                .ToListAsync();

            return new PagedResult<OutboundOrderDto>
            {
                Items = item,
                Total = total,
                Page = page,
                PageSize = pageSize,
            };
        }

        public async Task<OutboundOrderDto?> GetById(int id)
        {
            var order = await _db
                .OutboundOrders.Include(o => o.Customer)
                .Include(o => o.Operator)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return null;

            return new OutboundOrderDto
            {
                Id = order.Id,
                OrderNo = order.OrderNo,
                CustomerId = order.CustomerId,
                CustomerName = order.Customer != null ? order.Customer.Name : "",
                OperatorId = order.OperatorId,
                OperatorName = order.Operator != null ? order.Operator.DisplayName : "",
                TotalAmount = order.TotalAmount,
                Remark = order.Remark,
                CreatedAt = order.CreatedAt,
                Items = order
                    .Items.Select(i => new OutboundItemDto
                    {
                        Id = i.Id,
                        ProductId = i.ProductId,
                        ProductName = i.Product != null ? i.Product.Name : "",
                        ProductSpec = i.Product != null ? i.Product.Spec : "",
                        Quantity = i.Quantity,
                        SalePrice = i.SalePrice,
                    })
                    .ToList(),
            };
        }
    }
}
