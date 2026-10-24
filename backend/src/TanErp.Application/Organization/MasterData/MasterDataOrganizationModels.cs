using TanErp.Application.Common.Results;

namespace TanErp.Application.Organization.MasterData;

public sealed record DepartmentItem(Guid Id, string Code, string Name, string? NameEn, bool IsActive);
public sealed record CreateDepartmentInput(string Code, string Name, string? NameEn);

public sealed record JobTitleItem(Guid Id, Guid? DepartmentId, string Code, string Name, string? NameEn, bool IsActive);
public sealed record CreateJobTitleInput(string Code, string Name, Guid? DepartmentId, string? NameEn);

public interface IMasterDataOrganizationStore
{
    Task<IReadOnlyList<DepartmentItem>> ListDepartmentsAsync(Guid organizationId, bool onlyActive, CancellationToken ct);
    Task<Result<DepartmentItem>> CreateDepartmentAsync(Guid organizationId, CreateDepartmentInput input, CancellationToken ct);
    Task<IReadOnlyList<JobTitleItem>> ListJobTitlesAsync(Guid organizationId, Guid? departmentId, bool onlyActive, CancellationToken ct);
    Task<Result<JobTitleItem>> CreateJobTitleAsync(Guid organizationId, CreateJobTitleInput input, CancellationToken ct);
}
