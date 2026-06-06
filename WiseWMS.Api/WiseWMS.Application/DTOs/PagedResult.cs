using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiseWMS.Application.DTOs
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new(); // 当前页的数据
        public int Total { get; set; } // 总记录数
        public int Page { get; set; } // 当前页码
        public int PageSize { get; set; } // 每页条数
        public int TotalPages => (int)Math.Ceiling((double)Total / PageSize); // 总页数（自动算）
    }
}
