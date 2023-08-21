namespace API_Test1.Models.DTOs
{
    public class CartItem
    {
            public int? ProductId { get; set; }
            public string? ProductName { get; set; }
            public double? Price { get; set; }
            public int? DiscountPercentage { get; set; } // Tỷ lệ giảm giá cho sản phẩm
            public int? Quantity { get; set; }
    }
}
