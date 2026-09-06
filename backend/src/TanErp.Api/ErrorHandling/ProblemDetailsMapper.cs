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
        _ => StatusCodes.Status500InternalServerError
    };

    private static string GetKnownCode(string code) => GetStatus(code) switch
    {
        StatusCodes.Status401Unauthorized => code,
        StatusCodes.Status403Forbidden => code,
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
