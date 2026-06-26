using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using WiseWMS.Application.DTOs;
using WiseWMS.Application.Services;
using WiseWMS.Infrastructure.Data;
using WiseWMS.Infrastructure.Entities;

namespace WiseWMS.UnitTests.Services;

public class AuthServiceTests
{
    private AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"WiseWMS_Test_{Guid.NewGuid()}")
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsToken()
    {
        var db = CreateDbContext();
        db.Users.Add(new User
        {
            Id = 1,
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            DisplayName = "管理员",
            Role = "Admin",
            CreatedAt = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        var config = new Dictionary<string, string?>
        {
            ["Jwt:Key"] = "WiseWMS_SuperSecretKey_2026_MustBeAtLeast32Chars!",
            ["Jwt:Issuer"] = "WiseWMS",
            ["Jwt:Audience"] = "WiseWMS",
            ["Jwt:ExpireMinutes"] = "120"
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(config)
            .Build();
        var logger = Mock.Of<ILogger<AuthService>>();
        var service = new AuthService(db, configuration, logger);

        var result = await service.Login(new LoginDto { Username = "admin", Password = "admin123" });

        Assert.NotNull(result);
        Assert.Equal("管理员", result.DisplayName);
        Assert.Equal("Admin", result.Role);
        Assert.NotEmpty(result.Token);
    }

    [Fact]
    public async Task Login_WrongPassword_ReturnsNull()
    {
        var db = CreateDbContext();
        db.Users.Add(new User
        {
            Id = 1,
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            DisplayName = "管理员",
            Role = "Admin",
            CreatedAt = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        var config = new Dictionary<string, string?>();
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(config).Build();
        var logger = Mock.Of<ILogger<AuthService>>();
        var service = new AuthService(db, configuration, logger);

        var result = await service.Login(new LoginDto { Username = "admin", Password = "wrong" });

        Assert.Null(result);
    }

    [Fact]
    public async Task Login_NonExistingUser_ReturnsNull()
    {
        var db = CreateDbContext();
        var config = new Dictionary<string, string?>();
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(config).Build();
        var logger = Mock.Of<ILogger<AuthService>>();
        var service = new AuthService(db, configuration, logger);

        var result = await service.Login(new LoginDto { Username = "nobody", Password = "nope" });

        Assert.Null(result);
    }
}
