using Newtonsoft.Json;
namespace API_Test1.Services.CartServices
{
    public class CartServices : ICartServices
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _dbContext;
        private const string CART_COOKIE_NAME = "CartItems";

        public CartServices(IHttpContextAccessor httpContextAccessor, ApplicationDbContext dbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
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
        //them vao gio hàng
        public async Task<MessageStatus> AddToCart(int productId)
        {
            var cartItems = GetCartItems();
            var existingItem = cartItems.FirstOrDefault(item => item.ProductId == productId);
            var product = await Task.FromResult(_dbContext.Products.FirstOrDefaultAsync(x=> x.ProductID == productId)); 
            if (existingItem != null && product != null)
            {
                existingItem.Quantity++;
            }
            if ((existingItem == null && product != null))
            {
                cartItems.Add(new CartItem { ProductId = productId, Quantity = 1, 
                        Price = product.Result.Price, ProductName = product.Result.NameProduct, DiscountPercentage= product.Result.Discount });
            }
            if (product == null)
                return MessageStatus.Failed;
            UpdateCartItems(cartItems);
            return MessageStatus.Success;
        }
        //xoa 1 san pham trong gio hang
        public MessageStatus RemoveFromCart(int productId)
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
        //giam so luong
        public MessageStatus DecreaseQuantity(int productId)
        {
            var cartItems = GetCartItems();
            var existingItem = cartItems.FirstOrDefault(item => item.ProductId == productId);

            if (existingItem != null && existingItem.Quantity > 1)
            {
                existingItem.Quantity--;
                UpdateCartItems(cartItems);
            }
            return MessageStatus.Success;
        }
        //tang so luong 
        public MessageStatus IncreaseQuantity(int productId)
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
        //tong so luong sp
        public int? GetTotalQuantity()
        {
            var cartItems = GetCartItems();
            int? totalQuantity = 0;
            foreach (var cartItem in cartItems)
            {
                totalQuantity += cartItem.Quantity;
            }
            return totalQuantity;
        }
        //tong tien goc cua gio hang
        public double? GetOriginalTotalPrice()
        {
            var cartItems = GetCartItems();
            double? originalTotalPrice = 0;
            foreach (var cartItem in cartItems)
            {
                originalTotalPrice += cartItem.Quantity * cartItem.Price;
            }
            return originalTotalPrice;
        }

        public double? GetTotalPrice()
        {
            var cartItems = GetCartItems();
            double? totalPrice = GetOriginalTotalPrice();
            foreach (var cartItem in cartItems)
            {
                int? discountPercentage = cartItem.DiscountPercentage;
                double? discountAmount = cartItem.Price * (discountPercentage / 100.0);
                totalPrice -= discountAmount * cartItem.Quantity;
            }
            return totalPrice;
        }
        //xoa tat ca gio hang
        public async Task<MessageStatus> ClearCart()
        {
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(-1)
            };

            _httpContextAccessor.HttpContext.Response.Cookies.Delete(CART_COOKIE_NAME);
            await Task.CompletedTask;

            return MessageStatus.Success;
        }

        public bool IsCartEmpty()
        {
            var cartItems = GetCartItems();
            return cartItems.Count == 0;
        }
    }
}
