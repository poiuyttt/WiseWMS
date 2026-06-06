namespace WiseWMS.Infrastructure.Entities
{
    /// <summary>
    /// 入库明细
    /// </summary>
    public class InboundItem
    {
        public int Id { get; set; }
        public int InboundOrderId { get; set; } // 所属入库单 Id
        public int ProductId { get; set; } // 入库的商品 Id
        public int Quantity { get; set; } // 数量
        public decimal UnitPrice { get; set; } // 入库单价

        public InboundOrder? InboundOrder { get; set; }
        public Product? Product { get; set; }
    }
}
