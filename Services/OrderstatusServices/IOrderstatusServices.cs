namespace API_Test1.Services.OrderstatusServices
{
    public interface IOrderstatusServices
    {
        public Task<PageInfo<OrderStatuses>> GetOrderStatuses(Pagination page);
        public Task<MessageStatus> CreateOrderStatus(OrderStatusForm orderStatus);
        public Task<MessageStatus> UpdateOrderStatus(int orderStatusId, OrderStatusForm updatedOrderStatus);
        public Task<MessageStatus> DeleteOrderStatus(int orderStatusId);
    }
}
