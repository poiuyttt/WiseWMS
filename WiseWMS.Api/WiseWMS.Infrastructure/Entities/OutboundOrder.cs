using System;
using System.Collections.Generic;

namespace WiseWMS.Infrastructure.Entities
{
    /// <summary>
    /// 出库单
    /// </summary>
    public class OutboundOrder
    {
        public int Id { get; set; }
        public string OrderNo { get; set; } = string.Empty; // 单号，如 OUT20260606001
        public int CustomerId { get; set; } // 客户 Id
        public int OperatorId { get; set; } // 操作人 Id
        public decimal TotalAmount { get; set; } // 出库总金额
        public string Remark { get; set; } = string.Empty; // 备注
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Customer? Customer { get; set; }
        public User? Operator { get; set; }
        public List<OutboundItem> Items { get; set; } = new();
    }
}
