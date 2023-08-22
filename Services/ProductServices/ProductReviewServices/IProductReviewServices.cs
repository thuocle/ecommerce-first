namespace API_Test1.Services.ProductServices.ProductReviewServices
{
    public interface IProductReviewServices
    {
        public Task<MessageStatus> AddProductReview(int productId, ReviewProductModel reviewProduct);
        public Task<PageInfo<ProductReviewDTO>> GetProductReviewsWithContent(int productId, Pagination page);
        public Task<MessageStatus> UpdateProductReview(int reviewId, ReviewProductModel reviewProduct);
        public Task<MessageStatus> RemoveProductReview(int reviewId);
        #region for admin
        public Task<PageInfo<dynamic>> GetAllProductReviews(int productId, Pagination page);
        public Task<MessageStatus> UpdateProductReviewStatus(int reviewId, string status);
        #endregion
    }
}
