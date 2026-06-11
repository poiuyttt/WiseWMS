using System.ComponentModel.DataAnnotations;

namespace WiseWMS.Application.DTOs
{
    public class CreateInboundDto
    {
        [Required(ErrorMessage = "请选择供应商")]
        public int SupplierId { get; set; }

        [MaxLength(500, ErrorMessage = "备注不能超过500个字符")]
        public string Remark { get; set; } = string.Empty;

        [MinLength(1, ErrorMessage = "请添加至少一个商品")]
        public List<CreateInboundItemDto> Items { get; set; } = new();
    }

    public class CreateInboundItemDto
    {
        [Required(ErrorMessage = "请选择商品")]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "数量必须大于0")]
        public int Quantity { get; set; }

        [Range(0.01, 999999, ErrorMessage = "单价必须大于0")]
        public decimal UnitPrice { get; set; }
    }
}
