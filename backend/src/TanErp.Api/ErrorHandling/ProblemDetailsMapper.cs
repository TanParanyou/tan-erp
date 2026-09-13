using System.Diagnostics;
using System.Resources;
using Microsoft.AspNetCore.Mvc;

namespace TanErp.Api.ErrorHandling;

public static class ProblemDetailsMapper
{
    private static readonly ResourceManager ResourceManager = new("TanErp.Api.Resources.Errors", typeof(ProblemDetailsMapper).Assembly);

    public static int GetStatus(string code) => code switch
    {
        "AUTHENTICATION_REQUIRED" => StatusCodes.Status401Unauthorized,
        "AUTHENTICATION_INVALID" => StatusCodes.Status401Unauthorized,
        "MEMBERSHIP_CONTEXT_REQUIRED" => StatusCodes.Status400BadRequest,
        "REQUEST_VALIDATION_FAILED" => StatusCodes.Status400BadRequest,
        "IDEMPOTENCY_KEY_REQUIRED" => StatusCodes.Status400BadRequest,
        "IDEMPOTENCY_KEY_INVALID" => StatusCodes.Status400BadRequest,
        "CUSTOMER_CURSOR_INVALID" => StatusCodes.Status400BadRequest,
        "CUSTOMER_SORT_INVALID" => StatusCodes.Status400BadRequest,
        "CUSTOMER_SORT_ORDER_INVALID" => StatusCodes.Status400BadRequest,
        "OPPORTUNITY_SORT_INVALID" => StatusCodes.Status400BadRequest,
        "OPPORTUNITY_SORT_ORDER_INVALID" => StatusCodes.Status400BadRequest,
        "INVALID_CURSOR" => StatusCodes.Status400BadRequest,
        "USER_ACCESS_DISABLED" => StatusCodes.Status403Forbidden,
        "ACTIVE_MEMBERSHIP_REQUIRED" => StatusCodes.Status403Forbidden,
        "PERMISSION_DENIED" => StatusCodes.Status403Forbidden,
        "RESOURCE_NOT_FOUND" => StatusCodes.Status404NotFound,
        "IDEMPOTENCY_KEY_REUSED" => StatusCodes.Status409Conflict,
        "CUSTOMER_VERSION_CONFLICT" => StatusCodes.Status409Conflict,
        "CUSTOMER_INVALID_STATE" => StatusCodes.Status409Conflict,
        "OPPORTUNITY_VERSION_CONFLICT" => StatusCodes.Status409Conflict,
        "OPPORTUNITY_INVALID_TRANSITION" => StatusCodes.Status409Conflict,
        "ACTIVE_BRANCH_REQUIRED" => StatusCodes.Status422UnprocessableEntity,
        "CUSTOMER_FIELD_REQUIRED" => StatusCodes.Status422UnprocessableEntity,
        "CONTACT_FIELD_REQUIRED" => StatusCodes.Status422UnprocessableEntity,
        "SITE_FIELD_REQUIRED" => StatusCodes.Status422UnprocessableEntity,
        "OPPORTUNITY_FIELD_REQUIRED" => StatusCodes.Status422UnprocessableEntity,
        "IF_MATCH_REQUIRED" => StatusCodes.Status428PreconditionRequired,
        "INTERNAL_SERVER_ERROR" => StatusCodes.Status500InternalServerError,
        _ => StatusCodes.Status500InternalServerError
    };

    private static string GetKnownCode(string code) => GetStatus(code) switch
    {
        StatusCodes.Status400BadRequest => code,
        StatusCodes.Status401Unauthorized => code,
        StatusCodes.Status403Forbidden => code,
        StatusCodes.Status404NotFound => code,
        StatusCodes.Status409Conflict => code,
        StatusCodes.Status422UnprocessableEntity => code,
        StatusCodes.Status428PreconditionRequired => code,
        StatusCodes.Status500InternalServerError when code == "INTERNAL_SERVER_ERROR" => code,
        _ => "INTERNAL_SERVER_ERROR"
    };

    public static ApiProblemDetails CreateProblem(string code, HttpContext context)
    {
        var knownCode = GetKnownCode(code);
        var status = GetStatus(knownCode);
        return new ApiProblemDetails
        {
            Type = $"https://tan-erp.local/problems/{knownCode.ToLowerInvariant().Replace('_', '-')}",
            Title = ResourceManager.GetString($"{knownCode}_TITLE") ?? knownCode,
            Detail = ResourceManager.GetString($"{knownCode}_DETAIL") ?? knownCode,
            Status = status,
            Code = knownCode,
            TraceId = Activity.Current?.Id ?? context.TraceIdentifier,
            Instance = context.Request.Path
        };
    }

    public static ObjectResult CreateProblemResult(string code, HttpContext context)
    {
        var problem = CreateProblem(code, context);
        return new ObjectResult(problem)
        {
            StatusCode = problem.Status,
            ContentTypes = { "application/problem+json" }
        };
    }
}
