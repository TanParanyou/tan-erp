using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.IdentityAccess.Administration;

namespace TanErp.Application.Organization.Administration;

/// <summary>Every operation resolves its permission first; the store is never reached (and never queried) without it.</summary>
public sealed class OrganizationAdministrationHandler
{
    private readonly IRequestAccessResolver _access;
    private readonly IOrganizationAdministrationStore _store;

    public OrganizationAdministrationHandler(IRequestAccessResolver access, IOrganizationAdministrationStore store)
    {
        _access = access;
        _store = store;
    }

    private Task<Result<RequestAccessContext>> AccessAsync(AdminCaller caller, string permission, CancellationToken ct) =>
        _access.ResolveAsync(caller.FirebaseUid, caller.MembershipId, permission, ct);

    private static AdminActor Actor(RequestAccessContext access) => new(access.ActorUserId, access.MembershipId);

    private static Result<T> Denied<T>(Result<RequestAccessContext> access) => Result<T>.Failure(access.Error);

    public async Task<Result<OrganizationProfile>> GetProfileAsync(AdminCaller caller, CancellationToken ct)
    {
        var access = await AccessAsync(caller, OrganizationAdminPermissions.OrganizationsRead, ct);
        return access.IsFailure ? Denied<OrganizationProfile>(access) : await _store.GetProfileAsync(access.Value!.OrganizationId, ct);
    }

    public async Task<Result<OrganizationProfile>> UpdateProfileAsync(
        AdminCaller caller, Guid ifMatch, OrganizationProfileInput input, string traceId, CancellationToken ct)
    {
        var access = await AccessAsync(caller, OrganizationAdminPermissions.OrganizationsManage, ct);
        return access.IsFailure
            ? Denied<OrganizationProfile>(access)
            : await _store.UpdateProfileAsync(access.Value!.OrganizationId, input, ifMatch, Actor(access.Value), traceId, ct);
    }
}
