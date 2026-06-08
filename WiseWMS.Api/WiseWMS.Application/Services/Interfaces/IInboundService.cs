using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiseWMS.Application.DTOs;

namespace WiseWMS.Application.Services.Interfaces
{
    public interface IInboundService
    {
        Task<InboundOrderDto> Create(CreateInboundDto dto, int operatorId);
        Task<InboundOrderDto?> GetById(int id);
        Task<PagedResult<InboundOrderDto>> GetAll(string? keyword, int page, int pageSize);
    }
}
