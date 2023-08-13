using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API_Test1
{
    public class AccountReposity : IAccountReposity
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _dbContext;

        public AccountReposity(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _dbContext = dbContext;

        }
        //register user
        public async  Task<IdentityResult> Register(RegisterModel registerModel)
        {
            ApplicationUser user  = new()
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
                return IdentityResult.Failed(new IdentityError { Description = "Mat khau khong khop"});
            _dbContext.Add(user1);
            _dbContext.SaveChangesAsync();
            return await _userManager.CreateAsync(user, registerModel.PassWord);
        }

        public async Task<string> Login(LoginModel loginModel)
        {
            if (_userManager == null)
                return MessageStatus.Empty.ToString();
            var user = await _userManager.FindByNameAsync(loginModel.UserName);
            if(user != null && await _userManager.CheckPasswordAsync(user, loginModel.PassWord))
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };
                foreach(var userRole in userRoles)
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
                        signingCredentials: new SigningCredentials(authSignInKey, SecurityAlgorithms.HmacSha256)
                    );
                 return new JwtSecurityTokenHandler().WriteToken(token);
            }
            return MessageStatus.Fail.ToString();
        }

    }
}
