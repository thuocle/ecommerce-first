namespace API_Test1.Models.ViewModels
{
    public class UserProfileForm
    {
        public string? FullName { get; set; } = string.Empty;
        public string? Phone { get; set; } = string.Empty;
        public string? Address { get; set; } = string.Empty;
        public IFormFile? Avatar { get; set; } 
    }
}
