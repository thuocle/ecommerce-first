using API_Test1.Services.ProductServices.ProductReviewServices;
namespace API_Test1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductReviewController : ControllerBase
    {
        private readonly IProductReviewServices _productReviewServices;

        public ProductReviewController(IProductReviewServices productReviewServices)
        {
            _productReviewServices = productReviewServices;
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetAllProductReviews(int productId, [FromQuery] Pagination page)
        {
            var productReviews = await _productReviewServices.GetAllProductReviews(productId, page);
            return Ok(productReviews);
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
