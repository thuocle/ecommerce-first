using API_Test1.Models.DTOs;
using API_Test1.Models.ViewModels;

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
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {
            var re = await _accountServices.RegisterAsync(registerModel);
            if (re != MessageStatus.Success)
                return BadRequest(re);

            return Ok("Thành công");
        }
        [HttpPost("verifyAccount")]
        public async Task<IActionResult> VerifyAccount(string token)
        {
            var re = await _accountServices.VerifyAccountAsync(token);
            if (re != MessageStatus.Success)
                return BadRequest("Ma xac nhan khong dung hoac da het han");
            return Ok(re);
        }
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            var re = await _accountServices.LoginAsync(loginModel);
            if (re == null)
                return BadRequest("Dang nhap that bai");

            return Ok(re);
        }
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var re = await _accountServices.ForgotPasswordAsync(email);
            if (re != MessageStatus.Success)
                return BadRequest("Email khong dung hoac tai khoan khong ton tai");
            return Ok(re);
        }
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            var re = await _accountServices.ResetPasswordAsync(resetPasswordModel);
            if (re != MessageStatus.Success)
                return BadRequest("Ma xac nhan khong dung hoac da het han");
            return Ok(re);
        }
        [Authorize(Roles = UserRoles.User)]
        [HttpPut("UpdateUserProfile")]
        public async Task<IActionResult> UpdateUserProfile(string userID, [FromForm] UserProfileModel userProfileModel)
        {
            var re = await _accountServices.UpdateUserProfileAsync(userID, userProfileModel);
            if (re != MessageStatus.Success)
                return BadRequest("Khong thanh cong");
            return Ok(re);
        }
        [Authorize(Roles = UserRoles.User)]
        [HttpGet("GetUserProfile/{userID}")]
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


        #region for Admin
        [HttpPost]
        [Route("registerAdmin")]
        public async Task<IActionResult> RegisterAdmin(RegisterModel registerModel)
        {
            var re = await _accountServices.RegisterAdminAsync(registerModel);
            if (re != MessageStatus.Success)
                return BadRequest(re);

            return Ok("Thành công");
        }
        [HttpPost("LoginAdmin")]
        public async Task<IActionResult> LoginAdmin(LoginModel loginModel)
        {
            var result = await _accountServices.LoginAdminAsync(loginModel);

            if (result == MessageStatus.Empty.ToString())
            {
                return BadRequest("UserManager is not available.");
            }
            else if (result == MessageStatus.AccountNotFound.ToString())
            {
                return BadRequest("Account not found.");
            }

            return Ok(result);
        }
        [HttpPost("AddAccount")]
        public async Task<IActionResult> AddAccount(AccountManage account)
        {
            var result = await _accountServices.AddAccountAsync(account);

            if (result == MessageStatus.EmailOrUsernameAlreadyExists)
            {
                return Conflict("Email or username already exists.");
            }
            else if (result == MessageStatus.Success)
            {
                return Ok("Account added successfully.");
            }
            else
            {
                return BadRequest("Failed to add account.");
            }
        }


        [HttpPost("UpdateUserAccount/{userId}")]
        public async Task<IActionResult> UpdateUserAccount(string userId, AccountManage accountModel)
        {
            var result = await _accountServices.UpdateUserAccountAsync(userId, accountModel);

            if (result == MessageStatus.AccountNotFound)
            {
                return NotFound("Account not found.");
            }
            else if (result == MessageStatus.Success)
            {
                return Ok("Account updated successfully.");
            }
            else
            {
                return BadRequest("Failed to update account.");
            }
        }
        [HttpPut("RemoveUserAccount/{userID}")]
        public async Task<IActionResult> RemoveUserAccount(string userID)
        {
            try
            {
                var result = await _accountServices.RemoveUserAccountAsync(userID);

                if (result == MessageStatus.Success)
                {
                    return Ok();
                }
                else if (result == MessageStatus.AccountNotFound)
                {
                    return NotFound();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("GetAllUser")]
        public async Task<IActionResult> GetAllUser([FromQuery] Pagination page)
        {
            try
            {
                var result = await _accountServices.GetAllUserAsync(page);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

    }
}
