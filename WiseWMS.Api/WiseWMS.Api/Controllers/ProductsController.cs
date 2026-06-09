using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WiseWMS.Application.DTOs;
using WiseWMS.Application.Services.Interfaces;

namespace WiseWMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? keyword,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10
        )
        {
            var result = await _productService.GetAll(keyword, page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _productService.GetById(id);
            if (result == null)
                return NotFound(new { message = "商品不存在" });

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            var result = await _productService.Create(dto);
            if (result == null)
                return Conflict(new { message = "商品名称已存在" });
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
        {
            var result = await _productService.Update(id, dto);
            if (result == null)
                return NotFound(new { message = "商品不存在" });
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productService.Delete(id);
            if (!result)
                return NotFound(new { message = "商品不存在" });
            return Ok(new { message = "删除成功" });
        }

        [HttpGet("export")]
        public async Task<IActionResult> Export()
        {
            var products = await _productService.GetAll();

            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("商品列表");

            sheet.Cell(1, 1).Value = "商品名称";
            sheet.Cell(1, 2).Value = "规格";
            sheet.Cell(1, 3).Value = "单位";
            sheet.Cell(1, 4).Value = "分类";
            sheet.Cell(1, 5).Value = "售价";
            sheet.Cell(1, 6).Value = "库存";
            sheet.Cell(1, 7).Value = "预警线";
            sheet.Cell(1, 8).Value = "创建时间";

            for (int i = 0; i < products.Count; i++)
            {
                var p = products[i];
                int row = i + 2;
                sheet.Cell(row, 1).Value = p.Name;
                sheet.Cell(row, 2).Value = p.Spec;
                sheet.Cell(row, 3).Value = p.Unit;
                sheet.Cell(row, 4).Value = p.CategoryName;
                sheet.Cell(row, 5).Value = p.Price;
                sheet.Cell(row, 6).Value = p.Stock;
                sheet.Cell(row, 7).Value = p.MinStock;
                sheet.Cell(row, 8).Value = p.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
            }

            sheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "商品列表.xlsx"
            );
        }
    }
}
