namespace WiseWMS.Application.DTOs
{
    public class InboundOrderDto
    {
        public int Id { get; set; }
        public string OrderNo { get; set; } = string.Empty; // 单号，如 IN20260606001
        public int SupplierId { get; set; } // 供应商 Id
        public string SupplierName { get; set; } = string.Empty;
        public int OperatorId { get; set; } // 操作人 Id
        public string OperatorName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; } // 入库总金额
        public string Remark { get; set; } = string.Empty; // 备注
        public DateTime CreatedAt { get; set; }
        public List<InboundItemDto> Items { get; set; } = new();
    }

    public class InboundItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductSpec { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
