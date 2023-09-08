namespace API_Test1.Constant
{
    public class MessageStatus
    {
        public static readonly MessageStatus UnknownError = new MessageStatus("UnknownError", "Unknown error.");
        public static readonly MessageStatus InvalidCredentials = new MessageStatus("InvalidCredentials", "Invalid username or password.");
        public static readonly MessageStatus EmailOrUsernameAlreadyExists = new MessageStatus("EmailOrUsernameAlreadyExists", "Email or username already exists.");
        public static readonly MessageStatus MissMatchedPassword = new MessageStatus("MissMatchedPassword", "Email or password is invalid.");
        public static readonly MessageStatus AccountLocked = new MessageStatus("AccountLocked", "The account is locked.");
        public static readonly MessageStatus AccountNotFound = new MessageStatus("AccountNotFound", "The account does not exist.");
        public static readonly MessageStatus UnauthorizedAccess = new MessageStatus("UnauthorizedAccess", "Unauthorized access.");
        public static readonly MessageStatus InvalidToken = new MessageStatus("InvalidToken", "Invalid token.");
        public static readonly MessageStatus PasswordResetFailed = new MessageStatus("PasswordResetFailed", "Password reset failed.");
        public static readonly MessageStatus InactiveAccount = new MessageStatus("InactiveAccount", "The account is inactive.");
        public static readonly MessageStatus ExpiredAccount = new MessageStatus("ExpiredAccount", "The account has expired.");
        public static readonly MessageStatus Failed = new MessageStatus("Failed", "Operation failed.");
        public static readonly MessageStatus Success = new MessageStatus("Success", "Operation successful.");
        public static readonly MessageStatus Empty = new MessageStatus("Empty", "Empty.");
        public static readonly MessageStatus ExpiredToken = new MessageStatus("ExpiredToken", "Expired token.");

        public string Code { get; }
        public string Message { get; }

        private MessageStatus(string code, string message)
        {
            Code = code;
            Message = message;
        }
    }
}