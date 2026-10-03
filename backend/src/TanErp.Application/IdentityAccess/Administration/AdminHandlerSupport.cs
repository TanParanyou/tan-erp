using TanErp.Application.Common.Results;

namespace TanErp.Application.IdentityAccess.Administration;

/// <summary>Small helpers shared by the administration handlers.</summary>
internal static class AdminHandlerSupport
{
    public static Result<T> Validation<T>(string detail) => Result<T>.Failure(new Error("REQUEST_VALIDATION_FAILED", detail));

    public static Result<T> NotFound<T>(string resource) => Result<T>.Failure(new Error("RESOURCE_NOT_FOUND", $"{resource} was not found."));

    public static bool IsPlausibleEmail(string? email) =>
        !string.IsNullOrWhiteSpace(email)
        && email.Length <= 255
        && email.Count(c => c == '@') == 1
        && !email.StartsWith('@')
        && !email.EndsWith('@')
        && !email.Any(char.IsWhiteSpace);
}
