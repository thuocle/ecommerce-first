using API_Test1.Services.OrderServices;

namespace API_Test1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderServices _orderService;

        public OrderController(IOrderServices orderService)
        {
            _orderService = orderService;
        }
        //admin
        [HttpGet("admin/all-order")]
        public async Task<IActionResult> GetAllOrders([FromQuery] Pagination page)
        {
            var orders = await _orderService.GetAllOrder(page);
            return Ok(orders);
        }

        [HttpGet("admin/details")]
        public async Task<IActionResult> GetAllOrderDetails([FromQuery] Pagination page)
        {
            var orderDetails = await _orderService.GetAllOrderDetail(page);
            return Ok(orderDetails);
        }
        [HttpPost("admin/update-status")]
        public async Task<IActionResult> UpdateStatusOrderByAdmin(int statusId, string orderId)
        {
            var result = await _orderService.UpdateStatusOrderByAdmin(statusId, orderId);

            if (result == MessageStatus.Success)
            {
                return Ok("Order status updated successfully.");
            }
            else
            {
                return BadRequest("Failed to update order status.");
            }
        }


        //other 
        [HttpPost("update-status")]
        public async Task<IActionResult> UpdateStatusOrderByUser(int statusId, string orderId)
        {
            var result = await _orderService.UpdateStatusOrderByUser(statusId, orderId);

            if (result == MessageStatus.Success)
            {
                return Ok("Order status updated successfully.");
            }
            else
            {
                return BadRequest("Failed to update order status.");
            }
        }
        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder([FromForm] OrderInfo orderInfo)
        {
            var messageStatus = await _orderService.CreateOrder(orderInfo);

            if (messageStatus == MessageStatus.Success)
            {
                return Ok("Order created successfully.");
            }
            else
            {
                return BadRequest("Failed to create order.");
            }
        }

        // GetOrderDetail API
        [HttpGet("{orderID}/details")]
        public async Task<IActionResult> GetOrderDetail(string orderID)
        {
            var orderDetails = await _orderService.GetOrderDetail(orderID);

            if (orderDetails != null)
            {
                return Ok(orderDetails);
            }
            else
            {
                return NotFound("Order details not found.");
            }
        }

        // UpdateStatusOrderByUser API

        // GetAllOrderForUser API
        [HttpGet("order-list")]
        public async Task<IActionResult> GetAllOrderForUser([FromQuery] Pagination page, string userId)
        {
            var pageInfo = await _orderService.GetAllOrderForUser(page, userId);

            return Ok(pageInfo);
        }

        // FindOrderById API
        [HttpGet("{orderID}")]
        public async Task<IActionResult> FindOrderById(string orderID)
        {
            var orderDetails = await _orderService.FindOrderById(orderID);

            if (orderDetails != null)
            {
                return Ok(orderDetails);
            }
            else
            {
                return NotFound("Order not found.");
            }
        }
    }
}
