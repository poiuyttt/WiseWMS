using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using WiseWMS.Application.DTOs;
using WiseWMS.Application.Services;
using WiseWMS.Infrastructure.Data;
using WiseWMS.Infrastructure.Entities;

namespace WiseWMS.UnitTests.Services;

public class DashboardServiceTests
{
    private AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"WiseWMS_Test_{Guid.NewGuid()}")
            .Options;
        return new AppDbContext(options);
    }

    private DashboardService CreateService(AppDbContext db)
    {
        var logger = Mock.Of<ILogger<DashboardService>>();
        var cacheMock = new Mock<IDistributedCache>();
        cacheMock
            .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);
        return new DashboardService(db, logger, cacheMock.Object);
    }

    [Fact]
    public async Task GetDashboard_EmptyDatabase_ReturnsZeros()
    {
        var db = CreateDbContext();
        var service = CreateService(db);

        var result = await service.GetDashboard();

        Assert.Equal(0, result.TotalProducts);
        Assert.Equal(0, result.TotalStock);
        Assert.Equal(0, result.TodayInbound);
        Assert.Equal(0, result.TodayOutbound);
        Assert.Equal(0, result.LowStockCount);
    }

    [Fact]
    public async Task GetDashboard_WithData_ReturnsCorrectStats()
    {
        var db = CreateDbContext();
        db.Categories.Add(new Category { Id = 1, Name = "默认" });
        db.Products.AddRange(
            new Product { Id = 1, Name = "商品A", Spec = "S", Unit = "个", Price = 10, Stock = 100, MinStock = 5, CategoryId = 1, CreatedAt = DateTime.UtcNow },
            new Product { Id = 2, Name = "商品B", Spec = "M", Unit = "个", Price = 20, Stock = 2, MinStock = 5, CategoryId = 1, CreatedAt = DateTime.UtcNow }
        );
        await db.SaveChangesAsync();
        var service = CreateService(db);

        var result = await service.GetDashboard();

        Assert.Equal(2, result.TotalProducts);
        Assert.Equal(102, result.TotalStock);
        Assert.Equal(1, result.LowStockCount);
    }
}
