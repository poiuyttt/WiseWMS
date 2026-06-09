using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiseWMS.Application.DTOs
{
    public class OutboundOrderDto
    {
        public int Id { get; set; }
        public string OrderNo { get; set; } = string.Empty; // 单号，如 OUT20260606001
        public int CustomerId { get; set; } // 客户 Id
        public string CustomerName { get; set; } = string.Empty;
        public int OperatorId { get; set; } // 操作人 Id
        public string OperatorName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; } // 出库总金额
        public string Remark { get; set; } = string.Empty; // 备注
        public DateTime CreatedAt { get; set; }
        public List<OutboundItemDto> Items { get; set; } = new();
    }

    public class OutboundItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductSpec { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal SalePrice { get; set; }
    }
}
