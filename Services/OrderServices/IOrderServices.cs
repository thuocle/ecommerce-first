namespace API_Test1.Services.OrderServices
{
    public interface IOrderServices
    {
        #region admin
        public Task<PageInfo<Orders>> GetAllOrder(Pagination page);
        public Task<MessageStatus> UpdateStatusOrderByAdmin(int statusId, string orderId);
        public Task<PageInfo<OrderDetails>> GetAllOrderDetail(Pagination page);

        #endregion
        #region user
        public Task<PageInfo<dynamic>> GetAllOrderForUser(Pagination page, string userID);
        #endregion
        #region anonymous
        public Task<OrderForm> CreateOrder(OrderInfo orderInfo);
        public Task DeleteOrderAndOrderDetail(string orderId);
        public Task<MessageStatus> UpdateStatusOrderByUser(int statusId, string orderId);
        public Task<IEnumerable<dynamic>> GetOrderDetail(string orderID);
        public Task<IEnumerable<dynamic>> FindOrderById(string orderID);

        #endregion

    }
}
