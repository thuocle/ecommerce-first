using API_Test1.Services.FileServices;

namespace API_Test1.Services.ProductServices
{
    public class ProductService : IProductServices
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IFileServices _fileServices;

        public ProductService(ApplicationDbContext dbContext, IFileServices fileServices)
        {
            _dbContext = dbContext;
            _fileServices = fileServices;
        }
        #region private

        #endregion

        #region for admin
        //them sp
        public async Task<MessageStatus> AddProductAsync(ProductForm productModel)
        {
            var newProduct = new Products
            {
                ProductTypeID = productModel.ProductTypeID,
                NameProduct = productModel.NameProduct,
                Price = productModel.Price,
                Title = productModel.Title,
                Discount = productModel.Discount,
                Quantity = productModel.Quantity,
                Status = Status.Active,
                CreatedAt = DateTime.Now
            };
            if (productModel.AvtarProduct != null)
            {
                newProduct.AvatarImageProduct = await _fileServices.UploadImage(productModel.AvtarProduct);
            }
            _dbContext.Products.Add(newProduct);
            await _dbContext.SaveChangesAsync();
            return MessageStatus.Success;
        }
        //cap nhat
        public async Task<MessageStatus> UpdateProductAsync(int productId, ProductForm productModel)
        {
            var existingProduct = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductID == productId);

            if (existingProduct == null)
                return MessageStatus.Empty;

            existingProduct.ProductTypeID = productModel.ProductTypeID;
            existingProduct.NameProduct = productModel.NameProduct;
            existingProduct.Price = productModel.Price;
            existingProduct.Title = productModel.Title;
            existingProduct.Discount = productModel.Discount;
            existingProduct.Quantity = productModel.Quantity;
            existingProduct.Status = productModel.Status;
            existingProduct.UpdatedAt = DateTime.Now;
            if (productModel.AvtarProduct != null)
            {
                existingProduct.AvatarImageProduct = await _fileServices.UploadImage(productModel.AvtarProduct);
            }
            await _dbContext.SaveChangesAsync();

            return MessageStatus.Success;
        }
        //xoa
        public async Task<MessageStatus> RemoveProductAsync(int productId)
        {
            var existingProduct = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductID == productId);

            if (existingProduct == null)
                return MessageStatus.Empty;
            existingProduct.Status = Status.Disabled;
            existingProduct.UpdatedAt = DateTime.Now;
            await _dbContext.SaveChangesAsync();

            return MessageStatus.Success;
        }
        public async Task<PageInfo<Products>> GetAllProductForAdminAsync(Pagination page)
        {
            var allProduct = await Task.FromResult(_dbContext.Products
                .Select(x => new Products
                {
                    ProductID = x.ProductID,
                    ProductTypeID = x.ProductTypeID,
                    NameProduct = x.NameProduct,
                    Price = x.Price,
                    AvatarImageProduct = x.AvatarImageProduct,
                    Title = x.Title,
                    Discount = x.Discount,
                    Quantity = x.Quantity,
                    Status = x.Status,
                    NumberOfViews = x.NumberOfViews,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                })
                .AsQueryable());
            var data = PageInfo<Products>.ToPageInfo(page, allProduct);
            page.TotalItem = await allProduct.CountAsync();
            return new PageInfo<Products>(page, data);
        }
        public async Task<PageInfo<Products>> FindProductByCategoryForAdminAsync(Pagination page, int categoryID)
        {
            var allProduct = await Task.FromResult(_dbContext.Products.Where(x => x.ProductTypeID == categoryID)
                .Select(x => new Products
                {
                    ProductID = x.ProductID,
                    ProductTypeID = x.ProductTypeID,
                    NameProduct = x.NameProduct,
                    Price = x.Price,
                    AvatarImageProduct = x.AvatarImageProduct,
                    Title = x.Title,
                    Discount = x.Discount,
                    Quantity = x.Quantity,
                    Status = x.Status,
                    NumberOfViews = x.NumberOfViews,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                })
                .AsQueryable());
            var data = PageInfo<Products>.ToPageInfo(page, allProduct);
            page.TotalItem = await allProduct.CountAsync();
            return new PageInfo<Products>(page, data);
        }
        public async Task<PageInfo<Products>> FindProductByNameForAdminAsync(Pagination page, string keyWord)
        {
            var allProduct = await Task.FromResult(_dbContext.Products.Where(x => x.NameProduct.ToLower().Contains(keyWord.ToLower()))
                .Select(x => new Products
                {
                    ProductID = x.ProductID,
                    ProductTypeID = x.ProductTypeID,
                    NameProduct = x.NameProduct,
                    Price = x.Price,
                    AvatarImageProduct = x.AvatarImageProduct,
                    Title = x.Title,
                    Discount = x.Discount,
                    Quantity = x.Quantity,
                    Status = x.Status,
                    NumberOfViews = x.NumberOfViews,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                })
                .AsQueryable());
            var data = PageInfo<Products>.ToPageInfo(page, allProduct);
            page.TotalItem = await allProduct.CountAsync();
            return new PageInfo<Products>(page, data);
        }
        #endregion
        //chi tiet san pham
        public async Task<Products> GetProductByIdAsync(int productId)
        {
            if (productId <= 0)
            {
                return null;
            }
            var product = await _dbContext.Products
                .Where(x => x.ProductID == productId)
                .Select(x => new Products
                {
                    ProductID = x.ProductID,
                    ProductTypeID = x.ProductTypeID,
                    NameProduct = x.NameProduct,
                    Price = x.Price,
                    AvatarImageProduct = x.AvatarImageProduct,
                    Title = x.Title,
                    Discount = x.Discount,
                    Quantity = x.Quantity,
                    Status = x.Status,
                    NumberOfViews = x.NumberOfViews
                })
                .FirstOrDefaultAsync();

            if (product != null && product.Status == Status.Active)
            {
                product.NumberOfViews += 1;
                await _dbContext.SaveChangesAsync();
            }

            return product;
        }
        //danh sach san pham 
        public async Task<PageInfo<Products>> GetAllProductAsync(Pagination page)
        {
            var allProduct = await Task.FromResult(_dbContext.Products.Where(x => x.Status == Status.Active)
                .Select(x => new Products
                {
                    ProductID = x.ProductID,
                    ProductTypeID = x.ProductTypeID,
                    NameProduct = x.NameProduct,
                    Price = x.Price,
                    AvatarImageProduct = x.AvatarImageProduct,
                    Title = x.Title,
                    Discount = x.Discount,
                    Quantity = x.Quantity,
                    Status = x.Status,
                    NumberOfViews = x.NumberOfViews
                })
                .AsQueryable());
            var data = PageInfo<Products>.ToPageInfo(page, allProduct);
            page.TotalItem = await allProduct.CountAsync();
            return new PageInfo<Products>(page, data);
        }

        //lấy ra sản phẩm liên quan
        public async Task<IQueryable<Products>> GetRelatedProductsAsync(int productId)
        {
            var product = await _dbContext.Products.FindAsync(productId);

            var relatedProducts = await Task.FromResult(_dbContext.Products
                .Where(p => p.ProductTypeID == product.ProductTypeID && p.ProductID != productId && p.Status == Status.Active)
                .Select(x => new Products
                {
                    ProductID = x.ProductID,
                    ProductTypeID = x.ProductTypeID,
                    NameProduct = x.NameProduct,
                    Price = x.Price,
                    AvatarImageProduct = x.AvatarImageProduct,
                    Title = x.Title,
                    Discount = x.Discount,
                    Quantity = x.Quantity,
                    Status = x.Status,
                    NumberOfViews = x.NumberOfViews
                })
                .Take(5)
                .AsQueryable());
            return relatedProducts;
        }
        //lay ra tat ca san pham sap xep theo hot
        public async Task<PageInfo<Products>> GetAllByHotProductAsync(Pagination page)
        {
            var allProduct = await Task.FromResult(_dbContext.Products.Where(x => x.Status == Status.Active).OrderByDescending(x => x.NumberOfViews)
                .Select(x => new Products
                {
                    ProductID = x.ProductID,
                    ProductTypeID = x.ProductTypeID,
                    NameProduct = x.NameProduct,
                    Price = x.Price,
                    AvatarImageProduct = x.AvatarImageProduct,
                    Title = x.Title,
                    Discount = x.Discount,
                    Quantity = x.Quantity,
                    Status = x.Status,
                    NumberOfViews = x.NumberOfViews
                })
                .AsQueryable());
            var data = PageInfo<Products>.ToPageInfo(page, allProduct);
            page.TotalItem = await allProduct.CountAsync();
            return new PageInfo<Products>(page, data);
        }

        public async Task<PageInfo<Products>> FindProductByNameAsync(Pagination page, string keyWord)
        {
            var allProduct = await Task.FromResult(_dbContext.Products.Where(x => x.Status == Status.Active && x.NameProduct.ToLower().Contains(keyWord.ToLower()))
                .Select(x => new Products
                {
                    ProductID = x.ProductID,
                    ProductTypeID = x.ProductTypeID,
                    NameProduct = x.NameProduct,
                    Price = x.Price,
                    AvatarImageProduct = x.AvatarImageProduct,
                    Title = x.Title,
                    Discount = x.Discount,
                    Quantity = x.Quantity,
                    Status = x.Status,
                    NumberOfViews = x.NumberOfViews
                })
                .AsQueryable());
            var data = PageInfo<Products>.ToPageInfo(page, allProduct);
            page.TotalItem = await allProduct.CountAsync();
            return new PageInfo<Products>(page, data);
        }

        public async Task<PageInfo<Products>> FindProductByCategoryAsync(Pagination page, int categoryID)
        {
            var allProduct = await Task.FromResult(_dbContext.Products.Where(x => x.Status == Status.Active && x.ProductTypeID == categoryID)
                .Select(x => new Products
                {
                    ProductID = x.ProductID,
                    ProductTypeID = x.ProductTypeID,
                    NameProduct = x.NameProduct,
                    Price = x.Price,
                    AvatarImageProduct = x.AvatarImageProduct,
                    Title = x.Title,
                    Discount = x.Discount,
                    Quantity = x.Quantity,
                    Status = x.Status,
                    NumberOfViews = x.NumberOfViews
                })
                .AsQueryable());
            var data = PageInfo<Products>.ToPageInfo(page, allProduct);
            page.TotalItem = await allProduct.CountAsync();
            return new PageInfo<Products>(page, data);
        }

        public async Task<PageInfo<Products>> FilterProductAsync(Pagination page, FilterProductForm filter)
        {
            // Bước 1: Lấy danh sách sản phẩm từ nguồn dữ liệu của bạn (database, API, v.v.)
            var allProducts = _dbContext.Products.Where(x => x.Status == Status.Active).Select(x => new Products
            {
                ProductID = x.ProductID,
                ProductTypeID = x.ProductTypeID,
                NameProduct = x.NameProduct,
                Price = x.Price,
                AvatarImageProduct = x.AvatarImageProduct,
                Title = x.Title,
                Discount = x.Discount,
                Quantity = x.Quantity,
                Status = x.Status,
                NumberOfViews = x.NumberOfViews
            }).AsQueryable();
            // Bước 2: Áp dụng các bộ lọc lên danh sách sản phẩm
            var filteredProducts = allProducts;
            if (filter.minPrice.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.Price >= filter.minPrice.Value);
            }

            if (filter.maxPrice.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.Price <= filter.maxPrice.Value);
            }

            if (filter.categoryID.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.ProductTypeID == filter.categoryID.Value);
            }

            if (filter.desc.HasValue && filter.desc.Value)
            {
                filteredProducts = filteredProducts.OrderByDescending(p => p.Price);
            }
            if (filter.desc.HasValue)
            {
                filteredProducts = filteredProducts.OrderByDescending(p => p.Price);
            }
            if (filter.asc.HasValue)
            {
                filteredProducts = filteredProducts.OrderBy(p => p.Price);
            }
            if (filter.hotProduct.HasValue && filter.hotProduct.Value)
            {
                filteredProducts = filteredProducts.OrderByDescending(p => p.NumberOfViews);
            }
            var data = PageInfo<Products>.ToPageInfo(page, filteredProducts);
            page.TotalItem = await filteredProducts.CountAsync();
            return new PageInfo<Products>(page, data);
        }


    }
}
