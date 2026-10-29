using TanErp.Application.Common.Results;
using TanErp.Domain.Estimates;

namespace TanErp.Application.Estimates.CalculationPolicies;

public interface ICalculationPolicyStore
{
    Task<IReadOnlyList<CalculationPolicyVersion>> ListAsync(Guid organizationId, Guid? branchId, CancellationToken ct = default);
    Task<CalculationPolicyVersion?> GetByIdAsync(Guid organizationId, Guid id, CancellationToken ct = default);
    Task<Result<CalculationPolicyVersion>> CreateDraftAsync(
        Guid organizationId, Guid? branchId, string policyCode, int version,
        string overheadMethod, decimal overheadValue, string roundingMode,
        string unitConversionMode, DateTimeOffset effectiveFromUtc, DateTimeOffset? effectiveToUtc,
        Guid createdByUserId, CancellationToken ct = default);
    Task<Result<CalculationPolicyVersion>> PublishAsync(
        Guid organizationId, Guid id, Guid publisherUserId, DateTimeOffset publishedAtUtc, CancellationToken ct = default);
}
