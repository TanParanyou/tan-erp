using System.Text.RegularExpressions;

namespace TanErp.Infrastructure.Identity;

/// <summary>
/// DEVELOPMENT ONLY decorator. Accepts tokens of the exact form "dev-uid:&lt;uid&gt;" and delegates
/// everything else to the real verifier. Must only be registered in the Development environment.
/// </summary>
public sealed partial class DevelopmentTokenVerifier(IFirebaseTokenVerifier inner) : IFirebaseTokenVerifier
{
    [GeneratedRegex(@"^dev-uid:([A-Za-z0-9_-]{1,128})$")]
    private static partial Regex TokenPattern();

    public Task<string?> VerifyTokenAsync(string idToken, CancellationToken cancellationToken = default)
        => TokenPattern().IsMatch(idToken)
            ? Task.FromResult<string?>(idToken["dev-uid:".Length..])
            : inner.VerifyTokenAsync(idToken, cancellationToken);

    public Task<FirebaseIdentity?> VerifyIdentityAsync(string idToken, CancellationToken cancellationToken = default)
        => TokenPattern().IsMatch(idToken)
            ? Task.FromResult<FirebaseIdentity?>(new FirebaseIdentity(idToken["dev-uid:".Length..], null, false))
            : inner.VerifyIdentityAsync(idToken, cancellationToken);
}
