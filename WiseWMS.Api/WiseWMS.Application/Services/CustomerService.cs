using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WiseWMS.Application.DTOs;
using WiseWMS.Application.Services.Interfaces;
using WiseWMS.Infrastructure.Data;
using WiseWMS.Infrastructure.Entities;

namespace WiseWMS.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly AppDbContext _db;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(AppDbContext db, ILogger<CustomerService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<PagedResult<CustomerDto>> GetAll(string? keyword, int page, int pageSize)
        {
            var query = _db.Customers.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(c =>
                    c.Name.Contains(keyword)
                    || c.Contact.Contains(keyword)
                    || c.Phone.Contains(keyword)
                );
            }

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(c => c.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CustomerDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Contact = c.Contact,
                    Phone = c.Phone,
                    Address = c.Address,
                })
                .ToListAsync();

            return new PagedResult<CustomerDto>
            {
                Items = items,
                Total = total,
                Page = page,
                PageSize = pageSize,
            };
        }

        public async Task<List<CustomerDto>> GetAll()
        {
            return await _db
                .Customers.OrderByDescending(c => c.Id)
                .Select(c => new CustomerDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Contact = c.Contact,
                    Phone = c.Phone,
                    Address = c.Address,
                })
                .ToListAsync();
        }

        public async Task<CustomerDto?> GetById(int id)
        {
            var customer = await _db.Customers.FindAsync(id);
            if (customer == null)
                return null;

            return new CustomerDto
            {
                Id = customer.Id,
                Name = customer.Name,
                Contact = customer.Contact,
                Phone = customer.Phone,
                Address = customer.Address,
            };
        }

        public async Task<CustomerDto?> Create(CreateCustomerDto dto)
        {
            bool exists = await _db.Customers.AnyAsync(c => c.Name == dto.Name);
            if (exists)
            {
                _logger.LogWarning("新增客户失败：名称已存在：{Name}", dto.Name);
                return null;
            }

            var customer = new Customer
            {
                Name = dto.Name,
                Contact = dto.Contact,
                Phone = dto.Phone,
                Address = dto.Address,
            };

            await _db.Customers.AddAsync(customer);
            await _db.SaveChangesAsync();

            _logger.LogInformation(
                "新增客户成功：ID={Id}，名称={Name}",
                customer.Id,
                customer.Name
            );

            return new CustomerDto
            {
                Id = customer.Id,
                Name = customer.Name,
                Contact = customer.Contact,
                Phone = customer.Phone,
                Address = customer.Address,
            };
        }

        public async Task<CustomerDto?> Update(int id, UpdateCustomerDto dto)
        {
            var customer = await _db.Customers.FindAsync(id);
            if (customer == null)
            {
                _logger.LogWarning("客户不存在：ID={Id}", id);
                return null;
            }

            customer.Name = dto.Name;
            customer.Contact = dto.Contact;
            customer.Phone = dto.Phone;
            customer.Address = dto.Address;

            await _db.SaveChangesAsync();

            _logger.LogInformation("更新客户成功：ID={Id}", id);

            return new CustomerDto
            {
                Id = customer.Id,
                Name = customer.Name,
                Contact = customer.Contact,
                Phone = customer.Phone,
                Address = customer.Address,
            };
        }

        public async Task<bool> Delete(int id)
        {
            var customer = await _db.Customers.FindAsync(id);
            if (customer == null)
            {
                _logger.LogWarning("客户不存在：ID={Id}", id);
                return false;
            }

            bool hasOrders = await _db.OutboundOrders.AnyAsync(o => o.CustomerId == id);
            if (hasOrders)
            {
                _logger.LogWarning("删除客户失败：已被出库单引用，ID={Id}", id);
                return false;
            }

            _db.Customers.Remove(customer);
            await _db.SaveChangesAsync();

            _logger.LogInformation("删除客户成功：ID={Id}", id);

            return true;
        }
    }
}
