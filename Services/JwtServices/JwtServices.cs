using Microsoft.EntityFrameworkCore;

namespace API_Test1.Services.JwtServices
{
    public class JwtServices : IJwtServices
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _dbContext;

        public JwtServices(IHttpContextAccessor httpContextAccessor, IConfiguration configuration, ApplicationDbContext dbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _dbContext = dbContext;
        }

        public bool IsUserLoggedIn()
        {
            var token = GetTokenFromCookie();
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"])),
                ValidateIssuer = false,
                ValidateAudience = false
            };

            try
            {
                tokenHandler.ValidateToken(token, validationParameters, out _);
                return true;
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu token không hợp lệ
                return false;
            }
        }

        public  string GetUserId()
        {
            var userName = GetUsernameFromToken();
            var user =  _dbContext.Users.FirstOrDefault(x => x.UserName == userName);
            return user.Id;
        }
        public string GetUsernameFromToken()
        {
            var token = GetTokenFromCookie();
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jwtToken == null)
            {
                // Token không hợp lệ, xử lý tùy theo yêu cầu của bạn
                return null;
            }

            var username = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;

            return username;
        }
        private string GetTokenFromCookie()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            return httpContext.Request.Cookies["User"];
        }
    }
}