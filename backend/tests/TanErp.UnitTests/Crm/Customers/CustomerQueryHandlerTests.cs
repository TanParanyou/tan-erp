using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Crm.Customers;
using TanErp.Application.Crm.Customers.CheckDuplicates;
using TanErp.Application.Crm.Customers.GetCustomer;
using TanErp.Application.Crm.Customers.ListCustomers;
using Xunit;

namespace TanErp.UnitTests.Crm.Customers;

public class CustomerQueryHandlerTests
{
    private class FakeRequestAccessResolver : IRequestAccessResolver
    {
        public HashSet<string> GrantedPermissions { get; } = new(StringComparer.Ordinal);
        public Guid OrgId { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; } = Guid.NewGuid();

        public Task<Result<RequestAccessContext>> ResolveAsync(
            string firebaseUid,
            Guid membershipId,
            string permissionKey,
            CancellationToken cancellationToken = default)
        {
            if (GrantedPermissions.Contains(permissionKey))
            {
                return Task.FromResult(Result<RequestAccessContext>.Success(
                    new RequestAccessContext(UserId, membershipId, OrgId, null, permissionKey, "organization")));
            }

            return Task.FromResult(Result<RequestAccessContext>.Failure(
                new Error("PERMISSION_DENIED", "Permission denied.")));
        }
    }

    private class FakeCustomerReadStore : ICustomerReadStore
    {
        public CustomerProjection? SingleResult { get; set; }
        public CustomerPage ListResult { get; set; } = new(Array.Empty<CustomerProjection>(), null, 0, 1, 25);
        public CustomerListFilter? LastListFilter { get; private set; }
        public bool? LastIncludePii { get; private set; }

        public Task<CustomerPage> ListAsync(
            Guid organizationId,
            CustomerListFilter filter,
            bool includeContactPii,
            CancellationToken cancellationToken = default)
        {
            LastListFilter = filter;
            LastIncludePii = includeContactPii;
            return Task.FromResult(ListResult);
        }

        public Task<CustomerProjection?> GetAsync(
            Guid organizationId,
            Guid customerId,
            bool includeContactPii,
            CancellationToken cancellationToken = default)
        {
            LastIncludePii = includeContactPii;
            return Task.FromResult(SingleResult);
        }

        public IReadOnlyList<DuplicateCustomerProjection> DuplicateResults { get; set; } = Array.Empty<DuplicateCustomerProjection>();
        public string? LastNormalizedName { get; private set; }
        public string? LastNormalizedPhone { get; private set; }
        public string? LastNormalizedEmail { get; private set; }

        public Task<IReadOnlyList<DuplicateCustomerProjection>> FindDuplicatesAsync(
            Guid organizationId,
            string? normalizedName,
            string? normalizedPhone,
            string? normalizedEmail,
            bool includeContactPii,
            CancellationToken cancellationToken = default)
        {
            LastNormalizedName = normalizedName;
            LastNormalizedPhone = normalizedPhone;
            LastNormalizedEmail = normalizedEmail;
            LastIncludePii = includeContactPii;
            return Task.FromResult(DuplicateResults);
        }
    }

    [Fact]
    public async Task ListCustomers_WhenMissingCustomersRead_ReturnsPermissionDenied()
    {
        var resolver = new FakeRequestAccessResolver();
        var store = new FakeCustomerReadStore();
        var handler = new ListCustomersHandler(resolver, store);

        var query = new ListCustomersQuery("uid-1", Guid.NewGuid());
        var result = await handler.Handle(query);

        Assert.True(result.IsFailure);
        Assert.Equal("PERMISSION_DENIED", result.Error.Code);
    }

    [Fact]
    public async Task ListCustomers_WhenMalformedCursor_ReturnsCustomerCursorInvalid()
    {
        var resolver = new FakeRequestAccessResolver();
        resolver.GrantedPermissions.Add("customers.read");
        var store = new FakeCustomerReadStore();
        var handler = new ListCustomersHandler(resolver, store);

        var query = new ListCustomersQuery("uid-1", Guid.NewGuid(), Cursor: "invalid-not-base64!!!");
        var result = await handler.Handle(query);

        Assert.True(result.IsFailure);
        Assert.Equal("CUSTOMER_CURSOR_INVALID", result.Error.Code);
    }

    [Fact]
    public async Task ListCustomers_WhenInvalidCustomerType_ReturnsCustomerTypeInvalid()
    {
        var resolver = new FakeRequestAccessResolver();
        resolver.GrantedPermissions.Add("customers.read");
        var store = new FakeCustomerReadStore();
        var handler = new ListCustomersHandler(resolver, store);

        var query = new ListCustomersQuery("uid-1", Guid.NewGuid(), CustomerType: "invalid_type");
        var result = await handler.Handle(query);

        Assert.True(result.IsFailure);
        Assert.Equal("CUSTOMER_TYPE_INVALID", result.Error.Code);
    }

    [Fact]
    public async Task ListCustomers_WhenInvalidSortBy_ReturnsCustomerSortInvalid()
    {
        var resolver = new FakeRequestAccessResolver();
        resolver.GrantedPermissions.Add("customers.read");
        var store = new FakeCustomerReadStore();
        var handler = new ListCustomersHandler(resolver, store);

        var query = new ListCustomersQuery("uid-1", Guid.NewGuid(), SortBy: "dangerous_sql_field");
        var result = await handler.Handle(query);

        Assert.True(result.IsFailure);
        Assert.Equal("CUSTOMER_SORT_INVALID", result.Error.Code);
    }

    [Fact]
    public async Task ListCustomers_WhenInvalidSortOrder_ReturnsCustomerSortOrderInvalid()
    {
        var resolver = new FakeRequestAccessResolver();
        resolver.GrantedPermissions.Add("customers.read");
        var store = new FakeCustomerReadStore();
        var handler = new ListCustomersHandler(resolver, store);

        var query = new ListCustomersQuery("uid-1", Guid.NewGuid(), SortOrder: "ascending_unknown");
        var result = await handler.Handle(query);

        Assert.True(result.IsFailure);
        Assert.Equal("CUSTOMER_SORT_ORDER_INVALID", result.Error.Code);
    }

    [Fact]
    public async Task ListCustomers_WhenValidWithFiltersAndSort_PassesToStore()
    {
        var resolver = new FakeRequestAccessResolver();
        resolver.GrantedPermissions.Add("customers.read");
        var store = new FakeCustomerReadStore();
        var handler = new ListCustomersHandler(resolver, store);

        var query = new ListCustomersQuery(
            "uid-1",
            Guid.NewGuid(),
            Search: "  บริษัท  ",
            CustomerType: "organization",
            SortBy: "code",
            SortOrder: "desc",
            Limit: 30);
        var result = await handler.Handle(query);

        Assert.True(result.IsSuccess);
        Assert.False(store.LastIncludePii);
        Assert.NotNull(store.LastListFilter);
        Assert.Equal(30, store.LastListFilter.Limit);
        Assert.Equal("organization", store.LastListFilter.CustomerType);
        Assert.Equal("code", store.LastListFilter.SortBy);
        Assert.Equal("desc", store.LastListFilter.SortOrder);
    }

    [Fact]
    public async Task ListCustomers_WhenValidWithoutManagePermission_CallsStoreWithIncludePiiFalse()
    {
        var resolver = new FakeRequestAccessResolver();
        resolver.GrantedPermissions.Add("customers.read");
        var store = new FakeCustomerReadStore();
        var handler = new ListCustomersHandler(resolver, store);

        var query = new ListCustomersQuery("uid-1", Guid.NewGuid(), Search: "  บริษัท  ", Limit: 30);
        var result = await handler.Handle(query);

        Assert.True(result.IsSuccess);
        Assert.False(store.LastIncludePii);
        Assert.NotNull(store.LastListFilter);
        Assert.Equal(30, store.LastListFilter.Limit);
    }

    [Fact]
    public async Task GetCustomer_WhenCustomerNotFound_ReturnsResourceNotFound()
    {
        var resolver = new FakeRequestAccessResolver();
        resolver.GrantedPermissions.Add("customers.read");
        var store = new FakeCustomerReadStore();
        store.SingleResult = null;
        var handler = new GetCustomerHandler(resolver, store);

        var query = new GetCustomerQuery("uid-1", Guid.NewGuid(), Guid.NewGuid());
        var result = await handler.Handle(query);

        Assert.True(result.IsFailure);
        Assert.Equal("RESOURCE_NOT_FOUND", result.Error.Code);
    }

    [Fact]
    public async Task GetCustomer_WhenFound_ReturnsCustomerResult()
    {
        var resolver = new FakeRequestAccessResolver();
        resolver.GrantedPermissions.Add("customers.read");
        resolver.GrantedPermissions.Add("customer-contacts.manage");
        var customerId = Guid.NewGuid();

        var store = new FakeCustomerReadStore();
        store.SingleResult = new CustomerProjection(
            customerId, "CUS-123456789012", "organization", "บริษัท ก", null, "th", "draft",
            new CustomerContactProjection("คุณ ก", null, "+66812345678", null, "phone", IsMasked: false),
            Guid.NewGuid(), DateTimeOffset.UtcNow);

        var handler = new GetCustomerHandler(resolver, store);

        var query = new GetCustomerQuery("uid-1", Guid.NewGuid(), customerId);
        var result = await handler.Handle(query);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(customerId, result.Value.Customer.Id);
        Assert.True(store.LastIncludePii);
    }

    [Fact]
    public async Task CheckDuplicates_WhenMissingPermission_ReturnsPermissionDenied()
    {
        var resolver = new FakeRequestAccessResolver();
        var store = new FakeCustomerReadStore();
        var handler = new CheckCustomerDuplicatesHandler(resolver, store);

        var query = new CheckCustomerDuplicatesQuery("uid-1", Guid.NewGuid(), "บริษัท ทดสอบ", "0812345678", null);
        var result = await handler.Handle(query);

        Assert.True(result.IsFailure);
        Assert.Equal("PERMISSION_DENIED", result.Error.Code);
    }

    [Fact]
    public async Task CheckDuplicates_WhenAllEmpty_ReturnsEmptyWithoutStoreCall()
    {
        var resolver = new FakeRequestAccessResolver();
        resolver.GrantedPermissions.Add("customers.create");
        var store = new FakeCustomerReadStore();
        var handler = new CheckCustomerDuplicatesHandler(resolver, store);

        var query = new CheckCustomerDuplicatesQuery("uid-1", Guid.NewGuid(), "", null, "   ");
        var result = await handler.Handle(query);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value.Candidates);
        Assert.Null(store.LastNormalizedName);
    }

    [Fact]
    public async Task CheckDuplicates_WhenPhoneHasThaiCountryCode_NormalizesToDomesticZero()
    {
        var resolver = new FakeRequestAccessResolver();
        resolver.GrantedPermissions.Add("customers.create");
        var store = new FakeCustomerReadStore();
        var handler = new CheckCustomerDuplicatesHandler(resolver, store);

        var query = new CheckCustomerDuplicatesQuery("uid-1", Guid.NewGuid(), null, "+66 81 234 5678", null);
        var result = await handler.Handle(query);

        Assert.True(result.IsSuccess);
        Assert.Equal("0812345678", store.LastNormalizedPhone);
    }

    [Fact]
    public async Task CheckDuplicates_WhenFound_ReturnsCandidates()
    {
        var resolver = new FakeRequestAccessResolver();
        resolver.GrantedPermissions.Add("customers.create");
        resolver.GrantedPermissions.Add("customer-contacts.manage");
        var store = new FakeCustomerReadStore();
        var dupId = Guid.NewGuid();
        store.DuplicateResults = new List<DuplicateCustomerProjection>
        {
            new(dupId, "CUS-0001", "บริษัท ซ้ำ จำกัด", "0812345678", "dup@test.com")
        };

        var handler = new CheckCustomerDuplicatesHandler(resolver, store);
        var query = new CheckCustomerDuplicatesQuery("uid-1", Guid.NewGuid(), "บริษัท ซ้ำ", "0812345678", null);
        var result = await handler.Handle(query);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Candidates);
        Assert.Equal("CUS-0001", result.Value.Candidates[0].Code);
        Assert.True(store.LastIncludePii);
    }
}

