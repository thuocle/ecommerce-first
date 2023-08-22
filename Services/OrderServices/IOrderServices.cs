namespace API_Test1.Services.OrderServices
{
    public interface IOrderServices
    {
        #region admin
        public Task<PageInfo<Orders>> GetAllOrder(Pagination page);
        public Task<MessageStatus> UpdateStatusOrder(int statusId, string orderId);
        public Task<PageInfo<OrderDetails>> GetAllOrderDetail(Pagination page);

        #endregion
        #region user
        public Task<PageInfo<Orders>> GetAllOrderForUser(Pagination page, string userID);
        #endregion
        #region anonymous
        public Task<MessageStatus> CreateOrder(OrderInfo orderInfo);
        public Task<IEnumerable<OrderDetails>> GetOrderDetail(string orderID);
        public Task<IEnumerable<OrderDetails>> FindOrderById(string orderID);

        #endregion

    }
}
