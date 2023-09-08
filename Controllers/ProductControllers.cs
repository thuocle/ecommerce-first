using API_Test1.Services.ProductServices;

namespace API_Test1.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductControllers : ControllerBase
    {
            private readonly IProductServices _productService;

            public ProductControllers(IProductServices productService)
            {
                _productService = productService;
            }
            
            [HttpGet("{productId}")]
            public async Task<IActionResult> GetProductByIdAsync(int productId)
            {
                if (productId <= 0)
                {
                    return NotFound();
                }

                var product = await _productService.GetProductByIdAsync(productId);

                if (product == null)
                {
                    return NotFound();
                }

                return Ok(product);
            }

            [HttpGet("allproduct")]
            public async Task<IActionResult> GetAllProductAsync([FromQuery] Pagination page)
            {
                var result = await _productService.GetAllProductAsync(page);
                return Ok(result);
            }

            [HttpGet("related/{productId}")]
            public async Task<IActionResult> GetRelatedProductsAsync(int productId)
            {
                var result = await _productService.GetRelatedProductsAsync(productId);
                return Ok(result);
            }

            [HttpGet("hot")]
            public async Task<IActionResult> GetAllByHotProductAsync([FromQuery] Pagination page)
            {
                var result = await _productService.GetAllByHotProductAsync(page);
                return Ok(result);
            }

            [HttpGet("search")]
            public async Task<IActionResult> FindProductByNameAsync([FromQuery] Pagination page, string keyWord)
            {
                var result = await _productService.FindProductByNameAsync(page, keyWord);
                return Ok(result);
            }

            [HttpGet("category/{categoryId}")]
            public async Task<IActionResult> FindProductByCategoryAsync([FromQuery] Pagination page, int categoryId)
            {
                var result = await _productService.FindProductByCategoryAsync(page, categoryId);
                return Ok(result);
            }

            [HttpPost("filter")]
            public async Task<IActionResult> FilterProductAsync([FromQuery] Pagination page, [FromForm] FilterProductForm filter)
            {
                var result = await _productService.FilterProductAsync(page, filter);
                return Ok(result);
            }
        }
}
