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
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {
            var re = await _AccountReposity.Register(registerModel);
            return Ok(re);
        }
    }
}
