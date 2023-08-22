namespace API_Test1.Services.ProductServices.ProductTypeServices.ProductTypeServices
{
    public class ProductTypeServices : IProductTypeServices
    {
        private readonly ApplicationDbContext _dbContext;

        public ProductTypeServices(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<MessageStatus> AddProductTypeAsync(ProductTypes productType)
        {
            productType.CreatedAt = DateTime.Now;
            _dbContext.ProductTypes.Add(productType);
            await _dbContext.SaveChangesAsync();
            return MessageStatus.Success;
        }
        public async Task<MessageStatus> UpdateProductTypeAsync(int productTypeId, ProductTypes productType)
        {
            var existingProductType = await _dbContext.ProductTypes.FindAsync(productTypeId);
            if (existingProductType == null)
            {
                return MessageStatus.Empty;
            }
            existingProductType.NameProductType = productType.NameProductType;
            existingProductType.ImageTypeProduct = productType.ImageTypeProduct;
            existingProductType.UpdatedAt = DateTime.Now;

            await _dbContext.SaveChangesAsync();
            return MessageStatus.Success;
        }
        public async Task<MessageStatus> DeleteProductTypeAsync(int productTypeId)
        {
            var existingProductType = await _dbContext.ProductTypes.FindAsync(productTypeId);
            if (existingProductType == null)
            {
                return MessageStatus.Empty;
            }

            _dbContext.ProductTypes.Remove(existingProductType);
            await _dbContext.SaveChangesAsync();
            return MessageStatus.Success;
        }

        public async Task<PageInfo<ProductTypes>> GetAllProductTypesAsync(Pagination page)
        {
            var allProductType = await Task.FromResult(_dbContext.ProductTypes.AsQueryable());
            var data = PageInfo<ProductTypes>.ToPageInfo(page, allProductType);
            page.TotalItem = await allProductType.CountAsync();
            return new PageInfo<ProductTypes>(page, data);
        }

        public async Task<ProductTypes> GetProductTypeByIdAsync(int productTypeId)
        {
            return await _dbContext.ProductTypes.FindAsync(productTypeId);
        }


    }
}
