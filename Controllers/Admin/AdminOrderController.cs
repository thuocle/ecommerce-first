using API_Test1.Services.OrderServices;
namespace API_Test1.Controllers.Admin
{
    [Authorize(Roles = UserRoles.Admin)]
    [Route("api/admin")]
    [ApiController]
    public class AdminOrderController : ControllerBase
    {
        private readonly IOrderServices _orderService;

        public AdminOrderController(IOrderServices orderService)
        {
            _orderService = orderService;
        }
        #region order
        //admin
        [HttpGet("all-order")]
        public async Task<IActionResult> GetAllOrders([FromQuery] Pagination page)
        {
            var orders = await _orderService.GetAllOrder(page);
            return Ok(orders);
        }

        [HttpGet("all-order-detail")]
        public async Task<IActionResult> GetAllOrderDetails([FromQuery] Pagination page)
        {
            var orderDetails = await _orderService.GetAllOrderDetail(page);
            return Ok(orderDetails);
        }

        [HttpPost("update-order-status")]
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

        #endregion
    }
}
