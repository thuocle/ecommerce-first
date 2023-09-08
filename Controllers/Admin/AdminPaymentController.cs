using API_Test1.Services.PaymentServices;

namespace API_Test1.Controllers.Admin
{
    [Authorize(Roles = UserRoles.Admin)]

    [Route("api/admin")]
    [ApiController]
    public class AdminPaymentController : ControllerBase
    {
        private readonly IPaymentServices _paymentServices;

        public AdminPaymentController(IPaymentServices paymentServices)
        {
            _paymentServices = paymentServices;
        }
        [HttpGet("get-payment-methods")]
        public async Task<IActionResult> GetPaymentMethods([FromQuery] Pagination pagination)
        {
            var paymentMethods = await _paymentServices.GetPaymentMethods(pagination);
            return Ok(paymentMethods);
        }
        // API: Tạo phương thức thanh toán mới
        [HttpPost("add-payment-methods")]
        public async Task<IActionResult> CreatePaymentMethod([FromForm] Payments paymentMethod)
        {
            var messageStatus = await _paymentServices.CreatePaymentMethod(paymentMethod);

            if (messageStatus == MessageStatus.Success)
            {
                return Ok("Payment method created successfully.");
            }
            else
            {
                return BadRequest("Failed to create payment method.");
            }
        }

        // API: Cập nhật phương thức thanh toán
        [HttpPut("update-payment-methods/{paymentMethodId}")]
        public async Task<IActionResult> UpdatePaymentMethod(int paymentMethodId, [FromForm] Payments updatedPaymentMethod)
        {
            var messageStatus = await _paymentServices.UpdatePaymentMethod(paymentMethodId, updatedPaymentMethod);

            if (messageStatus == MessageStatus.Success)
            {
                return Ok("Payment method updated successfully.");
            }
            else
            {
                return NotFound("Payment method not found.");
            }
        }

        // API: Xóa phương thức thanh toán
        [HttpDelete("delete-payment-methods/{paymentMethodId}")]
        public async Task<IActionResult> DeletePaymentMethod(int paymentMethodId)
        {
            var messageStatus = await _paymentServices.DeletePaymentMethod(paymentMethodId);

            if (messageStatus == MessageStatus.Success)
            {
                return Ok("Payment method deleted successfully.");
            }
            else
            {
                return NotFound("Payment method not found.");
            }
        }
    }
}
