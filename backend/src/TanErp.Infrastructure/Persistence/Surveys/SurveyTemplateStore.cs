using Microsoft.EntityFrameworkCore;
using Npgsql;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Surveys;
using TanErp.Domain.Surveys;

namespace TanErp.Infrastructure.Persistence.Surveys;

public class SurveyTemplateStore : ISurveyTemplateStore
{
    private readonly AppDbContext _db;
    private readonly IClock _clock;

    public SurveyTemplateStore(AppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<IReadOnlyList<SurveyTemplateVersion>> ListAsync(CancellationToken ct)
    {
        var rows = await _db.SurveyTemplateVersions.AsNoTracking().OrderBy(t => t.EffectiveFromUtc).ThenBy(t => t.Code).ToListAsync(ct);
        return rows.Select(r => r.ToTemplate()).ToList();
    }

    public async Task<Result<SurveyTemplateVersion>> PublishAsync(PublishSurveyTemplateCommand command, Guid actorUserId, CancellationToken ct)
    {
        var now = _clock.UtcNow;
        SurveyTemplateVersionRow row;
        try
        {
            row = new SurveyTemplateVersionRow(
                Guid.NewGuid(), command.Code, command.RequiredChecklistItems, command.MinimumEvidenceCount,
                command.MinimumAreaEvidenceCount, command.SnapshotHashVersion, command.EffectiveFromUtc ?? now, now, actorUserId);
        }
        catch (SurveyTemplateInvalidException ex)
        {
            return Result<SurveyTemplateVersion>.Failure(new Error("SURVEY_TEMPLATE_INVALID", ex.Message));
        }

        if (await _db.SurveyTemplateVersions.AnyAsync(t => t.Code == row.Code, ct))
        {
            return Result<SurveyTemplateVersion>.Failure(new Error("SURVEY_TEMPLATE_CODE_EXISTS", $"Survey template '{row.Code}' already exists."));
        }

        _db.SurveyTemplateVersions.Add(row);
        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            return Result<SurveyTemplateVersion>.Failure(new Error("SURVEY_TEMPLATE_CODE_EXISTS", $"Survey template '{row.Code}' already exists."));
        }

        return Result<SurveyTemplateVersion>.Success(row.ToTemplate());
    }

    public async Task<Result<SurveyTemplateVersion>> RetireAsync(string code, CancellationToken ct)
    {
        var row = await _db.SurveyTemplateVersions.FirstOrDefaultAsync(t => t.Code == code, ct);
        if (row is null) return Result<SurveyTemplateVersion>.Failure(new Error("RESOURCE_NOT_FOUND", "Survey template not found."));

        try
        {
            row.Retire(_clock.UtcNow);
        }
        catch (SurveyTemplateInvalidException ex)
        {
            return Result<SurveyTemplateVersion>.Failure(new Error("SURVEY_TEMPLATE_INVALID", ex.Message));
        }

        await _db.SaveChangesAsync(ct);
        return Result<SurveyTemplateVersion>.Success(row.ToTemplate());
    }
}
