namespace WiseWMS.Infrastructure.Entities
{
    /// <summary>
    /// 出库明细
    /// </summary>
    public class OutboundItem
    {
        public int Id { get; set; }
        public int OutboundOrderId { get; set; } // 所属出库单 Id
        public int ProductId { get; set; } // 出库的商品 Id
        public int Quantity { get; set; } // 数量
        public decimal SalePrice { get; set; } // 售价（用于算销售额）

        public OutboundOrder? OutboundOrder { get; set; }
        public Product? Product { get; set; }
    }
}
