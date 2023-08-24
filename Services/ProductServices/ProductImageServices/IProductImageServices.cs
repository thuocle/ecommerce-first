namespace API_Test1.Services.ProductServices.ProductImageServices
{
    public interface IProductImageServices
    {
        public Task<PageInfo<ProductImages>> GetAllProductImagesForAdminAsync(Pagination page);
        /*public Task<PageInfo<ProductImages>> GetAllProductImagesAsync(Pagination page);*/
        public Task<ProductImages> GetProductImageByIdAsync(int productImageId);
        public Task<MessageStatus> AddProductImageAsync(ProductImageForm productImage);
        public Task<MessageStatus> UpdateProductImageAsync(int productImageID, ProductImageForm productImage);
        public Task<MessageStatus> DeleteProductImageAsync(int productImageId);
        public Task<ProductImages> GetProductImageByProductIdAsync(int productId);
    }
}
