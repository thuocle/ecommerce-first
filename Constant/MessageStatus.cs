namespace API_Test1.Constant
{
    public enum MessageStatus
    {
        UnknownError = 0,                    // Lỗi không xác định
        InvalidCredentials = 1,              // Thông tin đăng nhập không hợp lệ
        EmailOrUsernameAlreadyExists = 2,              // Email đã tồn tại trong hệ thống
        MissMatchedPassword = 3,          // Email hoặc mật khẩu không hợp lệ
        AccountLocked = 4,                   // Tài khoản bị khóa
        AccountNotFound = 5,                 // Tài khoản không tồn tại
        UnauthorizedAccess = 6,              // Truy cập không được ủy quyền
        InvalidToken = 7,                    // Mã thông báo không hợp lệ
        PasswordResetFailed = 8,             // Đặt lại mật khẩu không thành công
        InactiveAccount = 9,                 // Tài khoản không hoạt động
        ExpiredAccount = 10,                 // Tài khoản đã hết hạn
        Failed, 
        Success,
        Empty,
        ExpiredToken
    }
}
