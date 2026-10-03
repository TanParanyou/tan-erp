using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;

namespace TanErp.Application.IdentityAccess.Administration.GetUser;

public class GetAdminUserHandler
{
    private readonly IRequestAccessResolver _access;
    private readonly IIdentityAdministrationStore _store;

    public GetAdminUserHandler(IRequestAccessResolver access, IIdentityAdministrationStore store)
    {
        _access = access;
        _store = store;
    }

    public async Task<Result<AdminUser>> Handle(GetAdminUserQuery query, CancellationToken cancellationToken = default)
    {
        var access = await _access.ResolveAsync(query.Caller.FirebaseUid, query.Caller.MembershipId, AdministrationPermissions.UsersRead, cancellationToken);
        if (access.IsFailure) return Result<AdminUser>.Failure(access.Error);

        var user = await _store.GetUserAsync(access.Value!.OrganizationId, query.UserId, cancellationToken);
        return user is null ? AdminHandlerSupport.NotFound<AdminUser>("User") : Result<AdminUser>.Success(user);
    }
}
