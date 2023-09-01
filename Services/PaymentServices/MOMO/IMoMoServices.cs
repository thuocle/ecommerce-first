using API_Test1.Services.PaymentServices.MOMO.Model;

namespace API_Test1.Services.PaymentServices.MOMO
{
    public interface IMoMoServices
    {
        Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(OrderInfo model);
        MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection);
    }
}
