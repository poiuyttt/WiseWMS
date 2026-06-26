using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using WiseWMS.Application.DTOs;
using WiseWMS.Application.Profiles;
using WiseWMS.Application.Services;
using WiseWMS.Infrastructure.Data;
using WiseWMS.Infrastructure.Entities;

namespace WiseWMS.UnitTests.Services;

public class ProductServiceTests
{
    private AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"WiseWMS_Test_{Guid.NewGuid()}")
            .Options;
        var db = new AppDbContext(options);
        db.Categories.Add(new Category { Id = 1, Name = "默认分类" });
        db.SaveChanges();
        return db;
    }

    private IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        return config.CreateMapper();
    }

    private ProductService CreateService(AppDbContext db)
    {
        var logger = Mock.Of<ILogger<ProductService>>();
        var cache = Mock.Of<IDistributedCache>();
        var mapper = CreateMapper();
        return new ProductService(db, logger, cache, mapper);
    }

    [Fact]
    public async Task GetAll_EmptyDatabase_ReturnsEmptyPagedResult()
    {
        var db = CreateDbContext();
        var service = CreateService(db);

        var result = await service.GetAll(null, 1, 20);

        Assert.Empty(result.Items);
        Assert.Equal(0, result.Total);
        Assert.Equal(1, result.Page);
        Assert.Equal(20, result.PageSize);
    }

    [Fact]
    public async Task GetAll_WithProducts_ReturnsPagedProducts()
    {
        var db = CreateDbContext();
        db.Products.AddRange(
            new Product { Id = 1, Name = "重复", Spec = "重复规格", Unit = "个", Price = 10, Stock = 0, MinStock = 1, CategoryId = 1, CreatedAt = DateTime.UtcNow },
            new Product { Id = 2, Name = "其他", Spec = "其他规格", Unit = "个", Price = 15, Stock = 5, MinStock = 1, CategoryId = 1, CreatedAt = DateTime.UtcNow }
        );
        await db.SaveChangesAsync();
        var service = CreateService(db);

        var result = await service.GetAll(null, 1, 20);

        Assert.Equal(2, result.Total);
        Assert.Equal(2, result.Items.Count);
    }

    [Fact]
    public async Task GetAll_SearchByKeyword_FiltersResults()
    {
        var db = CreateDbContext();
        db.Products.AddRange(
            new Product { Id = 1, Name = "苹果", Spec = "500ml", Unit = "瓶", Price = 5, Stock = 50, MinStock = 5, CategoryId = 1, CreatedAt = DateTime.UtcNow },
            new Product { Id = 2, Name = "香蕉", Spec = "1kg", Unit = "把", Price = 8, Stock = 30, MinStock = 3, CategoryId = 1, CreatedAt = DateTime.UtcNow }
        );
        await db.SaveChangesAsync();
        var service = CreateService(db);

        var result = await service.GetAll("苹果", 1, 20);

        Assert.Equal(1, result.Total);
        Assert.Equal("苹果", result.Items[0].Name);
    }

    [Fact]
    public async Task GetById_ExistingProduct_ReturnsProduct()
    {
        var db = CreateDbContext();
        var product = new Product { Id = 1, Name = "测试", Spec = "S", Unit = "个", Price = 99, Stock = 10, MinStock = 1, CategoryId = 1, CreatedAt = DateTime.UtcNow };
        db.Products.Add(product);
        await db.SaveChangesAsync();
        var service = CreateService(db);

        var result = await service.GetById(product.Id);

        Assert.NotNull(result);
        Assert.Equal("测试", result.Name);
    }

    [Fact]
    public async Task GetById_NonExistingProduct_ReturnsNull()
    {
        var db = CreateDbContext();
        var service = CreateService(db);

        var result = await service.GetById(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task Create_ValidProduct_ReturnsDtoWithId()
    {
        var db = CreateDbContext();
        var service = CreateService(db);
        var dto = new CreateProductDto
        {
            Name = "新品",
            Spec = "新规格",
            Unit = "个",
            CategoryId = 1,
            Price = 100,
            MinStock = 5,
            Description = "测试"
        };

        var result = await service.Create(dto);

        Assert.NotNull(result);
        Assert.Equal("新品", result.Name);
        Assert.True(result.Id > 0);
    }

    [Fact]
    public async Task Create_DuplicateNameAndSpec_ReturnsNull()
    {
        var db = CreateDbContext();
        db.Products.Add(new Product { Id = 1, Name = "重复", Spec = "重复规格", Unit = "个", Price = 10, Stock = 0, MinStock = 1, CreatedAt = DateTime.UtcNow });
        await db.SaveChangesAsync();
        var service = CreateService(db);
        var dto = new CreateProductDto
        {
            Name = "重复",
            Spec = "重复规格",
            Unit = "个",
            CategoryId = 1,
            Price = 20,
            MinStock = 1
        };

        var result = await service.Create(dto);

        Assert.Null(result);
    }

    [Fact]
    public async Task Update_ExistingProduct_UpdatesFields()
    {
        var db = CreateDbContext();
        var product = new Product { Id = 1, Name = "旧名", Spec = "旧规格", Unit = "个", Price = 10, Stock = 50, MinStock = 5, CreatedAt = DateTime.UtcNow };
        db.Products.Add(product);
        await db.SaveChangesAsync();
        var service = CreateService(db);
        var dto = new UpdateProductDto
        {
            Name = "新名",
            Spec = "新规格",
            Unit = "箱",
            CategoryId = 2,
            Price = 99,
            MinStock = 10,
            Description = "已更新"
        };

        var result = await service.Update(product.Id, dto);

        Assert.NotNull(result);
        Assert.Equal("新名", result.Name);
    }

    [Fact]
    public async Task Update_NonExistingProduct_ReturnsNull()
    {
        var db = CreateDbContext();
        var service = CreateService(db);

        var result = await service.Update(999, new UpdateProductDto
        {
            Name = "无", Spec = "无", Unit = "个", CategoryId = 1, Price = 0, MinStock = 0
        });

        Assert.Null(result);
    }

    [Fact]
    public async Task Delete_ExistingProduct_ReturnsTrueAndRemoves()
    {
        var db = CreateDbContext();
        var product = new Product { Id = 1, Name = "待删", Spec = "X", Unit = "个", Price = 1, Stock = 1, MinStock = 0, CreatedAt = DateTime.UtcNow };
        db.Products.Add(product);
        await db.SaveChangesAsync();
        var service = CreateService(db);

        var result = await service.Delete(product.Id);

        Assert.True(result);
        Assert.Null(await db.Products.FindAsync(product.Id));
    }

    [Fact]
    public async Task Delete_NonExistingProduct_ReturnsFalse()
    {
        var db = CreateDbContext();
        var service = CreateService(db);

        var result = await service.Delete(999);

        Assert.False(result);
    }
}
