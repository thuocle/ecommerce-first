namespace API_Test1.Services.CartServices
{
    public interface ICartServices
    {
        public List<CartItem> GetCartItems();
        public Task<MessageStatus> AddToCart(int productId);
        public Task<MessageStatus> RemoveFromCart(int productId);
        public Task<MessageStatus> IncreaseQuantity(int productId);
        public Task<MessageStatus> DecreaseQuantity(int productId);

    }
}
