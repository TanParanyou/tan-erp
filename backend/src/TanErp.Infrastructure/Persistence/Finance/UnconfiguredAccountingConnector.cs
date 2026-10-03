using TanErp.Application.Finance;

namespace TanErp.Infrastructure.Persistence.Finance;

/// <summary>
/// Placeholder until an accounting system is chosen. It never reports success, so no billing or payment is ever shown as posted
/// when nothing was actually sent; messages wait in the outbox and are retried once a real connector replaces this one.
/// </summary>
public class UnconfiguredAccountingConnector : IAccountingConnector
{
    public Task<ConnectorResult> SendAsync(AccountingEnvelope envelope, CancellationToken ct = default) =>
        Task.FromResult(new ConnectorResult(false, null, "CONNECTOR_NOT_CONFIGURED"));
}
