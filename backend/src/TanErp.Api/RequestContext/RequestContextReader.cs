using System.Security.Claims;
using TanErp.Application.Common.Results;

namespace TanErp.Api.RequestContext;

public sealed record AuthenticatedRequest(string FirebaseUid, Guid MembershipId);
public sealed record IdempotentRequest(string FirebaseUid, Guid MembershipId, string IdempotencyKey);

public static class RequestContextReader
{
    public static Result<AuthenticatedRequest> ReadAuthenticatedRequest(HttpContext httpContext)
    {
        var uid = GetFirebaseUid(httpContext.User);
        if (string.IsNullOrWhiteSpace(uid))
        {
            return Result<AuthenticatedRequest>.Failure(new Error("AUTHENTICATION_REQUIRED", "Authentication is required."));
        }

        if (!httpContext.Request.Headers.TryGetValue("X-Membership-Id", out var membershipHeader) || membershipHeader.Count != 1)
        {
            return Result<AuthenticatedRequest>.Failure(new Error("MEMBERSHIP_CONTEXT_REQUIRED", "Header X-Membership-Id is required."));
        }

        var headerValue = membershipHeader.ToString().Trim();
        if (!Guid.TryParse(headerValue, out var membershipId) || membershipId == Guid.Empty)
        {
            return Result<AuthenticatedRequest>.Failure(new Error("MEMBERSHIP_CONTEXT_REQUIRED", "Header X-Membership-Id must be a valid non-empty UUID."));
        }

        return Result<AuthenticatedRequest>.Success(new AuthenticatedRequest(uid, membershipId));
    }

    public static Result<IdempotentRequest> ReadIdempotentRequest(HttpContext httpContext)
    {
        var authResult = ReadAuthenticatedRequest(httpContext);
        if (authResult.IsFailure)
        {
            return Result<IdempotentRequest>.Failure(authResult.Error);
        }

        if (!httpContext.Request.Headers.TryGetValue("Idempotency-Key", out var idempotencyHeader) || idempotencyHeader.Count != 1)
        {
            return Result<IdempotentRequest>.Failure(new Error("IDEMPOTENCY_KEY_REQUIRED", "Header Idempotency-Key is required."));
        }

        var key = idempotencyHeader.ToString().Trim();
        if (key.Length < 16 || key.Length > 128)
        {
            return Result<IdempotentRequest>.Failure(new Error("IDEMPOTENCY_KEY_INVALID", "Header Idempotency-Key must be between 16 and 128 characters."));
        }

        return Result<IdempotentRequest>.Success(new IdempotentRequest(authResult.Value!.FirebaseUid, authResult.Value.MembershipId, key));
    }

    private static string? GetFirebaseUid(ClaimsPrincipal user)
    {
        return user.FindFirst("firebase_uid")?.Value
            ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? user.FindFirst("sub")?.Value;
    }
}
