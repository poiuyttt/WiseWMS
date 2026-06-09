using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiseWMS.Application.DTOs;

namespace WiseWMS.Application.Services.Interfaces
{
    public interface IOutboundService
    {
        Task<OutboundOrderDto> Create(CreateOutboundDto dto, int operatorId);
        Task<OutboundOrderDto?> GetById(int id);
        Task<PagedResult<OutboundOrderDto>> GetAll(string? keyword, int page, int pageSize);
    }
}
