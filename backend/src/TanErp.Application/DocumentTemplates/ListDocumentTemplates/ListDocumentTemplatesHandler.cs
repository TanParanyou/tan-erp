using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Domain.DocumentTemplates;

namespace TanErp.Application.DocumentTemplates.ListDocumentTemplates;

public class ListDocumentTemplatesHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IApplicationDbContext _db;
    private readonly IClock _clock;

    public ListDocumentTemplatesHandler(
        IRequestAccessResolver accessResolver,
        IApplicationDbContext db,
        IClock clock)
    {
        _accessResolver = accessResolver;
        _db = db;
        _clock = clock;
    }

    public async Task<Result<IReadOnlyList<DocumentTemplateProjection>>> HandleAsync(
        ListDocumentTemplatesQuery query,
        CancellationToken cancellationToken = default)
    {
        var accessResult = await _accessResolver.ResolveAsync(
            query.FirebaseUid,
            query.MembershipId,
            "document-templates.manage",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return Result<IReadOnlyList<DocumentTemplateProjection>>.Failure(accessResult.Error);
        }

        var orgId = accessResult.Value!.OrganizationId;
        var dbQuery = _db.DocumentTemplates
            .AsNoTracking()
            .Where(t => t.OrganizationId == orgId && t.IsActive);

        if (!string.IsNullOrWhiteSpace(query.DocumentType))
        {
            var normalizedType = query.DocumentType.Trim().ToLowerInvariant();
            dbQuery = dbQuery.Where(t => t.DocumentType == normalizedType);
        }

        if (query.BranchId.HasValue)
        {
            dbQuery = dbQuery.Where(t => t.BranchId == null || t.BranchId == query.BranchId.Value);
        }

        var list = await dbQuery
            .OrderByDescending(t => t.IsDefault)
            .ThenBy(t => t.Name)
            .ToListAsync(cancellationToken);

        var projections = list.Select(t => new DocumentTemplateProjection(
            t.Id,
            t.OrganizationId,
            t.BranchId,
            t.DocumentType,
            t.Name,
            t.IsDefault,
            t.LayoutConfigJson,
            t.LogoFileId,
            t.IsActive,
            t.CreatedAtUtc,
            t.UpdatedAtUtc,
            t.RowVersion)).ToList();

        // If a specific document type was requested and no custom template exists, return a synthesized default
        if (!string.IsNullOrWhiteSpace(query.DocumentType) && projections.Count == 0)
        {
            var normalizedType = query.DocumentType.Trim().ToLowerInvariant();
            var defaultConfig = DocumentTemplateLayoutConfig.DefaultFor(normalizedType);
            var jsonOptions = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            var defaultConfigJson = JsonSerializer.Serialize(defaultConfig, jsonOptions);
            var dummyId = Guid.Empty;
            var now = _clock.UtcNow;

            projections.Add(new DocumentTemplateProjection(
                dummyId,
                orgId,
                null,
                normalizedType,
                "มาตรฐานองค์กร (Default)",
                true,
                defaultConfigJson,
                null,
                true,
                now,
                now,
                DocumentTemplate.ComputeRowVersion(orgId, dummyId, now)));
        }

        return Result<IReadOnlyList<DocumentTemplateProjection>>.Success(projections);
    }
}
