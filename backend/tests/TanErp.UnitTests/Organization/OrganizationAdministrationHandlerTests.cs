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
