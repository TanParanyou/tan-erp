using System.Text.Json;
using TanErp.Application.Notifications;
using TanErp.Domain.Notifications;
using Xunit;

namespace TanErp.UnitTests.Notifications;

public class NotificationSystemEventTests
{
    private static readonly Guid Org = Guid.NewGuid();
    private static readonly Guid Subject = Guid.NewGuid();

    private static NotificationEvent Expiring(int days = 7) =>
        NotificationEvents.MembershipExpiring(Org, null, Guid.NewGuid(), Guid.NewGuid(), Subject, "สมหญิง", days);

    [Fact]
    public void ASystemEvent_PlansWithoutAnActorName_AndItsPayloadHasNoActorField()
    {
        var admin = Guid.NewGuid();
        var plan = NotificationPublishPlanner.Plan(Expiring(), null, [admin]);
        Assert.True(plan.IsSuccess, plan.Error.Message);
        using var document = JsonDocument.Parse(plan.Value![0].PayloadJson);
        Assert.False(document.RootElement.TryGetProperty(NotificationFields.ActorDisplayName, out _));
        Assert.Equal("7", document.RootElement.GetProperty(NotificationFields.DaysRemaining).GetString());
    }

    [Fact]
    public void ASystemEvent_IgnoresASuppliedActorName_ButRejectsASuppliedActorId()
    {
        var admin = Guid.NewGuid();
        Assert.True(NotificationPublishPlanner.Plan(Expiring(), "ignored", [admin]).IsSuccess);

        var withActor = Expiring() with { ActorUserId = Guid.NewGuid() };
        var result = NotificationPublishPlanner.Plan(withActor, null, [admin]);
        Assert.True(result.IsFailure);
        Assert.Equal("NOTIFICATION_FIELD_INVALID", result.Error.Code);
    }

    [Fact]
    public void AnOrdinaryType_StillRequiresAnActor()
    {
        var fields = new Dictionary<string, string>
        {
            [NotificationFields.ResourceId] = Guid.NewGuid().ToString("D"),
            [NotificationFields.DocumentNumber] = "PO-1"
        };
        var evt = new NotificationEvent(NotificationTypes.PurchaseOrderApprovalRequested, Org, null, Guid.NewGuid(), null, fields, null, []);
        var result = NotificationPublishPlanner.Plan(evt, "ผู้ส่ง", [Guid.NewGuid()]);
        Assert.True(result.IsFailure);
        Assert.Equal("NOTIFICATION_FIELD_INVALID", result.Error.Code);
    }

    [Fact]
    public void TheDedupeKey_IsTypeAndTransition_SoTheSameTransitionNeverStoresTwice()
    {
        var evt = Expiring();
        var a = NotificationPublishPlanner.Plan(evt, null, [Guid.NewGuid()]).Value![0];
        var b = NotificationPublishPlanner.Plan(evt, null, [Guid.NewGuid()]).Value![0];
        Assert.Equal(a.DedupeKey, b.DedupeKey);
        Assert.Equal($"membership.expiring:{evt.TransitionId:N}", a.DedupeKey);
    }
}
