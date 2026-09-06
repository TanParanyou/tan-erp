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
        "USER_ACCESS_DISABLED" => StatusCodes.Status403Forbidden,
        "ACTIVE_MEMBERSHIP_REQUIRED" => StatusCodes.Status403Forbidden,
        "PERMISSION_DENIED" => StatusCodes.Status403Forbidden,
        "INTERNAL_SERVER_ERROR" => StatusCodes.Status500InternalServerError,
        _ => StatusCodes.Status400BadRequest
    };

    public static ApiProblemDetails CreateProblem(string code, HttpContext context)
    {
        var status = GetStatus(code);
        return new ApiProblemDetails
        {
            Type = $"https://tan-erp.local/problems/{code.ToLowerInvariant().Replace('_', '-')}",
            Title = ResourceManager.GetString($"{code}_TITLE") ?? code,
            Detail = ResourceManager.GetString($"{code}_DETAIL") ?? code,
            Status = status,
            Code = code,
            TraceId = Activity.Current?.Id ?? context.TraceIdentifier,
            Instance = context.Request.Path
        };
    }

    public static ObjectResult CreateProblemResult(string code, HttpContext context) =>
        new(CreateProblem(code, context))
        {
            StatusCode = GetStatus(code),
            ContentTypes = { "application/problem+json" }
        };
}
