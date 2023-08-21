using API_Test1.Services.CartServices;
namespace API_Test1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartServices _cartServices;

        public CartController(ICartServices cartServices)
        {
            _cartServices = cartServices;
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddToCart(int productId)
        {
            var result = await _cartServices.AddToCart(productId);
            if (result == MessageStatus.Success)
            {
                return Ok("Item added to cart.");
            }
            return BadRequest("Failed to add item to cart.");
        }

        [HttpPost("remove")]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var result = await _cartServices.RemoveFromCart(productId);
            if (result == MessageStatus.Success)
            {
                return Ok("Item removed from cart.");
            }
            return BadRequest("Failed to remove item from cart.");
        }

        [HttpPost("decrease")]
        public async Task<IActionResult> DecreaseQuantity(int productId)
        {
            var result = await _cartServices.DecreaseQuantity(productId);
            if (result == MessageStatus.Success)
            {
                return Ok("Quantity decreased.");
            }
            return BadRequest("Failed to decrease quantity.");
        }

        [HttpPost("increase")]
        public async Task<IActionResult> IncreaseQuantity(int productId)
        {
            var result = await _cartServices.IncreaseQuantity(productId);
            if (result == MessageStatus.Success)
            {
                return Ok("Quantity increased.");
            }
            return BadRequest("Failed to increase quantity.");
        }
    }
}
