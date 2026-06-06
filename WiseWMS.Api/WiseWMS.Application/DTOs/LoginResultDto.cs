using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiseWMS.Application.DTOs
{
    public class LoginResultDto
    {
        public string Token { get; set; } = string.Empty; // 登录成功后返回的 JWT 令牌
        public string DisplayName { get; set; } = string.Empty; // 用户显示名称
        public string Role { get; set; } = string.Empty; // 用户角色，如 Admin / User
    }
}
