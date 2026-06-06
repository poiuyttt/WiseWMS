using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiseWMS.Application.DTOs;

namespace WiseWMS.Application.Services.Interfaces
{
    /// <summary>
    /// 认证服务接口
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// 登录：校验账号密码，返回 JWT 令牌
        /// </summary>
        LoginResultDto? Login(LoginDto dto);
    }
}
