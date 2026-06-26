using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;
using WiseWMS.Application.DTOs;
using WiseWMS.Application.Services;
using WiseWMS.Infrastructure.Data;
using WiseWMS.Infrastructure.Entities;

namespace WiseWMS.UnitTests.Services;

public class InboundServiceTests
{
    private AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"WiseWMS_Test_{Guid.NewGuid()}")
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new AppDbContext(options);
    }

    private InboundService CreateService(AppDbContext db)
    {
        var logger = Mock.Of<ILogger<InboundService>>();
        return new InboundService(logger, db);
    }

    private async Task SeedProduct(AppDbContext db, int id, int stock = 10)
    {
        db.Categories.Add(new Category { Id = 1, Name = "默认分类" });
        db.Products.Add(new Product
        {
            Id = id, Name = $"商品{id}", Spec = "规格", Unit = "个",
            Price = 100, Stock = stock, MinStock = 1, CategoryId = 1,
            CreatedAt = DateTime.UtcNow
        });
        db.Suppliers.Add(new Supplier { Id = 1, Name = "默认供应商" });
        db.Users.Add(new User { Id = 1, Username = "admin", PasswordHash = "hash", DisplayName = "管理员", Role = "Admin", CreatedAt = DateTime.UtcNow });
        await db.SaveChangesAsync();
    }

    [Fact]
    public async Task Create_ValidInbound_CreatesOrderAndUpdatesStock()
    {
        var db = CreateDbContext();
        await SeedProduct(db, 1, stock: 10);
        var service = CreateService(db);

        var result = await service.Create(new CreateInboundDto
        {
            SupplierId = 1,
            Items = new List<CreateInboundItemDto>
            {
                new() { ProductId = 1, Quantity = 5, UnitPrice = 100 }
            }
        }, operatorId: 1);

        Assert.NotNull(result);
        Assert.StartsWith("IN-", result.OrderNo);
        Assert.True(result.TotalAmount > 0);

        var product = await db.Products.FindAsync(1);
        Assert.NotNull(product);
        Assert.True(product.Stock > 10);
    }


    [Fact]
    public async Task Create_NonExistingProduct_ThrowsException()
    {
        var db = CreateDbContext();
        var service = CreateService(db);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.Create(new CreateInboundDto
            {
                SupplierId = 1,
                Items = new List<CreateInboundItemDto>
                {
                    new() { ProductId = 999, Quantity = 1, UnitPrice = 10 }
                }
            }, operatorId: 1));
    }

    [Fact]
    public async Task GetAll_EmptyDatabase_ReturnsEmpty()
    {
        var db = CreateDbContext();
        var service = CreateService(db);

        var result = await service.GetAll(null, 1, 20);

        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task GetById_NonExisting_ReturnsNull()
    {
        var db = CreateDbContext();
        var service = CreateService(db);

        var result = await service.GetById(999);

        Assert.Null(result);
    }
}
