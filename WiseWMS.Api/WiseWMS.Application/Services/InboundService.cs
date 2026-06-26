using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WiseWMS.Application.DTOs;
using WiseWMS.Application.Publishers;
using WiseWMS.Application.Services.Interfaces;
using WiseWMS.Infrastructure.Data;
using WiseWMS.Infrastructure.Entities;

namespace WiseWMS.Application.Services
{
    public class InboundService : IInboundService
    {
        private readonly ILogger<InboundService> _logger;
        private readonly AppDbContext _db;
        private readonly MessagePublisher _publisher;
        private readonly IMapper _mapper;

        public InboundService(
            ILogger<InboundService> logger,
            AppDbContext db,
            MessagePublisher publisher,
            IMapper mapper
        )
        {
            _logger = logger;
            _db = db;
            _publisher = publisher;
            _mapper = mapper;
        }

        public async Task<InboundOrderDto> Create(CreateInboundDto dto, int operatorId)
        {
            using var tx = await _db.Database.BeginTransactionAsync();

            try
            {
                string today = DateTime.UtcNow.ToString("yyyyMMdd");

                int count = await _db.InboundOrders.CountAsync(o =>
                    o.CreatedAt >= DateTime.UtcNow.Date
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
                    // 使用 FirstOrDefault 而非 FindAsync，让后续 SaveChanges 之前
                    // 对同一 product 的修改都作用在同一个被追踪的实体上
                    var product = await _db.Products.FirstOrDefaultAsync(p =>
                        p.Id == item.ProductId
                    );

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
                await tx.CommitAsync();

                foreach (var item in dto.Items)
                {
                    var product = await _db.Products.FirstAsync(p => p.Id == item.ProductId);
                    await _publisher.PublishInventoryChange(item.ProductId, product.Stock, orderNo);
                }

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

                return _mapper.Map<InboundOrderDto>(order);
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
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

            var items = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<InboundOrderDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new PagedResult<InboundOrderDto>
            {
                Items = items,
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

            return _mapper.Map<InboundOrderDto>(order);
        }
    }
}
