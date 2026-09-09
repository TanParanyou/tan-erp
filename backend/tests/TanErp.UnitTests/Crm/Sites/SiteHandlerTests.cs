using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Crm.Sites;
using TanErp.Application.Crm.Sites.CreateSite;
using TanErp.Application.Crm.Sites.ListSites;
using TanErp.Domain.Crm.Sites;
using Xunit;

namespace TanErp.UnitTests.Crm.Sites;

public class SiteHandlerTests
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

    private class FakeSiteStore : ISiteStore
    {
        public CreateSiteCommand? LastCreateCommand { get; private set; }
        public string? LastKeyHash { get; private set; }
        public string? LastPayloadHash { get; private set; }
        public int CreateCallCount { get; private set; }
        public int ListCallCount { get; private set; }
        public Result<SiteProjection> CreateResult { get; set; } = Result<SiteProjection>.Failure(new Error("UNSET", "Unset"));
        public IReadOnlyList<SiteProjection>? ListResult { get; set; }

        public Task<Result<SiteProjection>> CreateAsync(
            RequestAccessContext access,
            CreateSiteCommand command,
            string keyHash,
            string payloadHash,
            CancellationToken cancellationToken = default)
        {
            CreateCallCount++;
            LastCreateCommand = command;
            LastKeyHash = keyHash;
            LastPayloadHash = payloadHash;
            return Task.FromResult(CreateResult);
        }

        public Task<IReadOnlyList<SiteProjection>?> ListByCustomerAsync(
            Guid organizationId,
            Guid customerId,
            CancellationToken cancellationToken = default)
        {
            ListCallCount++;
            return Task.FromResult(ListResult);
        }
    }

    private readonly FakeRequestAccessResolver _accessResolver = new();
    private readonly FakeSiteStore _store = new();

    private CreateSiteHandler CreateHandler() => new(_accessResolver, _store);
    private ListSitesHandler ListHandler() => new(_accessResolver, _store);

    [Fact]
    public async Task Create_PermissionDenied_ReturnsFailureAndDoesNotCallStore()
    {
        // sites.manage not granted
        var cmd = new CreateSiteCommand("uid", Guid.NewGuid(), Guid.NewGuid(), "key", "บ้าน", "123", "ต", "อ", "จ", "10000", "TH", null, null, null, "trace");
        var handler = CreateHandler();

        var result = await handler.Handle(cmd);

        Assert.True(result.IsFailure);
        Assert.Equal("PERMISSION_DENIED", result.Error.Code);
        Assert.Equal(0, _store.CreateCallCount);
    }

    [Theory]
    [InlineData("", "123", "ต", "อ", "จ", "10000", "TH")]
    [InlineData("บ้าน", "", "ต", "อ", "จ", "10000", "TH")]
    [InlineData("บ้าน", "123", "", "อ", "จ", "10000", "TH")]
    [InlineData("บ้าน", "123", "ต", "", "จ", "10000", "TH")]
    [InlineData("บ้าน", "123", "ต", "อ", "", "10000", "TH")]
    [InlineData("บ้าน", "123", "ต", "อ", "จ", "", "TH")]
    [InlineData("บ้าน", "123", "ต", "อ", "จ", "10000", "")]
    public async Task Create_MissingRequiredFields_ReturnsValidationErrorAndDoesNotCallStore(
        string label, string address, string subdistrict, string district, string province, string postal, string country)
    {
        _accessResolver.GrantedPermissions.Add("sites.manage");
        var cmd = new CreateSiteCommand("uid", Guid.NewGuid(), Guid.NewGuid(), "key", label, address, subdistrict, district, province, postal, country, null, null, null, "trace");
        var handler = CreateHandler();

        var result = await handler.Handle(cmd);

        Assert.True(result.IsFailure);
        Assert.Equal("SITE_FIELD_REQUIRED", result.Error.Code);
        Assert.Equal(0, _store.CreateCallCount);
    }

    [Theory]
    [InlineData("13.7563", null)]
    [InlineData(null, "100.5018")]
    public async Task Create_OneSidedCoordinate_ReturnsValidationError(string? latStr, string? lngStr)
    {
        _accessResolver.GrantedPermissions.Add("sites.manage");
        decimal? lat = latStr != null ? decimal.Parse(latStr) : null;
        decimal? lng = lngStr != null ? decimal.Parse(lngStr) : null;

        var cmd = new CreateSiteCommand("uid", Guid.NewGuid(), Guid.NewGuid(), "key", "บ้าน", "123", "ต", "อ", "จ", "10000", "TH", lat, lng, null, "trace");
        var handler = CreateHandler();

        var result = await handler.Handle(cmd);

        Assert.True(result.IsFailure);
        Assert.Equal("SITE_FIELD_REQUIRED", result.Error.Code);
        Assert.Equal(0, _store.CreateCallCount);
    }

    [Theory]
    [InlineData("-91.0", "100.0")]
    [InlineData("91.0", "100.0")]
    [InlineData("13.0", "-181.0")]
    [InlineData("13.0", "181.0")]
    public async Task Create_CoordinateOutOfRange_ReturnsValidationError(string latStr, string lngStr)
    {
        _accessResolver.GrantedPermissions.Add("sites.manage");
        var cmd = new CreateSiteCommand("uid", Guid.NewGuid(), Guid.NewGuid(), "key", "บ้าน", "123", "ต", "อ", "จ", "10000", "TH", decimal.Parse(latStr), decimal.Parse(lngStr), null, "trace");
        var handler = CreateHandler();

        var result = await handler.Handle(cmd);

        Assert.True(result.IsFailure);
        Assert.Equal("SITE_FIELD_REQUIRED", result.Error.Code);
        Assert.Equal(0, _store.CreateCallCount);
    }

    [Fact]
    public async Task Create_ValidCommand_HashesPayloadAndCallsStore()
    {
        _accessResolver.GrantedPermissions.Add("sites.manage");
        var customerId = Guid.NewGuid();
        var cmd = new CreateSiteCommand(
            "uid", Guid.NewGuid(), customerId, "test-key",
            " บ้านพักตากอากาศ  ", " 123/45 หมู่ 1 ", " ต.บางตลาด ", " อ.ปากเกร็ด ", " นนทบุรี ", " 11120 ", " TH ",
            13.7563m, 100.5018m, " มีที่จอดรถ ", "trace-123");

        var expectedProjection = new SiteProjection(
            Guid.NewGuid(), "SITE-01", customerId, "บ้านพักตากอากาศ",
            "123/45 หมู่ 1", "ต.บางตลาด", "อ.ปากเกร็ด", "นนทบุรี", "11120", "TH",
            13.7563m, 100.5018m, "มีที่จอดรถ", SiteStatus.Active, Guid.NewGuid(), DateTimeOffset.UtcNow);

        _store.CreateResult = Result<SiteProjection>.Success(expectedProjection);
        var handler = CreateHandler();

        var result = await handler.Handle(cmd);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, _store.CreateCallCount);
        Assert.NotNull(_store.LastKeyHash);
        Assert.NotNull(_store.LastPayloadHash);
        Assert.Equal(expectedProjection, result.Value);
    }

    [Fact]
    public async Task List_PermissionDenied_ReturnsFailureAndDoesNotCallStore()
    {
        var query = new ListSitesQuery("uid", Guid.NewGuid(), Guid.NewGuid(), "trace");
        var handler = ListHandler();

        var result = await handler.Handle(query);

        Assert.True(result.IsFailure);
        Assert.Equal("PERMISSION_DENIED", result.Error.Code);
        Assert.Equal(0, _store.ListCallCount);
    }

    [Fact]
    public async Task List_CustomerNotFoundOrInactive_ReturnsCustomerNotFound()
    {
        _accessResolver.GrantedPermissions.Add("sites.read");
        _store.ListResult = null; // Store returns null if customer is missing or out of scope
        var query = new ListSitesQuery("uid", Guid.NewGuid(), Guid.NewGuid(), "trace");
        var handler = ListHandler();

        var result = await handler.Handle(query);

        Assert.True(result.IsFailure);
        Assert.Equal("RESOURCE_NOT_FOUND", result.Error.Code);
        Assert.Equal(1, _store.ListCallCount);
    }

    [Fact]
    public async Task List_CustomerFound_ReturnsSites()
    {
        _accessResolver.GrantedPermissions.Add("sites.read");
        var customerId = Guid.NewGuid();
        var sites = new List<SiteProjection>
        {
            new(Guid.NewGuid(), "SITE-01", customerId, "บ้าน 1", "123", "ต", "อ", "จ", "10000", "TH", null, null, null, SiteStatus.Active, Guid.NewGuid(), DateTimeOffset.UtcNow)
        };
        _store.ListResult = sites;
        var query = new ListSitesQuery("uid", Guid.NewGuid(), customerId, "trace");
        var handler = ListHandler();

        var result = await handler.Handle(query);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        var item = Assert.Single(result.Value);
        Assert.Equal("SITE-01", item.Code);
    }
}
