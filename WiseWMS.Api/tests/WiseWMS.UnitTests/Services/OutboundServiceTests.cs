using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;
using WiseWMS.Application.DTOs;
using WiseWMS.Application.Profiles;
using WiseWMS.Application.Publishers;
using WiseWMS.Application.Services;
using WiseWMS.Infrastructure.Data;
using WiseWMS.Infrastructure.Entities;

namespace WiseWMS.UnitTests.Services;

public class OutboundServiceTests
{
    private AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"WiseWMS_Test_{Guid.NewGuid()}")
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new AppDbContext(options);
    }

    private IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        return config.CreateMapper();
    }

    private OutboundService CreateService(AppDbContext db)
    {
        var logger = Mock.Of<ILogger<OutboundService>>();
        var publisherMock = new Mock<MessagePublisher>(null!, Mock.Of<ILogger<MessagePublisher>>());
        publisherMock.Setup(p => p.PublishInventoryChange(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())).Returns(Task.CompletedTask);
        return new OutboundService(db, logger, publisherMock.Object, CreateMapper());
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
        db.Customers.Add(new Customer { Id = 1, Name = "默认客户" });
        db.Users.Add(new User { Id = 1, Username = "admin", PasswordHash = "hash", DisplayName = "管理员", Role = "Admin", CreatedAt = DateTime.UtcNow });
        await db.SaveChangesAsync();
    }

    [Fact]
    public async Task Create_ValidOutbound_CreatesOrderAndReducesStock()
    {
        var db = CreateDbContext();
        await SeedProduct(db, 1, stock: 10);
        var service = CreateService(db);

        var result = await service.Create(new CreateOutboundDto
        {
            CustomerId = 1,
            Items = new List<CreateOutboundItemDto>
            {
                new() { ProductId = 1, Quantity = 3, SalePrice = 150 }
            }
        }, operatorId: 1);

        Assert.NotNull(result);
        Assert.StartsWith("OUT-", result.OrderNo);
        Assert.True(result.TotalAmount > 0);

        var product = await db.Products.FindAsync(1);
        Assert.NotNull(product);
        Assert.True(product.Stock < 10);
    }

    [Fact]
    public async Task Create_InsufficientStock_ThrowsException()
    {
        var db = CreateDbContext();
        await SeedProduct(db, 1, stock: 2);
        var service = CreateService(db);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.Create(new CreateOutboundDto
            {
                CustomerId = 1,
                Items = new List<CreateOutboundItemDto>
                {
                    new() { ProductId = 1, Quantity = 5, SalePrice = 100 }
                }
            }, operatorId: 1));
    }

    [Fact]
    public async Task Create_NonExistingProduct_ThrowsException()
    {
        var db = CreateDbContext();
        var service = CreateService(db);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.Create(new CreateOutboundDto
            {
                CustomerId = 1,
                Items = new List<CreateOutboundItemDto>
                {
                    new() { ProductId = 999, Quantity = 1, SalePrice = 10 }
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
