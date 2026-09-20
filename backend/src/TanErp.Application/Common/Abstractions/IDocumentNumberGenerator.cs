using TanErp.Domain.DocumentNumbering;

namespace TanErp.Application.Common.Abstractions;

public interface IDocumentNumberGenerator
{
    Task<string> GenerateAsync(
        Guid organizationId,
        string documentType,
        Guid? branchId = null,
        DateTimeOffset? timestamp = null,
        CancellationToken cancellationToken = default);

    string Preview(
        string formatPattern,
        string prefix,
        string? branchCode = null,
        DateTimeOffset? timestamp = null,
        long sampleSequence = 1,
        int defaultPadding = 4);

    string ComputePeriodKey(ResetPeriod resetPeriod, DateTimeOffset timestamp);
}
