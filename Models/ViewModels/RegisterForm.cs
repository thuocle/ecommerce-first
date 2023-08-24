using System.ComponentModel;

namespace API_Test1.Models.ViewModels
{
    public class RegisterForm
    {
        [Required, MinLength(10)]
        public string? FullName { get; set; } = string.Empty;
        [Required]
        public string? UserName { get; set; } = string.Empty;
        [Required, EmailAddress]
        public string? Email { get; set; } = string.Empty;
        [Required, MinLength(6), PasswordPropertyText]
        public string? Password { get; set; } = string.Empty;
        [Required, MinLength(6), PasswordPropertyText, Compare("Password")]
        public string? ConfirmPassword { get; set; } = string.Empty;
    }
}
