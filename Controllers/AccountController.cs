using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Test1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountReposity _AccountReposity;
        public AccountController(IAccountReposity accountReposity)
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
