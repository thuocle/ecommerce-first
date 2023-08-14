using API_Test1.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API_Test1.Services.AccountServices
{
    public interface IAccountServices
    {
        public Task<IdentityResult> RegisterAsync(RegisterModel registerModel);
        public Task<IdentityResult> RegisterAdminAsync(RegisterModel registerModel);
        public Task<string> LoginAsync(LoginModel loginModel);
    }
}
