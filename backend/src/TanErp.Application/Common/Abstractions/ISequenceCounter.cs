namespace TanErp.Application.Common.Abstractions;

public interface ISequenceCounter
{
    Task<long> NextValueAsync(
        Guid organizationId,
        string documentType,
        Guid branchId,
        string periodKey,
        CancellationToken cancellationToken = default);
}
