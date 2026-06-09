using Microsoft.AspNetCore.Mvc;
using WiseWMS.Application.DTOs;
using WiseWMS.Application.Services.Interfaces;

namespace WiseWMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OutboundOrderController : ControllerBase
    {
        private readonly IOutboundService _outboundService;

        public OutboundOrderController(IOutboundService outboundService)
        {
            _outboundService = outboundService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? keyword,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10
        )
        {
            var result = await _outboundService.GetAll(keyword, page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _outboundService.GetById(id);

            if (result == null)
                return NotFound(new { message = "该出库订单不存在" });
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOutboundDto dto)
        {
            int operatorId = int.Parse(
                User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value
            );
            var result = await _outboundService.Create(dto, operatorId);
            return Ok(result);
        }
    }
}
