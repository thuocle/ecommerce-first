namespace API_Test1.Services.PaymentServices.MOMO
{
    public interface IMoMoServices
    {
        public Task<dynamic> CreatePaymentAsync(Orders model);
    }
}
