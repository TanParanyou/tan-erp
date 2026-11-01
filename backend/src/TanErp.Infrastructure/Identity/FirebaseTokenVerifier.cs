using TanErp.Application.Common.Abstractions;
using TanErp.Application.IdentityAccess.Authentication;

namespace TanErp.Infrastructure.Identity;

public sealed record FirebaseIdentity(
    string Uid,
    string? Email,
    bool EmailVerified,
    Guid? SessionId = null);

public interface IFirebaseTokenVerifier
{
    Task<string?> VerifyTokenAsync(string idToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies the bearer token and returns the identity claims (including optional active session ID).
    /// Verifiers that only know the UID return no email and a null SessionId.
    /// </summary>
    async Task<FirebaseIdentity?> VerifyIdentityAsync(string idToken, CancellationToken cancellationToken = default)
    {
        var uid = await VerifyTokenAsync(idToken, cancellationToken);
        return string.IsNullOrWhiteSpace(uid) ? null : new FirebaseIdentity(uid, null, false);
    }
}

public sealed class SelfHostedTokenVerifier : IFirebaseTokenVerifier
{
    private readonly ISelfHostedJwtService _jwtService;
    private readonly IClock _clock;

    public SelfHostedTokenVerifier(ISelfHostedJwtService jwtService, IClock clock)
    {
        _jwtService = jwtService;
        _clock = clock;
    }

    public async Task<string?> VerifyTokenAsync(string idToken, CancellationToken cancellationToken = default)
    {
        var identity = await VerifyIdentityAsync(idToken, cancellationToken);
        return identity?.Uid;
    }

    public Task<FirebaseIdentity?> VerifyIdentityAsync(string idToken, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var claims = _jwtService.VerifyAccessToken(idToken, _clock.UtcNow);
        if (claims is null)
        {
            return Task.FromResult<FirebaseIdentity?>(null);
        }

        return Task.FromResult<FirebaseIdentity?>(new FirebaseIdentity(
            claims.SubjectUid,
            claims.Email,
            claims.EmailVerified,
            claims.SessionId));
    }
}
