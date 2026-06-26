using AutoMapper;
using AutoMapper.QueryableExtensions;
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
        private readonly IMapper _mapper;

        public InventoryService(AppDbContext db, ILogger<InventoryService> logger, IMapper mapper)
        {
            _db = db;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<PagedResult<InventoryTransactionDto>> GetTransactions(int productId, int page, int pageSize)
        {
            var query = _db.InventoryTransactions.Include(q => q.Product).Include(q => q.Operator)
                .Where(q => q.ProductId == productId).AsQueryable();

            int total = await query.CountAsync();
            var items = await query.OrderByDescending(q => q.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize)
                .ProjectTo<InventoryTransactionDto>(_mapper.ConfigurationProvider).ToListAsync();

            return new PagedResult<InventoryTransactionDto> { Items = items, Total = total, Page = page, PageSize = pageSize };
        }

        public async Task<List<ProductDto>> GetLowStockProducts()
        {
            return await _db.Products.Include(p => p.Category)
                .Where(p => p.Stock <= p.MinStock)
                .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
    }
}
