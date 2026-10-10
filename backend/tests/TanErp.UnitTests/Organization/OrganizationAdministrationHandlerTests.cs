using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.IdentityAccess.Administration;
using TanErp.Application.Organization.Administration;
using Xunit;

namespace TanErp.UnitTests.Organization;

public class OrganizationAdministrationHandlerTests
{
    private static readonly Guid OrgId = Guid.NewGuid();
    private static readonly AdminCaller Caller = new("uid", Guid.NewGuid());
    private static readonly OrganizationProfileInput ProfileInput = new("n", null, null, null, null, null);

    private sealed class FakeAccess : IRequestAccessResolver
    {
        public string? RequestedKey { get; private set; }
        public bool Allow { get; init; } = true;

        public Task<Result<RequestAccessContext>> ResolveAsync(string firebaseUid, Guid membershipId, string permissionKey, CancellationToken cancellationToken = default)
        {
            RequestedKey = permissionKey;
            return Task.FromResult(Allow
                ? Result<RequestAccessContext>.Success(new RequestAccessContext(Guid.NewGuid(), membershipId, OrgId, null, permissionKey, "organization"))
                : Result<RequestAccessContext>.Failure(new Error("PERMISSION_DENIED", "denied")));
        }
    }

    private sealed class RecordingStore : IOrganizationAdministrationStore
    {
        public int Calls { get; private set; }

        public Task<Result<OrganizationProfile>> GetProfileAsync(Guid organizationId, CancellationToken ct)
        {
            Calls++;
            return Task.FromResult(Result<OrganizationProfile>.Failure(new Error("RESOURCE_NOT_FOUND", "x")));
        }

        public Task<Result<OrganizationProfile>> UpdateProfileAsync(Guid organizationId, OrganizationProfileInput input, Guid ifMatch, AdminActor actor, string traceId, CancellationToken ct) =>
            GetProfileAsync(organizationId, ct);

        public string? LastKeyHash { get; private set; }

        private static BranchDetail Branch() => new(Guid.NewGuid(), "B1", "n", null, null, null, null, null, true, Guid.NewGuid(), DateTimeOffset.UtcNow);

        public Task<IReadOnlyList<BranchDetail>> ListBranchesAsync(Guid organizationId, BranchStatusFilter filter, CancellationToken ct)
        {
            Calls++;
            return Task.FromResult<IReadOnlyList<BranchDetail>>(Array.Empty<BranchDetail>());
        }

        public Task<Result<BranchDetail>> GetBranchAsync(Guid organizationId, Guid branchId, CancellationToken ct)
        {
            Calls++;
            return Task.FromResult(Result<BranchDetail>.Success(Branch()));
        }

        public Task<Result<BranchDetail>> CreateBranchAsync(
            Guid organizationId, CreateBranchInput input, AdminActor actor, string keyHash, string payloadHash, string traceId, CancellationToken ct)
        {
            Calls++;
            LastKeyHash = keyHash;
            return Task.FromResult(Result<BranchDetail>.Success(Branch()));
        }

        public Task<Result<BranchDetail>> UpdateBranchAsync(
            Guid organizationId, Guid branchId, BranchInput input, Guid ifMatch, AdminActor actor, string traceId, CancellationToken ct) =>
            GetBranchAsync(organizationId, branchId, ct);
    }

    private static readonly BranchInput BranchInputValue = new("n", null, null, null, null, null);

    [Fact]
    public async Task BranchOperations_WithoutPermission_NeverTouchTheStore()
    {
        var store = new RecordingStore();
        var handler = new OrganizationAdministrationHandler(new FakeAccess { Allow = false }, store);

        var list = await handler.ListBranchesAsync(Caller, BranchStatusFilter.All, default);
        var get = await handler.GetBranchAsync(Caller, Guid.NewGuid(), default);
        var create = await handler.CreateBranchAsync(Caller, "key-0123456789abcdef", new CreateBranchInput("B2", BranchInputValue), "t", default);
        var update = await handler.UpdateBranchAsync(Caller, Guid.NewGuid(), Guid.NewGuid(), BranchInputValue, "t", default);

        Assert.True(list.IsFailure && get.IsFailure && create.IsFailure && update.IsFailure);
        Assert.Equal(0, store.Calls);
    }

    [Fact]
    public async Task CreateBranch_AsksForBranchesManageAndHashesTheKey()
    {
        var access = new FakeAccess();
        var store = new RecordingStore();
        var handler = new OrganizationAdministrationHandler(access, store);

        await handler.CreateBranchAsync(Caller, "plain-key-0123456789", new CreateBranchInput("B2", BranchInputValue), "t", default);

        Assert.Equal("branches.manage", access.RequestedKey);
        Assert.NotNull(store.LastKeyHash);
        Assert.DoesNotContain("plain-key", store.LastKeyHash);
    }

    [Fact]
    public async Task UpdateBranch_AsksForBranchesManage()
    {
        var access = new FakeAccess();
        var handler = new OrganizationAdministrationHandler(access, new RecordingStore());

        await handler.UpdateBranchAsync(Caller, Guid.NewGuid(), Guid.NewGuid(), BranchInputValue, "t", default);

        Assert.Equal("branches.manage", access.RequestedKey);
    }

    [Fact]
    public async Task WithoutPermission_NeverTouchesTheStore_SoNothingLeaksAboutExistence()
    {
        var store = new RecordingStore();
        var handler = new OrganizationAdministrationHandler(new FakeAccess { Allow = false }, store);

        var read = await handler.GetProfileAsync(Caller, default);
        var write = await handler.UpdateProfileAsync(Caller, Guid.NewGuid(), ProfileInput, "t", default);

        Assert.Equal("PERMISSION_DENIED", read.Error.Code);
        Assert.Equal("PERMISSION_DENIED", write.Error.Code);
        Assert.Equal(0, store.Calls);
    }

    [Fact]
    public async Task EachOperationAsksForItsOwnPermissionKey()
    {
        var access = new FakeAccess();
        var handler = new OrganizationAdministrationHandler(access, new RecordingStore());

        await handler.GetProfileAsync(Caller, default);
        Assert.Equal("organizations.read", access.RequestedKey);

        await handler.UpdateProfileAsync(Caller, Guid.NewGuid(), ProfileInput, "t", default);
        Assert.Equal("organizations.manage", access.RequestedKey);
    }
}
