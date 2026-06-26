namespace WiseWMS.Application.DTOs
{
    public class CreateInboundDto
    {
        public int SupplierId { get; set; }

        public string Remark { get; set; } = string.Empty;

        public List<CreateInboundItemDto> Items { get; set; } = new();
    }

    public class CreateInboundItemDto
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }
    }
}
