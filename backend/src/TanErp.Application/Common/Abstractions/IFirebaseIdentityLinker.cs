namespace TanErp.Application.Common.Abstractions;

/// <summary>
/// Links the first verified Firebase identity to an administrator-invited (pending) user by email.
/// </summary>
public interface IFirebaseIdentityLinker
{
    /// <summary>
    /// Links <paramref name="firebaseUid"/> to the single active pending user whose normalized email equals
    /// <paramref name="verifiedEmail"/>. Does nothing when the UID is already known, no pending user matches,
    /// or the user is inactive. Never re-links an already linked user.
    /// </summary>
    Task<bool> TryLinkAsync(string firebaseUid, string verifiedEmail, CancellationToken cancellationToken = default);
}
