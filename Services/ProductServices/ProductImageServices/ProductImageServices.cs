using API_Test1.Services.FileServices;
using Microsoft.VisualStudio.Web.CodeGeneration;

namespace API_Test1.Services.ProductServices.ProductImageServices
{
    public class ProductImageServices : IProductImageServices
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IFileServices _fileServices;

        public ProductImageServices(ApplicationDbContext dbContext, IFileServices fileServices)
        {
            _dbContext = dbContext;
            _fileServices = fileServices;
        }
        public async Task<PageInfo<ProductImages>> GetAllProductImagesForAdminAsync(Pagination page)
        {
            var allProduct = await Task.FromResult(_dbContext.ProductImages
                .Select(x => new ProductImages
                {
                    ProductImageID = x.ProductImageID,
                    ProductID = x.ProductID,
                    ImageProduct = x.ImageProduct,
                    Title = x.Title
                })
                .AsQueryable());
            var data = PageInfo<ProductImages>.ToPageInfo(page, allProduct);
            page.TotalItem = await allProduct.CountAsync();
            return new PageInfo<ProductImages>(page, data);
        }
        /*public async Task<PageInfo<ProductImages>> GetAllProductImagesAsync(Pagination page)
        {
            var allProduct = await Task.FromResult(_dbContext.ProductImages
                .Where(x => x.Status == Status.Active)
                .Select(x => new ProductImages
                {
                    ProductImageID = x.ProductImageID,
                    ProductID = x.ProductID,
                    ImageProduct = x.ImageProduct,
                    Title = x.Title
                })
                .AsQueryable());
            var data = PageInfo<ProductImages>.ToPageInfo(page, allProduct);
            page.TotalItem = await allProduct.CountAsync();
            return new PageInfo<ProductImages>(page, data);
        }*/

        public async Task<ProductImages> GetProductImageByIdAsync(int productImageId)
        {
            return await _dbContext.ProductImages
                .Where(x => x.ProductImageID == productImageId)
                .Select(x => new ProductImages
                {
                    ImageProduct = x.ImageProduct,
                    ProductID = x.ProductID,
                    Title = x.Title
                })
                .FirstOrDefaultAsync();
        }
        public async Task<ProductImages> GetProductImageByProductIdAsync(int productId)
        {
            return await _dbContext.ProductImages
                .Where(x => x.ProductID == productId)
                .Select(x => new ProductImages
                {
                    ImageProduct = x.ImageProduct,
                    ProductID = x.ProductID,
                    Title = x.Title
                })
                .FirstOrDefaultAsync();
        }
        public async Task<MessageStatus> AddProductImageAsync(ProductImageForm productImage)
        {
            var prImage = new ProductImages
            {
                Title = productImage.Title,
                ImageProduct = await _fileServices.UploadImage(productImage.ImageProduct),
                ProductID = productImage.ProductID,
                Status = productImage.Status,
                CreatedAt = DateTime.Now

            };
            _dbContext.ProductImages.Add(prImage);
            await _dbContext.SaveChangesAsync();
            return MessageStatus.Success;
        }

        public async Task<MessageStatus> UpdateProductImageAsync(int productImageID, ProductImageForm productImage)
        {
            var existingProductImage = await _dbContext.ProductImages.FirstOrDefaultAsync(x => x.ProductImageID == productImageID);
            if (existingProductImage != null)
            {
                existingProductImage.Title = productImage.Title;
                existingProductImage.ImageProduct = await _fileServices.UploadImage(productImage.ImageProduct) ;
                existingProductImage.ProductID = productImage.ProductID;
                existingProductImage.UpdatedAt = DateTime.Now;
                await _dbContext.SaveChangesAsync();
            }
            return MessageStatus.Success;
        }

        public async Task<MessageStatus> DeleteProductImageAsync(int productImageId)
        {
            var productImage = await _dbContext.ProductImages.FirstOrDefaultAsync(x => x.ProductImageID == productImageId);
            if (productImage != null)
            {
                productImage.Status = Status.Disabled;
                await _dbContext.SaveChangesAsync();
                return MessageStatus.Success;
            }
            return MessageStatus.Failed;
        }
    }
}
