using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Results;
using TanErp.Application.Estimates.CalculationPolicies;
using TanErp.Domain.Estimates;

namespace TanErp.Infrastructure.Persistence.Estimates;

public sealed class CalculationPolicyStore : ICalculationPolicyStore
{
    private readonly AppDbContext _db;

    public CalculationPolicyStore(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<CalculationPolicyVersion>> ListAsync(
        Guid organizationId, Guid? branchId, CancellationToken ct = default)
    {
        var query = _db.CalculationPolicyVersions.AsNoTracking()
            .Where(p => p.OrganizationId == organizationId);

        if (branchId.HasValue)
        {
            query = query.Where(p => p.BranchId == branchId.Value);
        }

        return await query
            .OrderBy(p => p.PolicyCode)
            .ThenByDescending(p => p.Version)
            .ToListAsync(ct);
    }

    public async Task<CalculationPolicyVersion?> GetByIdAsync(
        Guid organizationId, Guid id, CancellationToken ct = default)
    {
        return await _db.CalculationPolicyVersions
            .FirstOrDefaultAsync(p => p.OrganizationId == organizationId && p.Id == id, ct);
    }

    public async Task<Result<CalculationPolicyVersion>> CreateDraftAsync(
        Guid organizationId, Guid? branchId, string policyCode, int version,
        string overheadMethod, decimal overheadValue, string roundingMode,
        string unitConversionMode, DateTimeOffset effectiveFromUtc, DateTimeOffset? effectiveToUtc,
        Guid createdByUserId, CancellationToken ct = default)
    {
        var cleanCode = policyCode.Trim().ToUpperInvariant();

        var exists = await _db.CalculationPolicyVersions.AnyAsync(
            p => p.OrganizationId == organizationId &&
                 p.BranchId == branchId &&
                 p.PolicyCode == cleanCode &&
                 p.Version == version,
            ct);

        if (exists)
        {
            return Result<CalculationPolicyVersion>.Failure(
                new Error("CALCULATION_POLICY_CONFLICT", $"Calculation policy '{cleanCode}' version {version} already exists."));
        }

        CalculationPolicyVersion policy;
        try
        {
            policy = new CalculationPolicyVersion(
                Guid.NewGuid(),
                organizationId,
                branchId,
                cleanCode,
                version,
                overheadMethod,
                overheadValue,
                roundingMode,
                effectiveFromUtc,
                effectiveToUtc,
                createdByUserId,
                unitConversionMode);
        }
        catch (ArgumentException ex)
        {
            return Result<CalculationPolicyVersion>.Failure(
                new Error("INVALID_CALCULATION_POLICY", ex.Message));
        }

        _db.CalculationPolicyVersions.Add(policy);
        await _db.SaveChangesAsync(ct);

        return Result<CalculationPolicyVersion>.Success(policy);
    }

    public async Task<Result<CalculationPolicyVersion>> PublishAsync(
        Guid organizationId, Guid id, Guid publisherUserId, DateTimeOffset publishedAtUtc, CancellationToken ct = default)
    {
        var policy = await _db.CalculationPolicyVersions
            .FirstOrDefaultAsync(p => p.OrganizationId == organizationId && p.Id == id, ct);

        if (policy is null)
        {
            return Result<CalculationPolicyVersion>.Failure(
                new Error("RESOURCE_NOT_FOUND", "Calculation policy was not found."));
        }

        try
        {
            policy.Publish(publisherUserId, publishedAtUtc);
        }
        catch (EstimateInvalidStateException ex)
        {
            return Result<CalculationPolicyVersion>.Failure(
                new Error("INVALID_POLICY_STATE", ex.Message));
        }

        await _db.SaveChangesAsync(ct);
        return Result<CalculationPolicyVersion>.Success(policy);
    }
}
