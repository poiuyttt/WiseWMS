using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiseWMS.Application.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // 商品名称
        public string Spec { get; set; } = string.Empty; // 规格（如 500ml/瓶）
        public string Unit { get; set; } = string.Empty; // 单位（如 箱、kg）
        public int CategoryId { get; set; } // 分类 Id
        public string CategoryName { get; set; } = string.Empty; // 分类名称
        public decimal Price { get; set; } // 售价
        public int Stock { get; set; } // 当前库存
        public int MinStock { get; set; } // 最低库存预警线
        public string Description { get; set; } = string.Empty; // 商品描述
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // 创建时间}
    }
}
