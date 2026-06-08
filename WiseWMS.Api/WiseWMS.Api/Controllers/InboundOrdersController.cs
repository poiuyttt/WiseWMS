using Microsoft.AspNetCore.Mvc;
using WiseWMS.Application.DTOs;
using WiseWMS.Application.Services.Interfaces;

namespace WiseWMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InboundOrdersController : ControllerBase
    {
        private readonly IInboundService _inboundService;

        public InboundOrdersController(IInboundService inboundService)
        {
            _inboundService = inboundService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? keyword,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10
        )
        {
            var result = await _inboundService.GetAll(keyword, page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _inboundService.GetById(id);
            if (result == null)
                return NotFound(new { message = "该入库订单不存在" });
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateInboundDto dto)
        {
            int operatorId = int.Parse(
                User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value
            );
            var result = await _inboundService.Create(dto, operatorId);
            return Ok(result);
        }
    }
}
