using API_Test1.Base;

namespace API_Test1.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; } = string.Empty;
        public string? Address { get; set; } = string.Empty;
        public string? Avatar { get; set; } = string.Empty;
        public int? Status { get; set; }
        public string? VerifyToken { get; set; } = string.Empty;
        public DateTime? VerifyTokenExpiry { get; set; } = null; 
        public string? RefreshToken { get; set; } = string.Empty;
        public DateTime? RefreshTokenExpiry { get; set; } = DateTime.Now;
        public DateTime? RefreshTokenCreated { get; set; } = DateTime.Now;
        public string? ResetPasswordToken { get; set; } = string.Empty;
        public DateTime? ResetPasswordTokenExpiry { get; set; } = null;
        public DateTime? CreateAt { get; set; } = DateTime.Now;
        public DateTime? UpdateAt { get; set; } = null;
        public bool RefreshTokenIsValid(string refreshToken)
        {
            return RefreshToken == refreshToken && RefreshTokenExpiry > DateTime.UtcNow;
        }
        public void UpdateRefreshToken(string refreshToken)
        {
            RefreshToken = refreshToken;
            RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            RefreshTokenCreated = DateTime.Now;
        }
    }
}
