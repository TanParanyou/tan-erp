using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Surveys;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.Common.Results;
using TanErp.Application.Surveys.MasterData;

namespace TanErp.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/settings/surveys")]
public class SurveyMasterDataController : ControllerBase
{
    private readonly SurveyMasterDataHandler _handler;

    public SurveyMasterDataController(SurveyMasterDataHandler handler)
    {
        _handler = handler;
    }

    [HttpGet("design-styles")]
    [ProducesResponseType<IReadOnlyList<SurveyDesignStyleResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ListDesignStyles([FromQuery] bool onlyActive = false, CancellationToken cancellationToken = default)
    {
        var auth = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (auth.IsFailure) return Problem(auth.Error);

        var caller = new SurveyCaller(auth.Value!.FirebaseUid, auth.Value.MembershipId);
        var result = await _handler.ListDesignStylesAsync(caller, onlyActive, cancellationToken);
        if (result.IsFailure) return Problem(result.Error);

        var responses = result.Value!.Select(ToResponse).ToList();
        return Ok(responses);
    }

    [HttpPost("design-styles")]
    [ProducesResponseType<SurveyDesignStyleResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateDesignStyle([FromBody] CreateSurveyDesignStyleRequest request, CancellationToken cancellationToken = default)
    {
        var auth = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (auth.IsFailure) return Problem(auth.Error);

        var caller = new SurveyCaller(auth.Value!.FirebaseUid, auth.Value.MembershipId);
        var input = new CreateSurveyDesignStyleInput(
            request.Code,
            new LocalizedNameInput(request.Name.Thai, request.Name.English),
            request.Description != null ? new LocalizedNameInput(request.Description.Thai, request.Description.English) : null,
            request.SortOrder);

        var result = await _handler.CreateDesignStyleAsync(caller, input, cancellationToken);
        if (result.IsFailure) return Problem(result.Error);

        var response = ToResponse(result.Value!);
        return Created($"/api/v1/settings/surveys/design-styles/{response.Id}", response);
    }

    [HttpPut("design-styles/{id:guid}")]
    [ProducesResponseType<SurveyDesignStyleResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateDesignStyle([FromRoute] Guid id, [FromBody] UpdateSurveyDesignStyleRequest request, CancellationToken cancellationToken = default)
    {
        var auth = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (auth.IsFailure) return Problem(auth.Error);

        var caller = new SurveyCaller(auth.Value!.FirebaseUid, auth.Value.MembershipId);
        var input = new UpdateSurveyDesignStyleInput(
            new LocalizedNameInput(request.Name.Thai, request.Name.English),
            request.Description != null ? new LocalizedNameInput(request.Description.Thai, request.Description.English) : null,
            request.SortOrder,
            request.IsActive);

        var result = await _handler.UpdateDesignStyleAsync(caller, id, input, cancellationToken);
        if (result.IsFailure) return Problem(result.Error);

        return Ok(ToResponse(result.Value!));
    }

    [HttpGet("mood-tones")]
    [ProducesResponseType<IReadOnlyList<SurveyMoodToneResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ListMoodTones([FromQuery] bool onlyActive = false, CancellationToken cancellationToken = default)
    {
        var auth = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (auth.IsFailure) return Problem(auth.Error);

        var caller = new SurveyCaller(auth.Value!.FirebaseUid, auth.Value.MembershipId);
        var result = await _handler.ListMoodTonesAsync(caller, onlyActive, cancellationToken);
        if (result.IsFailure) return Problem(result.Error);

        var responses = result.Value!.Select(ToResponse).ToList();
        return Ok(responses);
    }

    [HttpPost("mood-tones")]
    [ProducesResponseType<SurveyMoodToneResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateMoodTone([FromBody] CreateSurveyMoodToneRequest request, CancellationToken cancellationToken = default)
    {
        var auth = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (auth.IsFailure) return Problem(auth.Error);

        var caller = new SurveyCaller(auth.Value!.FirebaseUid, auth.Value.MembershipId);
        var input = new CreateSurveyMoodToneInput(
            request.Code,
            new LocalizedNameInput(request.Name.Thai, request.Name.English),
            request.Description != null ? new LocalizedNameInput(request.Description.Thai, request.Description.English) : null,
            request.ColorPalette,
            request.SortOrder);

        var result = await _handler.CreateMoodToneAsync(caller, input, cancellationToken);
        if (result.IsFailure) return Problem(result.Error);

        var response = ToResponse(result.Value!);
        return Created($"/api/v1/settings/surveys/mood-tones/{response.Id}", response);
    }

    [HttpPut("mood-tones/{id:guid}")]
    [ProducesResponseType<SurveyMoodToneResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMoodTone([FromRoute] Guid id, [FromBody] UpdateSurveyMoodToneRequest request, CancellationToken cancellationToken = default)
    {
        var auth = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (auth.IsFailure) return Problem(auth.Error);

        var caller = new SurveyCaller(auth.Value!.FirebaseUid, auth.Value.MembershipId);
        var input = new UpdateSurveyMoodToneInput(
            new LocalizedNameInput(request.Name.Thai, request.Name.English),
            request.Description != null ? new LocalizedNameInput(request.Description.Thai, request.Description.English) : null,
            request.ColorPalette,
            request.SortOrder,
            request.IsActive);

        var result = await _handler.UpdateMoodToneAsync(caller, id, input, cancellationToken);
        if (result.IsFailure) return Problem(result.Error);

        return Ok(ToResponse(result.Value!));
    }

    private static SurveyDesignStyleResponse ToResponse(SurveyDesignStyleDto d) =>
        new(d.Id, d.Code, new SurveyLocalizedNameContract(d.Name.Thai, d.Name.English), d.Description != null ? new SurveyLocalizedNameContract(d.Description.Thai, d.Description.English) : null, d.SortOrder, d.IsActive, d.CreatedAtUtc);

    private static SurveyMoodToneResponse ToResponse(SurveyMoodToneDto d) =>
        new(d.Id, d.Code, new SurveyLocalizedNameContract(d.Name.Thai, d.Name.English), d.Description != null ? new SurveyLocalizedNameContract(d.Description.Thai, d.Description.English) : null, d.ColorPalette, d.SortOrder, d.IsActive, d.CreatedAtUtc);

    private ObjectResult Problem(Error error) =>
        ProblemDetailsMapper.CreateProblemResult(error.Code, HttpContext);
}
