using System.ComponentModel;

namespace API_Test1.Models.ViewModels
{
    public class AccountForm
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
        public string? Password { get; set; } = string.Empty;
        [Required, MinLength(6), PasswordPropertyText, Compare("Password")]
        public string? ConfirmPassword { get; set; } = string.Empty;
        [Required]
        public string? UserRoles { get; set; } = string.Empty;
        [Required]
        public int? Status { get; set; }
        [Required]
        public IFormFile? Avatar { get; set; }
    }
}
