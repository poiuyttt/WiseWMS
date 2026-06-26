using System.Text.Json;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using WiseWMS.Application.DTOs;
using WiseWMS.Application.Services.Interfaces;
using WiseWMS.Infrastructure.Data;
using WiseWMS.Infrastructure.Entities;

namespace WiseWMS.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _db;
        private readonly ILogger<ProductService> _logger;
        private readonly IDistributedCache _cache;
        private readonly IMapper _mapper;

        public ProductService(
            AppDbContext db,
            ILogger<ProductService> logger,
            IDistributedCache cache,
            IMapper mapper
        )
        {
            _db = db;
            _logger = logger;
            _cache = cache;
            _mapper = mapper;
        }

        public async Task<PagedResult<ProductDto>> GetAll(string? keyword, int page, int pageSize)
        {
            _logger.LogInformation(
                "查询商品列表：关键词={Keyword}, 页码={Page}, 每页={PageSize}",
                keyword,
                page,
                pageSize
            );

            var query = _db.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(p => p.Name.Contains(keyword) || p.Spec.Contains(keyword));

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new PagedResult<ProductDto>
            {
                Items = items,
                Total = total,
                Page = page,
                PageSize = pageSize,
            };
        }

        public async Task<List<ProductDto>> GetAll()
        {
            const string cacheKey = "products_all";

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
            {
                _logger.LogInformation("商品列表命中缓存");
                return JsonSerializer.Deserialize<List<ProductDto>>(cached) ?? [];
            }

            _logger.LogInformation("查询商品列表");

            var products = await _db
                .Products.Include(p => p.Category)
                .OrderByDescending(p => p.CreatedAt)
                .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(products),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                }
            );
            return products;
        }

        public async Task<ProductDto?> GetById(int id)
        {
            _logger.LogInformation("查询商品详情：ID={Id}", id);

            var product = await _db
                .Products.Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                _logger.LogWarning("商品不存在：ID={Id}", id);
                return null;
            }

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto?> Create(CreateProductDto dto)
        {
            bool exists = await _db.Products.AnyAsync(p =>
                p.Name == dto.Name && p.Spec == dto.Spec
            );
            if (exists)
            {
                _logger.LogWarning(
                    "新增商品失败：同名同规格商品已存在：名称={Name}, 规格={Spec}",
                    dto.Name,
                    dto.Spec
                );
                return null;
            }

            var product = _mapper.Map<Product>(dto);
            product.Stock = 0;
            product.CreatedAt = DateTime.UtcNow;

            _db.Products.Add(product);
            await _db.SaveChangesAsync();
            await _cache.RemoveAsync("products_all");

            _logger.LogInformation("新增商品成功：ID={Id}, 名称={Name}", product.Id, product.Name);

            await _db.Entry(product).Reference(p => p.Category).LoadAsync();
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto?> Update(int id, UpdateProductDto dto)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null)
            {
                _logger.LogWarning("商品不存在：ID={Id}", id);
                return null;
            }

            _mapper.Map(dto, product);
            await _db.SaveChangesAsync();
            await _cache.RemoveAsync("products_all");

            _logger.LogInformation("更新商品成功：ID={Id}", product.Id);

            await _db.Entry(product).Reference(p => p.Category).LoadAsync();
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<bool> Delete(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null)
            {
                _logger.LogWarning("商品不存在：ID={Id}", id);
                return false;
            }

            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            await _cache.RemoveAsync("products_all");

            _logger.LogInformation("删除商品成功：ID={Id}", id);
            return true;
        }
    }
}
