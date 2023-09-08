using API_Test1.Services.ProductServices.ProductReviewServices;
namespace API_Test1.Controllers.Admin
{
    [Authorize(Roles =UserRoles.Admin)]
    [Route("api/admin/product-review")]
    [ApiController]
    public class AdminProductReviewController : ControllerBase
    {
        private readonly IProductReviewServices _productReviewServices;

        public AdminProductReviewController(IProductReviewServices productReviewServices)
        {
            _productReviewServices = productReviewServices;
        }
        [HttpPost("{reviewId}/status")]
        public async Task<IActionResult> UpdateProductReviewStatus(int reviewId, [FromForm] int status)
        {
            var result = await _productReviewServices.UpdateProductReviewStatus(reviewId, status);
            if (result == MessageStatus.Success)
                return Ok();
            else
                return BadRequest();
        }
        [HttpGet("{productId}")]
        public async Task<IActionResult> GetAllProductReviews(int productId, [FromQuery] Pagination page)
        {
            var productReviews = await _productReviewServices.GetAllProductReviews(productId, page);
            return Ok(productReviews);
        }
    }
}
