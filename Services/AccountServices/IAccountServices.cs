namespace API_Test1.Services.AccountServices
{
    public interface IAccountServices
    {
        //for user
        public Task<MessageStatus> RegisterAsync(RegisterModel registerModel);
        public Task<MessageStatus> VerifyAccountAsync(string token);
        public Task<string> LoginAsync(LoginModel loginModel);
        public Task<MessageStatus> LogoutAsync();
        public Task<MessageStatus> ForgotPasswordAsync(string email);
        public Task<MessageStatus> ResetPasswordAsync(ResetPasswordModel request);
        public Task<MessageStatus> UpdateUserProfileAsync(string userID, UserProfileModel request);
        public IEnumerable<ApplicationUser> GetUserProfileAsync(string userID);
        //only admin
        public Task<MessageStatus> RegisterAdminAsync(RegisterModel registerModel);
        public Task<string> LoginAdminAsync(LoginModel loginModel);
        public Task<MessageStatus> AddAccountAsync(AccountManage account);
        public Task<MessageStatus> UpdateUserAccountAsync(string userID, AccountManage userProfile);
        public Task<MessageStatus> RemoveUserAccountAsync(string userID);
        public Task<PageInfo<AccountInfo>> GetAllUserAsync(Pagination page);
    }
}
