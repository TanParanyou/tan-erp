using System.Diagnostics;
using System.Resources;
using Microsoft.AspNetCore.Mvc;

namespace TanErp.Api.ErrorHandling;

public static class ProblemDetailsMapper
{
    private static readonly ResourceManager ResourceManager = new("TanErp.Api.Resources.Errors", typeof(ProblemDetailsMapper).Assembly);

    public static ObjectResult CreateProblemResult(string code, HttpContext context, string? customDetail = null)
    {
        var status = code switch
        {
            "AUTHENTICATION_REQUIRED" => StatusCodes.Status401Unauthorized,
            "AUTHENTICATION_INVALID" => StatusCodes.Status401Unauthorized,
            "USER_ACCESS_DISABLED" => StatusCodes.Status403Forbidden,
            "ACTIVE_MEMBERSHIP_REQUIRED" => StatusCodes.Status403Forbidden,
            "PERMISSION_DENIED" => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status400BadRequest
        };

        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

        var title = ResourceManager.GetString($"{code}_TITLE") ?? ResourceManager.GetString(code) ?? code;
        var detail = customDetail ?? ResourceManager.GetString($"{code}_DETAIL") ?? ResourceManager.GetString(code) ?? code;

        var problem = new ApiProblemDetails
        {
            Type = $"https://tan-erp.local/problems/{code.ToLowerInvariant().Replace('_', '-')}",
            Title = title,
            Status = status,
            Code = code,
            Detail = detail,
            TraceId = traceId,
            Instance = context.Request.Path
        };

        var result = new ObjectResult(problem)
        {
            StatusCode = status
        };
        result.ContentTypes.Add("application/problem+json");

        return result;
    }
}
