using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.IdentityAccess.Administration;

namespace TanErp.Application.Organization.MasterData;

public class MasterDataOrganizationHandler
{
    private readonly IRequestAccessResolver _access;
    private readonly IMasterDataOrganizationStore _store;

    public MasterDataOrganizationHandler(IRequestAccessResolver access, IMasterDataOrganizationStore store)
    {
        _access = access;
        _store = store;
    }

    public async Task<Result<IReadOnlyList<DepartmentItem>>> ListDepartmentsAsync(
        AdminCaller caller, bool onlyActive, CancellationToken ct)
    {
        var access = await ResolveReadAccessAsync(caller, ct);
        if (access.IsFailure) return Result<IReadOnlyList<DepartmentItem>>.Failure(access.Error);

        var items = await _store.ListDepartmentsAsync(access.Value!.OrganizationId, onlyActive, ct);
        return Result<IReadOnlyList<DepartmentItem>>.Success(items);
    }

    public async Task<Result<DepartmentItem>> CreateDepartmentAsync(
        AdminCaller caller, CreateDepartmentInput input, CancellationToken ct)
    {
        var access = await ResolveManageAccessAsync(caller, ct);
        if (access.IsFailure) return Result<DepartmentItem>.Failure(access.Error);

        if (string.IsNullOrWhiteSpace(input.Code) || input.Code.Trim().Length > 50)
            return Result<DepartmentItem>.Failure(new Error("REQUEST_VALIDATION_FAILED", "Department code is required (max 50 chars)."));

        if (string.IsNullOrWhiteSpace(input.Name) || input.Name.Trim().Length > 255)
            return Result<DepartmentItem>.Failure(new Error("REQUEST_VALIDATION_FAILED", "Department name is required (max 255 chars)."));

        return await _store.CreateDepartmentAsync(access.Value!.OrganizationId, input, ct);
    }

    public async Task<Result<IReadOnlyList<JobTitleItem>>> ListJobTitlesAsync(
        AdminCaller caller, Guid? departmentId, bool onlyActive, CancellationToken ct)
    {
        var access = await ResolveReadAccessAsync(caller, ct);
        if (access.IsFailure) return Result<IReadOnlyList<JobTitleItem>>.Failure(access.Error);

        var items = await _store.ListJobTitlesAsync(access.Value!.OrganizationId, departmentId, onlyActive, ct);
        return Result<IReadOnlyList<JobTitleItem>>.Success(items);
    }

    public async Task<Result<JobTitleItem>> CreateJobTitleAsync(
        AdminCaller caller, CreateJobTitleInput input, CancellationToken ct)
    {
        var access = await ResolveManageAccessAsync(caller, ct);
        if (access.IsFailure) return Result<JobTitleItem>.Failure(access.Error);

        if (string.IsNullOrWhiteSpace(input.Code) || input.Code.Trim().Length > 50)
            return Result<JobTitleItem>.Failure(new Error("REQUEST_VALIDATION_FAILED", "Job title code is required (max 50 chars)."));

        if (string.IsNullOrWhiteSpace(input.Name) || input.Name.Trim().Length > 255)
            return Result<JobTitleItem>.Failure(new Error("REQUEST_VALIDATION_FAILED", "Job title name is required (max 255 chars)."));

        return await _store.CreateJobTitleAsync(access.Value!.OrganizationId, input, ct);
    }

    private async Task<Result<Common.Models.RequestAccessContext>> ResolveReadAccessAsync(AdminCaller caller, CancellationToken ct)
    {
        var access = await _access.ResolveAsync(caller.FirebaseUid, caller.MembershipId, "users.read", ct);
        if (access.IsSuccess) return access;
        return await _access.ResolveAsync(caller.FirebaseUid, caller.MembershipId, "organizations.read", ct);
    }

    private async Task<Result<Common.Models.RequestAccessContext>> ResolveManageAccessAsync(AdminCaller caller, CancellationToken ct)
    {
        var access = await _access.ResolveAsync(caller.FirebaseUid, caller.MembershipId, "users.manage", ct);
        if (access.IsSuccess) return access;
        return await _access.ResolveAsync(caller.FirebaseUid, caller.MembershipId, "organizations.manage", ct);
    }
}
