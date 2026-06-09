using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiseWMS.Application.DTOs
{
    public class DashboardDto
    {
        public int TotalProducts { get; set; }
        public int TotalStock { get; set; }
        public int TodayInbound { get; set; }
        public int TodayOutbound { get; set; }
        public int LowStockCount { get; set; }
    }
}
