using Newtonsoft.Json;

namespace API_Test1.Services.CartServices
{
    public class CartServices : ICartServices
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string CART_COOKIE_NAME = "CartItems";

        public CartServices(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        #region private
        private void UpdateCartItems(List<CartItem> cartItems)
        {
            var cookieValue = JsonConvert.SerializeObject(cartItems);

            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddMonths(7),
                HttpOnly = true,
                SameSite = SameSiteMode.Lax
            };

            _httpContextAccessor.HttpContext.Response.Cookies.Append(CART_COOKIE_NAME, cookieValue, cookieOptions);
        }
        #endregion
        public List<CartItem> GetCartItems()
        {
            var cookieValue = _httpContextAccessor.HttpContext.Request.Cookies[CART_COOKIE_NAME];

            if (!string.IsNullOrEmpty(cookieValue))
            {
                var cartItems = JsonConvert.DeserializeObject<List<CartItem>>(cookieValue);
                return cartItems;
            }
            return new List<CartItem>();
        }
        public async Task<MessageStatus> AddToCart(int productId)
        {
            var cartItems = GetCartItems();
            var existingItem = cartItems.FirstOrDefault(item => item.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                cartItems.Add(new CartItem { ProductId = productId, Quantity = 1 });
            }

            UpdateCartItems(cartItems);
            return MessageStatus.Success;
        }
        public async Task<MessageStatus> RemoveFromCart(int productId)
        {
            var cartItems = GetCartItems();
            var existingItem = cartItems.FirstOrDefault(item => item.ProductId == productId);

            if (existingItem != null)
            {
                cartItems.Remove(existingItem);
                UpdateCartItems(cartItems);
            }
            return MessageStatus.Success;
        }

        public async Task<MessageStatus> DecreaseQuantity(int productId)
        {
            var cartItems = GetCartItems();
            var existingItem = cartItems.FirstOrDefault(item => item.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity--;

                if (existingItem.Quantity <= 0)
                {
                    cartItems.Remove(existingItem);
                }

                UpdateCartItems(cartItems);
            }
            return MessageStatus.Success;
        }
        public async Task<MessageStatus> IncreaseQuantity(int productId)
        {
            var cartItems = GetCartItems();
            var existingItem = cartItems.FirstOrDefault(item => item.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity++;
                UpdateCartItems(cartItems);
            }
            return MessageStatus.Success;
        }
    }
}
