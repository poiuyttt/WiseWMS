using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WiseWMS.Infrastructure.Entities;

namespace WiseWMS.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        // DbSet<T> 表示一张表，属性名就是表名
        public DbSet<User> Users => Set<User>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Supplier> Suppliers => Set<Supplier>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<InboundOrder> InboundOrders => Set<InboundOrder>();
        public DbSet<InboundItem> InboundItems => Set<InboundItem>();
        public DbSet<OutboundOrder> OutboundOrders => Set<OutboundOrder>();
        public DbSet<OutboundItem> OutboundItems => Set<OutboundItem>();
        public DbSet<InventoryTransaction> InventoryTransactions => Set<InventoryTransaction>();

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(e =>
            {
                e.ToTable("users");
                e.Property(u => u.Username).HasMaxLength(50);
                e.Property(u => u.PasswordHash).HasMaxLength(200);
                e.Property(u => u.DisplayName).HasMaxLength(50);
                e.Property(u => u.Role).HasMaxLength(20);
            });

            modelBuilder.Entity<Category>(e =>
            {
                e.ToTable("categories");
                e.Property(c => c.Name).HasMaxLength(100);
            });

            modelBuilder.Entity<Supplier>(e =>
            {
                e.ToTable("suppliers");
                e.Property(s => s.Name).HasMaxLength(100);
                e.Property(s => s.Contact).HasMaxLength(50);
                e.Property(s => s.Phone).HasMaxLength(20);
                e.Property(s => s.Address).HasMaxLength(200);
            });

            modelBuilder.Entity<Customer>(e =>
            {
                e.ToTable("customers");
                e.Property(c => c.Name).HasMaxLength(100);
                e.Property(c => c.Contact).HasMaxLength(50);
                e.Property(c => c.Phone).HasMaxLength(20);
                e.Property(c => c.Address).HasMaxLength(200);
            });

            modelBuilder.Entity<Product>(e =>
            {
                e.ToTable("products");
                e.Property(p => p.Name).HasMaxLength(100);
                e.Property(p => p.Spec).HasMaxLength(100);
                e.Property(p => p.Unit).HasMaxLength(10);
                e.Property(p => p.Price).HasColumnType("decimal(18,2)");
                e.Property(p => p.Description).HasMaxLength(500);
            });

            modelBuilder.Entity<InboundOrder>(e =>
            {
                e.ToTable("inbound_orders");
                e.Property(o => o.OrderNo).HasMaxLength(50);
                e.Property(o => o.Remark).HasMaxLength(500);
                e.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<InboundItem>(e =>
            {
                e.ToTable("inbound_items");
                e.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<OutboundOrder>(e =>
            {
                e.ToTable("outbound_orders");
                e.Property(o => o.OrderNo).HasMaxLength(50);
                e.Property(o => o.Remark).HasMaxLength(500);
                e.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<OutboundItem>(e =>
            {
                e.ToTable("outbound_items");
                e.Property(i => i.SalePrice).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<InventoryTransaction>(e =>
            {
                e.ToTable("inventory_transactions");
                e.Property(t => t.Type).HasMaxLength(10);
                e.Property(t => t.OrderNo).HasMaxLength(50);
                e.Property(t => t.Remark).HasMaxLength(200);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
