using API_Test1.Services.JwtServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Test1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JwtController : ControllerBase
    {
        private readonly IJwtServices _jwtServices;

        public JwtController(IJwtServices jwtServices)
        {
            _jwtServices = jwtServices;
        }

        [HttpGet("getuser")]
        public IActionResult GetUserId()
        {
            var re = _jwtServices.GetUserId();
            return Ok(re);
        }
        [HttpGet("isloggedin")]
        public IActionResult IsLogged()
        {
            var re =  _jwtServices.IsUserLoggedIn();
            return Ok(re);
        }

    }
}
