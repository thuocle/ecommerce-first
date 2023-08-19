using API_Test1.Services.FileServices;
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
        private readonly IFileServices _fileServices;

        public AccountServices(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, ApplicationDbContext dbContext, IMailServices mailServices, IHttpContextAccessor httpContext, IFileServices fileServices)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _dbContext = dbContext;
            _mailServices = mailServices;
            _httpContext = httpContext;
            _fileServices = fileServices;
        }
        #region private
        //generate token for authentication on login
        private async Task<string> GenerateAuthTokenAsync(ApplicationUser user)
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
            Random rd = new Random();
            return rd.Next(999999).ToString();
        }
        //check quyen
        private async Task EnsureRoleExists(string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
        #endregion
        #region For User

        //register user
        public async Task<MessageStatus> RegisterAsync(RegisterModel registerModel)
        {
            // validate
            var userNameExist = await _userManager.FindByNameAsync(registerModel.UserName);
            var emailExist = await _userManager.FindByEmailAsync(registerModel.Email);

            if (userNameExist != null || emailExist != null)
                return MessageStatus.EmailOrUsernameAlreadyExists;
            if (registerModel.Password != registerModel.ConfirmPassword)
                return MessageStatus.MissMatchedPassword;
            // tao 1 account moi
            ApplicationUser account = new()
            {
                FullName = registerModel.FullName,
                UserName = registerModel.UserName,
                Email = registerModel.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                Status = AccountStatus.Pending,
                VerifyToken = CreateRandomToken(),
                VerifyTokenExpiry = DateTime.Now.AddMinutes(15),
                CreateAt = DateTime.Now

            };
            //thêm vào DB
            var result = await _userManager.CreateAsync(account, registerModel.Password);
            if (!result.Succeeded)
            {
                return MessageStatus.Failed;
            }
            //Phân quyền, đặt mặc định là user
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
                Body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{
            font-family: Arial, sans-serif;
            font-size: 14px;
            line-height: 1.5;
            color: #333333;
        }}
        
        .container {{
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
        }}

        .header {{
            background-color: #f5f5f5;
            padding: 10px;
            text-align: center;
        }}

        .content {{
            padding: 20px;
            background-color: #ffffff;
            border: 1px solid #dddddd;
        }}

        .token {{
            font-weight: bold;
            font-size: 18px;
            color: #ff0000;
        }}

        .footer {{
            padding: 10px;
            text-align: center;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h2>Đăng ký thành công</h2>
        </div>
        
        <div class='content'>
            <p>
                Đăng ký thành công, hãy xác nhận để trải nghiệm.
                Đây là mã xác nhận: <span class='token'>{account.VerifyToken}</span>.
                Hãy kích hoạt trong thời gian còn hiệu lực.
                Thời gian hiệu lực kết thúc là {account.VerifyTokenExpiry} kể từ khi nhận được thông báo này!
            </p>
        </div>
        
        <div class='footer'>
            <p>Trân trọng,</p>
        </div>
    </div>
</body>
</html>", 
                Subject ="Đăng ký thành công!"
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
            account.Status = AccountStatus.Active;
            account.VerifyToken = string.Empty;
            account.VerifyTokenExpiry = null;
            await _userManager.UpdateAsync(account);
            return MessageStatus.Success;
        }
        //Login user
        public async Task<string> LoginAsync(LoginModel loginModel)
        {
            if (_userManager == null)
                return MessageStatus.Empty.ToString();
            var user = await _userManager.FindByNameAsync(loginModel.UserName);
            if (user != null && await _userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                var token = await GenerateAuthTokenAsync(user);
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
        //forgot
        public async Task<MessageStatus> ForgotPasswordAsync(string email)
        {
            var account = _userManager.Users.FirstOrDefault(x => x.Email == email);
            if (account == null && account.Status != AccountStatus.Active)
                return MessageStatus.AccountNotFound;
            account.ResetPasswordToken = CreateRandomToken();
            account.ResetPasswordTokenExpiry = DateTime.Now.AddMinutes(15);
            await _userManager.UpdateAsync(account);
            return MessageStatus.Success;
        }
        // reset
        public async Task<MessageStatus> ResetPasswordAsync(ResetPasswordModel request)
        {
            // kiem tra token và hieu luc
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
        //update profile for user
        public async Task<MessageStatus> UpdateUserProfileAsync(string accountID, UserProfileModel request)
        {
            var acc = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == accountID);
            if (acc == null)
                return MessageStatus.AccountNotFound;
            acc.FullName = request.FullName;
            acc.UserName = request.UserName;
            acc.Email = request.Email;
            acc.PhoneNumber = request.Phone;
            acc.Address = request.Address;
            acc.UpdateAt = DateTime.Now;
            if (request.Avatar != null)
            {
                // Tạo một đối tượng FileServices từ lớp chứa nó
                string avatarFileId = await _fileServices.UploadImage(request.Avatar);
                acc.Avatar = avatarFileId;
            }
            // Lưu các thay đổi vào cơ sở dữ liệu
            await _userManager.UpdateAsync(acc);
            return MessageStatus.Success;
        }
        // get a user
        public async Task<ApplicationUser> GetUserProfileAsync(string userID)
        {
            var user = await _userManager.Users
                .AsNoTracking() // Không theo dõi thay đổi để tăng hiệu suất
                .Where(x => x.Id == userID)
                .Select(x => new ApplicationUser
                {
                    FullName = x.FullName,
                    UserName = x.UserName,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber,
                    Address = x.Address,
                    Avatar = x.Avatar
                })
                .SingleOrDefaultAsync(); // Sử dụng SingleOrDefaultAsync thay vì AsQueryable để trả về một đối tượng duy nhất

            return user;
        }
        #endregion

        #region for admin
        //dùng cho lần đầu nhằm tạo các quyền và admin đầu tiên
        public async Task<MessageStatus> RegisterAdminAsync(RegisterModel registerModel)
        {
            var user = new ApplicationUser
            {
                UserName = registerModel.UserName,
                Email = registerModel.Email,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var userNameExist = await _userManager.FindByNameAsync(user.UserName);
            var emailExist = await _userManager.FindByEmailAsync(user.Email);

            if (userNameExist != null || emailExist != null)
                return MessageStatus.EmailOrUsernameAlreadyExists;

            if (registerModel.Password != registerModel.ConfirmPassword)
                return MessageStatus.MissMatchedPassword;

            var result = await _userManager.CreateAsync(user, registerModel.Password);

            if (result.Succeeded)
            {
                await EnsureRoleExists(UserRoles.Admin);
                await EnsureRoleExists(UserRoles.User);

                await _userManager.AddToRoleAsync(user, UserRoles.Admin);

                return MessageStatus.Success;
            }
            else
            {
                return MessageStatus.Failed;
            }
        }

        public async Task<string> LoginAdminAsync(LoginModel loginModel)
        {
            if (_userManager == null)
                return MessageStatus.Empty.ToString();

            var user = await _userManager.FindByNameAsync(loginModel.UserName);

            if (user != null && await _userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                var token = await GenerateAuthTokenAsync(user);

                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddYears(1),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None
                };

                _httpContext.HttpContext.Response.Cookies.Append("MyCookiesWithLove", token, cookieOptions);

                return token;
            }
            else
            {
                return MessageStatus.AccountNotFound.ToString();
            }
        }

        public async Task<MessageStatus> AddAccountAsync(AccountManage account)
        {
            var existingEmail = await _userManager.FindByEmailAsync(account.Email);
            var existingUsername = await _userManager.FindByNameAsync(account.UserName);

            if (existingEmail != null || existingUsername != null)
            {
                return MessageStatus.EmailOrUsernameAlreadyExists;
            }

            var user = new ApplicationUser
            {
                FullName = account.FullName,
                Email = account.Email,
                UserName = account.UserName,
                SecurityStamp = Guid.NewGuid().ToString(),
                Status = AccountStatus.Active,
                CreateAt = DateTime.Now
            };

            if (account.Avatar != null)
            {
                user.Avatar = await _fileServices.UploadImage(account.Avatar);
            }

            var result = await _userManager.CreateAsync(user, account.Password);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(account.UserRoles) && await _roleManager.RoleExistsAsync(account.UserRoles))
                {
                    await _userManager.AddToRoleAsync(user, account.UserRoles);
                }
                return MessageStatus.Success;
            }

            return MessageStatus.Failed;
        }
        public async Task<MessageStatus> UpdateUserAccountAsync(string userId, AccountManage accountModel)
        {
            var account = await _userManager.FindByIdAsync(userId);

            if (account == null)
            {
                return MessageStatus.AccountNotFound;
            }

            account.FullName = accountModel.FullName;
            account.UserName = accountModel.UserName;
            account.Email = accountModel.Email;
            account.PhoneNumber = accountModel.PhoneNumber;
            account.Status = accountModel.Status;
            account.UpdateAt = DateTime.Now;

            if (accountModel.Avatar != null)
            {
                string avatarFileId = await _fileServices.UploadImage(accountModel.Avatar);
                account.Avatar = avatarFileId;
            }

            if (!string.IsNullOrEmpty(accountModel.Password))
            {
                var removePasswordResult = await _userManager.RemovePasswordAsync(account);

                if (!removePasswordResult.Succeeded)
                {
                    return MessageStatus.Failed;
                }

                var addPasswordResult = await _userManager.AddPasswordAsync(account, accountModel.Password);

                if (!addPasswordResult.Succeeded)
                {
                    return MessageStatus.Failed;
                }
            }

            if (!string.IsNullOrEmpty(accountModel.UserRoles) && await _roleManager.RoleExistsAsync(accountModel.UserRoles))
            {
                var currentRoles = await _userManager.GetRolesAsync(account);
                await _userManager.RemoveFromRolesAsync(account, currentRoles);

                await _userManager.AddToRoleAsync(account, accountModel.UserRoles);
            }

            var result = await _userManager.UpdateAsync(account);

            if (result.Succeeded)
            {
                return MessageStatus.Success;
            }
            else
            {
                return MessageStatus.Failed;
            }
        }
        public async Task<MessageStatus> RemoveUserAccountAsync(string userID)
        {
            //chuyển active sang locked
            var account = await _userManager.FindByIdAsync(userID);
            account.Status = AccountStatus.Disabled;
            return MessageStatus.Success;
        }
        public async Task<PageInfo<AccountInfo>> GetAllUserAsync(Pagination page)
        {
            var allqr = _userManager.Users
                .Join(_dbContext.UserRoles, user => user.Id, roleuser => roleuser.UserId, (user, roleuser) => new { User = user, RoleUserId = roleuser.RoleId })
                .Join(_dbContext.Roles, temp => temp.RoleUserId, role => role.Id, (temp, role) => new AccountInfo { User = temp.User, RoleName = role.Name });

            var alluser = allqr.AsQueryable();
            var data = PageInfo<AccountInfo>.ToPageInfo(page, alluser);
            page.TotalItem = await alluser.CountAsync();
            return new PageInfo<AccountInfo>(page, data);
        }
        #endregion
        #region Anonymous
        public async Task<MessageStatus> LogoutAsync()
        {
            // Lấy thông tin user từ HttpContext
            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);

            if (user != null)
            {
                // Đăng xuất người dùng khỏi ứng dụng

                // Xóa cookie chứa token
                _httpContext.HttpContext.Response.Cookies.Delete("MyCookiesWithLove");

                return MessageStatus.Success;
            }

            return MessageStatus.Failed;
        }
        //ok
        #endregion

    }
}
