using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using WiseWMS.Application.DTOs;
using WiseWMS.Application.Profiles;
using WiseWMS.Application.Services;
using WiseWMS.Infrastructure.Data;
using WiseWMS.Infrastructure.Entities;

namespace WiseWMS.UnitTests.Services;

public class InventoryServiceTests
{
    private AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"WiseWMS_Test_{Guid.NewGuid()}")
            .Options;
        return new AppDbContext(options);
    }

    private IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        return config.CreateMapper();
    }

    private InventoryService CreateService(AppDbContext db)
    {
        var logger = Mock.Of<ILogger<InventoryService>>();
        return new InventoryService(db, logger, CreateMapper());
    }

    [Fact]
    public async Task GetTransactions_Empty_ReturnsEmpty()
    {
        var db = CreateDbContext();
        var service = CreateService(db);

        var result = await service.GetTransactions(1, 1, 20);

        Assert.Empty(result.Items);
        Assert.Equal(0, result.Total);
    }

    [Fact]
    public async Task GetTransactions_WithData_ReturnsPaged()
    {
        var db = CreateDbContext();
        db.Products.Add(new Product { Id = 1, Name = "测试", Spec = "S", Unit = "个", Price = 10, Stock = 0, MinStock = 1, CategoryId = 1, CreatedAt = DateTime.UtcNow });
        db.Categories.Add(new Category { Id = 1, Name = "默认" });
        db.InventoryTransactions.AddRange(
            new InventoryTransaction { Id = 1, ProductId = 1, Type = "In", Quantity = 10, StockBefore = 0, StockAfter = 10, OrderNo = "IN-001", CreatedAt = DateTime.UtcNow, OperatorId = 1 },
            new InventoryTransaction { Id = 2, ProductId = 1, Type = "Out", Quantity = 3, StockBefore = 10, StockAfter = 7, OrderNo = "OUT-001", CreatedAt = DateTime.UtcNow, OperatorId = 1 }
        );
        db.Users.Add(new User { Id = 1, Username = "admin", PasswordHash = "hash", DisplayName = "管理员", Role = "Admin", CreatedAt = DateTime.UtcNow });
        await db.SaveChangesAsync();
        var service = CreateService(db);

        var result = await service.GetTransactions(1, 1, 20);

        Assert.Equal(2, result.Total);
        Assert.Equal(2, result.Items.Count);
    }

    [Fact]
    public async Task GetLowStockProducts_WithLowStock_ReturnsFiltered()
    {
        var db = CreateDbContext();
        db.Categories.Add(new Category { Id = 1, Name = "默认" });
        db.Products.AddRange(
            new Product { Id = 1, Name = "低库存", Spec = "S", Unit = "个", Price = 10, Stock = 2, MinStock = 5, CategoryId = 1, CreatedAt = DateTime.UtcNow },
            new Product { Id = 2, Name = "充足", Spec = "M", Unit = "个", Price = 20, Stock = 50, MinStock = 5, CategoryId = 1, CreatedAt = DateTime.UtcNow }
        );
        await db.SaveChangesAsync();
        var service = CreateService(db);

        var result = await service.GetLowStockProducts();

        Assert.Single(result);
        Assert.Equal("低库存", result[0].Name);
    }

    [Fact]
    public async Task GetLowStockProducts_AllStockSufficient_ReturnsEmpty()
    {
        var db = CreateDbContext();
        db.Categories.Add(new Category { Id = 1, Name = "默认" });
        db.Products.AddRange(
            new Product { Id = 1, Name = "商品A", Spec = "S", Unit = "个", Price = 10, Stock = 100, MinStock = 5, CategoryId = 1, CreatedAt = DateTime.UtcNow },
            new Product { Id = 2, Name = "商品B", Spec = "M", Unit = "个", Price = 20, Stock = 50, MinStock = 10, CategoryId = 1, CreatedAt = DateTime.UtcNow }
        );
        await db.SaveChangesAsync();
        var service = CreateService(db);

        var result = await service.GetLowStockProducts();

        Assert.Empty(result);
    }
}
