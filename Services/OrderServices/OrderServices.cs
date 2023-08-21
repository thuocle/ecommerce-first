using API_Test1.Services.CartServices;
using API_Test1.Services.PaymentServices.MOMO;

namespace API_Test1.Services.OrderServices
{
    public class OrderServices : IOrderServices
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICartServices _cartServices;
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IMoMoServices _moMoServices;
        private const string CART_COOKIE_NAME = "CartItems";
        private const string USER_COOKIE_NAME = "jwt";
        public OrderServices(IHttpContextAccessor httpContextAccessor, ICartServices cartServices, ApplicationDbContext dbContext, IConfiguration configuration, IMoMoServices moMoServices)
        {
            _httpContextAccessor = httpContextAccessor;
            _cartServices = cartServices;
            _dbContext = dbContext;
            _configuration = configuration;
            _moMoServices = moMoServices;
        }
        public async Task<MessageStatus> CreateOrder(OrderInfo orderInfo)
        {
            var cartItems = _cartServices.GetCartItems();
            var newOrder = new Orders();
            // Kiểm tra xem có tồn tại cookie người dùng không
            if (_httpContextAccessor.HttpContext.Request.Cookies.TryGetValue(USER_COOKIE_NAME, out var token))
            {
                // Giải mã token
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]); // Thay thế YOUR_SECRET_KEY bằng khóa bí mật của bạn
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };

                try
                {
                    var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out _);

                    // Trích xuất thông tin người dùng từ các claim
                    var userNameClaim = claimsPrincipal.FindFirst("username");
                    if (userNameClaim != null)
                    {
                        var userName = userNameClaim.Value;
                        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserName == userName);
                        newOrder.UserId = user.Id;
                    }
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi khi giải mã token thất bại
                    // ...
                }
            }

            // Xử lý thông tin giỏ hàng và tính toán thành hóa đơn
            var originalPrice = CalculateOriginalPrice(cartItems);
            var actualPrice = CalculateActualPrice(originalPrice, cartItems);

            // Lưu thông tin hóa đơn vào 1 đối tượng
            newOrder.OrderID =Guid.NewGuid().ToString();
            newOrder.FullName = orderInfo.FullName;
            newOrder.Email = orderInfo.Email;
            newOrder.Phone = orderInfo.Phone;
            newOrder.Address = orderInfo.Address;
            newOrder.OriginalPrice = originalPrice;
            newOrder.ActualPrice = actualPrice;
            newOrder.PaymentID = orderInfo.PaymentID;
            newOrder.OrderStatusID = 1;
            newOrder.CreatedAt = DateTime.Now;
            
            // Xử lý các mục giỏ hàng, ví dụ: lưu các mục giỏ hàng vào bảng OrderItems
            //xử lý thanh toán
            if(orderInfo.PaymentID == 1)
            {
                var payMoMo = await _moMoServices.CreatePaymentAsync(newOrder);
                if(payMoMo == null)
                {
                    return MessageStatus.Failed;
                }
            }
            _dbContext.Orders.Add(newOrder);
            await _dbContext.SaveChangesAsync();

            // Lưu thông tin chi tiết đơn hàng vào bảng "OrderItems"
            foreach (var cartItem in cartItems)
            {
                var orderItem = new OrderDetails
                {
                    OrderID = newOrder.OrderID,
                    ProductID = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    PriceTotal = cartItem.Price,
                    CreatedAt = DateTime.Now
                };

                _dbContext.OrderDetails.Add(orderItem);

                // Cập nhật số lượng còn lại của sản phẩm sau khi mua thành công
                var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductID == cartItem.ProductId);
                if (product != null)
                {
                    product.Quantity -= cartItem.Quantity;
                }
            }
            await _dbContext.SaveChangesAsync();

            // Xóa cookie giỏ hàng
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(CART_COOKIE_NAME);

            return MessageStatus.Success;
        }
        private double? CalculateOriginalPrice(List<CartItem> cartItems)
        {
            // Tính toán giá trị tổng cộng của các mục giỏ hàng
            double? originalPrice = 0;

            foreach (var cartItem in cartItems)
            {
                originalPrice += cartItem.Quantity * cartItem.Price;
            }

            return originalPrice;
        }

        private double? CalculateActualPrice(double? originalPrice, List<CartItem> cartItems)
        {
            // Xử lý logic tính toán giá trị thực tế của hóa đơn
            double? actualPrice = originalPrice;

            foreach (var cartItem in cartItems)
            {
                int? discountPercentage = cartItem.DiscountPercentage;
                double? discountAmount = cartItem.Price * discountPercentage;
                actualPrice -= discountAmount * cartItem.Quantity;
            }

            return actualPrice;
        }
    }
}
