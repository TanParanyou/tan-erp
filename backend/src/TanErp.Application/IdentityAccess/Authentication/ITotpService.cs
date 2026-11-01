namespace TanErp.Application.IdentityAccess.Authentication;

public interface ITotpService
{
    string GenerateSecretKey();

    string BuildOtpAuthUri(string email, string base32Secret);

    string ComputeCode(string base32Secret, DateTimeOffset utcNow);

    bool VerifyCode(string base32Secret, string code, DateTimeOffset utcNow);

    IReadOnlyList<string> GenerateBackupCodes(int count = 8);

    string NormalizeBackupCode(string code);

    string HashBackupCode(string code);

    bool IsSixDigitTotp(string? code);

    bool IsBackupCodeCandidate(string? code);
}
