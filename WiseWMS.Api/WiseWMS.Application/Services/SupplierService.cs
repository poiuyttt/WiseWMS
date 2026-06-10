using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WiseWMS.Application.DTOs;
using WiseWMS.Application.Services.Interfaces;
using WiseWMS.Infrastructure.Data;
using WiseWMS.Infrastructure.Entities;

namespace WiseWMS.Application.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly AppDbContext _db;
        private readonly ILogger<SupplierService> _logger;

        public SupplierService(AppDbContext db, ILogger<SupplierService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<PagedResult<SupplierDto>> GetAll(string? keyword, int page, int pageSize)
        {
            var query = _db.Suppliers.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(s =>
                    s.Name.Contains(keyword)
                    || s.Contact.Contains(keyword)
                    || s.Phone.Contains(keyword)
                );
            }

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(s => s.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new SupplierDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Contact = s.Contact,
                    Phone = s.Phone,
                    Address = s.Address,
                })
                .ToListAsync();

            return new PagedResult<SupplierDto>
            {
                Items = items,
                Total = total,
                Page = page,
                PageSize = pageSize,
            };
        }

        public async Task<List<SupplierDto>> GetAll()
        {
            return await _db
                .Suppliers.OrderByDescending(s => s.Id)
                .Select(s => new SupplierDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Contact = s.Contact,
                    Phone = s.Phone,
                    Address = s.Address,
                })
                .ToListAsync();
        }

        public async Task<SupplierDto?> GetById(int id)
        {
            var supplier = await _db.Suppliers.FindAsync(id);
            if (supplier == null)
                return null;

            return new SupplierDto
            {
                Id = supplier.Id,
                Name = supplier.Name,
                Contact = supplier.Contact,
                Phone = supplier.Phone,
                Address = supplier.Address,
            };
        }

        public async Task<SupplierDto?> Create(CreateSupplierDto dto)
        {
            bool exists = await _db.Suppliers.AnyAsync(s => s.Name == dto.Name);
            if (exists)
            {
                _logger.LogWarning("新增供应商失败：名称已存在：{Name}", dto.Name);
                return null;
            }

            var supplier = new Supplier
            {
                Name = dto.Name,
                Contact = dto.Contact,
                Phone = dto.Phone,
                Address = dto.Address,
            };

            await _db.Suppliers.AddAsync(supplier);
            await _db.SaveChangesAsync();

            _logger.LogInformation(
                "新增供应商成功：ID={Id}，名称={Name}",
                supplier.Id,
                supplier.Name
            );

            return new SupplierDto
            {
                Id = supplier.Id,
                Name = supplier.Name,
                Contact = supplier.Contact,
                Phone = supplier.Phone,
                Address = supplier.Address,
            };
        }

        public async Task<SupplierDto?> Update(int id, UpdateSupplierDto dto)
        {
            var supplier = await _db.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                _logger.LogWarning("供应商不存在：ID={Id}", id);
                return null;
            }

            supplier.Name = dto.Name;
            supplier.Contact = dto.Contact;
            supplier.Phone = dto.Phone;
            supplier.Address = dto.Address;

            await _db.SaveChangesAsync();

            _logger.LogInformation("更新供应商成功：ID={Id}", id);

            return new SupplierDto
            {
                Id = supplier.Id,
                Name = supplier.Name,
                Contact = supplier.Contact,
                Phone = supplier.Phone,
                Address = supplier.Address,
            };
        }

        public async Task<bool> Delete(int id)
        {
            var supplier = await _db.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                _logger.LogWarning("供应商不存在：ID={Id}", id);
                return false;
            }

            bool hasOrders = await _db.InboundOrders.AnyAsync(o => o.SupplierId == id);
            if (hasOrders)
            {
                _logger.LogWarning("删除供应商失败：已被入库单引用，ID={Id}", id);
                return false;
            }

            _db.Suppliers.Remove(supplier);
            await _db.SaveChangesAsync();

            _logger.LogInformation("删除供应商成功：ID={Id}", id);

            return true;
        }
    }
}
