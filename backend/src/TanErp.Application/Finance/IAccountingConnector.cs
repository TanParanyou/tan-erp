namespace TanErp.Application.Finance;

/// <summary>
/// Boundary to the accounting system. No real system is wired in this round: the default implementation reports itself as not
/// configured, so nothing is ever claimed to be posted. The <see cref="AccountingEnvelope.DedupeKey"/> lets a real connector be idempotent.
/// </summary>
public interface IAccountingConnector
{
    Task<ConnectorResult> SendAsync(AccountingEnvelope envelope, CancellationToken ct = default);
}
