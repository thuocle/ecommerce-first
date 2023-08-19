using System.ComponentModel;

namespace API_Test1.Models.ViewModels
{
    public class AccountManage
    {
        [Required, MinLength(10)]
        public string? FullName { get; set; } = string.Empty;
        [Required]
        public string? UserName { get; set; } = string.Empty;
        [Required, EmailAddress]
        public string? Email { get; set; } = string.Empty;
        [Required, MinLength(10), MaxLength(11)]
        public string? PhoneNumber { get; set; } = string.Empty;
        [Required, MinLength(6), PasswordPropertyText]
        public string? PassWord { get; set; } = string.Empty;
        [Required, MinLength(6), PasswordPropertyText, Compare("PassWord")]
        public string? ConfirmPassWord { get; set; } = string.Empty;
        [Required]
        public string? UserRoles { get; set; } = string.Empty;
        [Required]
        public string? Status { get; set; } = string.Empty;
        [Required]
        public IFormFile? Avatar { get; set; }
    }
}
