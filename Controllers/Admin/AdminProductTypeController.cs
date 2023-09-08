using API_Test1.Services.ProductServices.ProductTypeServices.ProductTypeServices;
namespace API_Test1.Controllers.Admin
{
    [Authorize(Roles =UserRoles.Admin)]
    [Route("api/admin/product-type")]
    [ApiController]
    public class AdminProductTypeController : ControllerBase
    {
        private readonly IProductTypeServices _productTypeServices;

        public AdminProductTypeController(IProductTypeServices productTypeServices)
        {
            _productTypeServices = productTypeServices;
        }
        [HttpPost]
        public async Task<IActionResult> AddProductTypeAsync([FromForm] ProductTypeForm productType)
        {
            var result = await _productTypeServices.AddProductTypeAsync(productType);
            return Ok(result);
        }

        [HttpPut("{productTypeId}")]
        public async Task<IActionResult> UpdateProductTypeAsync(int productTypeId, [FromForm] ProductTypeForm productType)
        {
            var result = await _productTypeServices.UpdateProductTypeAsync(productTypeId, productType);
            return Ok(result);
        }

        [HttpDelete("{productTypeId}")]
        public async Task<IActionResult> DeleteProductTypeAsync(int productTypeId)
        {
            var result = await _productTypeServices.DeleteProductTypeAsync(productTypeId);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProductTypesAsync([FromQuery] Pagination page)
        {
            var result = await _productTypeServices.GetAllProductTypesAsync(page);
            return Ok(result);
        }

        [HttpGet("{productTypeId}")]
        public async Task<IActionResult> GetProductTypeByIdAsync(int productTypeId)
        {
            var result = await _productTypeServices.GetProductTypeByIdAsync(productTypeId);
            return Ok(result);
        }
    }
}
