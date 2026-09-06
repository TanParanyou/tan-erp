using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.IdentityAccess.CurrentUser.GetCurrentUser;
using Xunit;

namespace TanErp.UnitTests.IdentityAccess;

public class GetCurrentUserHandlerTests
{
    private class MockCurrentUserReader : ICurrentUserReader
    {
        private readonly Func<string, Task<Result<GetCurrentUserResult>>> _func;

        public MockCurrentUserReader(Func<string, Task<Result<GetCurrentUserResult>>> func)
        {
            _func = func;
        }

        public Task<Result<GetCurrentUserResult>> GetAsync(string firebaseUid, CancellationToken cancellationToken = default)
        {
            return _func(firebaseUid);
        }
    }

    [Fact]
    public async Task Handle_KnownActiveUserWithActiveMembership_ReturnsSuccess()
    {
        var expectedResult = new GetCurrentUserResult(
            new UserContext(Guid.NewGuid(), "ผู้ใช้ TEST_ONLY", "user@test.com"),
            new List<MembershipContext>
            {
                new(
                    Guid.NewGuid(),
                    new OrganizationContext(Guid.NewGuid(), "TEST_ONLY Org"),
                    new BranchContext(Guid.NewGuid(), "สาขาทดสอบ"),
                    new List<PermissionContext>
                    {
                        new("organizations.read", "organization", Guid.NewGuid())
                    })
            });

        var reader = new MockCurrentUserReader(_ => Task.FromResult(Result<GetCurrentUserResult>.Success(expectedResult)));
        var handler = new GetCurrentUserHandler(reader);

        var result = await handler.Handle(new GetCurrentUserQuery("uid-active"));

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("ผู้ใช้ TEST_ONLY", result.Value.User.DisplayName);
        Assert.Single(result.Value.Memberships);
    }

    [Fact]
    public async Task Handle_KnownDisabledUser_ReturnsUserAccessDisabled()
    {
        var reader = new MockCurrentUserReader(_ => Task.FromResult(Result<GetCurrentUserResult>.Failure(new Error("USER_ACCESS_DISABLED"))));
        var handler = new GetCurrentUserHandler(reader);

        var result = await handler.Handle(new GetCurrentUserQuery("uid-disabled"));

        Assert.True(result.IsFailure);
        Assert.Equal("USER_ACCESS_DISABLED", result.Error.Code);
    }

    [Fact]
    public async Task Handle_UnknownFirebaseUid_ReturnsActiveMembershipRequired()
    {
        var reader = new MockCurrentUserReader(_ => Task.FromResult(Result<GetCurrentUserResult>.Failure(new Error("ACTIVE_MEMBERSHIP_REQUIRED"))));
        var handler = new GetCurrentUserHandler(reader);

        var result = await handler.Handle(new GetCurrentUserQuery("uid-unknown"));

        Assert.True(result.IsFailure);
        Assert.Equal("ACTIVE_MEMBERSHIP_REQUIRED", result.Error.Code);
    }

    [Fact]
    public async Task Handle_KnownUserWithoutActiveMembership_ReturnsActiveMembershipRequired()
    {
        var reader = new MockCurrentUserReader(_ => Task.FromResult(Result<GetCurrentUserResult>.Failure(new Error("ACTIVE_MEMBERSHIP_REQUIRED"))));
        var handler = new GetCurrentUserHandler(reader);

        var result = await handler.Handle(new GetCurrentUserQuery("uid-no-membership"));

        Assert.True(result.IsFailure);
        Assert.Equal("ACTIVE_MEMBERSHIP_REQUIRED", result.Error.Code);
    }

    [Fact]
    public async Task Handle_EmptyFirebaseUid_ReturnsAuthenticationRequired()
    {
        var reader = new MockCurrentUserReader(_ => Task.FromResult(Result<GetCurrentUserResult>.Failure(new Error("UNKNOWN"))));
        var handler = new GetCurrentUserHandler(reader);

        var result = await handler.Handle(new GetCurrentUserQuery("   "));

        Assert.True(result.IsFailure);
        Assert.Equal("AUTHENTICATION_REQUIRED", result.Error.Code);
    }
}
