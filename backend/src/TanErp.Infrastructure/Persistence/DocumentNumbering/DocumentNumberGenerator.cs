using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.DocumentNumbering;
using TanErp.Domain.DocumentNumbering;

namespace TanErp.Infrastructure.Persistence.DocumentNumbering;

public class DocumentNumberGenerator : IDocumentNumberGenerator
{
    private readonly AppDbContext _db;
    private readonly ISequenceCounter _sequenceCounter;
    private readonly IClock _clock;

    public DocumentNumberGenerator(
        AppDbContext db,
        ISequenceCounter sequenceCounter,
        IClock clock)
    {
        _db = db;
        _sequenceCounter = sequenceCounter;
        _clock = clock;
    }

    public async Task<string> GenerateAsync(
        Guid organizationId,
        string documentType,
        Guid? branchId = null,
        DateTimeOffset? timestamp = null,
        CancellationToken cancellationToken = default)
    {
        var now = timestamp ?? _clock.UtcNow;
        var normalizedDocType = documentType.Trim().ToLowerInvariant();

        var definition = await _db.DocumentSequenceDefinitions
            .AsNoTracking()
            .FirstOrDefaultAsync(
                d => d.OrganizationId == organizationId && d.DocumentType == normalizedDocType && d.IsActive,
                cancellationToken);

        string prefix;
        string formatPattern;
        ResetPeriod resetPeriod;
        int padding;
        bool isBranchSpecific;

        if (definition != null)
        {
            prefix = definition.Prefix;
            formatPattern = definition.FormatPattern;
            resetPeriod = definition.ResetPeriod;
            padding = definition.Padding;
            isBranchSpecific = definition.IsBranchSpecific;
        }
        else
        {
            if (normalizedDocType == DocumentTypes.Customers)
            {
                prefix = "CUS-";
                formatPattern = "{PREFIX}{SEQ:5}";
                resetPeriod = ResetPeriod.Never;
                padding = 5;
                isBranchSpecific = false;
            }
            else
            {
                prefix = normalizedDocType switch
                {
                    DocumentTypes.Estimates => "EST",
                    DocumentTypes.Surveys => "SRV",
                    DocumentTypes.Opportunities => "OPP",
                    DocumentTypes.Quotations => "QT",
                    _ => normalizedDocType.ToUpperInvariant().Substring(0, Math.Min(3, normalizedDocType.Length))
                };
                formatPattern = "{PREFIX}-{YYYY}-{SEQ:4}";
                resetPeriod = ResetPeriod.Yearly;
                padding = 4;
                isBranchSpecific = false;
            }
        }

        string? branchCode = null;
        if (branchId.HasValue && branchId.Value != Guid.Empty && formatPattern.Contains("{BRANCH}", StringComparison.OrdinalIgnoreCase))
        {
            var branch = await _db.Branches
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.OrganizationId == organizationId && b.Id == branchId.Value, cancellationToken);
            branchCode = branch?.Code;
        }

        var periodKey = DocumentNumberParser.ComputePeriodKey(resetPeriod, now);
        var counterBranchId = isBranchSpecific ? (branchId ?? Guid.Empty) : Guid.Empty;

        var seq = await _sequenceCounter.NextValueAsync(
            organizationId,
            normalizedDocType,
            counterBranchId,
            periodKey,
            cancellationToken);

        return DocumentNumberParser.Format(formatPattern, prefix, branchCode, now, seq, padding);
    }

    public string Preview(
        string formatPattern,
        string prefix,
        string? branchCode = null,
        DateTimeOffset? timestamp = null,
        long sampleSequence = 1,
        int defaultPadding = 4)
    {
        var now = timestamp ?? _clock.UtcNow;
        return DocumentNumberParser.Format(formatPattern, prefix, branchCode, now, sampleSequence, defaultPadding);
    }

    public string ComputePeriodKey(ResetPeriod resetPeriod, DateTimeOffset timestamp)
    {
        return DocumentNumberParser.ComputePeriodKey(resetPeriod, timestamp);
    }
}
