using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiseWMS.Application.DTOs
{
    public class CreateOutboundDto
    {
        public int CustomerId { get; set; } // 客户
        public string Remark { get; set; } = string.Empty; // 备注
        public List<CreateOutboundItemDto> Items { get; set; } = new(); // 出库明细
    }

    public class CreateOutboundItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal SalePrice { get; set; }
    }
}
