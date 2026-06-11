using System.ComponentModel.DataAnnotations;

namespace WiseWMS.Application.DTOs
{
    public class UpdateCustomerDto
    {
        [Required(ErrorMessage = "客户名称不能为空")]
        [MaxLength(100, ErrorMessage = "名称不能超过100个字符")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50, ErrorMessage = "联系人不能超过50个字符")]
        public string Contact { get; set; } = string.Empty;

        [MaxLength(20, ErrorMessage = "电话不能超过20个字符")]
        public string Phone { get; set; } = string.Empty;

        [MaxLength(200, ErrorMessage = "地址不能超过200个字符")]
        public string Address { get; set; } = string.Empty;
    }
}
