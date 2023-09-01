namespace API_Test1.Models.DTOs
{
    public class RefreshToken
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expires { get; set; } 
        public DateTime Created { get; set; } 
    }
}
