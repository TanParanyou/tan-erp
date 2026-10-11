using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.IdentityAccess.Administration;
using TanErp.Application.Organization.Administration;
using TanErp.Domain.Common;
using TanErp.Domain.Organization;

namespace TanErp.Infrastructure.Persistence.OrganizationAdministration;

public sealed class OrganizationAdministrationStore : IOrganizationAdministrationStore
{
    private const string VersionConflict = "ADMIN_VERSION_CONFLICT";

    private readonly AppDbContext _db;
    private readonly IClock _clock;
    private readonly BranchDependencyInspector _inspector;

    public OrganizationAdministrationStore(AppDbContext db, IClock clock, BranchDependencyInspector inspector)
    {
        _db = db;
        _clock = clock;
        _inspector = inspector;
    }

    private static Result<T> Fail<T>(string code, string message) => Result<T>.Failure(new Error(code, message));

    private void AddAudit(
        Guid organizationId, AdminActor actor, string action, string resourceType, string resourceId, string traceId,
        object changes, Guid? versionBefore, Guid? versionAfter, Guid? branchId = null)
    {
        _db.AuditEvents.Add(new AuditEvent(
            Guid.NewGuid(), organizationId, actor.UserId, action, resourceType, resourceId, _clock.UtcNow, traceId,
            JsonSerializer.Serialize(changes), branchId, actorMembershipId: actor.MembershipId,
            rowVersionBefore: versionBefore, rowVersionAfter: versionAfter));
    }

    private static OrganizationProfile ToProfile(TanErp.Domain.Organization.Organization o) =>
        new(o.Id, o.Name, o.NameEn, o.TaxIdentifier, o.AddressTh, o.AddressEn, o.Phone, o.RowVersion);

    public async Task<Result<OrganizationProfile>> GetProfileAsync(Guid organizationId, CancellationToken ct)
    {
        var org = await _db.Organizations.AsNoTracking().FirstOrDefaultAsync(o => o.Id == organizationId, ct);
        return org is null ? Fail<OrganizationProfile>("RESOURCE_NOT_FOUND", "Organization was not found.") : Result<OrganizationProfile>.Success(ToProfile(org));
    }

    public Task<Result<OrganizationProfile>> UpdateProfileAsync(
        Guid organizationId, OrganizationProfileInput input, Guid ifMatch, AdminActor actor, string traceId, CancellationToken ct) =>
        SerializableTransactionRunner.RunAsync(_db, async () =>
        {
            var org = await _db.Organizations.FirstOrDefaultAsync(o => o.Id == organizationId, ct);
            if (org is null) return Fail<OrganizationProfile>("RESOURCE_NOT_FOUND", "Organization was not found.");
            if (org.RowVersion != ifMatch) return Fail<OrganizationProfile>(VersionConflict, "The organization was modified by another user.");

            var before = org.RowVersion;
            var changed = ChangedFields(org, input);
            try
            {
                org.UpdateProfile(input.Name, input.NameEn, input.TaxIdentifier, input.AddressTh, input.AddressEn, input.Phone);
            }
            catch (OrganizationDomainException ex)
            {
                return Fail<OrganizationProfile>(ex.Code, ex.Message);
            }

            AddAudit(organizationId, actor, "organization.profile-updated", "Organization", org.Id.ToString(), traceId,
                new { changedFields = changed }, before, org.RowVersion);
            await _db.SaveChangesAsync(ct);
            return Result<OrganizationProfile>.Success(ToProfile(org));
        }, VersionConflict, ct);

    /// <summary>Field names only: addresses and tax ids must never be copied into the audit log.</summary>
    private static string[] ChangedFields(TanErp.Domain.Organization.Organization o, OrganizationProfileInput i)
    {
        static string? N(string? v) => string.IsNullOrWhiteSpace(v) ? null : v.Trim();
        var changed = new List<string>();
        if (o.Name != N(i.Name)) changed.Add("name");
        if (o.NameEn != N(i.NameEn)) changed.Add("nameEn");
        if (o.TaxIdentifier != N(i.TaxIdentifier)) changed.Add("taxIdentifier");
        if (o.AddressTh != N(i.AddressTh)) changed.Add("addressTh");
        if (o.AddressEn != N(i.AddressEn)) changed.Add("addressEn");
        if (o.Phone != N(i.Phone)) changed.Add("phone");
        return changed.ToArray();
    }

    private const string CreateOperation = "admin.branches.create";
    private const string CodeIndex = "ix_branches_organization_id_branch_code";
    private const string TaxCodeIndex = "ix_branches_organization_id_tax_branch_code";

    private static BranchDetail ToDetail(Branch b) => new(
        b.Id, b.Code, b.Name, b.NameEn, b.TaxBranchCode, b.AddressTh, b.AddressEn, b.Phone, b.IsActive, b.RowVersion, b.CreatedAtUtc);

    public async Task<IReadOnlyList<BranchDetail>> ListBranchesAsync(Guid organizationId, BranchStatusFilter filter, CancellationToken ct)
    {
        var query = _db.Branches.AsNoTracking().Where(b => b.OrganizationId == organizationId);
        if (filter == BranchStatusFilter.Active) query = query.Where(b => b.IsActive);
        if (filter == BranchStatusFilter.Inactive) query = query.Where(b => !b.IsActive);

        var rows = await query.OrderBy(b => b.Code).ToListAsync(ct);
        return rows.Select(ToDetail).ToList();
    }

    public async Task<Result<BranchDetail>> GetBranchAsync(Guid organizationId, Guid branchId, CancellationToken ct)
    {
        // Always filtered by organization: an id from another organization is indistinguishable from a missing one.
        var branch = await _db.Branches.AsNoTracking().FirstOrDefaultAsync(b => b.Id == branchId && b.OrganizationId == organizationId, ct);
        return branch is null ? Fail<BranchDetail>("RESOURCE_NOT_FOUND", "Branch was not found.") : Result<BranchDetail>.Success(ToDetail(branch));
    }

    public Task<Result<BranchDetail>> CreateBranchAsync(
        Guid organizationId, CreateBranchInput input, AdminActor actor, string keyHash, string payloadHash, string traceId, CancellationToken ct) =>
        SerializableTransactionRunner.RunAsync(_db, async () =>
        {
            // The advisory lock is taken INSIDE the transaction so concurrent requests with the same key serialize and the loser replays.
            await _db.Database.ExecuteSqlInterpolatedAsync(
                $"SELECT pg_advisory_xact_lock(hashtextextended({organizationId.ToString("N") + ":" + CreateOperation + ":" + keyHash}, 0))", ct);

            var replay = await _db.IdempotencyRecords.AsNoTracking().FirstOrDefaultAsync(
                r => r.OrganizationId == organizationId && r.Operation == CreateOperation && r.KeyHash == keyHash, ct);
            if (replay is not null)
            {
                if (replay.PayloadHash != payloadHash)
                    return Fail<BranchDetail>("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload.");
                return await GetBranchAsync(organizationId, Guid.Parse(replay.ResourceId), ct);
            }

            Branch branch;
            try
            {
                var d = input.Details;
                branch = Branch.Create(Guid.NewGuid(), organizationId, input.Code, d.Name, d.NameEn, d.TaxBranchCode, d.AddressTh, d.AddressEn, d.Phone, _clock.UtcNow);
            }
            catch (OrganizationDomainException ex)
            {
                return Fail<BranchDetail>(ex.Code, ex.Message);
            }

            _db.Branches.Add(branch);
            _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), organizationId, CreateOperation, keyHash, payloadHash, branch.Id.ToString(), _clock.UtcNow));
            AddAudit(organizationId, actor, "branches.created", "Branch", branch.Id.ToString(), traceId,
                new { code = branch.Code, hasTaxBranchCode = branch.TaxBranchCode is not null }, null, branch.RowVersion, branch.Id);

            try
            {
                await _db.SaveChangesAsync(ct);
            }
            catch (DbUpdateException ex) when (SerializableTransactionRunner.IsUniqueViolation(ex, CodeIndex))
            {
                return Fail<BranchDetail>("BRANCH_CODE_ALREADY_EXISTS", "A branch with this code already exists.");
            }
            catch (DbUpdateException ex) when (SerializableTransactionRunner.IsUniqueViolation(ex, TaxCodeIndex))
            {
                return Fail<BranchDetail>("BRANCH_TAX_CODE_ALREADY_EXISTS", "A branch with this tax branch code already exists.");
            }

            return Result<BranchDetail>.Success(ToDetail(branch));
        }, VersionConflict, ct);

    public Task<Result<BranchDetail>> UpdateBranchAsync(
        Guid organizationId, Guid branchId, BranchInput input, Guid ifMatch, AdminActor actor, string traceId, CancellationToken ct) =>
        SerializableTransactionRunner.RunAsync(_db, async () =>
        {
            // The branch code is immutable: BranchInput carries no code, so only the detail fields can change here.
            var branch = await _db.Branches.FirstOrDefaultAsync(b => b.Id == branchId && b.OrganizationId == organizationId, ct);
            if (branch is null) return Fail<BranchDetail>("RESOURCE_NOT_FOUND", "Branch was not found.");
            if (branch.RowVersion != ifMatch) return Fail<BranchDetail>(VersionConflict, "The branch was modified by another user.");

            var before = branch.RowVersion;
            var changed = ChangedFields(branch, input);
            try
            {
                branch.UpdateDetails(input.Name, input.NameEn, input.TaxBranchCode, input.AddressTh, input.AddressEn, input.Phone);
            }
            catch (OrganizationDomainException ex)
            {
                return Fail<BranchDetail>(ex.Code, ex.Message);
            }

            AddAudit(organizationId, actor, "branches.updated", "Branch", branch.Id.ToString(), traceId,
                new { changedFields = changed }, before, branch.RowVersion, branch.Id);
            try
            {
                await _db.SaveChangesAsync(ct);
            }
            catch (DbUpdateException ex) when (SerializableTransactionRunner.IsUniqueViolation(ex, TaxCodeIndex))
            {
                return Fail<BranchDetail>("BRANCH_TAX_CODE_ALREADY_EXISTS", "A branch with this tax branch code already exists.");
            }

            return Result<BranchDetail>.Success(ToDetail(branch));
        }, VersionConflict, ct);

    /// <summary>Open work and memberships block deactivation; the last-active rule applies only to an active branch.</summary>
    private async Task<List<BranchBlocker>> BlockersAsync(Branch branch, CancellationToken ct)
    {
        var blockers = (await _inspector.InspectAsync(branch.OrganizationId, branch.Id, ct)).ToList();
        var otherActive = await _db.Branches.CountAsync(b => b.OrganizationId == branch.OrganizationId && b.IsActive && b.Id != branch.Id, ct);
        if (branch.IsActive && otherActive == 0) blockers.Add(new BranchBlocker(BranchBlockerTypes.LastActiveBranch, 1));
        return blockers;
    }

    /// <summary>Priority: open work first, then memberships, then the last-active-branch rule.</summary>
    private static string BlockerErrorCode(IReadOnlyList<BranchBlocker> blockers)
    {
        if (blockers.Any(b => b.Type is not (BranchBlockerTypes.Memberships or BranchBlockerTypes.LastActiveBranch))) return "BRANCH_HAS_OPEN_DOCUMENTS";
        return blockers.Any(b => b.Type == BranchBlockerTypes.Memberships) ? "BRANCH_HAS_ACTIVE_MEMBERSHIPS" : "BRANCH_LAST_ACTIVE";
    }

    public async Task<Result<BranchDeactivationCheck>> CheckDeactivationAsync(Guid organizationId, Guid branchId, CancellationToken ct)
    {
        var branch = await _db.Branches.AsNoTracking().FirstOrDefaultAsync(b => b.Id == branchId && b.OrganizationId == organizationId, ct);
        if (branch is null) return Fail<BranchDeactivationCheck>("RESOURCE_NOT_FOUND", "Branch was not found.");

        var blockers = await BlockersAsync(branch, ct);
        return Result<BranchDeactivationCheck>.Success(new BranchDeactivationCheck(blockers.Count == 0, blockers));
    }

    public Task<Result<BranchDetail>> SetBranchActiveAsync(
        Guid organizationId, Guid branchId, bool active, string? reason, Guid ifMatch, AdminActor actor, string traceId, CancellationToken ct) =>
        SerializableTransactionRunner.RunAsync(_db, async () =>
        {
            // One lock per organization, taken inside the transaction: two concurrent deactivations must not both see
            // "another active branch exists" and leave the organization with none.
            await _db.Database.ExecuteSqlInterpolatedAsync(
                $"SELECT pg_advisory_xact_lock(hashtextextended({"branch-active:" + organizationId.ToString("N")}, 0))", ct);

            var branch = await _db.Branches.FirstOrDefaultAsync(b => b.Id == branchId && b.OrganizationId == organizationId, ct);
            if (branch is null) return Fail<BranchDetail>("RESOURCE_NOT_FOUND", "Branch was not found.");
            if (branch.RowVersion != ifMatch) return Fail<BranchDetail>(VersionConflict, "The branch was modified by another user.");
            if (branch.IsActive == active) return Result<BranchDetail>.Success(ToDetail(branch));

            var before = branch.RowVersion;
            if (active)
            {
                branch.Activate();
                AddAudit(organizationId, actor, "branches.activated", "Branch", branch.Id.ToString(), traceId, new { }, before, branch.RowVersion, branch.Id);
            }
            else
            {
                // Guard runs in the same Serializable transaction as the write, so it cannot race with a concurrent document insert.
                var blockers = await BlockersAsync(branch, ct);
                if (blockers.Count > 0) return Fail<BranchDetail>(BlockerErrorCode(blockers), "The branch cannot be deactivated while it is in use.");

                branch.Deactivate();
                AddAudit(organizationId, actor, "branches.deactivated", "Branch", branch.Id.ToString(), traceId, new { reason }, before, branch.RowVersion, branch.Id);
            }

            await _db.SaveChangesAsync(ct);
            return Result<BranchDetail>.Success(ToDetail(branch));
        }, VersionConflict, ct);

    /// <summary>Field names only: addresses and tax codes must never be copied into the audit log.</summary>
    private static string[] ChangedFields(Branch b, BranchInput i)
    {
        static string? N(string? v) => string.IsNullOrWhiteSpace(v) ? null : v.Trim();
        var changed = new List<string>();
        if (b.Name != N(i.Name)) changed.Add("name");
        if (b.NameEn != N(i.NameEn)) changed.Add("nameEn");
        if (b.TaxBranchCode != N(i.TaxBranchCode)) changed.Add("taxBranchCode");
        if (b.AddressTh != N(i.AddressTh)) changed.Add("addressTh");
        if (b.AddressEn != N(i.AddressEn)) changed.Add("addressEn");
        if (b.Phone != N(i.Phone)) changed.Add("phone");
        return changed.ToArray();
    }
}
