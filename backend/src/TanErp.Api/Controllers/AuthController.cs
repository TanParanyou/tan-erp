using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TanErp.Api.Contracts.IdentityAccess.Authentication;
using TanErp.Api.ErrorHandling;
using TanErp.Application.Common.Results;
using TanErp.Application.IdentityAccess.Authentication;

namespace TanErp.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    public const string RefreshCookieName = "tan_erp_refresh";
    public const string RefreshCookiePath = "/api/v1/auth";

    private readonly AuthenticationHandler _handler;
    private readonly IWebHostEnvironment _environment;

    public AuthController(AuthenticationHandler handler, IWebHostEnvironment environment)
    {
        _handler = handler;
        _environment = environment;
    }

    [HttpPost("login")]
    [ProducesResponseType<AuthResultResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _handler.LoginAsync(
            new LoginCommand(
                request.Email,
                request.Password,
                BuildMetadata(request.ClientPlatform)),
            cancellationToken);

        return MapAuthResult(result, clearCookieOnFailure: false);
    }

    [HttpPost("google")]
    [ProducesResponseType<AuthResultResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _handler.GoogleLoginAsync(
            new GoogleLoginCommand(
                request.IdToken,
                BuildMetadata(request.ClientPlatform)),
            cancellationToken);

        return MapAuthResult(result, clearCookieOnFailure: false);
    }

    [HttpPost("2fa/verify-login")]
    [ProducesResponseType<AuthResultResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> VerifyTwoFactorLogin(
        [FromBody] VerifyTwoFactorLoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _handler.VerifyTwoFactorLoginAsync(
            new VerifyTwoFactorLoginCommand(
                request.ChallengeToken,
                request.Code,
                BuildMetadata(request.ClientPlatform)),
            cancellationToken);

        return MapAuthResult(result, clearCookieOnFailure: false);
    }

    [HttpPost("refresh")]
    [ProducesResponseType<AuthResultResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh(
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] RefreshSessionRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        var refreshToken = !string.IsNullOrWhiteSpace(request?.RefreshToken)
            ? request.RefreshToken
            : Request.Cookies[RefreshCookieName];

        var result = await _handler.RefreshSessionAsync(
            new RefreshSessionCommand(refreshToken, request?.ClientPlatform),
            cancellationToken);

        return MapAuthResult(result, clearCookieOnFailure: true);
    }

    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout(
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] LogoutRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        var subjectUid = User.FindFirst("firebase_uid")?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Guid? sessionId = Guid.TryParse(User.FindFirst("session_id")?.Value, out var parsedSid)
            ? parsedSid
            : null;

        var refreshToken = !string.IsNullOrWhiteSpace(request?.RefreshToken)
            ? request.RefreshToken
            : Request.Cookies[RefreshCookieName];

        await _handler.LogoutAsync(
            new LogoutCommand(subjectUid, sessionId, refreshToken),
            cancellationToken);

        DeleteRefreshCookie();
        return NoContent();
    }

    private IActionResult MapAuthResult(Result<AuthFlowResult> result, bool clearCookieOnFailure)
    {
        if (result.IsFailure)
        {
            if (clearCookieOnFailure)
            {
                DeleteRefreshCookie();
            }

            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var flow = result.Value!;
        if (!string.IsNullOrWhiteSpace(flow.CookieRefreshToken))
        {
            Response.Cookies.Append(
                RefreshCookieName,
                flow.CookieRefreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = Request.IsHttps || _environment.IsProduction(),
                    SameSite = SameSiteMode.Lax,
                    Path = RefreshCookiePath,
                    Expires = flow.SessionExpiresAtUtc
                });
        }

        return Ok(AuthResultResponse.From(flow));
    }

    private void DeleteRefreshCookie()
    {
        Response.Cookies.Delete(
            RefreshCookieName,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps || _environment.IsProduction(),
                SameSite = SameSiteMode.Lax,
                Path = RefreshCookiePath
            });
    }

    private ClientRequestMetadata BuildMetadata(string? clientPlatform)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        if (string.IsNullOrWhiteSpace(ipAddress)
            && Request.Headers.TryGetValue("X-Forwarded-For", out var forwarded))
        {
            ipAddress = forwarded.ToString().Split(',')[0].Trim();
        }

        return new ClientRequestMetadata(
            clientPlatform,
            ipAddress,
            Request.Headers.UserAgent.ToString(),
            HttpContext.TraceIdentifier);
    }
}
