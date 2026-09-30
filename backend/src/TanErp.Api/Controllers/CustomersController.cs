using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Crm.Customers;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.Crm.Customers;
using TanErp.Application.Crm.Customers.CheckDuplicates;
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
    private readonly CheckCustomerDuplicatesHandler _checkDuplicatesHandler;
    private readonly TanErp.Application.Crm.Customers.ActivateCustomer.ActivateCustomerHandler _activateHandler;
    private readonly TanErp.Application.Crm.Customers.UpdateCustomer.UpdateCustomerHandler _updateHandler;
    private readonly TanErp.Application.Crm.Customers.CustomerContactHandler _contactHandler;
    private readonly TanErp.Application.Crm.Customers.ChangeCustomerStatus.ChangeCustomerStatusHandler _statusHandler;
    private readonly TanErp.Application.Crm.Customers.CustomerAddressHandler _addressHandler;

    public CustomersController(
        CreateCustomerHandler createHandler,
        ListCustomersHandler listHandler,
        GetCustomerHandler getHandler,
        CheckCustomerDuplicatesHandler checkDuplicatesHandler,
        TanErp.Application.Crm.Customers.ActivateCustomer.ActivateCustomerHandler activateHandler,
        TanErp.Application.Crm.Customers.UpdateCustomer.UpdateCustomerHandler updateHandler,
        TanErp.Application.Crm.Customers.CustomerContactHandler contactHandler,
        TanErp.Application.Crm.Customers.ChangeCustomerStatus.ChangeCustomerStatusHandler statusHandler,
        TanErp.Application.Crm.Customers.CustomerAddressHandler addressHandler)
    {
        _createHandler = createHandler;
        _listHandler = listHandler;
        _getHandler = getHandler;
        _checkDuplicatesHandler = checkDuplicatesHandler;
        _activateHandler = activateHandler;
        _updateHandler = updateHandler;
        _contactHandler = contactHandler;
        _statusHandler = statusHandler;
        _addressHandler = addressHandler;
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
            request.LeadSource,
            request.LeadSourceNote,
            request.ImageFileId,
            request.FileUploadIntentId);


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
        [FromQuery] string? customerType = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortOrder = null,
        [FromQuery] int? page = null,
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
        var query = new ListCustomersQuery(auth.FirebaseUid, auth.MembershipId, search, status, customerType, sortBy, sortOrder, page, limit, cursor);

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

        var totalCount = result.Value.TotalCount;
        var pageSize = result.Value.PageSize;
        var currentPage = result.Value.Page;
        var totalPages = pageSize > 0 ? (int)Math.Ceiling((double)totalCount / pageSize) : 1;

        var pagination = new TanErp.Api.Contracts.Common.PaginationMetadataResponse(
            currentPage,
            pageSize,
            totalCount,
            totalPages,
            result.Value.NextCursor);

        var response = new CustomerListResponse(items, pagination);
        return Ok(response);
    }

    [HttpGet("check-duplicates")]
    [ProducesResponseType<IReadOnlyList<DuplicateCustomerResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CheckDuplicates(
        [FromQuery] string? name,
        [FromQuery] string? phone,
        [FromQuery] string? email,
        CancellationToken cancellationToken = default)
    {
        var contextResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;
        var query = new CheckCustomerDuplicatesQuery(auth.FirebaseUid, auth.MembershipId, name, phone, email);

        var result = await _checkDuplicatesHandler.Handle(query, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var response = result.Value!.Candidates.Select(c => new DuplicateCustomerResponse(
            c.Id,
            c.Code,
            c.DisplayNameTh,
            c.MaskedPhone,
            c.MaskedEmail)).ToList();

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

    [HttpPatch("{id:guid}")]
    [ProducesResponseType<CustomerResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status428PreconditionRequired)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var contextResult = RequestContextReader.ReadConditionalIdempotentRequest(HttpContext);
        if (contextResult.IsFailure) return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        var auth = contextResult.Value!;
        var command = new TanErp.Application.Crm.Customers.UpdateCustomer.UpdateCustomerCommand(
            auth.FirebaseUid, auth.MembershipId, id, auth.IfMatchRowVersion, auth.IdempotencyKey,
            request.CustomerType, request.DisplayNameTh, request.DisplayNameEn, request.PreferredLocale,
            request.LeadSource, request.LeadSourceNote, null, HttpContext.TraceIdentifier,
            request.LegalName, request.TaxIdentifier, request.BranchCode, request.CreditTermDays, request.CreditLimit,
            request.CurrencyCode, request.BillingCycle, request.BillingDay, request.PaymentConditionNote, request.CreditTermDays.HasValue, request.HasTaxIdentifier);
        var result = await _updateHandler.Handle(command, cancellationToken);
        if (result.IsFailure) return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return Ok(MapCustomerResponse(result.Value, null));
    }

    [HttpGet("{id:guid}/contacts")]
    [ProducesResponseType<CustomerContactListResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListContacts([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var context = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (context.IsFailure) return ProblemDetailsMapper.CreateProblemResult(context.Error.Code, HttpContext);
        var result = await _contactHandler.ListAsync(context.Value!.FirebaseUid, context.Value.MembershipId, id, cancellationToken);
        if (result.IsFailure) return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        return Ok(new CustomerContactListResponse(result.Value!.Select(ToContactResponse).ToList()));
    }

    [HttpPost("{id:guid}/contacts")]
    [ProducesResponseType<CustomerContactDetailResponse>(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateContact([FromRoute] Guid id, [FromBody] CreatePrimaryContactRequest request, CancellationToken cancellationToken)
    {
        var context = RequestContextReader.ReadIdempotentRequest(HttpContext);
        if (context.IsFailure) return ProblemDetailsMapper.CreateProblemResult(context.Error.Code, HttpContext);
        var auth = context.Value!;
        var result = await _contactHandler.CreateAsync(auth.FirebaseUid, auth.MembershipId, id,
            new TanErp.Domain.Crm.Customers.PrimaryContactInput(request.Name, request.RoleTitle, request.Phone, request.Email, request.PreferredChannel, request.LineId),
            auth.IdempotencyKey, HttpContext.TraceIdentifier, cancellationToken);
        if (result.IsFailure) return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return CreatedAtAction(nameof(ListContacts), new { id }, ToContactResponse(result.Value));
    }

    [HttpPatch("{id:guid}/contacts/{contactId:guid}")]
    [ProducesResponseType<CustomerContactDetailResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateContact([FromRoute] Guid id, [FromRoute] Guid contactId, [FromBody] CreatePrimaryContactRequest request, CancellationToken cancellationToken)
    {
        var context = RequestContextReader.ReadConditionalAuthenticatedRequest(HttpContext);
        if (context.IsFailure) return ProblemDetailsMapper.CreateProblemResult(context.Error.Code, HttpContext);
        var auth = context.Value!;
        var result = await _contactHandler.UpdateAsync(auth.FirebaseUid, auth.MembershipId, id, contactId, auth.IfMatchRowVersion,
            new TanErp.Domain.Crm.Customers.PrimaryContactInput(request.Name, request.RoleTitle, request.Phone, request.Email, request.PreferredChannel, request.LineId), HttpContext.TraceIdentifier, cancellationToken);
        if (result.IsFailure) return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return Ok(ToContactResponse(result.Value));
    }

    [HttpPost("{id:guid}/contacts/{contactId:guid}/primary")]
    [ProducesResponseType<CustomerContactDetailResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> SetPrimaryContact([FromRoute] Guid id, [FromRoute] Guid contactId, CancellationToken cancellationToken)
    {
        var context = RequestContextReader.ReadConditionalAuthenticatedRequest(HttpContext);
        if (context.IsFailure) return ProblemDetailsMapper.CreateProblemResult(context.Error.Code, HttpContext);
        var auth = context.Value!;
        var result = await _contactHandler.SetPrimaryAsync(auth.FirebaseUid, auth.MembershipId, id, contactId, auth.IfMatchRowVersion, HttpContext.TraceIdentifier, cancellationToken);
        if (result.IsFailure) return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return Ok(ToContactResponse(result.Value));
    }

    [HttpPost("{id:guid}/contacts/{contactId:guid}/deactivate")]
    [ProducesResponseType<CustomerContactDetailResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeactivateContact([FromRoute] Guid id, [FromRoute] Guid contactId, CancellationToken cancellationToken)
    {
        var context = RequestContextReader.ReadConditionalAuthenticatedRequest(HttpContext);
        if (context.IsFailure) return ProblemDetailsMapper.CreateProblemResult(context.Error.Code, HttpContext);
        var auth = context.Value!;
        var result = await _contactHandler.DeactivateAsync(auth.FirebaseUid, auth.MembershipId, id, contactId, auth.IfMatchRowVersion, HttpContext.TraceIdentifier, cancellationToken);
        if (result.IsFailure) return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return Ok(ToContactResponse(result.Value));
    }

    [HttpPost("{id:guid}/activate")]
    [ProducesResponseType<CustomerResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status428PreconditionRequired)]
    public async Task<IActionResult> Activate(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var contextResult = RequestContextReader.ReadConditionalIdempotentRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;
        var traceId = HttpContext.TraceIdentifier;

        var command = new TanErp.Application.Crm.Customers.ActivateCustomer.ActivateCustomerCommand(
            auth.FirebaseUid,
            auth.MembershipId,
            id,
            auth.IfMatchRowVersion,
            auth.IdempotencyKey,
            traceId);

        var result = await _activateHandler.Handle(command, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var customer = result.Value!;
        var response = MapCustomerResponse(customer, null);

        Response.Headers.ETag = $"\"{customer.RowVersion}\"";
        return Ok(response);
    }

    [HttpPost("{id:guid}/deactivate")]
    public Task<IActionResult> Deactivate([FromRoute] Guid id, [FromBody] DeactivateCustomerRequest request, CancellationToken cancellationToken) =>
        ChangeStatus(id, request.Reason, reactivate: false, cancellationToken);

    [HttpPost("{id:guid}/reactivate")]
    public Task<IActionResult> Reactivate([FromRoute] Guid id, CancellationToken cancellationToken) =>
        ChangeStatus(id, null, reactivate: true, cancellationToken);

    private async Task<IActionResult> ChangeStatus(Guid id, string? reason, bool reactivate, CancellationToken cancellationToken)
    {
        var context = RequestContextReader.ReadConditionalIdempotentRequest(HttpContext);
        if (context.IsFailure) return ProblemDetailsMapper.CreateProblemResult(context.Error.Code, HttpContext);
        var auth = context.Value!;
        var result = await _statusHandler.Handle(new TanErp.Application.Crm.Customers.ChangeCustomerStatus.ChangeCustomerStatusCommand(
            auth.FirebaseUid, auth.MembershipId, id, auth.IfMatchRowVersion, auth.IdempotencyKey, reason, reactivate, HttpContext.TraceIdentifier), cancellationToken);
        if (result.IsFailure) return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return Ok(MapCustomerResponse(result.Value, null));
    }

    [HttpGet("{id:guid}/addresses")]
    [ProducesResponseType<CustomerAddressListResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAddresses([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var context = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (context.IsFailure) return ProblemDetailsMapper.CreateProblemResult(context.Error.Code, HttpContext);
        var result = await _addressHandler.ListAsync(context.Value!.FirebaseUid, context.Value.MembershipId, id, cancellationToken);
        if (result.IsFailure) return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        return Ok(new CustomerAddressListResponse(result.Value!.Select(ToAddressResponse).ToList()));
    }

    [HttpPost("{id:guid}/addresses")]
    [ProducesResponseType<CustomerAddressResponse>(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAddress([FromRoute] Guid id, [FromBody] CreateCustomerAddressRequest request, CancellationToken cancellationToken)
    {
        var context = RequestContextReader.ReadIdempotentRequest(HttpContext);
        if (context.IsFailure) return ProblemDetailsMapper.CreateProblemResult(context.Error.Code, HttpContext);
        var auth = context.Value!;
        var data = new CreateCustomerAddressData(request.AddressType, request.Label, request.AddressLine1, request.Subdistrict, request.District, request.Province, request.PostalCode, request.CountryCode, request.IsPrimary);
        var result = await _addressHandler.CreateAsync(auth.FirebaseUid, auth.MembershipId, id, data, auth.IdempotencyKey, HttpContext.TraceIdentifier, cancellationToken);
        if (result.IsFailure) return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return CreatedAtAction(nameof(ListAddresses), new { id }, ToAddressResponse(result.Value));
    }

    [HttpPatch("{id:guid}/addresses/{addressId:guid}")]
    [ProducesResponseType<CustomerAddressResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateAddress([FromRoute] Guid id, [FromRoute] Guid addressId, [FromBody] UpdateCustomerAddressRequest request, CancellationToken cancellationToken)
    {
        var context = RequestContextReader.ReadConditionalAuthenticatedRequest(HttpContext);
        if (context.IsFailure) return ProblemDetailsMapper.CreateProblemResult(context.Error.Code, HttpContext);
        var auth = context.Value!;
        var data = new UpdateCustomerAddressData(request.Label, request.AddressLine1, request.Subdistrict, request.District, request.Province, request.PostalCode, request.CountryCode);
        var result = await _addressHandler.UpdateAsync(auth.FirebaseUid, auth.MembershipId, id, addressId, auth.IfMatchRowVersion, data, HttpContext.TraceIdentifier, cancellationToken);
        if (result.IsFailure) return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return Ok(ToAddressResponse(result.Value));
    }

    [HttpPost("{id:guid}/addresses/{addressId:guid}/primary")]
    [ProducesResponseType<CustomerAddressResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> SetPrimaryAddress([FromRoute] Guid id, [FromRoute] Guid addressId, CancellationToken cancellationToken)
    {
        var context = RequestContextReader.ReadConditionalAuthenticatedRequest(HttpContext);
        if (context.IsFailure) return ProblemDetailsMapper.CreateProblemResult(context.Error.Code, HttpContext);
        var auth = context.Value!;
        var result = await _addressHandler.SetPrimaryAsync(auth.FirebaseUid, auth.MembershipId, id, addressId, auth.IfMatchRowVersion, HttpContext.TraceIdentifier, cancellationToken);
        if (result.IsFailure) return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return Ok(ToAddressResponse(result.Value));
    }

    [HttpPost("{id:guid}/addresses/{addressId:guid}/deactivate")]
    [ProducesResponseType<CustomerAddressResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeactivateAddress([FromRoute] Guid id, [FromRoute] Guid addressId, CancellationToken cancellationToken)
    {
        var context = RequestContextReader.ReadConditionalAuthenticatedRequest(HttpContext);
        if (context.IsFailure) return ProblemDetailsMapper.CreateProblemResult(context.Error.Code, HttpContext);
        var auth = context.Value!;
        var result = await _addressHandler.DeactivateAsync(auth.FirebaseUid, auth.MembershipId, id, addressId, auth.IfMatchRowVersion, HttpContext.TraceIdentifier, cancellationToken);
        if (result.IsFailure) return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return Ok(ToAddressResponse(result.Value));
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
            customer.LeadSource,
            customer.LeadSourceNote,
            customer.ImageFileId,
            customer.LegalName,
            customer.TaxIdentifier,
            customer.BranchCode,
            customer.CreditTermDays,
            customer.CreditLimit,
            customer.CurrencyCode,
            customer.BillingCycle,
            customer.BillingDay,
            customer.PaymentConditionNote,
            customer.InactiveReason);
    }

    private static CustomerContactDetailResponse ToContactResponse(TanErp.Application.Crm.Customers.CustomerContactDetailProjection contact) =>
        new(contact.Id, contact.CustomerId, contact.Name, contact.RoleTitle, contact.Phone, contact.Email, contact.LineId, contact.PreferredChannel, contact.IsPrimary, contact.Status, contact.RowVersion);

    private static CustomerAddressResponse ToAddressResponse(CustomerAddressProjection address) =>
        new(address.Id, address.CustomerId, address.AddressType, address.Label, address.AddressLine1, address.Subdistrict, address.District, address.Province, address.PostalCode, address.CountryCode, address.Status, address.IsPrimary, address.RowVersion);
}
