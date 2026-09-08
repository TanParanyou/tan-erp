using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Crm.Customers;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.Crm.Customers;
using TanErp.Application.Crm.Customers.CreateCustomer;
using TanErp.Application.Crm.Customers.GetCustomer;
using TanErp.Application.Crm.Customers.ListCustomers;

namespace TanErp.Api.Controllers;

[ApiController]
[Route("api/v1/customers")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly CreateCustomerHandler _createHandler;
    private readonly ListCustomersHandler _listHandler;
    private readonly GetCustomerHandler _getHandler;

    public CustomersController(
        CreateCustomerHandler createHandler,
        ListCustomersHandler listHandler,
        GetCustomerHandler getHandler)
    {
        _createHandler = createHandler;
        _listHandler = listHandler;
        _getHandler = getHandler;
    }

    [HttpPost]
    [ProducesResponseType<CustomerResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(
        [FromBody] CreateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var contextResult = RequestContextReader.ReadIdempotentRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;
        var traceId = HttpContext.TraceIdentifier;

        var command = new CreateCustomerCommand(
            auth.FirebaseUid,
            auth.MembershipId,
            auth.IdempotencyKey,
            request.CustomerType,
            request.DisplayNameTh,
            request.DisplayNameEn,
            request.PreferredLocale,
            new CreatePrimaryContact(
                request.PrimaryContact.Name,
                request.PrimaryContact.RoleTitle,
                request.PrimaryContact.Phone,
                request.PrimaryContact.Email,
                request.PrimaryContact.PreferredChannel,
                request.PrimaryContact.LineId),
            traceId,
            request.LeadSource);

        var result = await _createHandler.Handle(command, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var customer = result.Value!.Customer;
        var duplicates = result.Value.DuplicateCandidates.Select(d => new DuplicateCustomerResponse(
            d.Id,
            d.Code,
            d.DisplayNameTh,
            d.MaskedPhone,
            d.MaskedEmail)).ToList();

        var response = MapCustomerResponse(customer, duplicates);

        Response.Headers.ETag = $"\"{customer.RowVersion}\"";
        return Created($"/api/v1/customers/{customer.Id}", response);
    }

    [HttpGet]
    [ProducesResponseType<CustomerListResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> List(
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] int limit = 25,
        [FromQuery] string? cursor = null,
        CancellationToken cancellationToken = default)
    {
        var contextResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;
        var query = new ListCustomersQuery(auth.FirebaseUid, auth.MembershipId, search, status, limit, cursor);

        var result = await _listHandler.Handle(query, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var items = result.Value!.Items.Select(c => new CustomerListItemResponse(
            c.Id,
            c.Code,
            c.CustomerType,
            c.DisplayNameTh,
            c.DisplayNameEn,
            c.PreferredLocale,
            c.Status,
            new CustomerContactResponse(
                c.PrimaryContact.Name,
                c.PrimaryContact.RoleTitle,
                c.PrimaryContact.Phone,
                c.PrimaryContact.Email,
                c.PrimaryContact.PreferredChannel,
                c.PrimaryContact.IsMasked,
                c.PrimaryContact.LineId),
            c.LeadSource)).ToList();

        var response = new CustomerListResponse(items, result.Value.NextCursor);
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<CustomerResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var contextResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;
        var query = new GetCustomerQuery(auth.FirebaseUid, auth.MembershipId, id);

        var result = await _getHandler.Handle(query, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var customer = result.Value!.Customer;
        var response = MapCustomerResponse(customer, null);

        Response.Headers.ETag = $"\"{customer.RowVersion}\"";
        return Ok(response);
    }

    private static CustomerResponse MapCustomerResponse(
        CustomerProjection customer,
        IReadOnlyList<DuplicateCustomerResponse>? duplicates)
    {
        return new CustomerResponse(
            customer.Id,
            customer.Code,
            customer.CustomerType,
            customer.DisplayNameTh,
            customer.DisplayNameEn,
            customer.PreferredLocale,
            customer.Status,
            new CustomerContactResponse(
                customer.PrimaryContact.Name,
                customer.PrimaryContact.RoleTitle,
                customer.PrimaryContact.Phone,
                customer.PrimaryContact.Email,
                customer.PrimaryContact.PreferredChannel,
                customer.PrimaryContact.IsMasked,
                customer.PrimaryContact.LineId),
            duplicates,
            customer.RowVersion,
            customer.CreatedAtUtc,
            customer.LeadSource);
    }
}
