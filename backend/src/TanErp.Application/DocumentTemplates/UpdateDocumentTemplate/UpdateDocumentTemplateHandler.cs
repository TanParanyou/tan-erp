using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Domain.Common;

namespace TanErp.Application.DocumentTemplates.UpdateDocumentTemplate;

public class UpdateDocumentTemplateHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IApplicationDbContext _db;
    private readonly IClock _clock;

    public UpdateDocumentTemplateHandler(
        IRequestAccessResolver accessResolver,
        IApplicationDbContext db,
        IClock clock)
    {
        _accessResolver = accessResolver;
        _db = db;
        _clock = clock;
    }

    public async Task<Result<DocumentTemplateProjection>> HandleAsync(
        UpdateDocumentTemplateCommand command,
        CancellationToken cancellationToken = default)
    {
        var accessResult = await _accessResolver.ResolveAsync(
            command.FirebaseUid,
            command.MembershipId,
            "document-templates.manage",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return Result<DocumentTemplateProjection>.Failure(accessResult.Error);
        }

        if (string.IsNullOrWhiteSpace(command.Name))
        {
            return Result<DocumentTemplateProjection>.Failure(new Error("INVALID_TEMPLATE_NAME", "Template name cannot be empty."));
        }

        if (string.IsNullOrWhiteSpace(command.LayoutConfigJson))
        {
            return Result<DocumentTemplateProjection>.Failure(new Error("INVALID_LAYOUT_CONFIG", "Layout config JSON cannot be empty."));
        }

        var orgId = accessResult.Value!.OrganizationId;
        var template = await _db.DocumentTemplates
            .FirstOrDefaultAsync(t => t.Id == command.TemplateId && t.OrganizationId == orgId && t.IsActive, cancellationToken);

        if (template == null)
        {
            return Result<DocumentTemplateProjection>.Failure(
                new Error("DOCUMENT_TEMPLATE_NOT_FOUND", $"Document template with id {command.TemplateId} was not found."));
        }

        if (command.IfMatchRowVersion != template.RowVersion)
        {
            return Result<DocumentTemplateProjection>.Failure(
                new Error("DOCUMENT_TEMPLATE_VERSION_CONFLICT", "The document template was modified by another user. Please refresh and try again."));
        }

        var trimmedName = command.Name.Trim();
        if (!string.Equals(template.Name, trimmedName, StringComparison.OrdinalIgnoreCase))
        {
            var nameExists = await _db.DocumentTemplates
                .AnyAsync(t => t.OrganizationId == orgId &&
                               t.DocumentType == template.DocumentType &&
                               t.Id != template.Id &&
                               t.Name.ToLower() == trimmedName.ToLower() &&
                               t.IsActive, cancellationToken);

            if (nameExists)
            {
                return Result<DocumentTemplateProjection>.Failure(
                    new Error("TEMPLATE_NAME_DUPLICATE", $"A template named '{trimmedName}' already exists for this document type."));
            }
        }

        var now = _clock.UtcNow;
        if (now <= template.UpdatedAtUtc)
        {
            now = template.UpdatedAtUtc.AddMilliseconds(1);
        }

        if (command.IsDefault && !template.IsDefault)
        {
            var existingDefaults = await _db.DocumentTemplates
                .Where(t => t.OrganizationId == orgId &&
                            t.DocumentType == template.DocumentType &&
                            t.BranchId == template.BranchId &&
                            t.Id != template.Id &&
                            t.IsDefault &&
                            t.IsActive)
                .ToListAsync(cancellationToken);

            foreach (var existing in existingDefaults)
            {
                existing.SetDefault(false, now);
            }
        }

        template.Update(
            trimmedName,
            command.IsDefault,
            command.LayoutConfigJson.Trim(),
            command.LogoFileId,
            now);

        var auditEvent = new AuditEvent(
            Guid.NewGuid(),
            orgId,
            accessResult.Value!.ActorUserId,
            "document_template.updated",
            "DocumentTemplate",
            template.Id.ToString(),
            now,
            string.Empty,
            JsonSerializer.Serialize(new
            {
                templateId = template.Id,
                name = trimmedName,
                isDefault = command.IsDefault
            }));
        _db.AddAuditEvent(auditEvent);

        await _db.SaveChangesAsync(cancellationToken);

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
