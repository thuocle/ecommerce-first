namespace API_Test1.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; } = string.Empty;
        public string? Address { get; set; } = string.Empty;
        public string? Avatar { get; set; } = string.Empty;
        public string? Status { get; set; } = string.Empty;
        public string? VerifyToken { get; set; } = string.Empty;
        public DateTime? VerifyTokenExpiry { get; set; } = null;
        public string? ResetPasswordToken { get; set; } = string.Empty;
        public DateTime? ResetPasswordTokenExpiry { get; set; } = null;
        public DateTime? CreateAt { get; set; } = null;
        public DateTime? UpdateAt { get; set; } = null;
    }
}
