using System.ComponentModel.DataAnnotations.Schema;

namespace API_Test1.Models.ViewModels
{
    public class UserProfileModel
    {
        [Required]
        public string? FullName { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Avatar { get; set; }
    }
}
