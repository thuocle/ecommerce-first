namespace API_Test1.Services.OrderServices
{
    public interface IOrderServices
    {
        public Task<MessageStatus> CreateOrder(OrderInfo orderInfo);
    }
}
