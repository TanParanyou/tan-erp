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

        var patternValidationResult = ValidateFormatPattern(command.FormatPattern);
        if (patternValidationResult.IsFailure)
        {
            return Result<DocumentSequenceProjection>.Failure(patternValidationResult.Error);
        }

        if (command.Padding < 1 || command.Padding > 10)
        {
            return Result<DocumentSequenceProjection>.Failure(new Error("INVALID_PADDING", "Padding must be between 1 and 10."));
        }

        if (string.IsNullOrWhiteSpace(command.ResetPeriod) ||
            !Enum.TryParse<ResetPeriod>(command.ResetPeriod, true, out var resetPeriod) ||
            !Enum.IsDefined(typeof(ResetPeriod), resetPeriod))
        {
            return Result<DocumentSequenceProjection>.Failure(new Error("INVALID_RESET_PERIOD", $"Invalid reset period: {command.ResetPeriod}"));
        }

        var orgId = accessResult.Value!.OrganizationId;
        var normalizedType = command.DocumentType.Trim().ToLowerInvariant();

        var def = await _db.DocumentSequenceDefinitions
            .FirstOrDefaultAsync(d => d.OrganizationId == orgId && d.DocumentType == normalizedType, cancellationToken);

        var currentVersion = def != null
            ? def.RowVersion
            : DocumentSequenceDefinition.ComputeDefaultRowVersion(orgId, normalizedType);

        if (command.ExpectedVersion != currentVersion)
        {
            return Result<DocumentSequenceProjection>.Failure(
                new Error("DOCUMENT_SEQUENCE_VERSION_CONFLICT", "The document sequence configuration was modified by another user."));
        }

        var rawNow = _clock.UtcNow;
        var now = new DateTimeOffset((rawNow.Ticks / 10) * 10, rawNow.Offset);
        if (def != null && now <= def.UpdatedAtUtc)
        {
            now = def.UpdatedAtUtc.AddMilliseconds(1);
        }

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

        var auditPayload = System.Text.Json.JsonSerializer.Serialize(new
        {
            documentType = normalizedType,
            prefix = command.Prefix,
            formatPattern = command.FormatPattern,
            resetPeriod = resetPeriod.ToString(),
            padding = command.Padding,
            isBranchSpecific = command.IsBranchSpecific
        });

        var auditEvent = new AuditEvent(
            Guid.NewGuid(),
            orgId,
            accessResult.Value!.ActorUserId,
            "document_sequence.updated",
            "DocumentSequenceDefinition",
            def.Id.ToString(),
            now,
            string.Empty,
            auditPayload);
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
            preview,
            def.RowVersion));
    }

    public static Result<bool> ValidateFormatPattern(string? formatPattern)
    {
        if (string.IsNullOrWhiteSpace(formatPattern))
        {
            return Result<bool>.Failure(new Error("INVALID_FORMAT_PATTERN", "Format pattern cannot be blank."));
        }

        var trimmed = formatPattern.Trim();
        var matches = System.Text.RegularExpressions.Regex.Matches(trimmed, @"\{[^{}]*\}");
        if (matches.Count == 0)
        {
            return Result<bool>.Failure(new Error("INVALID_FORMAT_PATTERN", "Format pattern must contain {SEQ} exactly once."));
        }

        int seqCount = 0;
        foreach (System.Text.RegularExpressions.Match m in matches)
        {
            var token = m.Value;
            if (System.Text.RegularExpressions.Regex.IsMatch(token, @"^\{SEQ(:\d+)?\}$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
            {
                if (token.Contains(':'))
                {
                    var numPart = token.Substring(5, token.Length - 6);
                    if (!int.TryParse(numPart, out var pad) || pad < 1 || pad > 10)
                    {
                        return Result<bool>.Failure(new Error("INVALID_FORMAT_PATTERN", $"Invalid padding in sequence token: {token}"));
                    }
                }
                seqCount++;
            }
            else if (token.Equals("{PREFIX}", StringComparison.OrdinalIgnoreCase) ||
                     token.Equals("{BRANCH}", StringComparison.OrdinalIgnoreCase) ||
                     token.Equals("{YYYY}", StringComparison.OrdinalIgnoreCase) ||
                     token.Equals("{YY}", StringComparison.OrdinalIgnoreCase) ||
                     token.Equals("{BBBB}", StringComparison.OrdinalIgnoreCase) ||
                     token.Equals("{BB}", StringComparison.OrdinalIgnoreCase) ||
                     token.Equals("{MM}", StringComparison.OrdinalIgnoreCase) ||
                     token.Equals("{DD}", StringComparison.OrdinalIgnoreCase))
            {
                // Allowed token
            }
            else
            {
                return Result<bool>.Failure(new Error("INVALID_FORMAT_PATTERN", $"Format pattern contains unsupported token: {token}"));
            }
        }

        if (seqCount != 1)
        {
            return Result<bool>.Failure(new Error("INVALID_FORMAT_PATTERN", "Format pattern must contain {SEQ} exactly once."));
        }

        return Result<bool>.Success(true);
    }
}
