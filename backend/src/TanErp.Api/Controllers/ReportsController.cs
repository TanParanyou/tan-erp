using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Reporting;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.Reporting;

namespace TanErp.Api.Controllers;

[ApiController]
[Route("api/v1/reports")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly ReportingHandler _handler;

    public ReportsController(ReportingHandler handler)
    {
        _handler = handler;
    }

    private ReportingCaller? ReadAuthenticated(out IActionResult? failure)
    {
        var contextResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            failure = ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
            return null;
        }

        failure = null;
        var auth = contextResult.Value!;
        return new ReportingCaller(auth.FirebaseUid, auth.MembershipId, HttpContext.TraceIdentifier);
    }

    private IActionResult Problem(string code) => ProblemDetailsMapper.CreateProblemResult(code, HttpContext);

    private static DateOnly? ParseAsOfDate(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return null;
        if (!DateOnly.TryParseExact(input, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            if (!DateOnly.TryParse(input, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                return null;
            }
        }
        if (date.Year > 2400)
        {
            date = date.AddYears(-543);
        }
        return date;
    }

    [HttpGet("dashboard-summary")]
    [ProducesResponseType<DashboardSummaryResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetDashboardSummary(
        [FromQuery] Guid? branchId,
        [FromQuery] string? asOfDate,
        CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;

        var parsedDate = ParseAsOfDate(asOfDate);
        var result = await _handler.GetDashboardSummaryAsync(caller, branchId, parsedDate, ct);
        if (result.IsFailure) return Problem(result.Error.Code);

        return Ok(ReportingMapper.To(result.Value!));
    }

    [HttpGet("pipeline")]
    [ProducesResponseType<PipelineReportResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetPipelineReport(
        [FromQuery] Guid? branchId,
        CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;

        var result = await _handler.GetPipelineReportAsync(caller, branchId, ct);
        if (result.IsFailure) return Problem(result.Error.Code);

        return Ok(ReportingMapper.To(result.Value!));
    }

    [HttpGet("quotations")]
    [ProducesResponseType<QuotationReportResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetQuotationReport(
        [FromQuery] Guid? branchId,
        CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;

        var result = await _handler.GetQuotationReportAsync(caller, branchId, ct);
        if (result.IsFailure) return Problem(result.Error.Code);

        return Ok(ReportingMapper.To(result.Value!));
    }

    [HttpGet("projects-budget")]
    [ProducesResponseType<ProjectBudgetReportResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetProjectBudgetReport(
        [FromQuery] Guid? branchId,
        CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;

        var result = await _handler.GetProjectBudgetReportAsync(caller, branchId, ct);
        if (result.IsFailure) return Problem(result.Error.Code);

        return Ok(ReportingMapper.To(result.Value!));
    }

    [HttpGet("stock-value")]
    [ProducesResponseType<StockValueReportResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetStockValueReport(
        [FromQuery] Guid? branchId,
        CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;

        var result = await _handler.GetStockValueReportAsync(caller, branchId, ct);
        if (result.IsFailure) return Problem(result.Error.Code);

        return Ok(ReportingMapper.To(result.Value!));
    }

    [HttpGet("ar-aging")]
    [ProducesResponseType<ReportingArAgingResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetArAgingReport(
        [FromQuery] Guid? branchId,
        [FromQuery] string? asOfDate,
        CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;

        var parsedDate = ParseAsOfDate(asOfDate);
        var result = await _handler.GetArAgingReportAsync(caller, branchId, parsedDate, ct);
        if (result.IsFailure) return Problem(result.Error.Code);

        return Ok(ReportingMapper.To(result.Value!));
    }

    [HttpGet("service-sla")]
    [ProducesResponseType<ServiceSlaReportResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetServiceSlaReport(
        [FromQuery] Guid? branchId,
        CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;

        var result = await _handler.GetServiceSlaReportAsync(caller, branchId, ct);
        if (result.IsFailure) return Problem(result.Error.Code);

        return Ok(ReportingMapper.To(result.Value!));
    }
}
