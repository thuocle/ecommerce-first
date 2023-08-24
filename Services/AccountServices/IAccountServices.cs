namespace API_Test1.Services.AccountServices
{
    public interface IAccountServices
    {
        //for user
        public Task<MessageStatus> RegisterAsync(RegisterForm registerModel);
        public Task<MessageStatus> VerifyAccountAsync(string token);
        public Task<string> LoginAsync(LoginForm loginModel);
        public Task<MessageStatus> LogoutAsync();
        public Task<MessageStatus> ForgotPasswordAsync(string email);
        public Task<MessageStatus> ResetPasswordAsync(ResetPasswordForm request);
        public Task<MessageStatus> UpdateUserProfileAsync(string userID, UserProfileForm request);
        public Task<ApplicationUser> GetUserProfileAsync(string userID);
        //only admin
        public Task<MessageStatus> RegisterAdminAsync(RegisterForm registerModel);
        public Task<string> LoginAdminAsync(LoginForm loginModel);
        public Task<MessageStatus> AddAccountAsync(AccountForm account);
        public Task<MessageStatus> UpdateUserAccountAsync(string userID, AccountForm userProfile);
        public Task<MessageStatus> RemoveUserAccountAsync(string userID);
        public Task<PageInfo<AccountInfo>> GetAllUserAsync(Pagination page);
    }
}
