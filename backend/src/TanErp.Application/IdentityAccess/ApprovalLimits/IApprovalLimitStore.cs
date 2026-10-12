using TanErp.Application.Common.Results;
using TanErp.Application.IdentityAccess.Administration;

namespace TanErp.Application.IdentityAccess.ApprovalLimits;

/// <summary>Persistence for approval limits, always scoped to <c>organizationId</c>. Writes audit in the same transaction.</summary>
public interface IApprovalLimitStore
{
    Task<ApprovalLimitPage> ListAsync(Guid organizationId, ApprovalLimitListQuery query, CancellationToken ct);

    Task<ApprovalLimitOptions> GetOptionsAsync(Guid organizationId, CancellationToken ct);

    Task<Result<ApprovalLimitView>> GetAsync(Guid organizationId, Guid limitId, CancellationToken ct);

    Task<Result<ApprovalLimitView>> CreateAsync(
        Guid organizationId, Guid roleId, string documentType, Guid? branchId, string currency, bool isUnlimited, decimal? maxAmount,
        AdminActor actor, string keyHash, string payloadHash, string traceId, CancellationToken ct);

    Task<Result<ApprovalLimitView>> UpdateAsync(
        Guid organizationId, Guid limitId, Guid ifMatch, bool isUnlimited, decimal? maxAmount, bool isActive,
        AdminActor actor, string traceId, CancellationToken ct);
}
