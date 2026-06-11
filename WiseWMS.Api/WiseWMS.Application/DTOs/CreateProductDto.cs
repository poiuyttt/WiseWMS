using System.ComponentModel.DataAnnotations;

namespace WiseWMS.Application.DTOs
{
    public class CreateProductDto
    {
        [Required(ErrorMessage = "商品名称不能为空")]
        [MaxLength(100, ErrorMessage = "商品名称不能超过100个字符")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "规格不能为空")]
        [MaxLength(100, ErrorMessage = "规格不能超过100个字符")]
        public string Spec { get; set; } = string.Empty;

        [Required(ErrorMessage = "单位不能为空")]
        [MaxLength(10, ErrorMessage = "单位不能超过10个字符")]
        public string Unit { get; set; } = string.Empty;

        [Required(ErrorMessage = "请选择分类")]
        public int CategoryId { get; set; }

        [Range(0.01, 999999, ErrorMessage = "价格必须大于0")]
        public decimal Price { get; set; }

        [Range(0, 999999, ErrorMessage = "预警库存不能小于0")]
        public int MinStock { get; set; }

        [MaxLength(500, ErrorMessage = "描述不能超过500个字符")]
        public string Description { get; set; } = string.Empty;
    }
}
