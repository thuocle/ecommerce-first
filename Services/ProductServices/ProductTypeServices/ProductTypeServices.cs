using API_Test1.Services.FileServices;

namespace API_Test1.Services.ProductServices.ProductTypeServices.ProductTypeServices
{
    public class ProductTypeServices : IProductTypeServices
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IFileServices _fileServices;

        public ProductTypeServices(ApplicationDbContext dbContext, IFileServices fileServices)
        {
            _dbContext = dbContext;
            _fileServices = fileServices;
        }

        public async Task<MessageStatus> AddProductTypeAsync(ProductTypeForm productType)
        {
            ProductTypes newPrType = new()
            {
                CreatedAt = DateTime.Now,
                NameProductType = productType.NameProductType,
                ImageTypeProduct = await _fileServices.UploadImage(productType.ImageTypeProduct)
            };
            _dbContext.ProductTypes.Add(newPrType);
            await _dbContext.SaveChangesAsync();
            return MessageStatus.Success;
        }
        public async Task<MessageStatus> UpdateProductTypeAsync(int productTypeId, ProductTypeForm productType)
        {
            var existingProductType = await _dbContext.ProductTypes.FindAsync(productTypeId);
            if (existingProductType == null)
            {
                return MessageStatus.Empty;
            }
            existingProductType.NameProductType = productType.NameProductType;
            existingProductType.ImageTypeProduct = await _fileServices.UploadImage(productType.ImageTypeProduct);
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
            var allProductType = await Task.FromResult(_dbContext.ProductTypes
                .Select(x=> new ProductTypes
                {
                    ProductTypeID = x.ProductTypeID,
                    NameProductType = x.NameProductType,
                    ImageTypeProduct = x.ImageTypeProduct,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                })
                .AsQueryable());
            var data = PageInfo<ProductTypes>.ToPageInfo(page, allProductType);
            page.TotalItem = await allProductType.CountAsync();
            return new PageInfo<ProductTypes>(page, data);
        }

        public async Task<ProductTypes> GetProductTypeByIdAsync(int productTypeId)
        {
            var prType = await _dbContext.ProductTypes
                .Where(x => x.ProductTypeID == productTypeId)
                .Select(x => new ProductTypes
                {
                    ProductTypeID = x.ProductTypeID,
                    NameProductType = x.NameProductType,
                    ImageTypeProduct = x.ImageTypeProduct,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                })
                .FirstOrDefaultAsync();
            return prType;
        }


    }
}
