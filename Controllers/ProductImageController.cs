using API_Test1.Services.ProductServices.ProductImageServices;

namespace API_Test1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductImageController : ControllerBase
    {
        private readonly IProductImageServices _productImageService;

        public ProductImageController(IProductImageServices productImage)
        {
            _productImageService = productImage;
        }

        //lấy ra danh sách ảnh của 1 sản phẩm
        [HttpGet("product/{productId}/images")]
        public async Task<IActionResult> GetAllProductImagesByProductId(int productId)
        {
            var productImages = await _productImageService.GetProductImageByProductIdAsync(productId);
            return Ok(productImages);
        }
        //lấy ra danh sách tất cả ảnh sản phẩm
        [HttpGet("images")]
        public async Task<IActionResult> GetAllProductImages(Pagination pagination)
        {
            var productImages = await _productImageService.GetAllProductImagesForAdminAsync(pagination);
            return Ok(productImages);
        }
        //lấy ra 1 ảnh sản phẩm
        [HttpGet("images/{id}")]
        public async Task<IActionResult> GetProductImages(int id)
        {
            var productImages = await _productImageService.GetProductImageByIdAsync(id);
            return Ok(productImages);
        }
        [HttpPost]
        public async Task<IActionResult> AddProductImage([FromForm] ProductImageForm imageForm)
        {
            var productImages = await _productImageService.AddProductImageAsync(imageForm);
            return Ok(productImages);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateProductImage(int id, [FromForm] ProductImageForm imageForm)
        {
            var productImages = await _productImageService.UpdateProductImageAsync(id,imageForm);
            return Ok(productImages);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteProductImage(int id)
        {
            var productImages = await _productImageService.DeleteProductImageAsync(id);
            return Ok(productImages);
        }



    }
}
