using API_Test1.Services.CartServices;
using API_Test1.Services.JwtServices;
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
        private readonly IJwtServices _jwtServices;
        private const string CART_COOKIE_NAME = "CartItems";
        private const string USER_COOKIE_NAME = "User";
        public OrderServices(IHttpContextAccessor httpContextAccessor, ICartServices cartServices, ApplicationDbContext dbContext, IConfiguration configuration, IMoMoServices moMoServices, IJwtServices jwtServices)
        {
            _httpContextAccessor = httpContextAccessor;
            _cartServices = cartServices;
            _dbContext = dbContext;
            _configuration = configuration;
            _moMoServices = moMoServices;
            _jwtServices = jwtServices;
        }
        #region private
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
                double? discountAmount = cartItem.Price * (discountPercentage/100);
                actualPrice -= discountAmount * cartItem.Quantity;
            }

            return actualPrice;
        }
        #endregion
        #region for admin
        public async Task<PageInfo<Orders>> GetAllOrder(Pagination page)
        {
            var query = _dbContext.Orders.AsQueryable();
            var data = PageInfo<Orders>.ToPageInfo(page, query);
            page.TotalItem = await query.CountAsync();
            return new PageInfo<Orders>(page, data);
        }
        public async Task<PageInfo<OrderDetails>> GetAllOrderDetail(Pagination page)
        {
            var query = _dbContext.OrderDetails.AsQueryable();
            var data = PageInfo<OrderDetails>.ToPageInfo(page, query);
            page.TotalItem = await query.CountAsync();
            return new PageInfo<OrderDetails>(page, data);
        }
        #endregion

        public async Task<MessageStatus> CreateOrder(OrderInfo orderInfo)
        {
            var cartItems = _cartServices.GetCartItems();
            var newOrder = new Orders();
            // Kiểm tra xem user co login khi đặt hàng không: có -> lưu id, không-> mua không lưu id
            if (_jwtServices.IsUserLoggedIn())
            {
                newOrder.UserId = _jwtServices.GetUserId();
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
            //lưu đon hàng v
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
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
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

       
        public async Task<IEnumerable<OrderDetails>> GetOrderDetail(string orderID)
        {
            var query = await Task.FromResult(_dbContext.OrderDetails.Where(x => x.OrderID == orderID).AsQueryable());
            return query;
        }

        public async Task<MessageStatus> UpdateStatusOrder(int statusId, string orderId)
        {
            var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.OrderID == orderId);
            if (order == null)
            {
                return MessageStatus.Failed;
            }
            order.UpdatedAt = DateTime.Now;
            order.OrderStatusID = statusId;
            await _dbContext.SaveChangesAsync();

            return MessageStatus.Success;
        }

        public async Task<PageInfo<Orders>> GetAllOrderForUser(Pagination page, string userID)
        {
            var query = _dbContext.Orders
                .Where(o => o.UserId == userID)
                .AsQueryable();
            var data = PageInfo<Orders>.ToPageInfo(page, query);
            page.TotalItem = await query.CountAsync();
            return new PageInfo<Orders>(page, data);
        }
        public async Task<IEnumerable<OrderDetails>> FindOrderById(string orderID)
        {

            var orderDetails = await _dbContext.OrderDetails
                .Where(od => od.OrderID == orderID)
                .ToListAsync();
            return orderDetails;
        }


    }
}
