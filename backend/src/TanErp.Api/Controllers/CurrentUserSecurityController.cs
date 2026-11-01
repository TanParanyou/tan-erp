using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.IdentityAccess.Authentication;
using TanErp.Api.ErrorHandling;
using TanErp.Application.IdentityAccess.Authentication;

namespace TanErp.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/me")]
public class CurrentUserSecurityController : ControllerBase
{
    private readonly CurrentUserSecurityHandler _handler;

    public CurrentUserSecurityController(CurrentUserSecurityHandler handler)
    {
        _handler = handler;
    }

    [HttpGet("security")]
    [ProducesResponseType<MySecurityStatusResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetSecurity(CancellationToken cancellationToken)
    {
        var uid = ReadSubjectUid(out var failure);
        if (failure is not null) return failure;

        var result = await _handler.GetSecurityStatusAsync(uid!, cancellationToken);
        return result.IsFailure
            ? ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext)
            : Ok(MySecurityStatusResponse.From(result.Value!));
    }

    [HttpPut("password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangeMyPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var uid = ReadSubjectUid(out var failure);
        if (failure is not null) return failure;

        var result = await _handler.ChangePasswordAsync(
            new ChangeMyPasswordCommand(
                uid!,
                ReadCurrentSessionId(),
                request.CurrentPassword,
                request.NewPassword),
            cancellationToken);

        return result.IsFailure
            ? ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext)
            : NoContent();
    }

    [HttpPost("2fa/setup")]
    [ProducesResponseType<TotpSetupPayloadDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SetupTwoFactor(CancellationToken cancellationToken)
    {
        var uid = ReadSubjectUid(out var failure);
        if (failure is not null) return failure;

        var result = await _handler.SetupTwoFactorAsync(uid!, cancellationToken);
        return result.IsFailure
            ? ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext)
            : Ok(new TotpSetupPayloadDto(result.Value!.SecretKey, result.Value.OtpAuthUri));
    }

    [HttpPost("2fa/enable")]
    [ProducesResponseType<TwoFactorBackupCodesResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> EnableTwoFactor(
        [FromBody] EnableTwoFactorRequest request,
        CancellationToken cancellationToken)
    {
        var uid = ReadSubjectUid(out var failure);
        if (failure is not null) return failure;

        var result = await _handler.EnableTwoFactorAsync(uid!, request.Code, cancellationToken);
        return result.IsFailure
            ? ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext)
            : Ok(new TwoFactorBackupCodesResponse(result.Value!));
    }

    [HttpPost("2fa/disable")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DisableTwoFactor(
        [FromBody] DisableTwoFactorRequest request,
        CancellationToken cancellationToken)
    {
        var uid = ReadSubjectUid(out var failure);
        if (failure is not null) return failure;

        var result = await _handler.DisableTwoFactorAsync(uid!, request.Code, cancellationToken);
        return result.IsFailure
            ? ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext)
            : NoContent();
    }

    [HttpPost("2fa/backup-codes/regenerate")]
    [ProducesResponseType<TwoFactorBackupCodesResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RegenerateBackupCodes(
        [FromBody] RegenerateBackupCodesRequest request,
        CancellationToken cancellationToken)
    {
        var uid = ReadSubjectUid(out var failure);
        if (failure is not null) return failure;

        var result = await _handler.RegenerateBackupCodesAsync(uid!, request.Code, cancellationToken);
        return result.IsFailure
            ? ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext)
            : Ok(new TwoFactorBackupCodesResponse(result.Value!));
    }

    [HttpGet("sessions")]
    [ProducesResponseType<ActiveSessionListResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ListSessions(CancellationToken cancellationToken)
    {
        var uid = ReadSubjectUid(out var failure);
        if (failure is not null) return failure;

        var result = await _handler.ListSessionsAsync(uid!, ReadCurrentSessionId(), cancellationToken);
        return result.IsFailure
            ? ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext)
            : Ok(new ActiveSessionListResponse(result.Value!.Select(ActiveSessionItemDto.From).ToList()));
    }

    [HttpDelete("sessions/{sessionId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RevokeSession([FromRoute] Guid sessionId, CancellationToken cancellationToken)
    {
        var uid = ReadSubjectUid(out var failure);
        if (failure is not null) return failure;

        var result = await _handler.RevokeSessionAsync(uid!, sessionId, cancellationToken);
        return result.IsFailure
            ? ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext)
            : NoContent();
    }

    [HttpPost("sessions/revoke-others")]
    [ProducesResponseType<RevokeSessionsResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RevokeOtherSessions(CancellationToken cancellationToken)
    {
        var uid = ReadSubjectUid(out var failure);
        if (failure is not null) return failure;

        var result = await _handler.RevokeOtherSessionsAsync(uid!, ReadCurrentSessionId(), cancellationToken);
        return result.IsFailure
            ? ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext)
            : Ok(new RevokeSessionsResponse(result.Value));
    }

    private string? ReadSubjectUid(out IActionResult? failure)
    {
        var uid = User.FindFirst("firebase_uid")?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(uid))
        {
            failure = ProblemDetailsMapper.CreateProblemResult("AUTHENTICATION_REQUIRED", HttpContext);
            return null;
        }

        failure = null;
        return uid;
    }

    private Guid? ReadCurrentSessionId() =>
        Guid.TryParse(User.FindFirst("session_id")?.Value, out var sessionId)
            ? sessionId
            : null;
}
