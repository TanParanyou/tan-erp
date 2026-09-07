using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TanErp.Api.RequestContext;
using Xunit;

namespace TanErp.UnitTests.RequestContext;

public class RequestContextReaderTests
{
    [Fact]
    public void ReadAuthenticatedRequest_WhenUserNotAuthenticated_ReturnsAuthenticationRequired()
    {
        var context = new DefaultHttpContext();
        var result = RequestContextReader.ReadAuthenticatedRequest(context);

        Assert.True(result.IsFailure);
        Assert.Equal("AUTHENTICATION_REQUIRED", result.Error.Code);
    }

    [Fact]
    public void ReadAuthenticatedRequest_WhenMembershipHeaderMissing_ReturnsMembershipContextRequired()
    {
        var context = new DefaultHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity([new Claim("firebase_uid", "uid-123")]));

        var result = RequestContextReader.ReadAuthenticatedRequest(context);

        Assert.True(result.IsFailure);
        Assert.Equal("MEMBERSHIP_CONTEXT_REQUIRED", result.Error.Code);
    }

    [Fact]
    public void ReadAuthenticatedRequest_WhenMembershipHeaderNotGuid_ReturnsMembershipContextRequired()
    {
        var context = new DefaultHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity([new Claim("firebase_uid", "uid-123")]));
        context.Request.Headers["X-Membership-Id"] = "invalid-guid";

        var result = RequestContextReader.ReadAuthenticatedRequest(context);

        Assert.True(result.IsFailure);
        Assert.Equal("MEMBERSHIP_CONTEXT_REQUIRED", result.Error.Code);
    }

    [Fact]
    public void ReadAuthenticatedRequest_WhenValid_ReturnsSuccess()
    {
        var membershipId = Guid.NewGuid();
        var context = new DefaultHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity([new Claim("firebase_uid", "uid-123")]));
        context.Request.Headers["X-Membership-Id"] = membershipId.ToString();

        var result = RequestContextReader.ReadAuthenticatedRequest(context);

        Assert.True(result.IsSuccess);
        Assert.Equal("uid-123", result.Value!.FirebaseUid);
        Assert.Equal(membershipId, result.Value.MembershipId);
    }

    [Fact]
    public void ReadIdempotentRequest_WhenKeyMissing_ReturnsIdempotencyKeyRequired()
    {
        var membershipId = Guid.NewGuid();
        var context = new DefaultHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity([new Claim("firebase_uid", "uid-123")]));
        context.Request.Headers["X-Membership-Id"] = membershipId.ToString();

        var result = RequestContextReader.ReadIdempotentRequest(context);

        Assert.True(result.IsFailure);
        Assert.Equal("IDEMPOTENCY_KEY_REQUIRED", result.Error.Code);
    }

    [Fact]
    public void ReadIdempotentRequest_WhenKeyTooShort_ReturnsIdempotencyKeyInvalid()
    {
        var membershipId = Guid.NewGuid();
        var context = new DefaultHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity([new Claim("firebase_uid", "uid-123")]));
        context.Request.Headers["X-Membership-Id"] = membershipId.ToString();
        context.Request.Headers["Idempotency-Key"] = "short";

        var result = RequestContextReader.ReadIdempotentRequest(context);

        Assert.True(result.IsFailure);
        Assert.Equal("IDEMPOTENCY_KEY_INVALID", result.Error.Code);
    }

    [Fact]
    public void ReadIdempotentRequest_WhenValid_ReturnsSuccess()
    {
        var membershipId = Guid.NewGuid();
        var context = new DefaultHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity([new Claim("firebase_uid", "uid-123")]));
        context.Request.Headers["X-Membership-Id"] = membershipId.ToString();
        context.Request.Headers["Idempotency-Key"] = "valid-key-with-at-least-16-characters";

        var result = RequestContextReader.ReadIdempotentRequest(context);

        Assert.True(result.IsSuccess);
        Assert.Equal("uid-123", result.Value!.FirebaseUid);
        Assert.Equal(membershipId, result.Value.MembershipId);
        Assert.Equal("valid-key-with-at-least-16-characters", result.Value.IdempotencyKey);
    }
}
