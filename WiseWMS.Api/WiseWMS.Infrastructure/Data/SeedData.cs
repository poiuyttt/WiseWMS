using WiseWMS.Infrastructure.Entities;

namespace WiseWMS.Infrastructure.Data
{
    public static class SeedData
    {
        public static void Initialize(AppDbContext db)
        {
            if (!db.Suppliers.Any())
            {
                db.Suppliers.Add(
                    new Supplier
                    {
                        Name = "默认供应商",
                        Contact = "管理员",
                        Phone = "13800000000",
                        Address = "默认地址",
                    }
                );
                db.SaveChanges();
            }
            if (!db.Categories.Any())
            {
                db.Categories.AddRange(
                    new Category { Name = "食品饮料" },
                    new Category { Name = "日用品" },
                    new Category { Name = "电子数码" },
                    new Category { Name = "办公用品" }
                );
                db.SaveChanges();
            }

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
