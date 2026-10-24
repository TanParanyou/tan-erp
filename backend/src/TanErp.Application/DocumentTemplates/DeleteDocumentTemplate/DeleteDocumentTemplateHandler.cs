using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Domain.Common;

namespace TanErp.Application.DocumentTemplates.DeleteDocumentTemplate;

public class DeleteDocumentTemplateHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IApplicationDbContext _db;
    private readonly IClock _clock;

    public DeleteDocumentTemplateHandler(
        IRequestAccessResolver accessResolver,
        IApplicationDbContext db,
        IClock clock)
    {
        _accessResolver = accessResolver;
        _db = db;
        _clock = clock;
    }

    public async Task<Result<bool>> HandleAsync(
        DeleteDocumentTemplateCommand command,
        CancellationToken cancellationToken = default)
    {
        var accessResult = await _accessResolver.ResolveAsync(
            command.FirebaseUid,
            command.MembershipId,
            "document-templates.manage",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return Result<bool>.Failure(accessResult.Error);
        }

        var orgId = accessResult.Value!.OrganizationId;
        var template = await _db.DocumentTemplates
            .FirstOrDefaultAsync(t => t.Id == command.TemplateId && t.OrganizationId == orgId && t.IsActive, cancellationToken);

        if (template == null)
        {
            return Result<bool>.Failure(
                new Error("DOCUMENT_TEMPLATE_NOT_FOUND", $"Document template with id {command.TemplateId} was not found."));
        }

        if (template.IsDefault)
        {
            return Result<bool>.Failure(
                new Error("CANNOT_DELETE_DEFAULT_TEMPLATE", "Cannot delete the default template. Please designate another template as default first."));
        }

        var now = _clock.UtcNow;
        template.Deactivate(now);

        var auditEvent = new AuditEvent(
            Guid.NewGuid(),
            orgId,
            accessResult.Value!.ActorUserId,
            "document_template.deleted",
            "DocumentTemplate",
            template.Id.ToString(),
            now,
            string.Empty,
            JsonSerializer.Serialize(new
            {
                templateId = template.Id,
                name = template.Name,
                documentType = template.DocumentType
            }));
        _db.AddAuditEvent(auditEvent);

        await _db.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
