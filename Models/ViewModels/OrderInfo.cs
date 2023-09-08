namespace API_Test1.Models.ViewModels
{
    public class OrderInfo
    {
        public string? FullName { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? Phone { get; set; } = string.Empty;
        public string? Address { get; set; } = string.Empty;
        public int? PaymentID { get; set; }
    }
}
