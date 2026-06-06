using Microsoft.EntityFrameworkCore;
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

        public ProductService(AppDbContext db, ILogger<ProductService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<PagedResult<ProductDto>> GetAll(string? keyword, int page, int pageSize)
        {
            _logger.LogInformation($"查询商品列表：关键词={keyword}, 页码={page}, 每页={pageSize}");

            // 1. 查所有商品，带上分类信息
            var query = _db.Products.Include(p => p.Category).AsQueryable();

            // 2. 有关键词就按名称或规格搜索
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(p => p.Name.Contains(keyword) || p.Spec.Contains(keyword));
            }

            // 3. 算总数
            var total = await query.CountAsync();

            // 4. 分页（Skip 跳过前面的，Take 取当前页的）
            var items = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(product => new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Spec = product.Spec,
                    Unit = product.Unit,
                    CategoryId = product.CategoryId,
                    CategoryName = product.Category.Name,
                    Price = product.Price,
                    Stock = product.Stock,
                    MinStock = product.MinStock,
                    Description = product.Description,
                    CreatedAt = product.CreatedAt,
                })
                .ToListAsync();

            return new PagedResult<ProductDto>
            {
                Items = items,
                Total = total,
                Page = page,
                PageSize = pageSize,
            };
        }

        public async Task<ProductDto?> GetById(int id)
        {
            _logger.LogInformation($"查询商品详情：ID={id}");

            var product = await _db
                .Products.Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                _logger.LogWarning($"商品不存在：ID={id}");
                return null;
            }

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Spec = product.Spec,
                Unit = product.Unit,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.Name,
                Price = product.Price,
                Stock = product.Stock,
                MinStock = product.MinStock,
                Description = product.Description,
                CreatedAt = product.CreatedAt,
            };
        }

        public async Task<ProductDto?> Create(CreateProductDto dto)
        {
            bool exists = await _db.Products.AnyAsync(p =>
                p.Name == dto.Name && p.Spec == dto.Spec
            );
            if (exists)
            {
                _logger.LogWarning(
                    $"新增商品失败：同名同规格商品已存在：名称={dto.Name}, 规格={dto.Spec}"
                );
                return null;
            }
            var product = new Product
            {
                Name = dto.Name,
                Spec = dto.Spec,
                Unit = dto.Unit,
                CategoryId = dto.CategoryId,
                Price = dto.Price,
                Stock = 0,
                MinStock = dto.MinStock,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow,
            };

            await _db.Products.AddAsync(product);
            await _db.SaveChangesAsync();

            _logger.LogInformation($"新增商品成功：ID={product.Id}，名称={product.Name}");

            await _db.Entry(product).Reference(p => p.Category).LoadAsync();

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Spec = product.Spec,
                Unit = product.Unit,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.Name,
                Price = product.Price,
                Stock = product.Stock,
                MinStock = product.MinStock,
                Description = product.Description,
                CreatedAt = product.CreatedAt,
            };
        }

        public async Task<ProductDto?> Update(int id, UpdateProductDto dto)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null)
            {
                _logger.LogWarning($"商品不存在：ID={id}");
                return null;
            }

            product.Name = dto.Name;
            product.Spec = dto.Spec;
            product.Unit = dto.Unit;
            product.CategoryId = dto.CategoryId;
            product.Price = dto.Price;
            product.MinStock = dto.MinStock;
            product.Description = dto.Description;

            await _db.SaveChangesAsync();

            _logger.LogInformation($"更新商品成功：ID={product.Id}");

            await _db.Entry(product).Reference(p => p.Category).LoadAsync();

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Spec = product.Spec,
                Unit = product.Unit,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.Name,
                Price = product.Price,
                Stock = product.Stock,
                MinStock = product.MinStock,
                Description = product.Description,
                CreatedAt = product.CreatedAt,
            };
        }

        public async Task<bool> Delete(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null)
            {
                _logger.LogWarning($"商品不存在：ID={id}");
                return false;
            }

            _db.Products.Remove(product);
            await _db.SaveChangesAsync();

            _logger.LogInformation($"删除商品成功：ID={id}");

            return true;
        }
    }
}
