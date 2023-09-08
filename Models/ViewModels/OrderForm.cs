namespace API_Test1.Models.ViewModels
{
    public class OrderForm
    {
        public string? FullName { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? OrderId { get; set; } = string.Empty;
        public string? OrderInfo { get; set; } = string.Empty;
        public double? Amount { get; set; }

    }
}
