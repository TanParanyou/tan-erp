using Microsoft.AspNetCore.Mvc;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.QuickEstimates;

namespace TanErp.Api.Controllers;

public abstract class QuickEstimateControllerBase : ControllerBase
{
    protected QeCaller? ReadAuthenticated(out IActionResult? failure)
    {
        var contextResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            failure = ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
            return null;
        }

        failure = null;
        var auth = contextResult.Value!;
        return new QeCaller(auth.FirebaseUid, auth.MembershipId, HttpContext.TraceIdentifier);
    }

    protected QeCaller? ReadConditional(out Guid expectedVersion, out IActionResult? failure)
    {
        var contextResult = RequestContextReader.ReadConditionalAuthenticatedRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            expectedVersion = Guid.Empty;
            failure = ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
            return null;
        }

        failure = null;
        var auth = contextResult.Value!;
        expectedVersion = auth.IfMatchRowVersion;
        return new QeCaller(auth.FirebaseUid, auth.MembershipId, HttpContext.TraceIdentifier);
    }

    protected QeCaller? ReadIdempotent(out string idempotencyKey, out IActionResult? failure)
    {
        var contextResult = RequestContextReader.ReadIdempotentRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            idempotencyKey = string.Empty;
            failure = ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
            return null;
        }

        failure = null;
        var auth = contextResult.Value!;
        idempotencyKey = auth.IdempotencyKey;
        return new QeCaller(auth.FirebaseUid, auth.MembershipId, HttpContext.TraceIdentifier);
    }

    protected IActionResult Problem(string code) => ProblemDetailsMapper.CreateProblemResult(code, HttpContext);
}
