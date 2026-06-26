using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using WiseWMS.Application.DTOs;
using WiseWMS.Application.Services.Interfaces;
using WiseWMS.Infrastructure.Data;
using WiseWMS.Infrastructure.Entities;

namespace WiseWMS.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _db;
        private readonly ILogger<CategoryService> _logger;
        private readonly IDistributedCache _cache;

        public CategoryService(AppDbContext db, ILogger<CategoryService> logger, IDistributedCache cache)
        {
            _db = db;
            _logger = logger;
            _cache = cache;
        }

        public async Task<List<CategoryDto>> GetAll()
        {
            const string cacheKey = "categories_all";

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
            {
                _logger.LogInformation("分类列表命中缓存");
                return JsonSerializer.Deserialize<List<CategoryDto>>(cached) ?? [];
            }

            var categories = await _db.Categories
                .OrderBy(c => c.Id)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                })
                .ToListAsync();

            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(categories),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                }
            );

            _logger.LogInformation("分类列表已写入缓存，共 {Count} 条", categories.Count);
            return categories;
        }

        public async Task<CategoryDto?> Create(CreateCategoryDto dto)
        {
            bool exists = await _db.Categories.AnyAsync(c => c.Name == dto.Name);
            if (exists)
            {
                _logger.LogWarning("新增分类失败：同名分类已存在：名称={Name}", dto.Name);
                return null;
            }

            var category = new Category { Name = dto.Name };
            _db.Categories.Add(category);
            await _db.SaveChangesAsync();

            await _cache.RemoveAsync("categories_all");

            _logger.LogInformation("新增分类成功：ID={Id}, 名称={Name}", category.Id, category.Name);

            return new CategoryDto { Id = category.Id, Name = category.Name };
        }

        public async Task<bool> Delete(int id)
        {
            var category = await _db.Categories.FindAsync(id);
            if (category == null)
            {
                _logger.LogWarning("分类不存在：ID={Id}", id);
                return false;
            }

            bool hasProducts = await _db.Products.AnyAsync(p => p.CategoryId == id);
            if (hasProducts)
            {
                _logger.LogWarning("删除分类失败：分类下有商品，ID={Id}", id);
                return false;
            }

            _db.Categories.Remove(category);
            await _db.SaveChangesAsync();

            await _cache.RemoveAsync("categories_all");

            _logger.LogInformation("删除分类成功：ID={Id}", id);
            return true;
        }
    }
}
