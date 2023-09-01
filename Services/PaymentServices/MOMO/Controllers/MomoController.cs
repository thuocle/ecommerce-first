using API_Test1.Models.ViewModels;
using API_Test1.Services.CartServices;
using API_Test1.Services.OrderServices;
using API_Test1.Services.PaymentServices.MOMO.Model;
using NuGet.Protocol;

namespace API_Test1.Services.PaymentServices.MOMO.Controllers
{
    [Route("api/momo")]
    [ApiController]
    public class MomoController : ControllerBase
    {
        private readonly IMoMoServices _momoService;
        private readonly ICartServices _cartServices;
        private readonly IOrderServices _orderServices;

        public MomoController(IMoMoServices momoService, ICartServices cartServices, IOrderServices orderServices)
        {
            _momoService = momoService;
            _cartServices = cartServices;
            _orderServices = orderServices;
        }
        [HttpPost]
        public async Task<ActionResult<string>> CreatePaymentUrl([FromForm]OrderInfo model)
        {
            var res = await _momoService.CreatePaymentAsync(model);
            return res.PayUrl;
        }
        [HttpGet("return")]
        public async Task<ActionResult<MomoExecuteResponseModel>> PaymentCallBack()
        {
            var momoReturn = HttpContext.Request.Query;
            if (momoReturn.Count() == 0)
                return BadRequest();

            var response = _momoService.PaymentExecuteAsync(momoReturn);

            if (response.ErrorCode == 0)
            {
                var email = await _orderServices.GetEmailByOrderId(response.OrderId);
                var name = await _orderServices.GetFullNameByOrderId(response.OrderId);
                var sendMail = _orderServices.SendOrderConfirmationEmail(email, name);
                var result = _cartServices.ClearCart();

                if (result)
                    return response;
            }

            await _orderServices.DeleteOrderAndOrderDetail(response.OrderId);
            return BadRequest();
        }
    }
}
