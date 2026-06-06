using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiseWMS.Infrastructure.Entities
{
    /// <summary>
    /// 商品分类
    /// </summary>
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // 分类名称
        public int? ParentId { get; set; } // 父级分类 Id（顶级分类为 null）
    }
}
