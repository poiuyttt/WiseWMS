using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiseWMS.Application.DTOs
{
    public class CreateInboundDto
    {
        public int SupplierId { get; set; } // 供应商
        public string Remark { get; set; } = string.Empty; // 备注
        public List<CreateInboundItemDto> Items { get; set; } = new(); // 入库明细
    }

    public class CreateInboundItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
