namespace API_Test1.Services.PaymentServices
{
    public interface IPaymentServices
    {
        public Task<PageInfo<Payments>> GetPaymentMethods(Pagination page);
        public Task<MessageStatus> CreatePaymentMethod(Payments paymentMethod);
        public Task<MessageStatus> UpdatePaymentMethod(int paymentMethodId, Payments updatedPaymentMethod);
        public Task<MessageStatus> DeletePaymentMethod(int paymentMethodId);

    }
}
