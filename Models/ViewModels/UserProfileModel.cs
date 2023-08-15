using System.ComponentModel.DataAnnotations.Schema;

namespace API_Test1.Models.ViewModels
{
    public class UserProfileModel
    {
        [Required]
        public string? FullName { get; set; } = string.Empty;
        public string? UserName { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? Phone { get; set; } = string.Empty;
        public string? Address { get; set; } = string.Empty;
        public string? Avatar { get; set; } = string.Empty;
    }
}
