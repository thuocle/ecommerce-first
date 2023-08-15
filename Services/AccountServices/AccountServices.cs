

using Microsoft.AspNetCore.Http;

namespace API_Test1.Services.AccountServices
{
    public class AccountServices : IAccountServices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _dbContext;
        private readonly IMailServices _mailServices;
        private readonly IHttpContextAccessor _httpContext;

        public AccountServices(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, ApplicationDbContext dbContext, IMailServices mailServices, IHttpContextAccessor httpContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _dbContext = dbContext;
            _mailServices = mailServices;
            _httpContext = httpContext;
        }
        #region private
        //generate token for authentication on login
        private async Task<string> GenerateAuthenTokenAsync(ApplicationUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };
            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }
            var authSignInKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var token = new JwtSecurityToken
            (
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSignInKey, SecurityAlgorithms.HmacSha512Signature)
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        //create token for verify email, reset password =>STMP
        private string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }
        #endregion
        #region For User

        //register user
        public async Task<MessageStatus> RegisterAsync(RegisterModel registerModel)
        {
            ApplicationUser account = new()
            {
                FullName = registerModel.FullName,
                UserName = registerModel.UserName,
                Email = registerModel.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                Status = AccountStatus.Pending,
                VerifyToken = CreateRandomToken(),
                CreateAt = DateTime.Now
            };
            var userNameExist = await _userManager.FindByNameAsync(account.UserName);
            var emailExist = await _userManager.FindByEmailAsync(account.Email);

            if (userNameExist != null || emailExist != null)
                return MessageStatus.EmailOrUsernameAlreadyExists;
            if (registerModel.PassWord != registerModel.ConfirmPassWord)
                return MessageStatus.MissMatchedPassword;
            var result = await _userManager.CreateAsync(account, registerModel.PassWord);
            if (result != IdentityResult.Success)
            {
                return MessageStatus.Failed;
            }
            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));
            if (await _roleManager.RoleExistsAsync(UserRoles.User))
            {
                await _userManager.AddToRoleAsync(account, UserRoles.User);
            }
            //send mail confirm
            _mailServices.SendMail(new MailDTOs() { 
                To = registerModel.Email, 
                Body = $"<p>Đăng ký thành công, hãy xác nhận để trải nghiệm. Đây là mã xác nhận <b>{account.VerifyToken}</b>, hãy kích hoạt trong thời gian còn hiệu lực. Thời gian hiệu lực kết thúc là {account.VerifyTokenExpiry} kể từ khi nhận được thông báo này! Trân trọng</p>", 
                Subject="Đăng ký thành công!"
            });
            return MessageStatus.Success;
        }
        //verify email
        public async Task<MessageStatus> VerifyAccountAsync(string token)
        {
            var account = _userManager.Users.FirstOrDefault(x=> x.VerifyToken == token);
            if (account == null)
                return MessageStatus.InvalidToken;
            if (account.VerifyTokenExpiry < DateTime.Now)
                return MessageStatus.ExpiredToken;
            // neu dung token => active => xoa token
            account.VerifyToken = string.Empty;
            account.VerifyTokenExpiry = null;
            account.Status = AccountStatus.Active;
            await _userManager.UpdateAsync(account);
            return MessageStatus.Success;
        }
        //Login user
        public async Task<string> LoginAsync(LoginModel loginModel)
        {
            if (_userManager == null)
                return MessageStatus.Empty.ToString();
            var user = await _userManager.FindByNameAsync(loginModel.UserName);
            if (user != null && await _userManager.CheckPasswordAsync(user, loginModel.PassWord))
            {
                var token = await GenerateAuthenTokenAsync(user);
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddHours(3),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None
                };

                _httpContext.HttpContext.Response.Cookies.Append("MyCookiesWithLove", token, cookieOptions);
                return token;
            }
            return MessageStatus.AccountNotFound.ToString();
        }
      
        public async Task<MessageStatus> ForgotPasswordAsync(string email)
        {
            var account = _userManager.Users.FirstOrDefault(x => x.Email == email);
            if (account == null)
                return MessageStatus.AccountNotFound;
            account.ResetPasswordToken = CreateRandomToken();
            account.ResetPasswordTokenExpiry = DateTime.Now.AddMinutes(15);
            await _userManager.UpdateAsync(account);
            return MessageStatus.Success;
        }

        public async Task<MessageStatus> ResetPasswordAsync(ResetPasswordModel request)
        {
            var account = _userManager.Users.FirstOrDefault(x => x.ResetPasswordToken == request.Token);
            if (account == null)
                return MessageStatus.InvalidToken;
            if (account.ResetPasswordTokenExpiry < DateTime.Now)
                return MessageStatus.ExpiredToken;
            // neu dung token => active => xoa token
            var passwordHash = _userManager.PasswordHasher.HashPassword(account, request.Password);
            account.PasswordHash = passwordHash;
            account.ResetPasswordToken = string.Empty;
            account.ResetPasswordTokenExpiry = null;
            await _userManager.UpdateAsync(account);
            return MessageStatus.Success;
        }

        public async Task<MessageStatus> UpdateUserProfileAsync(string accountID, UserProfileModel request)
        {
            var acc = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == accountID);
            if (acc == null)
                return MessageStatus.AccountNotFound;
            acc.FullName = request.FullName;
            acc.UserName = request.UserName;
            acc.Email = request.Email;
            acc.Phone = request.Phone;
            acc.Address = request.Address;
            acc.Avatar = request.Avatar;
            return MessageStatus.Success;
        }

        public async Task<ApplicationUser> GetUserProfileAsync(string userID)
        {
            return await _userManager.FindByIdAsync(userID);
        }
        #endregion

        #region for admin
        //dùng cho lần đầu nhằm tạo các quyền và admin đầu tiên
        public async Task<MessageStatus> RegisterAdminAsync(RegisterModel registerModel)
        {
            ApplicationUser user = new()
            {
                UserName = registerModel.UserName,
                Email = registerModel.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
            };
            var userNameExist = await _userManager.FindByNameAsync(user.UserName);
            var emailExist = await _userManager.FindByEmailAsync(user.Email);
            if (userNameExist != null || emailExist != null)
                return MessageStatus.EmailOrUsernameAlreadyExists;
            if (registerModel.PassWord != registerModel.ConfirmPassWord)
                return MessageStatus.MissMatchedPassword;
            var result = await _userManager.CreateAsync(user, registerModel.PassWord);
            if (result != IdentityResult.Success)
            {
                return MessageStatus.Failed;
            }
            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));
            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Admin);
            }
            return MessageStatus.Success;
        }

        public async Task<string> LoginAdminAsync(LoginModel loginModel)
        {
            return MessageStatus.Success.ToString();

        }

        public async Task<MessageStatus> AddAccountAsync(RegisterModel registerModel)
        {
            return MessageStatus.Success;

        }

        public async Task<MessageStatus> UpdateUserAccountAsync(string userID, UserProfileModel userProfile)
        {
            return MessageStatus.Success;
        }

        public async Task<MessageStatus> UpdateAdminProfileAsync(string adminID, AdminProfileModel adminProfile)
        {
            return MessageStatus.Success;

        }

        public async Task<MessageStatus> RemoveUserAccountAsync(string userID)
        {
            return MessageStatus.Success;

        }

        /*public async Task<PageInfo<ApplicationUser>> GetAllUserAsync(Pagination page)
        {

        }*/
        #endregion
        #region Anonymous
        public async Task<MessageStatus> LogoutAsync()
        {
            return MessageStatus.Success;
        }

        
        #endregion

    }
}
