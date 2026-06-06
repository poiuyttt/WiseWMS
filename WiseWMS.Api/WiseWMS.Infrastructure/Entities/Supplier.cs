using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiseWMS.Infrastructure.Entities
{
    /// <summary>
    /// 供应商
    /// </summary>
    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // 供应商名称
        public string Contact { get; set; } = string.Empty; // 联系人
        public string Phone { get; set; } = string.Empty; // 电话
        public string Address { get; set; } = string.Empty; // 地址
    }
}
