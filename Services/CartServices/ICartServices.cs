namespace API_Test1.Services.CartServices
{
    public interface ICartServices
    {
        public List<CartItem> GetCartItems();
        public Task<MessageStatus> AddToCart(int productId);
        public MessageStatus RemoveFromCart(int productId);
        public MessageStatus IncreaseQuantity(int productId);
        public MessageStatus DecreaseQuantity(int productId);
        public int GetTotalQuantity();
        public double GetOriginalTotalPrice();
        public double GetTotalPrice();
        public bool ClearCart();
        public bool IsCartEmpty();
    }
}
