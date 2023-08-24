namespace API_Test1.Services.ProductServices.ProductTypeServices.ProductTypeServices
{
    public interface IProductTypeServices
    {
        Task<PageInfo<ProductTypes>> GetAllProductTypesAsync(Pagination page);
        Task<ProductTypes> GetProductTypeByIdAsync(int productTypeId);
        Task<MessageStatus> AddProductTypeAsync(ProductTypeForm productType);
        Task<MessageStatus> UpdateProductTypeAsync(int productTypeId, ProductTypeForm productType);
        Task<MessageStatus> DeleteProductTypeAsync(int productTypeId);
    }
}
