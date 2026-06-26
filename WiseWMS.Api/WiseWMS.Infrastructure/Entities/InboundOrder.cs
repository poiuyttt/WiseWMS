namespace WiseWMS.Infrastructure.Entities
{
    /// <summary>
    /// 入库单
    /// </summary>
    public class InboundOrder
    {
        public int Id { get; set; }
        public string OrderNo { get; set; } = string.Empty; // 单号，如 IN20260606001
        public int SupplierId { get; set; } // 供应商 Id
        public int OperatorId { get; set; } // 操作人 Id
        public decimal TotalAmount { get; set; } // 入库总金额
        public string Remark { get; set; } = string.Empty; // 备注
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // 导航属性 EF Core 自动关联
        public Supplier? Supplier { get; set; }
        public User? Operator { get; set; }
        public List<InboundItem> Items { get; set; } = new();
    }
}
