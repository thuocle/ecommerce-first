using API_Test1.Models.DTOs;
using API_Test1.Services.OrderServices;
using API_Test1.Services.OrderstatusServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Test1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderStatusController : ControllerBase
    {
        private readonly IOrderstatusServices _orderstatus;

        public OrderStatusController(IOrderstatusServices orderstatus)
        {
            _orderstatus = orderstatus;
        }
        // API: Lấy danh sách trạng thái đơn hàng
        [HttpGet]
        public async Task<IActionResult> GetOrderStatuses([FromQuery] Pagination pagination)
        {
            var orderStatuses = await _orderstatus.GetOrderStatuses(pagination);
            return Ok(orderStatuses);
        }

        // API: Tạo trạng thái đơn hàng mới
        [HttpPost]
        public async Task<IActionResult> CreateOrderStatus([FromBody] OrderStatuses orderStatus)
        {
            var messageStatus = await _orderstatus.CreateOrderStatus(orderStatus);

            if (messageStatus == MessageStatus.Success)
            {
                return Ok("Order status created successfully.");
            }
            else
            {
                return BadRequest("Failed to create order status.");
            }
        }

        // API: Cập nhật trạng thái đơn hàng
        [HttpPut("{orderStatusId}")]
        public async Task<IActionResult> UpdateOrderStatus(int orderStatusId, [FromBody] OrderStatuses updatedOrderStatus)
        {
            var messageStatus = await _orderstatus.UpdateOrderStatus(orderStatusId, updatedOrderStatus);

            if (messageStatus == MessageStatus.Success)
            {
                return Ok("Order status updated successfully.");
            }
            else
            {
                return NotFound("Order status not found.");
            }
        }

        // API: Xóa trạng thái đơn hàng
        [HttpDelete("{orderStatusId}")]
        public async Task<IActionResult> DeleteOrderStatus(int orderStatusId)
        {
            var messageStatus = await _orderstatus.DeleteOrderStatus(orderStatusId);

            if (messageStatus == MessageStatus.Success)
            {
                return Ok("Order status deleted successfully.");
            }
            else
            {
                return NotFound("Order status not found.");
            }
        }
    }
}
