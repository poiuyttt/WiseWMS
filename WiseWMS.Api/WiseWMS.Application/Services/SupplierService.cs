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
    public class SupplierService : ISupplierService
    {
        private readonly AppDbContext _db;
        private readonly ILogger<SupplierService> _logger;
        private readonly IMapper _mapper;

        public SupplierService(AppDbContext db, ILogger<SupplierService> logger, IMapper mapper)
        {
            _db = db;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<PagedResult<SupplierDto>> GetAll(string? keyword, int page, int pageSize)
        {
            var query = _db.Suppliers.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(s =>
                    s.Name.Contains(keyword)
                    || s.Contact.Contains(keyword)
                    || s.Phone.Contains(keyword)
                );

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(s => s.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<SupplierDto>(_mapper.ConfigurationProvider)
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
                .ProjectTo<SupplierDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<SupplierDto?> GetById(int id)
        {
            var supplier = await _db.Suppliers.FindAsync(id);
            return supplier == null ? null : _mapper.Map<SupplierDto>(supplier);
        }

        public async Task<SupplierDto?> Create(CreateSupplierDto dto)
        {
            if (await _db.Suppliers.AnyAsync(s => s.Name == dto.Name))
            {
                _logger.LogWarning("新增供应商失败：名称已存在：{Name}", dto.Name);
                return null;
            }

            var supplier = _mapper.Map<Supplier>(dto);
            _db.Suppliers.Add(supplier);
            await _db.SaveChangesAsync();
            _logger.LogInformation(
                "新增供应商成功：ID={Id}, 名称={Name}",
                supplier.Id,
                supplier.Name
            );
            return _mapper.Map<SupplierDto>(supplier);
        }

        public async Task<SupplierDto?> Update(int id, UpdateSupplierDto dto)
        {
            var supplier = await _db.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                _logger.LogWarning("供应商不存在：ID={Id}", id);
                return null;
            }

            _mapper.Map(dto, supplier);
            await _db.SaveChangesAsync();
            _logger.LogInformation("更新供应商成功：ID={Id}", id);
            return _mapper.Map<SupplierDto>(supplier);
        }

        public async Task<bool> Delete(int id)
        {
            var supplier = await _db.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                _logger.LogWarning("供应商不存在：ID={Id}", id);
                return false;
            }

            if (await _db.InboundOrders.AnyAsync(o => o.SupplierId == id))
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
