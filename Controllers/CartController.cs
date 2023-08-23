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
        [HttpGet]
        public IActionResult GetCartItems()
        {
            var cartItems = _cartServices.GetCartItems();
            return Ok(cartItems);
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
        public IActionResult RemoveFromCart(int productId)
        {
            var result = _cartServices.RemoveFromCart(productId);
            if (result == MessageStatus.Success)
            {
                return Ok("Item removed from cart.");
            }
            return BadRequest("Failed to remove item from cart.");
        }

        [HttpPost("decrease")]
        public IActionResult DecreaseQuantity(int productId)
        {
            var result =  _cartServices.DecreaseQuantity(productId);
            if (result == MessageStatus.Success)
            {
                return Ok("Quantity decreased.");
            }
            return BadRequest("Failed to decrease quantity.");
        }

        [HttpPost("increase")]
        public IActionResult IncreaseQuantity(int productId)
        {
            var result =  _cartServices.IncreaseQuantity(productId);
            if (result == MessageStatus.Success)
            {
                return Ok("Quantity increased.");
            }
            return BadRequest("Failed to increase quantity.");
        }
        [HttpGet("quantity")]
        public IActionResult GetTotalQuantity()
        {
            var totalQuantity = _cartServices.GetTotalQuantity();
            return Ok(totalQuantity);
        }

        [HttpGet("originaltotalprice")]
        public IActionResult GetOriginalTotalPrice()
        {
            var originalTotalPrice = _cartServices.GetOriginalTotalPrice();
            return Ok(originalTotalPrice);
        }

        [HttpGet("totalprice")]
        public IActionResult GetTotalPrice()
        {
            var totalPrice = _cartServices.GetTotalPrice();
            return Ok(totalPrice);
        }

        [HttpPost("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var result = await _cartServices.ClearCart();
            if (result == MessageStatus.Success)
                return Ok();
            else
                return BadRequest();
        }

        [HttpGet("isempty")]
        public IActionResult IsCartEmpty()
        {
            var isEmpty = _cartServices.IsCartEmpty();
            return Ok(isEmpty);
        }
    }
}
