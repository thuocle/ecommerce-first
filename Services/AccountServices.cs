namespace API_Test1.Services
{
    public class AccountServices : IAccountServices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _dbContext;

        public AccountServices(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _dbContext = dbContext;

        }
        //register user
        public async Task<IdentityResult> RegisterAsync(RegisterModel registerModel)
        {
            ApplicationUser user = new()
            {
                UserName = registerModel.UserName,
                Email = registerModel.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
            };
            Users user1 = new()
            {
                UserID = user.Id,
                UserName = registerModel.UserName,
                FullName = registerModel.FullName,
                Email = registerModel.Email,
                Phone = string.Empty,
                Address = string.Empty
            };
            var userNameExist = await _userManager.FindByNameAsync(user.UserName);
            var emailExist = await _userManager.FindByEmailAsync(user.Email);

            if (userNameExist != null || emailExist != null)
                return IdentityResult.Failed(new IdentityError { Description = "UserName hoac Email da duoc dung" });
            if (registerModel.PassWord != registerModel.ConfirmPassWord)
                return IdentityResult.Failed(new IdentityError { Description = "Mat khau khong khop" });
            _dbContext.Add(user1);
            _dbContext.SaveChangesAsync();
            var result = await _userManager.CreateAsync(user, registerModel.PassWord);
            if (result != IdentityResult.Success)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Loi" });
            }
            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));
            if (await _roleManager.RoleExistsAsync(UserRoles.User))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.User);
            }
            return IdentityResult.Success;
        }
        //generate token for authentication on login
        public async Task<string> GenerateAuthenTokenAsync(ApplicationUser user)
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

        public async Task<string> LoginAsync(LoginModel loginModel)
        {
            if (_userManager == null)
                return MessageStatus.Empty.ToString();
            var user = await _userManager.FindByNameAsync(loginModel.UserName);
            if (user != null && await _userManager.CheckPasswordAsync(user, loginModel.PassWord))
            {
                var token = await GenerateAuthenTokenAsync(user);
                return token;
            }
            return MessageStatus.Fail.ToString();
        }

        public async Task<IdentityResult> RegisterAdminAsync(RegisterModel registerModel)
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
                return IdentityResult.Failed(new IdentityError { Description = "UserName hoac Email da duoc dung" });
            if (registerModel.PassWord != registerModel.ConfirmPassWord)
                return IdentityResult.Failed(new IdentityError { Description = "Mat khau khong khop" });
            var result = await _userManager.CreateAsync(user, registerModel.PassWord);
            if (result != IdentityResult.Success)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Loi" });
            }
            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));
            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Admin);
            }
            return IdentityResult.Success;
        }


        //create token for verify email, reset password =>STMP
        private string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }
    }
}
