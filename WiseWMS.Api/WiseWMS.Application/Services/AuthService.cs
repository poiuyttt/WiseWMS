using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using WiseWMS.Application.DTOs;
using WiseWMS.Application.Services.Interfaces;
using WiseWMS.Infrastructure.Data;
using WiseWMS.Infrastructure.Entities;

namespace WiseWMS.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthService> _logger;

        public AuthService(AppDbContext db, IConfiguration config, ILogger<AuthService> logger)
        {
            _dbContext = db;
            _config = config;
            _logger = logger;
        }

        public async Task<LoginResultDto?> Login(LoginDto dto)
        {
            User? user = await _dbContext.Users.FirstOrDefaultAsync(u =>
                u.Username == dto.Username
            );
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                _logger.LogWarning("用户 {Username} 登录失败：账号或密码错误", dto.Username);
                return null;
            }

            _logger.LogInformation("用户 {Username} 登录成功", user.Username);
            string token = GenerateJwt(user);

            return new LoginResultDto
            {
                Token = token,
                DisplayName = user.DisplayName,
                Role = user.Role,
            };
        }

        private string GenerateJwt(User user)
        {
            string key = _config["Jwt:Key"]!;
            string issuer = _config["Jwt:Issuer"]!;
            string audience = _config["Jwt:Audience"]!;
            int expireMinutes = int.Parse(_config["Jwt:ExpireMinutes"]!);

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
            };

            SymmetricSecurityKey signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                signingCredentials: new SigningCredentials(
                    signingKey,
                    SecurityAlgorithms.HmacSha256
                )
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
