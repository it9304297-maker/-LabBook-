using Microsoft.AspNetCore.Identity;


namespace RazorPageBase.Areas.Identity;

public class LocalizedIdentityErrorDescriber : IdentityErrorDescriber
{
    public override IdentityError DefaultError() => new IdentityError { Code = nameof(DefaultError), Description = "發生未知的錯誤。" };

    public override IdentityError DuplicateUserName(string userName) => new IdentityError { Code = nameof(DuplicateUserName), Description = $"帳號 '{userName}' 已被註冊。" };

    public override IdentityError DuplicateEmail(string email) => new IdentityError { Code = nameof(DuplicateEmail), Description = $"電子郵件 '{email}' 已被註冊。" };

    public override IdentityError InvalidEmail(string email) => new IdentityError { Code = nameof(InvalidEmail), Description = $"電子郵件 '{email}' 格式不正確。" };

    public override IdentityError PasswordTooShort(int length) => new IdentityError { Code = nameof(PasswordTooShort), Description = $"密碼長度至少需要 {length} 個字元。" };

    public override IdentityError PasswordRequiresNonAlphanumeric() => new IdentityError { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "密碼至少需要包含一個特殊符號（如 @、# 等）。" };

    public override IdentityError PasswordRequiresDigit() => new IdentityError { Code = nameof(PasswordRequiresDigit), Description = "密碼至少需要包含一個數字 ('0'-'9')。" };

    public override IdentityError PasswordRequiresLower() => new IdentityError { Code = nameof(PasswordRequiresLower), Description = "密碼至少需要包含一個英文小寫字母 ('a'-'z')。" };

    public override IdentityError PasswordRequiresUpper() => new IdentityError { Code = nameof(PasswordRequiresUpper), Description = "密碼至少需要包含一個英文大寫字母 ('A'-'Z')。" };

    public override IdentityError PasswordMismatch() => new IdentityError { Code = nameof(PasswordMismatch), Description = "密碼與確認密碼不符。" };

    public override IdentityError InvalidToken() => new IdentityError { Code = nameof(InvalidToken), Description = "無效的安全權杖。" };

    public override IdentityError LoginAlreadyAssociated() => new IdentityError { Code = nameof(LoginAlreadyAssociated), Description = "此外部帳號已經與現有的使用者建立連結。" };
}
