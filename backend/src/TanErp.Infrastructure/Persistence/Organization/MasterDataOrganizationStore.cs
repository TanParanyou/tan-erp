using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Results;
using TanErp.Application.Organization.MasterData;
using TanErp.Domain.Common;
using TanErp.Domain.Organization;

namespace TanErp.Infrastructure.Persistence.OrgMasterData;

public class MasterDataOrganizationStore : IMasterDataOrganizationStore
{
    private readonly AppDbContext _db;

    public MasterDataOrganizationStore(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<DepartmentItem>> ListDepartmentsAsync(
        Guid organizationId, bool onlyActive, CancellationToken ct)
    {
        var query = _db.Departments.AsNoTracking()
            .Where(d => d.OrganizationId == organizationId);

        if (onlyActive)
        {
            query = query.Where(d => d.IsActive);
        }

        return await query
            .OrderBy(d => d.Code)
            .Select(d => new DepartmentItem(d.Id, d.Code, d.Name, d.NameEn, d.IsActive))
            .ToListAsync(ct);
    }

    public async Task<Result<DepartmentItem>> CreateDepartmentAsync(
        Guid organizationId, CreateDepartmentInput input, CancellationToken ct)
    {
        var code = input.Code.Trim().ToUpperInvariant();
        var exists = await _db.Departments.AsNoTracking()
            .AnyAsync(d => d.OrganizationId == organizationId && d.Code == code, ct);

        if (exists)
        {
            return Result<DepartmentItem>.Failure(new Error("DEPARTMENT_CODE_ALREADY_EXISTS", "Department code already exists in this organization."));
        }

        var department = new Department(Guid.NewGuid(), organizationId, code, input.Name, input.NameEn);
        _db.Departments.Add(department);
        await _db.SaveChangesAsync(ct);

        return Result<DepartmentItem>.Success(new DepartmentItem(department.Id, department.Code, department.Name, department.NameEn, department.IsActive));
    }

    public async Task<IReadOnlyList<JobTitleItem>> ListJobTitlesAsync(
        Guid organizationId, Guid? departmentId, bool onlyActive, CancellationToken ct)
    {
        var query = _db.JobTitles.AsNoTracking()
            .Where(j => j.OrganizationId == organizationId);

        if (onlyActive)
        {
            query = query.Where(j => j.IsActive);
        }

        if (departmentId.HasValue)
        {
            query = query.Where(j => j.DepartmentId == departmentId || j.DepartmentId == null);
        }

        return await query
            .OrderBy(j => j.Code)
            .Select(j => new JobTitleItem(j.Id, j.DepartmentId, j.Code, j.Name, j.NameEn, j.IsActive))
            .ToListAsync(ct);
    }

    public async Task<Result<JobTitleItem>> CreateJobTitleAsync(
        Guid organizationId, CreateJobTitleInput input, CancellationToken ct)
    {
        var code = input.Code.Trim().ToUpperInvariant();
        var exists = await _db.JobTitles.AsNoTracking()
            .AnyAsync(j => j.OrganizationId == organizationId && j.Code == code, ct);

        if (exists)
        {
            return Result<JobTitleItem>.Failure(new Error("JOB_TITLE_CODE_ALREADY_EXISTS", "Job title code already exists in this organization."));
        }

        if (input.DepartmentId.HasValue)
        {
            var deptExists = await _db.Departments.AsNoTracking()
                .AnyAsync(d => d.OrganizationId == organizationId && d.Id == input.DepartmentId.Value, ct);
            if (!deptExists)
            {
                return Result<JobTitleItem>.Failure(new Error("DEPARTMENT_NOT_FOUND", "Specified department does not exist."));
            }
        }

        var jobTitle = new JobTitle(Guid.NewGuid(), organizationId, code, input.Name, input.DepartmentId, input.NameEn);
        _db.JobTitles.Add(jobTitle);
        await _db.SaveChangesAsync(ct);

        return Result<JobTitleItem>.Success(new JobTitleItem(jobTitle.Id, jobTitle.DepartmentId, jobTitle.Code, jobTitle.Name, jobTitle.NameEn, jobTitle.IsActive));
    }
}
