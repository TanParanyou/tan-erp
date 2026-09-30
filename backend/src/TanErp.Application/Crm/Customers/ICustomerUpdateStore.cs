using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Crm.Customers.UpdateCustomer;

namespace TanErp.Application.Crm.Customers;

public interface ICustomerUpdateStore
{
    Task<Result<CustomerProjection>> UpdateAsync(RequestAccessContext access, UpdateCustomerCommand command, string keyHash, string payloadHash, string traceId, CancellationToken cancellationToken = default);
}
