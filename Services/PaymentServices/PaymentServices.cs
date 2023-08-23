
namespace API_Test1.Services.PaymentServices
{
    public class PaymentServices : IPaymentServices
    {
        private readonly ApplicationDbContext _dbContext;

        public PaymentServices(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        // Hàm chức năng: Lấy danh sách phương thức thanh toán
        public async Task<PageInfo<Payments>> GetPaymentMethods(Pagination page)
        {
            var paymentMethods = await Task.FromResult(_dbContext.Payments.AsQueryable());

            var data = PageInfo<Payments>.ToPageInfo(page, paymentMethods);
            page.TotalItem = await paymentMethods.CountAsync();
            return new PageInfo<Payments>(page, data);
        }

        // API: Lấy danh sách phương thức thanh toán
       
        // Hàm chức năng: Tạo phương thức thanh toán mới
        public async Task<MessageStatus> CreatePaymentMethod(Payments paymentMethod)
        {
            paymentMethod.CreatedAt = DateTime.Now;
            paymentMethod.Status = Status.Active;
            _dbContext.Payments.Add(paymentMethod);
            await _dbContext.SaveChangesAsync();
            return MessageStatus.Success;
        }

        

        // Hàm chức năng: Cập nhật phương thức thanh toán
        public async Task<MessageStatus> UpdatePaymentMethod(int paymentMethodId, Payments updatedPaymentMethod)
        {
            var paymentMethod = await _dbContext.Payments.FirstOrDefaultAsync(pm => pm.PaymentID == paymentMethodId);

            if (paymentMethod != null)
            {
                paymentMethod.PaymentMethod = updatedPaymentMethod.PaymentMethod;
                paymentMethod.Status = updatedPaymentMethod.Status;
                paymentMethod.UpdatedAt = DateTime.Now;
                await _dbContext.SaveChangesAsync();
                return MessageStatus.Success;
            }
            else
            {
                return MessageStatus.Failed;
            }
        }

       

        // Hàm chức năng: Xóa phương thức thanh toán
        public async Task<MessageStatus> DeletePaymentMethod(int paymentMethodId)
        {
            var paymentMethod = await _dbContext.Payments.FirstOrDefaultAsync(pm => pm.PaymentID == paymentMethodId);

            if (paymentMethod != null)
            {
                _dbContext.Payments.Remove(paymentMethod);
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
