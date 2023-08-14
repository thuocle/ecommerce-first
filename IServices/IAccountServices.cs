﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API_Test1.IServices
{
    public interface IAccountServices
    {
        public Task<IdentityResult> RegisterAsync(RegisterModel registerModel);
        public Task<IdentityResult> RegisterAdminAsync(RegisterModel registerModel);
        public Task<string> LoginAsync(LoginModel loginModel);
    }
}