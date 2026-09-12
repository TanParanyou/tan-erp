using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.IdentityAccess.Users;
using TanErp.Application.IdentityAccess.Users.ListUsers;
using TanErp.Domain.IdentityAccess;
using Xunit;

namespace TanErp.UnitTests.IdentityAccess;

public class ListUsersHandlerTests
{
    private class FakeRequestAccessResolver : IRequestAccessResolver
    {
        public bool ShouldSucceed { get; set; } = true;
        public Guid OrgId { get; set; } = Guid.NewGuid();

        public Task<Result<RequestAccessContext>> ResolveAsync(
            string firebaseUid,
            Guid membershipId,
            string permissionKey,
            CancellationToken cancellationToken = default)
        {
            if (!ShouldSucceed)
            {
                return Task.FromResult(Result<RequestAccessContext>.Failure(
                    new Error("PERMISSION_DENIED", "Denied")));
            }

            return Task.FromResult(Result<RequestAccessContext>.Success(
                new RequestAccessContext(
                    Guid.NewGuid(),
                    membershipId,
                    OrgId,
                    Guid.NewGuid(),
                    permissionKey,
                    PermissionScope.Organization)));
        }
    }

    private class FakeUserReadStore : IUserReadStore
    {
        public int LastLimit { get; private set; }
        public string? LastSearch { get; private set; }
        public Guid? LastBranchId { get; private set; }
        public IReadOnlyList<UserListItem> ResultToReturn { get; set; } = new List<UserListItem>();

        public Task<IReadOnlyList<UserListItem>> ListActiveUsersAsync(
            Guid organizationId,
            Guid? branchId,
            string? search,
            int limit,
            CancellationToken cancellationToken = default)
        {
            LastLimit = limit;
            LastSearch = search;
            LastBranchId = branchId;
            return Task.FromResult(ResultToReturn);
        }
    }

    [Fact]
    public async Task Handle_WhenAccessResolverFails_ReturnsFailure()
    {
        var resolver = new FakeRequestAccessResolver { ShouldSucceed = false };
        var store = new FakeUserReadStore();
        var handler = new ListUsersHandler(resolver, store);

        var query = new ListUsersQuery("uid-1", Guid.NewGuid());
        var result = await handler.Handle(query);

        Assert.True(result.IsFailure);
        Assert.Equal("PERMISSION_DENIED", result.Error.Code);
    }

    [Theory]
    [InlineData(0, 25)]
    [InlineData(-5, 25)]
    [InlineData(150, 100)]
    [InlineData(50, 50)]
    public async Task Handle_ClampsLimitCorrectly(int inputLimit, int expectedLimit)
    {
        var resolver = new FakeRequestAccessResolver();
        var store = new FakeUserReadStore();
        var handler = new ListUsersHandler(resolver, store);

        var query = new ListUsersQuery("uid-1", Guid.NewGuid(), Limit: inputLimit);
        var result = await handler.Handle(query);

        Assert.True(result.IsSuccess);
        Assert.Equal(expectedLimit, store.LastLimit);
    }

    [Fact]
    public async Task Handle_TrimsSearchQuery()
    {
        var resolver = new FakeRequestAccessResolver();
        var store = new FakeUserReadStore();
        var handler = new ListUsersHandler(resolver, store);

        var query = new ListUsersQuery("uid-1", Guid.NewGuid(), Search: "  Somchai  ");
        var result = await handler.Handle(query);

        Assert.True(result.IsSuccess);
        Assert.Equal("Somchai", store.LastSearch);
    }

    [Fact]
    public async Task Handle_WhenValid_ReturnsUsersList()
    {
        var resolver = new FakeRequestAccessResolver();
        var branchId = Guid.NewGuid();
        var expectedUsers = new List<UserListItem>
        {
            new(Guid.NewGuid(), "Somchai", "somchai@example.com", branchId),
            new(Guid.NewGuid(), "Somsak", "somsak@example.com", branchId)
        };
        var store = new FakeUserReadStore { ResultToReturn = expectedUsers };
        var handler = new ListUsersHandler(resolver, store);

        var query = new ListUsersQuery("uid-1", Guid.NewGuid(), BranchId: branchId);
        var result = await handler.Handle(query);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.TotalCount);
        Assert.Equal(2, result.Value.Items.Count);
        Assert.Equal("Somchai", result.Value.Items[0].DisplayName);
    }
}
