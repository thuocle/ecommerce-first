namespace API_Test1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountServices _accountServices;
        public AccountController(IAccountServices accountServices)
        {
            _accountServices = accountServices;
        }

        #region for user
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromForm] RegisterForm registerModel)
        {
            var re = await _accountServices.RegisterAsync(registerModel);
            if (re != MessageStatus.Success)
                return BadRequest(re);

            return Ok("Thành công");
        }
        [HttpPost("verify-account")]
        public async Task<IActionResult> VerifyAccount(string token)
        {
            var re = await _accountServices.VerifyAccountAsync(token);
            if (re != MessageStatus.Success)
                return BadRequest("Ma xac nhan khong dung hoac da het han");
            return Ok(re);
        }
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromForm] LoginForm loginModel)
        {
            var re = await _accountServices.LoginAsync(loginModel);
            if (re == null)
                return BadRequest("Dang nhap that bai");

            return Ok(re);
        }
        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> ResfreshToken()
        {
            var re = await _accountServices.RefreshToken();
            if (re == null)
                return BadRequest("that bai");

            return Ok(re);
        }
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var re = await _accountServices.ForgotPasswordAsync(email);
            if (re != MessageStatus.Success)
                return BadRequest("Email khong dung hoac tai khoan khong ton tai");
            return Ok(re);
        }
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordForm resetPasswordModel)
        {
            var re = await _accountServices.ResetPasswordAsync(resetPasswordModel);
            if (re != MessageStatus.Success)
                return BadRequest("Ma xac nhan khong dung hoac da het han");
            return Ok(re);
        }
        [Authorize(Roles = UserRoles.User)]
        [HttpPut("update-user-profile")]
        public async Task<IActionResult> UpdateUserProfile(string userID, [FromForm] UserProfileForm userProfileModel)
        {
            var re = await _accountServices.UpdateUserProfileAsync(userID, userProfileModel);
            if (re != MessageStatus.Success)
                return BadRequest("Khong thanh cong");
            return Ok(re);
        }
        [Authorize(Roles = UserRoles.User)]
        [HttpGet("get-user-profile/{userID}")]
        public async Task<IActionResult> GetUserProfile(string userID)
        {
            var user = await _accountServices.GetUserProfileAsync(userID);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
        #endregion

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var result = await _accountServices.LogoutAsync();
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
