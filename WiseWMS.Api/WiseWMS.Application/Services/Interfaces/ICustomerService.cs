using WiseWMS.Application.DTOs;

namespace WiseWMS.Application.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<PagedResult<CustomerDto>> GetAll(string? keyword, int page, int pageSize);
        Task<List<CustomerDto>> GetAll();
        Task<CustomerDto?> GetById(int id);
        Task<CustomerDto?> Create(CreateCustomerDto dto);
        Task<CustomerDto?> Update(int id, UpdateCustomerDto dto);
        Task<bool> Delete(int id);
    }
}
