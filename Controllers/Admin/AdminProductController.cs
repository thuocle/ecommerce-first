using API_Test1.Services.ProductServices;

namespace API_Test1.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    [Route("api/admin")]
    [ApiController]
    public class AdminProductController : ControllerBase
    {
        private readonly IProductServices _productService;

        public AdminProductController(IProductServices productService)
        {
            _productService = productService;
        }
        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct([FromForm] ProductForm productModel)
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
        public async Task<IActionResult> UpdateProduct(int productId, [FromForm] ProductForm productModel)
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
    }
}
