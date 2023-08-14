
namespace API_Test1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountServices _AccountReposity;
        public AccountController(IAccountServices accountReposity)
        {
            _AccountReposity = accountReposity;
        }
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register( RegisterModel registerModel)
        {
            var re = await _AccountReposity.RegisterAsync(registerModel);
            return Ok(re);
        }
        [HttpPost]
        [Route("registerAdmin")]
        public async Task<IActionResult> RegisterAdmin( RegisterModel registerModel)
        {
            var re = await _AccountReposity.RegisterAdminAsync(registerModel);
            return Ok(re);
        }
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login( LoginModel loginModel)
        {
            var re = await _AccountReposity.LoginAsync(loginModel);
            return Ok(re);
        }
    }
}
