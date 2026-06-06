using WiseWMS.Infrastructure.Entities;

namespace WiseWMS.Infrastructure.Data
{
    public static class SeedData
    {
        public static void Initialize(AppDbContext db)
        {
            if (db.Users.Any())
                return;

            db.Users.Add(
                new User
                {
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    DisplayName = "系统管理员",
                    Role = "Admin",
                    CreatedAt = DateTime.UtcNow,
                }
            );

            db.SaveChanges();
        }
    }
}
