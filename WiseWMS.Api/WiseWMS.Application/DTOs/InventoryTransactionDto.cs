using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiseWMS.Application.DTOs
{
    public class InventoryTransactionDto
    {
        public long Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductSpec { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int StockBefore { get; set; }
        public int StockAfter { get; set; }
        public string OrderNo { get; set; } = string.Empty;
        public string OperatorName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
