using API_Test1.Models.DTOs;
using API_Test1.Services.OrderServices;
using System.Drawing;

namespace API_Test1.Controllers.Admin
{
    [Route("api/admin")]
    [ApiController]
    public class AdminAccountController : ControllerBase
    {
        private readonly IAccountServices _accountServices;

        public AdminAccountController(IAccountServices accountServices)
        {
            _accountServices = accountServices;
        }

        #region for account
/*        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromForm] RegisterForm registerModel)
        {
            var re = await _accountServices.RegisterAdminAsync(registerModel);
            if (re != MessageStatus.Success)
                return BadRequest(re);

            return Ok("Thành công");
        }*/
        [HttpPost("login")]
        public async Task<IActionResult> LoginAdmin([FromForm] LoginForm loginModel)
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
        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost("add-account")]
        public async Task<IActionResult> AddAccount([FromForm] AccountForm account)
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

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPut("update-user-account/{userId}")]
        public async Task<IActionResult> UpdateUserAccount(string userId, [FromForm] AccountForm accountModel)
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

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPut("remove-account/{userID}")]
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

        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet("get-all-user")]
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
