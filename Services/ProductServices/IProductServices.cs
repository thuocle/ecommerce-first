namespace API_Test1.Services.ProductServices
{
    public interface IProductServices
    {
        #region for admin
        public Task<MessageStatus> AddProductAsync(ProductModel productModel);
        public Task<MessageStatus> UpdateProductAsync( int id,ProductModel productModel);
        public Task<MessageStatus> RemoveProductAsync(int id);
        public Task<PageInfo<Products>> GetAllProductForAdminAsync(Pagination pagination);
        public Task<PageInfo<Products>> FindProductByNameForAdminAsync(Pagination pagination, string keyWord);
        public Task<PageInfo<Products>> FindProductByCategoryForAdminAsync(Pagination pagination, int categoryID);
        #endregion
        public Task<PageInfo<Products>> GetAllProductAsync(Pagination pagination);
        public Task<IQueryable<Products>> GetRelatedProductsAsync(int productId);
        public Task<PageInfo<Products>> GetAllByHotProductAsync(Pagination pagination);
        public Task<PageInfo<Products>> FindProductByNameAsync(Pagination pagination, string keyWord);
        public Task<PageInfo<Products>> FindProductByCategoryAsync(Pagination pagination, int categoryID);
        public Task<Products> GetProductByIdAsync(int id);
        public Task<PageInfo<Products>> FilterProductAsync(Pagination pagination, FilterProduct filter);


    }
}
