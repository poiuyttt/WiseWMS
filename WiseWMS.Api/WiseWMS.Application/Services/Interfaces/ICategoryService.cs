using WiseWMS.Application.DTOs;

namespace WiseWMS.Application.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetAll();
        Task<CategoryDto?> Create(CreateCategoryDto dto);
        Task<bool> Delete(int id);
    }
}
