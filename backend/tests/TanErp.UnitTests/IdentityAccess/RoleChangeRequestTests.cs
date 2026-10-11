using TanErp.Domain.IdentityAccess;
using Xunit;

namespace TanErp.UnitTests.IdentityAccess;

public class RoleChangeRequestTests
{
    private static readonly Guid Maker = Guid.NewGuid();
    private static readonly Guid Checker = Guid.NewGuid();
    private static readonly DateTimeOffset Now = new(2026, 10, 11, 0, 0, 0, TimeSpan.Zero);

    private static RoleChangeRequest NewRequest() =>
        new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Maker, Now, Guid.NewGuid(), "Name", null, "[]");

    [Fact]
    public void DecidingRequiresAnIndependentUser_AndOnlyTheRequesterMayCancel()
    {
        var request = NewRequest();
        var before = request.RowVersion;

        Assert.False(request.TryApprove(Maker, Now));
        Assert.False(request.TryCancel(Checker, Now));
        Assert.True(request.IsPending);
        Assert.True(request.TryApprove(Checker, Now));

        Assert.Equal(RoleAssignmentRequestStatus.Approved, request.Status);
        Assert.Equal(Checker, request.DecidedByUserId);
        Assert.NotEqual(before, request.RowVersion);
        Assert.False(request.TryReject(Guid.NewGuid(), Now)); // already decided
    }

    [Fact]
    public void Reject_AndCancel_RecordTheirOwnStatus()
    {
        var rejected = NewRequest();
        var cancelled = NewRequest();

        Assert.True(rejected.TryReject(Checker, Now));
        Assert.True(cancelled.TryCancel(Maker, Now));

        Assert.Equal(RoleAssignmentRequestStatus.Rejected, rejected.Status);
        Assert.Equal(RoleAssignmentRequestStatus.Cancelled, cancelled.Status);
    }
}
