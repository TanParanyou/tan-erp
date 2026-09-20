using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;

namespace TanErp.Application.DocumentNumbering.PreviewDocumentSequence;

public class PreviewDocumentSequenceHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IDocumentNumberGenerator _generator;

    public PreviewDocumentSequenceHandler(
        IRequestAccessResolver accessResolver,
        IDocumentNumberGenerator generator)
    {
        _accessResolver = accessResolver;
        _generator = generator;
    }

    public async Task<Result<string>> HandleAsync(
        PreviewDocumentSequenceQuery query,
        CancellationToken cancellationToken = default)
    {
        var accessResult = await _accessResolver.ResolveAsync(
            query.FirebaseUid,
            query.MembershipId,
            "document-sequences.read",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return Result<string>.Failure(accessResult.Error);
        }

        var preview = _generator.Preview(
            query.FormatPattern,
            query.Prefix,
            query.BranchCode,
            sampleSequence: query.SampleSequence <= 0 ? 1 : query.SampleSequence,
            defaultPadding: query.Padding <= 0 ? 4 : query.Padding);

        return Result<string>.Success(preview);
    }
}
