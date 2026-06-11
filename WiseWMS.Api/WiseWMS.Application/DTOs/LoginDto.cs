using System.ComponentModel.DataAnnotations;

namespace WiseWMS.Application.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "用户名不能为空")]
        [MaxLength(50, ErrorMessage = "用户名不能超过50个字符")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "密码不能为空")]
        [MaxLength(100, ErrorMessage = "密码不能超过100个字符")]
        public string Password { get; set; } = string.Empty;
    }
}
