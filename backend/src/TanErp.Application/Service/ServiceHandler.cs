using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Domain.Service;

namespace TanErp.Application.Service;

public sealed record ServiceCaller(string FirebaseUid, Guid MembershipId, string TraceId);

/// <summary>Installation, handover, warranty and after-sales use cases. Each call resolves its permission from PostgreSQL first.</summary>
public class ServiceHandler
{
    public const int MaxPageSize = 100;
    public const int DefaultPageSize = 25;

    private readonly IRequestAccessResolver _accessResolver;
    private readonly IServiceStore _store;

    public ServiceHandler(IRequestAccessResolver accessResolver, IServiceStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    private Task<Result<RequestAccessContext>> AccessAsync(ServiceCaller caller, string permission, CancellationToken ct) =>
        _accessResolver.ResolveAsync(caller.FirebaseUid, caller.MembershipId, permission, ct);

    private static Result<T> Fail<T>(string code, string message) => Result<T>.Failure(new Error(code, message));

    private static (int Page, int PageSize) Paging(int page, int pageSize) =>
        (Math.Max(1, page), pageSize <= 0 ? DefaultPageSize : Math.Min(pageSize, MaxPageSize));

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    // ----- installation ----------------------------------------------------------------------------

    public async Task<Result<InstallationProjection>> CreateInstallationAsync(ServiceCaller caller, string idempotencyKey, InstallationInput? input, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "installations.manage", ct);
        if (access.IsFailure) return Result<InstallationProjection>.Failure(access.Error);
        if (input is null || input.ProjectId == Guid.Empty) return Fail<InstallationProjection>("INSTALLATION_FIELD_REQUIRED", "A project is required.");
        if (input.ScheduledEnd < input.ScheduledStart) return Fail<InstallationProjection>("INSTALLATION_FIELD_INVALID", "The end date cannot be before the start date.");
        var checklist = input.Checklist ?? Array.Empty<ChecklistInput>();
        if (checklist.Count > InstallationJob.MaxChecklistItems || checklist.Any(c => string.IsNullOrWhiteSpace(c.Title) || c.Title.Trim().Length > 200))
        {
            return Fail<InstallationProjection>("INSTALLATION_FIELD_INVALID", $"A checklist can have up to {InstallationJob.MaxChecklistItems} items with titles of 1-200 characters.");
        }

        var keyHash = Sha256Hex.Compute(idempotencyKey);
        var items = string.Join(";", checklist.Select(c => $"{c.Title.Trim()}:{c.Required}"));
        var payloadHash = Sha256Hex.Compute($"{input.ProjectId}|{input.ScheduledStart:O}|{input.ScheduledEnd:O}|{Clean(input.CrewName)}|{Clean(input.Note)}|{items}");
        return await _store.CreateInstallationAsync(access.Value!, input with { Checklist = checklist }, keyHash, payloadHash, caller.TraceId, ct);
    }

    private static string PermissionFor(InstallationCommand command) => command switch
    {
        CancelInstallation => "installations.manage",
        RecordHandover => "installations.handover",
        _ => "installations.operate"
    };

    public async Task<Result<InstallationProjection>> InstallationCommandAsync(ServiceCaller caller, Guid installationId, Guid expectedVersion, InstallationCommand command, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, PermissionFor(command), ct);
        if (access.IsFailure) return Result<InstallationProjection>.Failure(access.Error);
        if (expectedVersion == Guid.Empty) return Fail<InstallationProjection>("INSTALLATION_FIELD_REQUIRED", "Expected version is required.");
        return await _store.InstallationCommandAsync(access.Value!, installationId, expectedVersion, command, caller.TraceId, ct);
    }

    public async Task<Result<InstallationProjection>> GetInstallationAsync(ServiceCaller caller, Guid installationId, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "installations.read", ct);
        if (access.IsFailure) return Result<InstallationProjection>.Failure(access.Error);
        var job = await _store.GetInstallationAsync(access.Value!.OrganizationId, installationId, ct);
        return job is null ? Fail<InstallationProjection>("RESOURCE_NOT_FOUND", "Installation not found.") : Result<InstallationProjection>.Success(job);
    }

    public async Task<Result<PagedInstallations>> ListInstallationsAsync(ServiceCaller caller, string? search, string? status, Guid? projectId, int page, int pageSize, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "installations.read", ct);
        if (access.IsFailure) return Result<PagedInstallations>.Failure(access.Error);
        if (!string.IsNullOrWhiteSpace(status) && !InstallationStatus.All.Contains(status.Trim())) return Fail<PagedInstallations>("INSTALLATION_FIELD_INVALID", "The status filter is invalid.");
        var (p, size) = Paging(page, pageSize);
        return Result<PagedInstallations>.Success(await _store.ListInstallationsAsync(access.Value!.OrganizationId, new InstallationListQuery(Clean(search), Clean(status), projectId, p, size), ct));
    }

    // ----- warranty --------------------------------------------------------------------------------

    public async Task<Result<WarrantyProjection>> GetWarrantyAsync(ServiceCaller caller, Guid warrantyId, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "warranties.read", ct);
        if (access.IsFailure) return Result<WarrantyProjection>.Failure(access.Error);
        var warranty = await _store.GetWarrantyAsync(access.Value!.OrganizationId, warrantyId, ct);
        return warranty is null ? Fail<WarrantyProjection>("RESOURCE_NOT_FOUND", "Warranty not found.") : Result<WarrantyProjection>.Success(warranty);
    }

    public async Task<Result<PagedWarranties>> ListWarrantiesAsync(ServiceCaller caller, string? search, string? state, Guid? projectId, int page, int pageSize, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "warranties.read", ct);
        if (access.IsFailure) return Result<PagedWarranties>.Failure(access.Error);
        if (!string.IsNullOrWhiteSpace(state) && state.Trim() is not ("active" or "expired")) return Fail<PagedWarranties>("SERVICE_FIELD_INVALID", "The warranty state filter is invalid.");
        var (p, size) = Paging(page, pageSize);
        return Result<PagedWarranties>.Success(await _store.ListWarrantiesAsync(access.Value!.OrganizationId, new WarrantyListQuery(Clean(search), Clean(state), projectId, p, size), ct));
    }

    // ----- service requests ------------------------------------------------------------------------

    public async Task<Result<ServiceRequestProjection>> CreateServiceRequestAsync(ServiceCaller caller, string idempotencyKey, ServiceRequestInput? input, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "service-requests.manage", ct);
        if (access.IsFailure) return Result<ServiceRequestProjection>.Failure(access.Error);
        if (input is null || input.ProjectId == Guid.Empty) return Fail<ServiceRequestProjection>("SERVICE_FIELD_REQUIRED", "A project is required.");
        if (string.IsNullOrWhiteSpace(input.Title) || string.IsNullOrWhiteSpace(input.Description) || !ServicePriority.All.Contains(input.Priority ?? string.Empty))
        {
            return Fail<ServiceRequestProjection>("SERVICE_FIELD_INVALID", "A title, a description and a valid priority are required.");
        }

        var keyHash = Sha256Hex.Compute(idempotencyKey);
        var payloadHash = Sha256Hex.Compute($"{input.ProjectId}|{input.Title.Trim()}|{input.Description.Trim()}|{input.Priority}");
        return await _store.CreateServiceRequestAsync(access.Value!, input, keyHash, payloadHash, caller.TraceId, ct);
    }

    public async Task<Result<ServiceRequestProjection>> ServiceRequestCommandAsync(ServiceCaller caller, Guid requestId, Guid expectedVersion, ServiceRequestCommand command, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "service-requests.manage", ct);
        if (access.IsFailure) return Result<ServiceRequestProjection>.Failure(access.Error);
        if (expectedVersion == Guid.Empty) return Fail<ServiceRequestProjection>("SERVICE_FIELD_REQUIRED", "Expected version is required.");
        return await _store.ServiceRequestCommandAsync(access.Value!, requestId, expectedVersion, command, caller.TraceId, ct);
    }

    public async Task<Result<ServiceRequestProjection>> GetServiceRequestAsync(ServiceCaller caller, Guid requestId, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "service-requests.read", ct);
        if (access.IsFailure) return Result<ServiceRequestProjection>.Failure(access.Error);
        var request = await _store.GetServiceRequestAsync(access.Value!.OrganizationId, requestId, ct);
        return request is null ? Fail<ServiceRequestProjection>("RESOURCE_NOT_FOUND", "Service request not found.") : Result<ServiceRequestProjection>.Success(request);
    }

    public async Task<Result<PagedServiceRequests>> ListServiceRequestsAsync(ServiceCaller caller, string? search, string? status, Guid? projectId, bool? inWarranty, int page, int pageSize, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "service-requests.read", ct);
        if (access.IsFailure) return Result<PagedServiceRequests>.Failure(access.Error);
        if (!string.IsNullOrWhiteSpace(status) && !ServiceRequestStatus.All.Contains(status.Trim())) return Fail<PagedServiceRequests>("SERVICE_FIELD_INVALID", "The status filter is invalid.");
        var (p, size) = Paging(page, pageSize);
        return Result<PagedServiceRequests>.Success(await _store.ListServiceRequestsAsync(access.Value!.OrganizationId, new ServiceRequestListQuery(Clean(search), Clean(status), projectId, inWarranty, p, size), ct));
    }
}
