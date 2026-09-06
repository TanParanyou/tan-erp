using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using TanErp.Api.ErrorHandling;
using TanErp.Infrastructure.Identity;

namespace TanErp.Api.Authentication;

public class FirebaseAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IFirebaseTokenVerifier _tokenVerifier;

    public const string SchemeName = "Firebase";

    public FirebaseAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IFirebaseTokenVerifier tokenVerifier)
        : base(options, logger, encoder)
    {
        _tokenVerifier = tokenVerifier;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("Authorization", out var authHeaderValues))
        {
            return AuthenticateResult.NoResult();
        }

        var authHeader = authHeaderValues.ToString();
        if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return AuthenticateResult.Fail("AUTHENTICATION_INVALID");
        }

        var token = authHeader["Bearer ".Length..].Trim();
        if (string.IsNullOrWhiteSpace(token))
        {
            return AuthenticateResult.Fail("AUTHENTICATION_INVALID");
        }

        var uid = await _tokenVerifier.VerifyTokenAsync(token, Context.RequestAborted);
        if (string.IsNullOrWhiteSpace(uid))
        {
            return AuthenticateResult.Fail("AUTHENTICATION_INVALID");
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, uid),
            new Claim("firebase_uid", uid)
        };

        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);

        return AuthenticateResult.Success(ticket);
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        Response.ContentType = "application/problem+json";

        var authResult = await AuthenticateAsync();
        var code = authResult?.Failure != null
            ? "AUTHENTICATION_INVALID"
            : "AUTHENTICATION_REQUIRED";

        var problemResult = ProblemDetailsMapper.CreateProblemResult(code, Context);
        await Response.WriteAsync(JsonSerializer.Serialize(problemResult.Value, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }

    protected override async Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = StatusCodes.Status403Forbidden;
        Response.ContentType = "application/problem+json";

        var problemResult = ProblemDetailsMapper.CreateProblemResult("PERMISSION_DENIED", Context);
        await Response.WriteAsync(JsonSerializer.Serialize(problemResult.Value, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}
