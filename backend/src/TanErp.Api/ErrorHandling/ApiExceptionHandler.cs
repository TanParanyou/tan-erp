using Microsoft.AspNetCore.Diagnostics;

namespace TanErp.Api.ErrorHandling;

public sealed class ApiExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problem = ProblemDetailsMapper.CreateProblem(
            "INTERNAL_SERVER_ERROR", httpContext);
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(
            problem,
            options: null,
            contentType: "application/problem+json",
            cancellationToken: cancellationToken);
        return true;
    }
}
