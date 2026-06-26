using AutoMapper;
using AutoMapper.QueryableExtensions;
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
        private readonly IMapper _mapper;

        public CustomerService(AppDbContext db, ILogger<CustomerService> logger, IMapper mapper)
        {
            _db = db;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<PagedResult<CustomerDto>> GetAll(string? keyword, int page, int pageSize)
        {
            var query = _db.Customers.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(c =>
                    c.Name.Contains(keyword)
                    || c.Contact.Contains(keyword)
                    || c.Phone.Contains(keyword)
                );

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(c => c.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<CustomerDto>(_mapper.ConfigurationProvider)
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
                .ProjectTo<CustomerDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<CustomerDto?> GetById(int id)
        {
            var customer = await _db.Customers.FindAsync(id);
            return customer == null ? null : _mapper.Map<CustomerDto>(customer);
        }

        public async Task<CustomerDto?> Create(CreateCustomerDto dto)
        {
            if (await _db.Customers.AnyAsync(c => c.Name == dto.Name))
            {
                _logger.LogWarning("新增客户失败：名称已存在：{Name}", dto.Name);
                return null;
            }

            var customer = _mapper.Map<Customer>(dto);
            _db.Customers.Add(customer);
            await _db.SaveChangesAsync();
            _logger.LogInformation(
                "新增客户成功：ID={Id}, 名称={Name}",
                customer.Id,
                customer.Name
            );
            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task<CustomerDto?> Update(int id, UpdateCustomerDto dto)
        {
            var customer = await _db.Customers.FindAsync(id);
            if (customer == null)
            {
                _logger.LogWarning("客户不存在：ID={Id}", id);
                return null;
            }

            _mapper.Map(dto, customer);
            await _db.SaveChangesAsync();
            _logger.LogInformation("更新客户成功：ID={Id}", id);
            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task<bool> Delete(int id)
        {
            var customer = await _db.Customers.FindAsync(id);
            if (customer == null)
            {
                _logger.LogWarning("客户不存在：ID={Id}", id);
                return false;
            }

            if (await _db.OutboundOrders.AnyAsync(o => o.CustomerId == id))
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
