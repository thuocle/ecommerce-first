using API_Test1.Services.ProductServices.ProductReviewServices;
namespace API_Test1.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductReviewController : ControllerBase
    {
        private readonly IProductReviewServices _productReviewServices;

        [HttpPost("{productId}")]
        public async Task<IActionResult> AddProductReview(int productId, [FromForm] ReviewProductForm reviewProduct)
        {
            var result = await _productReviewServices.AddProductReview(productId, reviewProduct);
            if (result == MessageStatus.Success)
                return Ok();
            else if (result == MessageStatus.UnauthorizedAccess)
                return Unauthorized();
            else
                return BadRequest();
        }

        [HttpGet("{productId}/content")]
        public async Task<IActionResult> GetProductReviewsWithContent(int productId, [FromQuery] Pagination page)
        {
            var productReviews = await _productReviewServices.GetProductReviewsWithContent(productId, page);
            return Ok(productReviews);
        }

        [HttpPut("{reviewId}")]
        public async Task<IActionResult> UpdateProductReview(int reviewId, [FromForm] ReviewProductForm reviewProduct)
        {
            var result = await _productReviewServices.UpdateProductReview(reviewId, reviewProduct);
            if (result == MessageStatus.Success)
                return Ok();
            else if (result == MessageStatus.Empty)
                return NotFound();
            else
                return BadRequest();
        }

        [HttpDelete("{reviewId}")]
        public async Task<IActionResult> RemoveProductReview(int reviewId)
        {
            var result = await _productReviewServices.RemoveProductReview(reviewId);
            if (result == MessageStatus.Success)
                return Ok();
            else if (result == MessageStatus.Empty)
                return NotFound();
            else
                return BadRequest();
        }
    }
}
