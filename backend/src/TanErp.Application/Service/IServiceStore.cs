using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Service;

public interface IServiceStore
{
    Task<Result<InstallationProjection>> CreateInstallationAsync(RequestAccessContext access, InstallationInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default);
    Task<Result<InstallationProjection>> InstallationCommandAsync(RequestAccessContext access, Guid installationId, Guid expectedVersion, InstallationCommand command, string traceId, CancellationToken ct = default);
    Task<InstallationProjection?> GetInstallationAsync(Guid organizationId, Guid installationId, CancellationToken ct = default);
    Task<PagedInstallations> ListInstallationsAsync(Guid organizationId, InstallationListQuery query, CancellationToken ct = default);

    Task<WarrantyProjection?> GetWarrantyAsync(Guid organizationId, Guid warrantyId, CancellationToken ct = default);
    Task<PagedWarranties> ListWarrantiesAsync(Guid organizationId, WarrantyListQuery query, CancellationToken ct = default);

    Task<Result<ServiceRequestProjection>> CreateServiceRequestAsync(RequestAccessContext access, ServiceRequestInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default);
    Task<Result<ServiceRequestProjection>> ServiceRequestCommandAsync(RequestAccessContext access, Guid requestId, Guid expectedVersion, ServiceRequestCommand command, string traceId, CancellationToken ct = default);
    Task<ServiceRequestProjection?> GetServiceRequestAsync(Guid organizationId, Guid requestId, CancellationToken ct = default);
    Task<PagedServiceRequests> ListServiceRequestsAsync(Guid organizationId, ServiceRequestListQuery query, CancellationToken ct = default);
}
