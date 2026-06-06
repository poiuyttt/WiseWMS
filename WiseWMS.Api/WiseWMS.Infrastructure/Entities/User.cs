using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiseWMS.Infrastructure.Entities
{
    /// <summary>
    /// 用户表
    /// </summary>
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty; // 登录名
        public string PasswordHash { get; set; } = string.Empty; // BCrypt 加密后的密码
        public string DisplayName { get; set; } = string.Empty; // 显示名
        public string Role { get; set; } = "Operator"; // Admin 或 Operator
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // 创建时间
    }
}
