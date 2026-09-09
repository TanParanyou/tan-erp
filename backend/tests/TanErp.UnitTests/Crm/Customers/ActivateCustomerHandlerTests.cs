using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Crm.Customers;
using TanErp.Application.Crm.Customers.ActivateCustomer;
using TanErp.Domain.Crm.Customers;
using Xunit;

namespace TanErp.UnitTests.Crm.Customers;

public class ActivateCustomerHandlerTests
{
    private class FakeRequestAccessResolver : IRequestAccessResolver
    {
        public HashSet<string> GrantedPermissions { get; } = new(StringComparer.Ordinal);
        public Guid UserId { get; set; } = Guid.NewGuid();
        public Guid OrgId { get; set; } = Guid.NewGuid();
        public Guid? BranchId { get; set; } = Guid.NewGuid();

        public Task<Result<RequestAccessContext>> ResolveAsync(
            string firebaseUid,
            Guid membershipId,
            string permissionKey,
            CancellationToken cancellationToken = default)
        {
            if (GrantedPermissions.Contains(permissionKey))
            {
                return Task.FromResult(Result<RequestAccessContext>.Success(
                    new RequestAccessContext(UserId, membershipId, OrgId, BranchId, permissionKey, "organization")));
            }

            return Task.FromResult(Result<RequestAccessContext>.Failure(
                new Error("PERMISSION_DENIED", "Permission denied.")));
        }
    }

    private class FakeCustomerLifecycleStore : ICustomerLifecycleStore
    {
        public RequestAccessContext? LastAccess { get; private set; }
        public Guid LastCustomerId { get; private set; }
        public Guid LastExpectedRowVersion { get; private set; }
        public string? LastKeyHash { get; private set; }
        public string? LastPayloadHash { get; private set; }
        public Result<CustomerProjection> ResultToReturn { get; set; } = Result<CustomerProjection>.Failure(new Error("TEST", "Not set"));

        public Task<Result<CustomerProjection>> ActivateAsync(
            RequestAccessContext access,
            Guid customerId,
            Guid expectedRowVersion,
            string keyHash,
            string payloadHash,
            string traceId,
            CancellationToken cancellationToken = default)
        {
            LastAccess = access;
            LastCustomerId = customerId;
            LastExpectedRowVersion = expectedRowVersion;
            LastKeyHash = keyHash;
            LastPayloadHash = payloadHash;
            return Task.FromResult(ResultToReturn);
        }
    }

    [Fact]
    public async Task Handle_WhenMissingPermission_ReturnsPermissionDeniedWithoutCallingStore()
    {
        var resolver = new FakeRequestAccessResolver();
        var store = new FakeCustomerLifecycleStore();
        var handler = new ActivateCustomerHandler(resolver, store);

        var command = new ActivateCustomerCommand(
            "uid-1", Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "key-1234567890123456", "trace-1");

        var result = await handler.Handle(command);

        Assert.True(result.IsFailure);
        Assert.Equal("PERMISSION_DENIED", result.Error.Code);
        Assert.Null(store.LastAccess);
    }

    [Fact]
    public async Task Handle_WhenAuthorized_ComputesHashesAndDelegatesToStore()
    {
        var resolver = new FakeRequestAccessResolver();
        resolver.GrantedPermissions.Add("customers.activate");
        var store = new FakeCustomerLifecycleStore();

        var customerId = Guid.NewGuid();
        var expectedRowVersion = Guid.NewGuid();
        var expectedProjection = new CustomerProjection(
            customerId,
            "CUS-TEST",
            CustomerType.Person,
            "คุณลูกค้า",
            null,
            PreferredLocale.Thai,
            CustomerStatus.Active,
            new CustomerContactProjection("คุณติดต่อ", null, "0812345678", null, ContactChannel.Phone, false),
            Guid.NewGuid(),
            DateTimeOffset.UtcNow);

        store.ResultToReturn = Result<CustomerProjection>.Success(expectedProjection);
        var handler = new ActivateCustomerHandler(resolver, store);

        var command = new ActivateCustomerCommand(
            "uid-1", Guid.NewGuid(), customerId, expectedRowVersion, "key-1234567890123456", "trace-1");

        var result = await handler.Handle(command);

        Assert.True(result.IsSuccess);
        Assert.Equal(CustomerStatus.Active, result.Value!.Status);
        Assert.NotNull(store.LastKeyHash);
        Assert.NotNull(store.LastPayloadHash);
        Assert.Equal(customerId, store.LastCustomerId);
        Assert.Equal(expectedRowVersion, store.LastExpectedRowVersion);
    }
}
