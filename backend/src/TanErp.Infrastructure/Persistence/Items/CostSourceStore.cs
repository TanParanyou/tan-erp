using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Items;
using TanErp.Domain.Common;
using TanErp.Domain.DocumentNumbering;
using TanErp.Domain.Items;
using TanErp.Infrastructure.Persistence.DocumentNumbering;

namespace TanErp.Infrastructure.Persistence.Items;

public sealed class CostSourceStore : ICostSourceStore
{
    private const string CreateOperation = "item-master.cost-source.create";
    private readonly AppDbContext _db;
    private readonly IClock _clock;
    private readonly IDocumentNumberGenerator _documentNumberGenerator;

    public CostSourceStore(AppDbContext db, IClock clock, IDocumentNumberGenerator documentNumberGenerator)
    {
        _db = db;
        _clock = clock;
        _documentNumberGenerator = documentNumberGenerator;
    }

    public async Task<IReadOnlyList<CostSourceProjection>> ListAsync(Guid organizationId, CancellationToken ct) =>
        await _db.CostSources.AsNoTracking().Where(x => x.OrganizationId == organizationId)
            .OrderBy(x => x.Priority).ThenBy(x => x.Code).ThenBy(x => x.Id)
            .Select(x => new CostSourceProjection(x.Id, x.Code, x.Name, x.SourceType, x.Priority, x.IsActive, x.RowVersion))
            .ToListAsync(ct);

    public async Task<CostSourceProjection?> GetAsync(Guid organizationId, Guid id, CancellationToken ct) =>
        await _db.CostSources.AsNoTracking().Where(x => x.OrganizationId == organizationId && x.Id == id)
            .Select(x => new CostSourceProjection(x.Id, x.Code, x.Name, x.SourceType, x.Priority, x.IsActive, x.RowVersion))
            .FirstOrDefaultAsync(ct);

    public async Task<Result<CostSourceProjection>> CreateAsync(CreateCostSourceData data, RequestAccessContext access, string idempotencyKey, string traceId, CancellationToken ct)
    {
        var payloadHash = Hash($"{data.Code?.Trim().ToUpperInvariant()}|{data.Name.Thai}|{data.Name.English}");
        await using var tx = await _db.Database.BeginTransactionAsync(ct);
        var replay = await _db.IdempotencyRecords.FirstOrDefaultAsync(x => x.OrganizationId == access.OrganizationId && x.Operation == CreateOperation && x.KeyHash == Hash(idempotencyKey), ct);
        if (replay is not null)
        {
            if (replay.PayloadHash != payloadHash) return Fail("IDEMPOTENCY_KEY_REUSED");
            var replayed = Guid.TryParse(replay.ResourceId, out var replayId) ? await GetAsync(access.OrganizationId, replayId, ct) : null;
            return replayed is null ? Fail("RESOURCE_NOT_FOUND") : Result<CostSourceProjection>.Success(replayed);
        }
        var resolvedCode = await MasterDataCodeAllocator.ResolveAsync(
            _documentNumberGenerator,
            access.OrganizationId,
            DocumentTypes.CostSources,
            data.Code,
            32,
            "COST_SOURCE_CODE_DUPLICATE",
            async (code, token) => await _db.CostSources.AnyAsync(x => x.OrganizationId == access.OrganizationId && x.Code == code.Trim().ToUpperInvariant(), token),
            ct);
        if (resolvedCode.IsFailure) return Result<CostSourceProjection>.Failure(resolvedCode.Error);
        var code = resolvedCode.Value!.Trim().ToUpperInvariant();
        var priority = (await _db.CostSources.Where(x => x.OrganizationId == access.OrganizationId).Select(x => (int?)x.Priority).MaxAsync(ct) ?? 0) + 1;
        var now = _clock.UtcNow;
        var source = new CostSource(Guid.NewGuid(), access.OrganizationId, code, data.Name, priority, true, now);
        _db.CostSources.Add(source);
        _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), access.OrganizationId, CreateOperation, Hash(idempotencyKey), payloadHash, source.Id.ToString(), now));
        _db.AddAuditEvent(Audit(source, access, "cost-source.created", traceId, null, data.Code is null));
        try { await _db.SaveChangesAsync(ct); await tx.CommitAsync(ct); }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException postgresException &&
            postgresException.SqlState == PostgresErrorCodes.UniqueViolation &&
            postgresException.ConstraintName == "IX_cost_sources_organization_id_code")
        {
            return Fail("COST_SOURCE_CODE_DUPLICATE");
        }
        return Result<CostSourceProjection>.Success(Project(source));
    }

    public async Task<Result<CostSourceProjection>> UpdateAsync(UpdateCostSourceData data, RequestAccessContext access, string traceId, CancellationToken ct)
    {
        var source = await _db.CostSources.FirstOrDefaultAsync(x => x.OrganizationId == access.OrganizationId && x.Id == data.Id, ct);
        if (source is null) return Fail("RESOURCE_NOT_FOUND");
        var normalizedCode = data.Code.Trim().ToUpperInvariant();
        if (!string.Equals(source.Code, normalizedCode, StringComparison.Ordinal) &&
            await _db.CostRecords.AnyAsync(record => record.OrganizationId == access.OrganizationId && record.CostSourceId == source.Id, ct))
            return Fail("COST_SOURCE_CODE_IMMUTABLE");
        var before = source.RowVersion;
        if (!source.Update(data.Code, data.Name, source.Priority, data.ExpectedRowVersion, _clock.UtcNow)) return Fail("COST_SOURCE_VERSION_CONFLICT");
        if (await _db.CostSources.AnyAsync(x => x.OrganizationId == access.OrganizationId && x.Id != source.Id && x.Code == source.Code, ct)) return Fail("COST_SOURCE_CODE_DUPLICATE");
        _db.AddAuditEvent(Audit(source, access, "cost-source.updated", traceId, before));
        try { await _db.SaveChangesAsync(ct); } catch (DbUpdateConcurrencyException) { return Fail("COST_SOURCE_VERSION_CONFLICT"); }
        return Result<CostSourceProjection>.Success(Project(source));
    }

    public async Task<Result<CostSourceProjection>> DeactivateAsync(Guid id, Guid rowVersion, RequestAccessContext access, string traceId, CancellationToken ct)
    {
        var source = await _db.CostSources.FirstOrDefaultAsync(x => x.OrganizationId == access.OrganizationId && x.Id == id, ct);
        if (source is null) return Fail("RESOURCE_NOT_FOUND");
        var before = source.RowVersion;
        if (!source.Deactivate(rowVersion, _clock.UtcNow)) return Fail("COST_SOURCE_VERSION_CONFLICT");
        if (before != source.RowVersion) _db.AddAuditEvent(Audit(source, access, "cost-source.deactivated", traceId, before));
        try { await _db.SaveChangesAsync(ct); } catch (DbUpdateConcurrencyException) { return Fail("COST_SOURCE_VERSION_CONFLICT"); }
        return Result<CostSourceProjection>.Success(Project(source));
    }

    private AuditEvent Audit(CostSource source, RequestAccessContext access, string action, string traceId, Guid? before, bool? codeGenerated = null) =>
        new(Guid.NewGuid(), access.OrganizationId, access.ActorUserId, action, "CostSource", source.Id.ToString(), _clock.UtcNow, traceId,
            JsonSerializer.Serialize(new { source.Code, source.SourceType, source.IsActive, codeGenerated }), access.BranchId, access.MembershipId, rowVersionBefore: before, rowVersionAfter: source.RowVersion);
    private static CostSourceProjection Project(CostSource x) => new(x.Id, x.Code, x.Name, x.SourceType, x.Priority, x.IsActive, x.RowVersion);
    private static string Hash(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();
    private static Result<CostSourceProjection> Fail(string code) => Result<CostSourceProjection>.Failure(new Error(code, code));
}
