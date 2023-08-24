using API_Test1.Services.CartServices;
using API_Test1.Services.JwtServices;
using API_Test1.Services.PaymentServices.MOMO;
using Microsoft.CodeAnalysis;

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
        private readonly IMailServices _mailServices;
        private const string CART_COOKIE_NAME = "CartItems";
        private const string USER_COOKIE_NAME = "User";
        public OrderServices(IHttpContextAccessor httpContextAccessor, ICartServices cartServices, ApplicationDbContext dbContext, IConfiguration configuration, IMoMoServices moMoServices, IJwtServices jwtServices, IMailServices mailServices)
        {
            _httpContextAccessor = httpContextAccessor;
            _cartServices = cartServices;
            _dbContext = dbContext;
            _configuration = configuration;
            _moMoServices = moMoServices;
            _jwtServices = jwtServices;
            _mailServices = mailServices;
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
                double? discountAmount = cartItem.Price * (discountPercentage / 100.0);
                actualPrice -= discountAmount * cartItem.Quantity;
            }

            return actualPrice;
        }
        #endregion

        #region for admin
        //xem danh sách đơn hàng
        public async Task<PageInfo<Orders>> GetAllOrder(Pagination page)
        {
            var query = _dbContext.Orders
                .Select(x => new Orders
                {
                    OrderID = x.OrderID,
                    PaymentID = x.PaymentID,
                    UserId = x.UserId,
                    OriginalPrice = x.OriginalPrice,
                    ActualPrice = x.ActualPrice,
                    FullName = x.FullName,
                    Email = x.Email,
                    Phone = x.Phone,
                    Address = x.Address,
                    OrderStatusID = x.OrderStatusID,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt


                })
                .AsQueryable();
            var data = PageInfo<Orders>.ToPageInfo(page, query);
            page.TotalItem = await query.CountAsync();
            return new PageInfo<Orders>(page, data);
        }
        // xem danh sách chi tiết đơn 
        public async Task<PageInfo<OrderDetails>> GetAllOrderDetail(Pagination page)
        {
            var query = _dbContext.OrderDetails
                        .OrderByDescending(x => x.UpdatedAt)
                        .Select(x => new OrderDetails
                        {
                            OrderDetailID = x.OrderDetailID,
                            OrderID = x.OrderID,
                            ProductID = x.ProductID,
                            PriceTotal = x.PriceTotal,
                            Quantity = x.Quantity,
                            CreatedAt = x.CreatedAt

                        })
                        .AsQueryable();
            var data = PageInfo<OrderDetails>.ToPageInfo(page, query);
            page.TotalItem = await query.CountAsync();
            return new PageInfo<OrderDetails>(page, data);
        }
        //admin cập nhật các trangj thái đơn
        public async Task<MessageStatus> UpdateStatusOrderByAdmin(int statusId, string orderId)
        {
            var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.OrderID == orderId);
            if (order == null)
                return MessageStatus.Failed;

            var currentStatus = order.OrderStatusID;

            switch (statusId)
            {
                //đơn đã đặt -> đang chuẩn bị
                case OrderStatus.Preparing:
                    if (currentStatus == OrderStatus.Placed)
                        order.OrderStatusID = statusId;
                    break;
                //đơn đang chuẩn bị -> đnag vận chuyển
                case OrderStatus.Shipping:
                    if (currentStatus == OrderStatus.Preparing)
                        order.OrderStatusID = statusId;
                    break;
                // có yêu cầu hủy đơn => có thể hủy hoặc từ chối
                case OrderStatus.CancelRejected:
                case OrderStatus.Cancelled:
                    if (currentStatus == OrderStatus.CancelRequest)
                        order.OrderStatusID = statusId;
                    break;
                //cos yc trả hàng => có thể trả hoặc từ chối
                case OrderStatus.ReturnRejected:
                case OrderStatus.Returned:
                    if (currentStatus == OrderStatus.ReturnRequest)
                        order.OrderStatusID = statusId;
                    break;
                default:
                    return MessageStatus.Failed;
            }

            order.UpdatedAt = DateTime.Now;
            await _dbContext.SaveChangesAsync();
            return MessageStatus.Success;
        }
        #endregion

        //Đặt hàng 
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
            newOrder.OrderID = Guid.NewGuid().ToString();
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
            /*if (orderInfo.PaymentID == 5)
            {
                var payMoMo = await _moMoServices.CreatePaymentAsync(newOrder);
                if (payMoMo == null)
                {
                    return MessageStatus.Failed;
                }
                //gửi mail xác nhận
                _mailServices.SendMail(new MailDTOs()
                {
                    To = orderInfo.Email,
                    Body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{
            font-family: Arial, sans-serif;
            font-size: 14px;
            line-height: 1.5;
            color: #333333;
        }}
        
        .container {{
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
        }}

        .header {{
            background-color: #f5f5f5;
            padding: 10px;
            text-align: center;
        }}

        .content {{
            padding: 20px;
            background-color: #ffffff;
            border: 1px solid #dddddd;
        }}

        .token {{
            font-weight: bold;
            font-size: 18px;
            color: #ff0000;
        }}

        .footer {{
            padding: 10px;
            text-align: center;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h2>Đặt hàng thành công</h2>
        </div>
        
        <div class='content'>
            <p>
               <span> Hello ${orderInfo.FullName}!</span>
               <br>
               <span> Thank you for placing your order with [company’s name/e-store’s name]! We really appreciate that you chose our store, it means the world to us!</span>
               <br>
               <span> We want to let you know that we will inform you about the subsequent stages of order processing via separate emails.</span>
               <br>
               <span>Have a great day!</span>
               <br>
               
            </p>
        </div>
        
        <div class='footer'>
            <span><b><i>The Love at HappyLucky
</i></b></span>
        </div>
    </div>
    <img src=""https://www.wordstream.com/wp-content/uploads/2022/07/full-size-thank-you-for-your-order-images-13.png"" alt=""Cảm ơn bạn đã đặt hàng"" >
</body>
</html>",
                    Subject = "Đặt hàng thành công!"
                });
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
            }*/

            return MessageStatus.Success;
        }

        //xem chi tiết hóa đơn
        public async Task<IEnumerable<dynamic>> GetOrderDetail(string orderID)
        {
            var orderDetails = from orderDetail in _dbContext.OrderDetails
                               join product in _dbContext.Products on orderDetail.ProductID equals product.ProductID
                               where orderDetail.OrderID == orderID
                               select new
                               {
                                   orderDetail.CreatedAt,
                                   orderDetail.PriceTotal,
                                   orderDetail.Quantity,
                                   product.AvatarImageProduct,
                                   product.NameProduct,

                               };
            return orderDetails;
        }
        // cập nhật trạng thái đơn hàng for user
        public async Task<MessageStatus> UpdateStatusOrderByUser(int statusId, string orderId)
        {
            var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.OrderID == orderId);
            if (order == null)
                return MessageStatus.Failed;

            var currentStatus = order.OrderStatusID;

            switch (statusId)
            {
                //có thể hủy đơn khi đã đặt hàng và chuẩn bị hàng
                case OrderStatus.CancelRequest:
                    if (currentStatus == OrderStatus.Placed || currentStatus == OrderStatus.Preparing)
                        order.OrderStatusID = statusId;
                    break;
                //xác nhận đã nhận hàng khi đang vận chuyển => hoàn thành
                case OrderStatus.Delivered:
                    if (currentStatus == OrderStatus.Shipping)
                        order.OrderStatusID = (int)OrderStatus.Completed;
                    break;
                //yêu cầu hủy trả hàng khi đơn đã hoàn thành
                case OrderStatus.ReturnRequest:
                    if (currentStatus == OrderStatus.Completed)
                        order.OrderStatusID = statusId;
                    break;
                // khi đơn đã yêu cầu hủy, hoặc bị bên bán từ chôis => có thể chọn tiếp tục giao = đơn đang chuẩn bị
                case OrderStatus.ResumingDelivery:
                    if (currentStatus == OrderStatus.CancelRequest || currentStatus == OrderStatus.CancelRejected)
                        order.OrderStatusID = (int)OrderStatus.Preparing;
                    break;
                //đơn đang là yêu cầu trả hàng, hoặc bị bên bán từ chối hủy, có thể hủy trả => đơn hoàn thành 
                case OrderStatus.ReturnCancelled:
                    if (currentStatus == OrderStatus.ReturnRequest || currentStatus == OrderStatus.ReturnRejected)
                        order.OrderStatusID = (int)OrderStatus.Completed;
                    break;
                default:
                    return MessageStatus.Failed;
            }

            order.UpdatedAt = DateTime.Now;
            await _dbContext.SaveChangesAsync();
            return MessageStatus.Success;
        }
        //lấy ra tất cả đơn hàng của 1 user
        public async Task<PageInfo<dynamic>> GetAllOrderForUser(Pagination page, string userID)
        {
            var query = from order in _dbContext.Orders
                        join pay in _dbContext.Payments on order.PaymentID equals pay.PaymentID
                        join orderstt in _dbContext.OrderStatuses on order.OrderStatusID equals orderstt.OrderStatusID
                        where order.UserId == userID
                        select new 
                        { 
                            order.OrderID,
                            order.ActualPrice,
                            order.CreatedAt,
                            pay.PaymentMethod,
                            orderstt.StatusName
                        };
            var data = PageInfo<dynamic>.ToPageInfo(page, query);
            page.TotalItem = await query.CountAsync();
            return new PageInfo<dynamic>(page, data);
        }
        //tim hóa đơn theo id
        public async Task<IEnumerable<dynamic>> FindOrderById(string orderID)
        {
               var orderDetails = from order in _dbContext.Orders
                           join pay in _dbContext.Payments on order.PaymentID equals pay.PaymentID
                           join orderstt in _dbContext.OrderStatuses on order.OrderStatusID equals orderstt.OrderStatusID
                           where order.OrderID == orderID
                                  select new
                           {
                               order.OrderID,
                               order.ActualPrice,
                               order.CreatedAt,
                               pay.PaymentMethod,
                               orderstt.StatusName
                           };
            return orderDetails;
        }
    }
}
