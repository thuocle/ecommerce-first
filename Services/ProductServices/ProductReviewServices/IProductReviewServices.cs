namespace API_Test1.Services.ProductServices.ProductReviewServices
{
    public interface IProductReviewServices
    {
        public Task<MessageStatus> AddProductReview(int productId, ReviewProductForm reviewProduct);
        public Task<PageInfo<ProductReviewForm>> GetProductReviewsWithContent(int productId, Pagination page);
        public Task<MessageStatus> UpdateProductReview(int reviewId, ReviewProductForm reviewProduct);
        public Task<MessageStatus> RemoveProductReview(int reviewId);
        #region for admin
        public Task<PageInfo<dynamic>> GetAllProductReviews(int productId, Pagination page);
        public Task<MessageStatus> UpdateProductReviewStatus(int reviewId, int status);
        #endregion
    }
}
