using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Domain.DocumentNumbering;

namespace TanErp.Application.DocumentNumbering.ListDocumentSequences;

public class ListDocumentSequencesHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IApplicationDbContext _db;
    private readonly IDocumentNumberGenerator _generator;

    public ListDocumentSequencesHandler(
        IRequestAccessResolver accessResolver,
        IApplicationDbContext db,
        IDocumentNumberGenerator generator)
    {
        _accessResolver = accessResolver;
        _db = db;
        _generator = generator;
    }

    public async Task<Result<IReadOnlyList<DocumentSequenceProjection>>> HandleAsync(
        ListDocumentSequencesQuery query,
        CancellationToken cancellationToken = default)
    {
        var accessResult = await _accessResolver.ResolveAsync(
            query.FirebaseUid,
            query.MembershipId,
            "document-sequences.read",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return Result<IReadOnlyList<DocumentSequenceProjection>>.Failure(accessResult.Error);
        }

        var orgId = accessResult.Value!.OrganizationId;
        var existing = await _db.DocumentSequenceDefinitions
            .AsNoTracking()
            .Where(d => d.OrganizationId == orgId)
            .ToListAsync(cancellationToken);

        var existingMap = existing.ToDictionary(d => d.DocumentType.ToLowerInvariant());
        var results = new List<DocumentSequenceProjection>();

        foreach (var docType in DocumentTypes.All)
        {
            if (existingMap.TryGetValue(docType.ToLowerInvariant(), out var def))
            {
                var preview = _generator.Preview(def.FormatPattern, def.Prefix, "HQ", sampleSequence: 1, defaultPadding: def.Padding);
                results.Add(new DocumentSequenceProjection(
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
            else
            {
                string defaultPrefix;
                string defaultPattern;
                ResetPeriod defaultResetPeriod;
                int defaultPadding;

                if (docType == DocumentTypes.Customers)
                {
                    defaultPrefix = "CUS-";
                    defaultPattern = "{PREFIX}{SEQ:5}";
                    defaultResetPeriod = ResetPeriod.Never;
                    defaultPadding = 5;
                }
                else
                {
                    defaultPrefix = docType switch
                    {
                        DocumentTypes.Estimates => "EST",
                        DocumentTypes.Surveys => "SRV",
                        DocumentTypes.Opportunities => "OPP",
                        DocumentTypes.Quotations => "QT",
                        _ => docType.ToUpperInvariant().Substring(0, Math.Min(3, docType.Length))
                    };
                    defaultPattern = "{PREFIX}-{YYYY}-{SEQ:4}";
                    defaultResetPeriod = ResetPeriod.Yearly;
                    defaultPadding = 4;
                }

                var preview = _generator.Preview(defaultPattern, defaultPrefix, "HQ", sampleSequence: 1, defaultPadding: defaultPadding);
                var defaultRowVersion = DocumentSequenceDefinition.ComputeDefaultRowVersion(orgId, docType);

                results.Add(new DocumentSequenceProjection(
                    null,
                    docType,
                    defaultPrefix,
                    defaultPattern,
                    defaultResetPeriod.ToString(),
                    defaultPadding,
                    false,
                    true,
                    preview,
                    defaultRowVersion));
            }
        }

        return Result<IReadOnlyList<DocumentSequenceProjection>>.Success(results);
    }
}
