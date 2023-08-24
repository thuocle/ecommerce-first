namespace API_Test1.Services.OrderstatusServices
{
    public class OrderstatusServices : IOrderstatusServices
    {
        private readonly ApplicationDbContext _dbContext;

        public OrderstatusServices(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Hàm chức năng: Lấy danh sách trạng thái đơn hàng
        public async Task<PageInfo<OrderStatuses>> GetOrderStatuses(Pagination page)
        {
            var orderStatuses = await Task.FromResult(_dbContext.OrderStatuses
                .Select(x=>new OrderStatuses 
                {
                    OrderStatusID = x.OrderStatusID,
                    StatusName = x.StatusName
                })
                .AsQueryable());
            var data = PageInfo<OrderStatuses>.ToPageInfo(page, orderStatuses);
            page.TotalItem = await orderStatuses.CountAsync();
            return new PageInfo<OrderStatuses>(page, data);
        }

        // Hàm chức năng: Tạo trạng thái đơn hàng mới
        public async Task<MessageStatus> CreateOrderStatus(OrderStatuses orderStatus)
        {
            _dbContext.OrderStatuses.Add(orderStatus);
            await _dbContext.SaveChangesAsync();
            return MessageStatus.Success;
        }

        

        // Hàm chức năng: Cập nhật trạng thái đơn hàng
        public async Task<MessageStatus> UpdateOrderStatus(int orderStatusId, OrderStatuses updatedOrderStatus)
        {
            var orderStatus = await _dbContext.OrderStatuses.FirstOrDefaultAsync(os => os.OrderStatusID == orderStatusId);

            if (orderStatus != null)
            {
                orderStatus.StatusName = updatedOrderStatus.StatusName;
                await _dbContext.SaveChangesAsync();
                return MessageStatus.Success;
            }
            else
            {
                return MessageStatus.Failed;
            }
        }

        // Hàm chức năng: Xóa trạng thái đơn hàng
        public async Task<MessageStatus> DeleteOrderStatus(int orderStatusId)
        {
            var orderStatus = await _dbContext.OrderStatuses.FirstOrDefaultAsync(os => os.OrderStatusID == orderStatusId);

            if (orderStatus != null)
            {
                _dbContext.OrderStatuses.Remove(orderStatus);
                await _dbContext.SaveChangesAsync();
                return MessageStatus.Success;
            }
            else
            {
                return MessageStatus.Failed;
            }
        }
    }
}
