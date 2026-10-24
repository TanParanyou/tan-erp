namespace TanErp.Api.Contracts.Organization;

public record DepartmentResponse(Guid Id, string Code, string Name, string? NameEn, bool IsActive);
public record CreateDepartmentRequest(string Code, string Name, string? NameEn = null);

public record JobTitleResponse(Guid Id, Guid? DepartmentId, string Code, string Name, string? NameEn, bool IsActive);
public record CreateJobTitleRequest(string Code, string Name, Guid? DepartmentId = null, string? NameEn = null);
