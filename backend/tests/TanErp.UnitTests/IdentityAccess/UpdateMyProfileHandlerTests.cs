using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.IdentityAccess.Administration;
using TanErp.Application.IdentityAccess.CurrentUser.GetCurrentUser;
using TanErp.Application.IdentityAccess.CurrentUser.UpdateMyProfile;
using TanErp.Domain.IdentityAccess;
using Xunit;

namespace TanErp.UnitTests.IdentityAccess;

public class UpdateMyProfileHandlerTests
{
    private sealed class FakeAccess : IRequestAccessResolver
    {
        private readonly Result<RequestAccessContext> _membership;

        public FakeAccess(Result<RequestAccessContext> membership) => _membership = membership;

        public Task<Result<RequestAccessContext>> ResolveAsync(string firebaseUid, Guid membershipId, string permissionKey, CancellationToken cancellationToken = default) =>
            throw new InvalidOperationException("Self-service profile must not require a permission key.");

        public Task<Result<RequestAccessContext>> ResolveMembershipAsync(string firebaseUid, Guid membershipId, CancellationToken cancellationToken = default) =>
            Task.FromResult(_membership);
    }

    private sealed class FakeStore : IMyProfileStore
    {
        public UpdateMyProfileInput? Received { get; private set; }

        public Task<Result<UserContext>> UpdateAsync(Guid organizationId, AdminActor actor, UpdateMyProfileInput input, string traceId, CancellationToken cancellationToken = default)
        {
            Received = input;
            return Task.FromResult(Result<UserContext>.Success(
                new UserContext(actor.UserId, input.DisplayName, "me@test.com", input.PhoneNumber, input.AvatarFileId, input.PreferredLocale)));
        }
    }

    private static Result<RequestAccessContext> ActiveMembership() =>
        Result<RequestAccessContext>.Success(new RequestAccessContext(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), null, "membership", "own"));

    private static UpdateMyProfileCommand Command(string name = "TEST_ONLY User", string? phone = null, string? locale = null) =>
        new("uid", Guid.NewGuid(), name, phone, locale, null, "trace");

    [Fact]
    public async Task Handle_ValidInput_TrimsAndNormalizesBeforeStoring()
    {
        var store = new FakeStore();
        var handler = new UpdateMyProfileHandler(new FakeAccess(ActiveMembership()), store);

        var result = await handler.Handle(Command("  TEST_ONLY User  ", " 0812345678 ", " EN "));

        Assert.True(result.IsSuccess);
        Assert.Equal("TEST_ONLY User", store.Received!.DisplayName);
        Assert.Equal("0812345678", store.Received.PhoneNumber);
        Assert.Equal("en", store.Received.PreferredLocale);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Handle_BlankDisplayName_FailsValidation(string name)
    {
        var store = new FakeStore();
        var handler = new UpdateMyProfileHandler(new FakeAccess(ActiveMembership()), store);

        var result = await handler.Handle(Command(name));

        Assert.True(result.IsFailure);
        Assert.Equal("REQUEST_VALIDATION_FAILED", result.Error.Code);
        Assert.Null(store.Received);
    }

    [Fact]
    public async Task Handle_UnsupportedLocale_FailsValidation()
    {
        var handler = new UpdateMyProfileHandler(new FakeAccess(ActiveMembership()), new FakeStore());

        var result = await handler.Handle(Command(locale: "fr"));

        Assert.True(result.IsFailure);
        Assert.Equal("REQUEST_VALIDATION_FAILED", result.Error.Code);
    }

    [Fact]
    public async Task Handle_PhoneExceeds50Chars_FailsValidation()
    {
        var handler = new UpdateMyProfileHandler(new FakeAccess(ActiveMembership()), new FakeStore());

        var result = await handler.Handle(Command(phone: new string('9', 51)));

        Assert.True(result.IsFailure);
        Assert.Equal("REQUEST_VALIDATION_FAILED", result.Error.Code);
    }

    [Fact]
    public async Task Handle_DisplayNameExceeds255Chars_FailsValidation()
    {
        var handler = new UpdateMyProfileHandler(new FakeAccess(ActiveMembership()), new FakeStore());

        var result = await handler.Handle(Command(name: new string('A', 256)));

        Assert.True(result.IsFailure);
        Assert.Equal("REQUEST_VALIDATION_FAILED", result.Error.Code);
    }

    [Fact]
    public async Task Handle_StoreFailure_PropagatesError()
    {
        var failingStore = new MockFailingStore(new Error("FILE_NOT_FOUND", "Avatar file not found."));
        var handler = new UpdateMyProfileHandler(new FakeAccess(ActiveMembership()), failingStore);

        var result = await handler.Handle(Command());

        Assert.True(result.IsFailure);
        Assert.Equal("FILE_NOT_FOUND", result.Error.Code);
    }

    private sealed class MockFailingStore : IMyProfileStore
    {
        private readonly Error _error;
        public MockFailingStore(Error error) => _error = error;
        public Task<Result<UserContext>> UpdateAsync(Guid organizationId, AdminActor actor, UpdateMyProfileInput input, string traceId, CancellationToken cancellationToken = default) =>
            Task.FromResult(Result<UserContext>.Failure(_error));
    }


    [Fact]
    public async Task Handle_NoActiveMembership_ReturnsAccessFailureWithoutStoring()
    {
        var store = new FakeStore();
        var denied = Result<RequestAccessContext>.Failure(new Error("ACTIVE_MEMBERSHIP_REQUIRED", "denied"));
        var handler = new UpdateMyProfileHandler(new FakeAccess(denied), store);

        var result = await handler.Handle(Command());

        Assert.True(result.IsFailure);
        Assert.Equal("ACTIVE_MEMBERSHIP_REQUIRED", result.Error.Code);
        Assert.Null(store.Received);
    }

    [Fact]
    public void User_UpdateOwnProfile_RejectsUnsupportedLocaleAndKeepsAdminFields()
    {
        var user = new User(Guid.NewGuid(), "uid", "Old", "me@test.com", employeeCode: "emp1");

        Assert.Throws<ArgumentException>(() => user.UpdateOwnProfile("New", null, "fr", null));

        var before = user.RowVersion;
        user.UpdateOwnProfile("New", "081", "th", null);

        Assert.Equal("New", user.DisplayName);
        Assert.Equal("th", user.PreferredLocale);
        Assert.Equal("EMP1", user.EmployeeCode);
        Assert.NotEqual(before, user.RowVersion);
    }
}
