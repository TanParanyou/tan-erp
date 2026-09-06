using Microsoft.AspNetCore.Diagnostics;

namespace TanErp.Api.ErrorHandling;

public sealed class ApiExceptionHandler : IExceptionHandler
{
    private readonly ILogger<ApiExceptionHandler> _logger;

    public ApiExceptionHandler(ILogger<ApiExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is OperationCanceledException
            && httpContext.RequestAborted.IsCancellationRequested)
        {
            return false;
        }

        var problem = ProblemDetailsMapper.CreateProblem(
            "INTERNAL_SERVER_ERROR", httpContext);
        _logger.LogError(
            "Unhandled request. ErrorCode: {ErrorCode}; TraceId: {TraceId}; ExceptionType: {ExceptionType}",
            problem.Code,
            problem.TraceId,
            exception.GetType().Name);
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(
            problem,
            options: null,
            contentType: "application/problem+json",
            cancellationToken: cancellationToken);
        return true;
    }
}
