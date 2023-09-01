using API_Test1.Services.CartServices;
using API_Test1.Services.OrderServices;
using API_Test1.Services.PaymentServices.MOMO.Model;
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
        public async Task<ActionResult<string>> CreatePaymentUrl(OrderInfo model)
        {
            var res = await _momoService.CreatePaymentAsync(model);
            return res.PayUrl;
        }
        [HttpGet("return")]
        public ActionResult<MomoExecuteResponseModel> PaymentCallBack()
        {
            var momoReturn = HttpContext.Request.Query;
            if (momoReturn.Count()>0)
                return BadRequest();
            var response = _momoService.PaymentExecuteAsync(momoReturn);
            if(response.ErrorCode != 0) 
            {
                _orderServices.DeleteOrderAndOrderDetail(response.OrderId);
            }
            _cartServices.ClearCart();
            return response;
        }
    }
}
