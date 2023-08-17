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
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {
            var re = await _AccountReposity.RegisterAsync(registerModel);
            if (re != MessageStatus.Success)
                return BadRequest(re);

            return Ok("Thành công");
        }
        [HttpPost("verifyAccount")]
        public async Task<IActionResult> VerifyAccount(string token)
        {
            var re = await _AccountReposity.VerifyAccountAsync(token);
            if (re != MessageStatus.Success)
                return BadRequest("Ma xac nhan khong dung hoac da het han");
            return Ok(re);
        }
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            var re = await _AccountReposity.LoginAsync(loginModel);
            if (re == null)
                return BadRequest("Dang nhap that bai");

            return Ok(re);
        }
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var re = await _AccountReposity.ForgotPasswordAsync(email);
            if (re != MessageStatus.Success)
                return BadRequest("Email khong dung hoac tai khoan khong ton tai");
            return Ok(re);
        }
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            var re = await _AccountReposity.ResetPasswordAsync(resetPasswordModel);
            if (re != MessageStatus.Success)
                return BadRequest("Ma xac nhan khong dung hoac da het han");
            return Ok(re);
        }
        [Authorize(Roles = UserRoles.User)]
        [HttpPut("UpdateUserProfile")]
        public async Task<IActionResult> UpdateUserProfile(string userID, [FromForm] UserProfileModel userProfileModel)
        {
            var re = await _AccountReposity.UpdateUserProfileAsync(userID, userProfileModel);
            if (re != MessageStatus.Success)
                return BadRequest("Khong thanh cong");
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
