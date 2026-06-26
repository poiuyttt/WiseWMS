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

public class CustomerServiceTests
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

    private CustomerService CreateService(AppDbContext db)
    {
        var logger = Mock.Of<ILogger<CustomerService>>();
        return new CustomerService(db, logger, CreateMapper());
    }

    [Fact]
    public async Task GetAll_EmptyDatabase_ReturnsEmptyPagedResult()
    {
        var db = CreateDbContext();
        var service = CreateService(db);

        var result = await service.GetAll(null, 1, 20);

        Assert.Empty(result.Items);
        Assert.Equal(0, result.Total);
    }

    [Fact]
    public async Task GetAll_WithCustomers_ReturnsPagedCustomers()
    {
        var db = CreateDbContext();
        db.Customers.AddRange(
            new Customer { Id = 1, Name = "客户A" },
            new Customer { Id = 2, Name = "客户B" }
        );
        await db.SaveChangesAsync();
        var service = CreateService(db);

        var result = await service.GetAll(null, 1, 20);

        Assert.Equal(2, result.Total);
    }

    [Fact]
    public async Task GetAll_SearchByKeyword_FiltersResults()
    {
        var db = CreateDbContext();
        db.Customers.AddRange(
            new Customer { Id = 1, Name = "腾讯", Contact = "王五" },
            new Customer { Id = 2, Name = "阿里", Contact = "赵六" }
        );
        await db.SaveChangesAsync();
        var service = CreateService(db);

        var result = await service.GetAll("腾讯", 1, 20);

        Assert.Equal(1, result.Total);
        Assert.Equal("腾讯", result.Items[0].Name);
    }

    [Fact]
    public async Task Create_ValidCustomer_ReturnsDtoWithId()
    {
        var db = CreateDbContext();
        var service = CreateService(db);

        var result = await service.Create(new CreateCustomerDto { Name = "新客户" });

        Assert.NotNull(result);
        Assert.Equal("新客户", result.Name);
        Assert.True(result.Id > 0);
    }

    [Fact]
    public async Task Update_ExistingCustomer_UpdatesFields()
    {
        var db = CreateDbContext();
        db.Customers.Add(new Customer { Id = 1, Name = "旧名" });
        await db.SaveChangesAsync();
        var service = CreateService(db);

        var result = await service.Update(1, new UpdateCustomerDto { Name = "新名" });

        Assert.NotNull(result);
        Assert.Equal("新名", result.Name);
    }

    [Fact]
    public async Task Delete_ExistingCustomer_ReturnsTrue()
    {
        var db = CreateDbContext();
        db.Customers.Add(new Customer { Id = 1, Name = "待删" });
        await db.SaveChangesAsync();
        var service = CreateService(db);

        var result = await service.Delete(1);

        Assert.True(result);
    }

    [Fact]
    public async Task Delete_NonExistingCustomer_ReturnsFalse()
    {
        var db = CreateDbContext();
        var service = CreateService(db);

        var result = await service.Delete(999);

        Assert.False(result);
    }
}
