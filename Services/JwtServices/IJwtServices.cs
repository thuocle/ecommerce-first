namespace API_Test1.Services.JwtServices
{
    public interface IJwtServices
    {
        //kiem tra login chưa
        public bool IsUserLoggedIn();
        public string GetUserId();
    }
}
