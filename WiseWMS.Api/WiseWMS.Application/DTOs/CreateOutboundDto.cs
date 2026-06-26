namespace WiseWMS.Application.DTOs
{
    public class CreateOutboundDto
    {
        public int CustomerId { get; set; }

        public string Remark { get; set; } = string.Empty;

        public List<CreateOutboundItemDto> Items { get; set; } = new();
    }

    public class CreateOutboundItemDto
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal SalePrice { get; set; }
    }
}
