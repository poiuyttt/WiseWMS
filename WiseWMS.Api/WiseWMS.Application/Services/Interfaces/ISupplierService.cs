using WiseWMS.Application.DTOs;

namespace WiseWMS.Application.Services.Interfaces
{
    public interface ISupplierService
    {
        Task<PagedResult<SupplierDto>> GetAll(string? keyword, int page, int pageSize);
        Task<List<SupplierDto>> GetAll();
        Task<SupplierDto?> GetById(int id);
        Task<SupplierDto?> Create(CreateSupplierDto dto);
        Task<SupplierDto?> Update(int id, UpdateSupplierDto dto);
        Task<bool> Delete(int id);
    }
}
