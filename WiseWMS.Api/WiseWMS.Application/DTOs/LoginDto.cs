using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiseWMS.Application.DTOs
{
    public class LoginDto
    {
        public string Username { get; set; } = string.Empty; // 用户名
        public string Password { get; set; } = string.Empty; // 密码
    }
}
