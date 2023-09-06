using API_Test1.Models.ViewModels;
using API_Test1.Services.CartServices;
using API_Test1.Services.JwtServices;
using API_Test1.Services.PaymentServices.MOMO;
using API_Test1.Services.PaymentServices.MOMO.Model;
using Microsoft.CodeAnalysis;
using System.Net.Http;

namespace API_Test1.Services.OrderServices
{
    public class OrderServices : IOrderServices
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICartServices _cartServices;
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IJwtServices _jwtServices;
        private readonly IMailServices _mailServices;
        private readonly HttpClient _httpClient;
        private const string CART_COOKIE_NAME = "CartItems";
        private const string USER_COOKIE_NAME = "User";
        public OrderServices(IHttpContextAccessor httpContextAccessor, ICartServices cartServices, ApplicationDbContext dbContext, IConfiguration configuration,  IJwtServices jwtServices, IMailServices mailServices, HttpClient httpClient)
        {
            _httpContextAccessor = httpContextAccessor;
            _cartServices = cartServices;
            _dbContext = dbContext;
            _configuration = configuration;
            _jwtServices = jwtServices;
            _mailServices = mailServices;
            _httpClient = httpClient;
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


        public async Task<OrderForm> CreateOrder(OrderInfo orderInfo)
        {
            var cartItems = _cartServices.GetCartItems();
            var newOrder = BuildOrderFromOrderInfo(orderInfo);
            //xác thực login
            if (_jwtServices.IsUserLoggedIn())
            {
                newOrder.UserId = _jwtServices.GetUserId();
            }

            var originalPrice = _cartServices.GetOriginalTotalPrice();
            var actualPrice = _cartServices.GetTotalPrice();

            newOrder.OriginalPrice = originalPrice;
            newOrder.ActualPrice = actualPrice;
            newOrder.OrderID = Guid.NewGuid().ToString();

            await SaveOrderAndOrderItems(newOrder, cartItems);

            var newPay = new OrderForm { Amount = actualPrice, FullName = newOrder.FullName, OrderId = newOrder.OrderID};
                
            return newPay;
        }
        // xóa đơn khi thanh toán thất bại => gọi ở api return từ momo
        public async Task DeleteOrderAndOrderDetail(string orderId)
        {
            var order = await _dbContext.Orders.FirstOrDefaultAsync(x => x.OrderID == orderId);

            if (order != null)
            {
                _dbContext.Orders.Remove(order);
                await _dbContext.SaveChangesAsync();
            }
        }
        private Orders BuildOrderFromOrderInfo(OrderInfo orderInfo)
        {
            return new Orders
            {
                OrderID = Guid.NewGuid().ToString(),
                FullName = orderInfo.FullName,
                Email = orderInfo.Email,
                Phone = orderInfo.Phone,
                Address = orderInfo.Address,
                PaymentID = orderInfo.PaymentID,
                OrderStatusID = 1,
                CreatedAt = DateTime.Now
            };
        }

        /*private async Task<string> ProcessMomoPayment(Orders newOrder)
        {
            var orderForm = new OrderForm
            {
                OrderID = newOrder.OrderID,
                ActualPrice = newOrder.ActualPrice,
                FullName = newOrder.FullName
            };

            return  _moMoServices.MomoPay(orderForm); // Đảm bảo rằng phương thức MomoPay cũng là async
        }*/

        public async Task<string> GetEmailByOrderId(string orderId)
        {
            var order = await _dbContext.Orders.FirstOrDefaultAsync(x => x.OrderID == orderId);
            var email = order.Email;
            if (order == null || email == null)
                return MessageStatus.Failed.ToString("empty");
            return email;
        }
        public async Task<string> GetFullNameByOrderId(string orderId)
        {
            var order = await _dbContext.Orders.FirstOrDefaultAsync(x => x.OrderID == orderId);
            var name = order.FullName;
            if (order == null || name == null)
                return MessageStatus.Failed.ToString("empty");
            return name;
        }
        public async Task SendOrderConfirmationEmail(string email, string fullName)
        {
            var mailDto = new MailDTOs
            {
                To = email,
                Body = GenerateOrderConfirmationEmailBody(fullName),
                Subject = "Đặt hàng thành công!"
            };

             _mailServices.SendMail(mailDto); // Đảm bảo rằng phương thức SendMail cũng là async
        }

        private string GenerateOrderConfirmationEmailBody(string fullName)
        {
            // Generate the email body based on the order information and the customer's name
            var body = $@"
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
               <span> Hello {fullName}!</span>
               <br>
               <span> Thank you for placing your order with Our Store! We really appreciate that you chose our store, it means the world to us!</span>
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
</html>";
            return body;
        }

        private async Task SaveOrderAndOrderItems(Orders newOrder, List<CartItem> cartItems)
        {
            _dbContext.Orders.Add(newOrder);
            await _dbContext.SaveChangesAsync();

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

                var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductID == cartItem.ProductId);
                if (product != null)
                {
                    product.Quantity -= cartItem.Quantity;
                }
            }

            await _dbContext.SaveChangesAsync();
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
