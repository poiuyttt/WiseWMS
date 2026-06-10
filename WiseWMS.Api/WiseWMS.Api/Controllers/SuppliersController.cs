using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WiseWMS.Application.DTOs;
using WiseWMS.Application.Services.Interfaces;

namespace WiseWMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _supplierService;

        public SuppliersController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? keyword,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10
        )
        {
            var result = await _supplierService.GetAll(keyword, page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _supplierService.GetById(id);
            if (result == null)
                return NotFound(new { message = "供应商不存在" });
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSupplierDto dto)
        {
            var result = await _supplierService.Create(dto);
            if (result == null)
                return Conflict(new { message = "供应商名称已存在" });
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSupplierDto dto)
        {
            var result = await _supplierService.Update(id, dto);
            if (result == null)
                return NotFound(new { message = "供应商不存在" });
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _supplierService.Delete(id);
            if (!result)
                return NotFound(new { message = "供应商不存在或已被引用" });
            return Ok(new { message = "删除成功" });
        }
    }
}
