using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiseWMS.Application.DTOs;

namespace WiseWMS.Application.Services.Interfaces
{
    public interface IProductService
    {
        Task<PagedResult<ProductDto>> GetAll(string? keyword, int page, int pageSize);
        Task<List<ProductDto>> GetAll();
        Task<ProductDto?> GetById(int id);
        Task<ProductDto?> Create(CreateProductDto dto);
        Task<ProductDto?> Update(int id, UpdateProductDto dto);
        Task<bool> Delete(int id);
    }
}
