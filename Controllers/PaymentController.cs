using API_Test1.Services.PaymentServices;

namespace API_Test1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentServices _paymentServices;

        public PaymentController( IPaymentServices paymentServices)
        {
            _paymentServices = paymentServices;
        }
        [HttpGet]
        public async Task<IActionResult> GetPaymentMethods([FromQuery] Pagination pagination)
        {
            var paymentMethods = await _paymentServices.GetPaymentMethods(pagination);
            return Ok(paymentMethods);
        }
        // API: Tạo phương thức thanh toán mới
        [HttpPost]
        public async Task<IActionResult> CreatePaymentMethod([FromBody] Payments paymentMethod)
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
        [HttpPut("{paymentMethodId}")]
        public async Task<IActionResult> UpdatePaymentMethod(int paymentMethodId, [FromBody] Payments updatedPaymentMethod)
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
        [HttpDelete("{paymentMethodId}")]
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
