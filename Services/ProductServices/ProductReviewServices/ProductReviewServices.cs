using API_Test1.Services.JwtServices;

namespace API_Test1.Services.ProductServices.ProductReviewServices
{
    public class ProductReviewServices : IProductReviewServices
    {
        private readonly IJwtServices _jwtServices;
        private readonly ApplicationDbContext _dbContext;

        public ProductReviewServices(IJwtServices jwtServices, ApplicationDbContext dbContext)
        {
            _jwtServices = jwtServices;
            _dbContext = dbContext;
        }

        #region private
        //check da mua san pham nay chua
        private bool CheckIfUserPurchasedProduct(string userId, int productId)
        {
            // Kiểm tra thông tin đơn hàng của người dùng
            var order = _dbContext.Orders.FirstOrDefault(o => o.UserId == userId && o.OrderStatusID == (int)OrderStatus.Completed);
            if (order == null)
            {
                return false; // Người dùng chưa có đơn hàng hoặc đơn hàng chưa hoàn thành
            }

            // Kiểm tra xem sản phẩm có trong đơn hàng của người dùng hay không
            var orderItem = _dbContext.OrderDetails.FirstOrDefault(oi => oi.OrderID == order.OrderID && oi.ProductID == productId);
            if (orderItem == null)
            {
                return false; // Sản phẩm không có trong đơn hàng của người dùng
            }

            return true; // Người dùng đã mua sản phẩm
        }
        #endregion
        
        #region admin
        //xem danh sách các review
        public async Task<PageInfo<dynamic>> GetAllProductReviews(int productId, Pagination page)
        {
            var reviews = from pr in _dbContext.ProductReviews
                          join u in _dbContext.Users on pr.UserId equals u.Id
                          where pr.ProductID == productId
                          select new
                          {
                              pr,
                              u.UserName
                          };

            reviews = reviews.AsQueryable();
            var data = PageInfo<dynamic>.ToPageInfo(page, reviews);
            page.TotalItem = await reviews.CountAsync();
            return new PageInfo<dynamic>(page, data);
        }
        //cap nhat satutus của review
        public async Task<MessageStatus> UpdateProductReviewStatus(int reviewId, string status)
        {
            var review = await _dbContext.ProductReviews.FirstOrDefaultAsync(x => x.ProductReviewID == reviewId);
            if (review == null)
                return MessageStatus.Failed;
            review.UpdatedAt = DateTime.Now;
            review.Status = status;
            await _dbContext.SaveChangesAsync();
            return MessageStatus.Success;
        }
        #endregion

        /// <summary>
        /// cho ca user
        /// </summary>
        /// <param name="tat ca"></param>
        /// <returns></returns>
        public async Task<MessageStatus> AddProductReview(int productId, ReviewProductModel reviewProduct)
        {
            //xác nhan da login chua
            if (!_jwtServices.IsUserLoggedIn())
            {
                return MessageStatus.UnauthorizedAccess;
            }
            // lay userid
            var userId = _jwtServices.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return MessageStatus.Failed;
            }
            //kiem tra da mua hang hay chua =>mua roi se duoc danh gia: status order = Da nhan hang
            if (!CheckIfUserPurchasedProduct(userId, productId))
                return MessageStatus.Failed;
            // Thực hiện kết nối và lưu trữ đánh giá vào cơ sở dữ liệu
            var review = new ProductReviews
            {
                ProductID = productId,
                UserId = userId,
                ContentRated = reviewProduct.ContentRated,
                PointEvaluation = reviewProduct.PointEvaluation,
                Status = Status.Active,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _dbContext.ProductReviews.Add(review);
            await _dbContext.SaveChangesAsync();
            return MessageStatus.Success;
        }
        //hien thi cac danh gia Trang chi tiết sản phẩm
        public async Task<PageInfo<ProductReviewDTO>> GetProductReviewsWithContent(int productId, Pagination page)
        {
            var reviews =
                            from pr in _dbContext.ProductReviews
                            join u in _dbContext.Users on pr.UserId equals u.Id
                            where pr.ProductID == productId && pr.Status == Status.Active
                            select new ProductReviewDTO
                            {
                                UserName = u.UserName,
                                ContentRated = pr.ContentRated,
                                PointEvaluation = pr.PointEvaluation,
                                CreatedAt = pr.CreatedAt
                            };
            reviews = reviews.AsQueryable();
            var data = PageInfo<ProductReviewDTO>.ToPageInfo(page, reviews);
            page.TotalItem = await reviews.CountAsync();
            return new PageInfo<ProductReviewDTO>(page, data);
        }
        //user cập nhật review
        public async Task<MessageStatus> UpdateProductReview(int reviewId, ReviewProductModel reviewProduct)
        {
            // Kiểm tra xem reviewId có tồn tại và thuộc về user hiện tại hay không
            var existingReview = await _dbContext.ProductReviews.FirstOrDefaultAsync(pr => pr.ProductReviewID == reviewId && pr.UserId == _jwtServices.GetUserId());
            if (existingReview == null)
            {
                return MessageStatus.Empty;
            }

            // Cập nhật thông tin đánh giá
            existingReview.PointEvaluation = reviewProduct.PointEvaluation;
            existingReview.ContentRated = reviewProduct.ContentRated;
            existingReview.UpdatedAt = DateTime.Now;
            await _dbContext.SaveChangesAsync();

            return MessageStatus.Success;
        }
        //user xoa review
        public async Task<MessageStatus> RemoveProductReview(int reviewId)
        {
            var existingReview = await _dbContext.ProductReviews.FirstOrDefaultAsync(pr => pr.ProductReviewID == reviewId && pr.UserId == _jwtServices.GetUserId());
            if (existingReview == null)
            {
                return MessageStatus.Empty;
            }
            existingReview.UpdatedAt = DateTime.Now;
            existingReview.Status = Status.Disabled;
            await _dbContext.SaveChangesAsync();
            return MessageStatus.Success;
        }

    }
}
