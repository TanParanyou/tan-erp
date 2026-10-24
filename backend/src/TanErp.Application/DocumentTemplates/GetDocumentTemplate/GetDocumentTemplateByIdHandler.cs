using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;

namespace TanErp.Application.DocumentTemplates.GetDocumentTemplate;

public class GetDocumentTemplateByIdHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IApplicationDbContext _db;

    public GetDocumentTemplateByIdHandler(
        IRequestAccessResolver accessResolver,
        IApplicationDbContext db)
    {
        _accessResolver = accessResolver;
        _db = db;
    }

    public async Task<Result<DocumentTemplateProjection>> HandleAsync(
        GetDocumentTemplateByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var accessResult = await _accessResolver.ResolveAsync(
            query.FirebaseUid,
            query.MembershipId,
            "document-templates.manage",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return Result<DocumentTemplateProjection>.Failure(accessResult.Error);
        }

        var orgId = accessResult.Value!.OrganizationId;
        var template = await _db.DocumentTemplates
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == query.TemplateId && t.OrganizationId == orgId && t.IsActive, cancellationToken);

        if (template == null)
        {
            return Result<DocumentTemplateProjection>.Failure(
                new Error("DOCUMENT_TEMPLATE_NOT_FOUND", $"Document template with id {query.TemplateId} was not found."));
        }

        return Result<DocumentTemplateProjection>.Success(new DocumentTemplateProjection(
            template.Id,
            template.OrganizationId,
            template.BranchId,
            template.DocumentType,
            template.Name,
            template.IsDefault,
            template.LayoutConfigJson,
            template.LogoFileId,
            template.IsActive,
            template.CreatedAtUtc,
            template.UpdatedAtUtc,
            template.RowVersion));
    }
}
