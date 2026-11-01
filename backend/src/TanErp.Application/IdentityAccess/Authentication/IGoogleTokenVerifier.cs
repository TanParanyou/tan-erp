using TanErp.Application.Common.Results;

namespace TanErp.Application.IdentityAccess.Authentication;

public sealed record GoogleVerifiedIdentity(string SubjectId, string Email, string? DisplayName);

public interface IGoogleTokenVerifier
{
    Task<Result<GoogleVerifiedIdentity>> VerifyIdTokenAsync(string idToken, CancellationToken cancellationToken = default);
}
