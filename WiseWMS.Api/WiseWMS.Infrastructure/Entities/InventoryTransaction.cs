using System;

namespace WiseWMS.Infrastructure.Entities
{
    /// <summary>
    /// 库存流水日志 — 每一次库存变动都记在这里
    /// </summary>
    public class InventoryTransaction
    {
        public long Id { get; set; } // 流水很多，用 long 防止不够
        public int ProductId { get; set; } // 哪个商品
        public string Type { get; set; } = string.Empty; // In / Out / Adjustment
        public int Quantity { get; set; } // 变动数量（正=入库，负=出库）
        public int StockBefore { get; set; } // 变动前库存
        public int StockAfter { get; set; } // 变动后库存
        public string OrderNo { get; set; } = string.Empty; // 关联单号
        public string Remark { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int OperatorId { get; set; } // 谁操作的
    }
}
