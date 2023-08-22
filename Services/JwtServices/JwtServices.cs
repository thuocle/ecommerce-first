namespace API_Test1.Services.JwtServices
{
    public class JwtServices
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public JwtServices(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
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

        public string GetUserId()
        {
            var token = GetTokenFromCookie();
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            return userId;
        }

        private string GetTokenFromCookie()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            return httpContext.Request.Cookies["User"];
        }
    }
}