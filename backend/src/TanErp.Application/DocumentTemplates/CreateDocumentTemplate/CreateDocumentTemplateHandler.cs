using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Domain.Common;
using TanErp.Domain.DocumentTemplates;

namespace TanErp.Application.DocumentTemplates.CreateDocumentTemplate;

public class CreateDocumentTemplateHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IApplicationDbContext _db;
    private readonly IClock _clock;

    public CreateDocumentTemplateHandler(
        IRequestAccessResolver accessResolver,
        IApplicationDbContext db,
        IClock clock)
    {
        _accessResolver = accessResolver;
        _db = db;
        _clock = clock;
    }

    public async Task<Result<DocumentTemplateProjection>> HandleAsync(
        CreateDocumentTemplateCommand command,
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

        if (string.IsNullOrWhiteSpace(command.DocumentType))
        {
            return Result<DocumentTemplateProjection>.Failure(new Error("INVALID_DOCUMENT_TYPE", "Document type is required."));
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
        var normalizedDocType = command.DocumentType.Trim().ToLowerInvariant();
        var trimmedName = command.Name.Trim();

        var nameExists = await _db.DocumentTemplates
            .AnyAsync(t => t.OrganizationId == orgId &&
                           t.DocumentType == normalizedDocType &&
                           t.Name.ToLower() == trimmedName.ToLower() &&
                           t.IsActive, cancellationToken);

        if (nameExists)
        {
            return Result<DocumentTemplateProjection>.Failure(
                new Error("TEMPLATE_NAME_DUPLICATE", $"A template named '{trimmedName}' already exists for this document type."));
        }

        var now = _clock.UtcNow;

        // If setting as default, clear existing default for this scope
        if (command.IsDefault)
        {
            var existingDefaults = await _db.DocumentTemplates
                .Where(t => t.OrganizationId == orgId &&
                            t.DocumentType == normalizedDocType &&
                            t.BranchId == command.BranchId &&
                            t.IsDefault &&
                            t.IsActive)
                .ToListAsync(cancellationToken);

            foreach (var existing in existingDefaults)
            {
                existing.SetDefault(false, now);
            }
        }

        var template = new DocumentTemplate(
            Guid.NewGuid(),
            orgId,
            command.BranchId,
            normalizedDocType,
            trimmedName,
            command.IsDefault,
            command.LayoutConfigJson.Trim(),
            command.LogoFileId,
            now);

        _db.DocumentTemplates.Add(template);

        var auditEvent = new AuditEvent(
            Guid.NewGuid(),
            orgId,
            accessResult.Value!.ActorUserId,
            "document_template.created",
            "DocumentTemplate",
            template.Id.ToString(),
            now,
            string.Empty,
            JsonSerializer.Serialize(new
            {
                templateId = template.Id,
                documentType = normalizedDocType,
                name = trimmedName,
                isDefault = command.IsDefault,
                branchId = command.BranchId
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
