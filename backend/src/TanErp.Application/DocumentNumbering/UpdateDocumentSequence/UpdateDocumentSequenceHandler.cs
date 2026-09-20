using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Domain.Common;
using TanErp.Domain.DocumentNumbering;

namespace TanErp.Application.DocumentNumbering.UpdateDocumentSequence;

public class UpdateDocumentSequenceHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IApplicationDbContext _db;
    private readonly IDocumentNumberGenerator _generator;
    private readonly IClock _clock;

    public UpdateDocumentSequenceHandler(
        IRequestAccessResolver accessResolver,
        IApplicationDbContext db,
        IDocumentNumberGenerator generator,
        IClock clock)
    {
        _accessResolver = accessResolver;
        _db = db;
        _generator = generator;
        _clock = clock;
    }

    public async Task<Result<DocumentSequenceProjection>> HandleAsync(
        UpdateDocumentSequenceCommand command,
        CancellationToken cancellationToken = default)
    {
        var accessResult = await _accessResolver.ResolveAsync(
            command.FirebaseUid,
            command.MembershipId,
            "document-sequences.manage",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return Result<DocumentSequenceProjection>.Failure(accessResult.Error);
        }

        if (string.IsNullOrWhiteSpace(command.DocumentType) || !DocumentTypes.IsValid(command.DocumentType))
        {
            return Result<DocumentSequenceProjection>.Failure(new Error("INVALID_DOCUMENT_TYPE", $"Invalid document type: {command.DocumentType}"));
        }

        if (string.IsNullOrWhiteSpace(command.FormatPattern))
        {
            return Result<DocumentSequenceProjection>.Failure(new Error("INVALID_FORMAT_PATTERN", "Format pattern cannot be blank."));
        }

        if (command.Padding < 1 || command.Padding > 10)
        {
            return Result<DocumentSequenceProjection>.Failure(new Error("INVALID_PADDING", "Padding must be between 1 and 10."));
        }

        if (!Enum.TryParse<ResetPeriod>(command.ResetPeriod, true, out var resetPeriod))
        {
            resetPeriod = ResetPeriod.Yearly;
        }

        var orgId = accessResult.Value!.OrganizationId;
        var normalizedType = command.DocumentType.Trim().ToLowerInvariant();
        var now = _clock.UtcNow;

        var def = await _db.DocumentSequenceDefinitions
            .FirstOrDefaultAsync(d => d.OrganizationId == orgId && d.DocumentType == normalizedType, cancellationToken);

        if (def == null)
        {
            def = new DocumentSequenceDefinition(
                Guid.NewGuid(),
                orgId,
                normalizedType,
                command.Prefix,
                command.FormatPattern,
                resetPeriod,
                command.Padding,
                command.IsBranchSpecific,
                now);
            _db.DocumentSequenceDefinitions.Add(def);
        }
        else
        {
            def.UpdateFormat(
                command.Prefix,
                command.FormatPattern,
                resetPeriod,
                command.Padding,
                command.IsBranchSpecific,
                now);
        }

        var auditEvent = new AuditEvent(
            Guid.NewGuid(),
            orgId,
            accessResult.Value!.ActorUserId,
            "document_sequence.updated",
            "DocumentSequenceDefinition",
            def.Id.ToString(),
            now,
            string.Empty,
            $"{{\"documentType\":\"{normalizedType}\",\"formatPattern\":\"{command.FormatPattern}\"}}");
        _db.AddAuditEvent(auditEvent);

        await _db.SaveChangesAsync(cancellationToken);

        var preview = _generator.Preview(def.FormatPattern, def.Prefix, "HQ", sampleSequence: 1, defaultPadding: def.Padding);
        return Result<DocumentSequenceProjection>.Success(new DocumentSequenceProjection(
            def.Id,
            def.DocumentType,
            def.Prefix,
            def.FormatPattern,
            def.ResetPeriod.ToString(),
            def.Padding,
            def.IsBranchSpecific,
            def.IsActive,
            preview));
    }
}
