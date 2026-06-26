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

public class SupplierServiceTests
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

    private SupplierService CreateService(AppDbContext db)
    {
        var logger = Mock.Of<ILogger<SupplierService>>();
        return new SupplierService(db, logger, CreateMapper());
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
    public async Task GetAll_WithSuppliers_ReturnsPagedSuppliers()
    {
        var db = CreateDbContext();
        db.Suppliers.AddRange(
            new Supplier { Id = 1, Name = "供应商A" },
            new Supplier { Id = 2, Name = "供应商B" }
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
        db.Suppliers.AddRange(
            new Supplier { Id = 1, Name = "华为", Contact = "张三" },
            new Supplier { Id = 2, Name = "小米", Contact = "李四" }
        );
        await db.SaveChangesAsync();
        var service = CreateService(db);

        var result = await service.GetAll("华为", 1, 20);

        Assert.Equal(1, result.Total);
        Assert.Equal("华为", result.Items[0].Name);
    }

    [Fact]
    public async Task GetById_ExistingSupplier_ReturnsSupplier()
    {
        var db = CreateDbContext();
        db.Suppliers.Add(new Supplier { Id = 1, Name = "测试供应商" });
        await db.SaveChangesAsync();
        var service = CreateService(db);

        var result = await service.GetById(1);

        Assert.NotNull(result);
        Assert.Equal("测试供应商", result.Name);
    }

    [Fact]
    public async Task GetById_NonExistingSupplier_ReturnsNull()
    {
        var db = CreateDbContext();
        var service = CreateService(db);

        var result = await service.GetById(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task Create_ValidSupplier_ReturnsDtoWithId()
    {
        var db = CreateDbContext();
        var service = CreateService(db);
        var dto = new CreateSupplierDto { Name = "新供应商" };

        var result = await service.Create(dto);

        Assert.NotNull(result);
        Assert.Equal("新供应商", result.Name);
        Assert.True(result.Id > 0);
    }

    [Fact]
    public async Task Update_ExistingSupplier_UpdatesFields()
    {
        var db = CreateDbContext();
        db.Suppliers.Add(new Supplier { Id = 1, Name = "旧名" });
        await db.SaveChangesAsync();
        var service = CreateService(db);

        var result = await service.Update(1, new UpdateSupplierDto { Name = "新名" });

        Assert.NotNull(result);
        Assert.Equal("新名", result.Name);
    }

    [Fact]
    public async Task Update_NonExistingSupplier_ReturnsNull()
    {
        var db = CreateDbContext();
        var service = CreateService(db);

        var result = await service.Update(999, new UpdateSupplierDto { Name = "无" });

        Assert.Null(result);
    }

    [Fact]
    public async Task Delete_ExistingSupplier_ReturnsTrue()
    {
        var db = CreateDbContext();
        db.Suppliers.Add(new Supplier { Id = 1, Name = "待删" });
        await db.SaveChangesAsync();
        var service = CreateService(db);

        var result = await service.Delete(1);

        Assert.True(result);
        Assert.Null(await db.Suppliers.FindAsync(1));
    }

    [Fact]
    public async Task Delete_NonExistingSupplier_ReturnsFalse()
    {
        var db = CreateDbContext();
        var service = CreateService(db);

        var result = await service.Delete(999);

        Assert.False(result);
    }
}
