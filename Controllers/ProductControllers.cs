using API_Test1.Services.ProductServices;

namespace API_Test1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductControllers : ControllerBase
    {
            private readonly IProductServices _productService;

            public ProductControllers(IProductServices productService)
            {
                _productService = productService;
            }

            [HttpPost("AddProduct")]
            public async Task<IActionResult> AddProduct([FromForm]ProductForm productModel)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _productService.AddProductAsync(productModel);

                if (result == MessageStatus.Empty)
                {
                    return NotFound();
                }

                if (result == MessageStatus.Failed)
                {
                    return StatusCode(500); // Internal Server Error
                }

                return Ok(result);
            }

            [HttpPut("UpdateProduct/{productId}")]
            public async Task<IActionResult> UpdateProduct(int productId,[FromForm] ProductForm productModel)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _productService.UpdateProductAsync(productId, productModel);

                if (result == MessageStatus.Empty)
                {
                    return NotFound();
                }

                if (result == MessageStatus.Failed)
                {
                    return StatusCode(500); // Internal Server Error
                }

                return Ok(result);
            }

            [HttpDelete("RemoveProduct/{productId}")]
            public async Task<IActionResult> RemoveProduct(int productId)
            {
                var result = await _productService.RemoveProductAsync(productId);

                if (result == MessageStatus.Empty)
                {
                    return NotFound();
                }

                if (result == MessageStatus.Failed)
                {
                    return StatusCode(500); // Internal Server Error
                }

                return Ok(result);
            }

            [HttpGet("getallforAdmin")]
            public async Task<IActionResult> GetAllProductsForAdmin([FromQuery] Pagination page)
            {
                var result = await _productService.GetAllProductForAdminAsync(page);
                return Ok(result);
            }

            [HttpGet("categoryForAdmin/{categoryID}")]
            public async Task<IActionResult> FindProductsByCategoryForAdmin([FromQuery] Pagination page, int categoryID)
            {
                var result = await _productService.FindProductByCategoryForAdminAsync(page, categoryID);
                return Ok(result);
            }

            [HttpGet("nameForAdmin")]
            public async Task<IActionResult> FindProductsByNameForAdmin([FromQuery] Pagination page, string keyWord)
            {
                var result = await _productService.FindProductByNameForAdminAsync(page, keyWord);
                return Ok(result);
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
            public async Task<IActionResult> FilterProductAsync([FromQuery] Pagination page, [FromBody] FilterProductForm filter)
            {
                var result = await _productService.FilterProductAsync(page, filter);
                return Ok(result);
            }
        }
}
