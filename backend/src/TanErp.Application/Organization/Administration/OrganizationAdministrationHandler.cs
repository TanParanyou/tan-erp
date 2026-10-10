using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Security;
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

    public async Task<Result<IReadOnlyList<BranchDetail>>> ListBranchesAsync(AdminCaller caller, BranchStatusFilter filter, CancellationToken ct)
    {
        var access = await AccessAsync(caller, OrganizationAdminPermissions.BranchesManage, ct);
        if (access.IsFailure) return Result<IReadOnlyList<BranchDetail>>.Failure(access.Error);
        return Result<IReadOnlyList<BranchDetail>>.Success(await _store.ListBranchesAsync(access.Value!.OrganizationId, filter, ct));
    }

    public async Task<Result<BranchDetail>> GetBranchAsync(AdminCaller caller, Guid branchId, CancellationToken ct)
    {
        var access = await AccessAsync(caller, OrganizationAdminPermissions.BranchesManage, ct);
        return access.IsFailure ? Denied<BranchDetail>(access) : await _store.GetBranchAsync(access.Value!.OrganizationId, branchId, ct);
    }

    public async Task<Result<BranchDetail>> CreateBranchAsync(
        AdminCaller caller, string idempotencyKey, CreateBranchInput input, string traceId, CancellationToken ct)
    {
        var access = await AccessAsync(caller, OrganizationAdminPermissions.BranchesManage, ct);
        if (access.IsFailure) return Denied<BranchDetail>(access);

        var d = input.Details;
        var payloadHash = Sha256Hex.Compute($"{input.Code}|{d.Name}|{d.NameEn}|{d.TaxBranchCode}|{d.AddressTh}|{d.AddressEn}|{d.Phone}");
        return await _store.CreateBranchAsync(
            access.Value!.OrganizationId, input, Actor(access.Value), Sha256Hex.Compute(idempotencyKey), payloadHash, traceId, ct);
    }
}
