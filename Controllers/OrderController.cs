using API_Test1.Models.DTOs;
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
        //other 
        [Authorize]
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
        //ship-code
        [HttpPost("order-cod")]
        public async Task<IActionResult> CreateOrderCOD([FromForm] OrderInfo orderInfo)
        {
            var result = await _orderService.OrderByShipCOD(orderInfo);

            if (result == MessageStatus.Success)
            {
                return Ok("Order successfully.");
            }
            else
            {
                return BadRequest("Failed to order .");
            }
        }
        [Authorize]
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
        [Authorize]
        // GetAllOrderForUser API
        [HttpGet("order-list")]
        public async Task<IActionResult> GetAllOrderForUser([FromQuery] Pagination page, string userId)
        {
            var pageInfo = await _orderService.GetAllOrderForUser(page, userId);

            return Ok(pageInfo);
        }

        [Authorize]
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
