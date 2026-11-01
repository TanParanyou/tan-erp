using TanErp.Application.Common.Results;

namespace TanErp.Application.IdentityAccess.Authentication;

public interface IPasswordHasher
{
    string HashPassword(string password);

    bool VerifyPassword(string password, string passwordHash);
}

public static class PasswordPolicy
{
    public const int MinLength = 8;
    public const int MaxLength = 128;

    public static Result<bool> Validate(string? password)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(password) || password.Length < MinLength)
        {
            return Result<bool>.Failure(new Error("PASSWORD_TOO_SHORT", "Password must be at least 8 characters long."));
        }

        if (password.Length > MaxLength)
        {
            return Result<bool>.Failure(new Error("REQUEST_VALIDATION_FAILED", "Password must not exceed 128 characters."));
        }

        return Result<bool>.Success(true);
    }
}
