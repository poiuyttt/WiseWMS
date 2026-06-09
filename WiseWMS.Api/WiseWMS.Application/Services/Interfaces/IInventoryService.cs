using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiseWMS.Application.DTOs;

namespace WiseWMS.Application.Services.Interfaces
{
    public interface IInventoryService
    {
        Task<PagedResult<InventoryTransactionDto>> GetTransactions(
            int productId,
            int page,
            int pageSize
        );
        Task<List<ProductDto>> GetLowStockProducts();
    }
}
