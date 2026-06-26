using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using WiseWMS.Application.DTOs;
using WiseWMS.Application.Services;
using WiseWMS.Infrastructure.Data;
using WiseWMS.Infrastructure.Entities;

namespace WiseWMS.UnitTests.Services;

public class CategoryServiceTests
{
    private AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"WiseWMS_Test_{Guid.NewGuid()}")
            .Options;
        return new AppDbContext(options);
    }

    private CategoryService CreateService(AppDbContext db)
    {
        var logger = Mock.Of<ILogger<CategoryService>>();
        var cacheMock = new Mock<IDistributedCache>();
        cacheMock
            .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);
        return new CategoryService(db, logger, cacheMock.Object);
    }

    [Fact]
    public async Task GetAll_EmptyDatabase_ReturnsEmpty()
    {
        var db = CreateDbContext();
        var service = CreateService(db);

        var result = await service.GetAll();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAll_WithCategories_ReturnsAll()
    {
        var db = CreateDbContext();
        db.Categories.AddRange(
            new Category { Id = 1, Name = "食品" },
            new Category { Id = 2, Name = "饮料" },
            new Category { Id = 3, Name = "日用品" }
        );
        await db.SaveChangesAsync();
        var service = CreateService(db);

        var result = await service.GetAll();

        Assert.Equal(3, result.Count);
        Assert.Equal("食品", result[0].Name);
        Assert.Equal("饮料", result[1].Name);
    }

    [Fact]
    public async Task Create_ValidCategory_ReturnsDtoWithId()
    {
        var db = CreateDbContext();
        var service = CreateService(db);

        var result = await service.Create(new CreateCategoryDto { Name = "新分类" });

        Assert.NotNull(result);
        Assert.Equal("新分类", result.Name);
        Assert.True(result.Id > 0);
    }

    [Fact]
    public async Task Create_DuplicateName_ReturnsNull()
    {
        var db = CreateDbContext();
        db.Categories.Add(new Category { Id = 1, Name = "已有分类" });
        await db.SaveChangesAsync();
        var service = CreateService(db);

        var result = await service.Create(new CreateCategoryDto { Name = "已有分类" });

        Assert.Null(result);
    }

    [Fact]
    public async Task Delete_ExistingCategory_ReturnsTrue()
    {
        var db = CreateDbContext();
        db.Categories.Add(new Category { Id = 1, Name = "待删" });
        await db.SaveChangesAsync();
        var service = CreateService(db);

        var result = await service.Delete(1);

        Assert.True(result);
    }

    [Fact]
    public async Task Delete_NonExistingCategory_ReturnsFalse()
    {
        var db = CreateDbContext();
        var service = CreateService(db);

        var result = await service.Delete(999);

        Assert.False(result);
    }

    [Fact]
    public async Task Delete_CategoryWithProducts_ReturnsFalse()
    {
        var db = CreateDbContext();
        db.Categories.Add(new Category { Id = 1, Name = "有商品" });
        db.Products.Add(new Product { Id = 1, Name = "测试", Spec = "S", Unit = "个", Price = 10, Stock = 0, MinStock = 1, CategoryId = 1, CreatedAt = DateTime.UtcNow });
        await db.SaveChangesAsync();
        var service = CreateService(db);

        var result = await service.Delete(1);

        Assert.False(result);
    }
}
