namespace TanErp.Application.IdentityAccess.CurrentUser.UpdateMyPreferences;

public sealed record UpdateMyPreferencesCommand(
    string FirebaseUid,
    Guid MembershipId,
    string? ThemeMode,
    string? Density,
    int? TextScale,
    string? PreferredLocale,
    string TraceId);

public sealed record UpdateMyPreferencesInput(string ThemeMode, string Density, int TextScale, string? PreferredLocale);
