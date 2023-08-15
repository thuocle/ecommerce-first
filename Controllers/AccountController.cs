using API_Test1.Models.DTOs;
using API_Test1.Models.ViewModels;

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

        #region for user
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {
            var re = await _AccountReposity.RegisterAsync(registerModel);
            if (re != MessageStatus.Success)
                return BadRequest(re);

            return Ok("Thành công");
        }
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            var re = await _AccountReposity.LoginAsync(loginModel);
            if (re == null)
                return BadRequest("Dang nhap that bai");

            return Ok(re);
        }
        #endregion


        #region for Admin
        [HttpPost]
        [Route("registerAdmin")]
        public async Task<IActionResult> RegisterAdmin(RegisterModel registerModel)
        {
            var re = await _AccountReposity.RegisterAdminAsync(registerModel);
            if (re != MessageStatus.Success)
                return BadRequest(re);

            return Ok("Thành công");
        }
        #endregion

    }
}
