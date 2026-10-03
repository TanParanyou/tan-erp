namespace TanErp.Application.IdentityAccess.CurrentUser.GetCurrentUser;

/// <param name="VerifiedEmail">Email from a verified Firebase token; null when the token has no verified email.</param>
public sealed record GetCurrentUserQuery(string FirebaseUid, string? VerifiedEmail = null);
