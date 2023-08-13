using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API_Test1
{
    public interface IAccountReposity
    {
        public Task<IdentityResult> Register(RegisterModel registerModel);
        public Task<string> Login(LoginModel loginModel);
    }
}
