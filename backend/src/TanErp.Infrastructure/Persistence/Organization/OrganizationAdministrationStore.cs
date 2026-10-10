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

    public OrganizationAdministrationStore(AppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
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
}
