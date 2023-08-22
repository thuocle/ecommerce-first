using API_Test1.Services.ProductServices.ProductTypeServices.ProductTypeServices;
namespace API_Test1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductTypeController : ControllerBase
    {
        private readonly IProductTypeServices _productTypeServices;

        public ProductTypeController(IProductTypeServices productTypeServices)
        {
            _productTypeServices = productTypeServices;
        }
        [HttpPost]
        public async Task<IActionResult> AddProductTypeAsync([FromBody] ProductTypes productType)
        {
            var result = await _productTypeServices.AddProductTypeAsync(productType);
            return Ok(result);
        }

        [HttpPut("{productTypeId}")]
        public async Task<IActionResult> UpdateProductTypeAsync(int productTypeId, [FromBody] ProductTypes productType)
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
