using System.Text.Json;
using TanErp.Application.Notifications;
using TanErp.Domain.Notifications;
using Xunit;

namespace TanErp.UnitTests.Notifications;

public class NotificationPublishPlannerTests
{
    private static readonly Guid Org = Guid.NewGuid();
    private static readonly Guid Maker = Guid.NewGuid();
    private static readonly Guid Transition = Guid.NewGuid();

    private static NotificationEvent PurchaseOrder(IReadOnlyCollection<Guid>? excluded = null, IReadOnlyCollection<Guid>? explicitRecipients = null, Dictionary<string, string>? extra = null)
    {
        var fields = new Dictionary<string, string>
        {
            [NotificationFields.ResourceId] = Guid.NewGuid().ToString("D"),
            [NotificationFields.DocumentNumber] = "PO-2026-0001"
        };
        if (extra is not null) foreach (var pair in extra) fields[pair.Key] = pair.Value;
        return new NotificationEvent(NotificationTypes.PurchaseOrderApprovalRequested, Org, Guid.NewGuid(), Transition, Maker, fields, explicitRecipients, excluded ?? []);
    }

    [Fact]
    public void TheMaker_IsNeverANotifiedRecipient_EvenWhenTheyHoldThePermission()
    {
        var checker = Guid.NewGuid();
        var result = NotificationPublishPlanner.Plan(PurchaseOrder(), "ผู้จัดทำ", [Maker, checker]);
        Assert.True(result.IsSuccess);
        Assert.Equal([checker], result.Value!.Select(p => p.RecipientUserId).ToArray());
    }

    [Fact]
    public void ExplicitlyExcludedUsers_AreDropped_AndDuplicatesCollapse()
    {
        var creator = Guid.NewGuid();
        var checker = Guid.NewGuid();
        var result = NotificationPublishPlanner.Plan(PurchaseOrder(excluded: [creator]), "ผู้ส่ง", [creator, checker, checker, Guid.Empty]);
        Assert.Equal([checker], result.Value!.Select(p => p.RecipientUserId).ToArray());
    }

    [Fact]
    public void NoEligibleRecipient_YieldsAnEmptyPlan_NotAnError()
    {
        var result = NotificationPublishPlanner.Plan(PurchaseOrder(), "ผู้ส่ง", [Maker]);
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!);
    }

    [Fact]
    public void RecipientsPerEvent_AreCapped_InAStableOrder()
    {
        var candidates = Enumerable.Range(0, NotificationLimits.MaxRecipientsPerEvent + 20).Select(_ => Guid.NewGuid()).ToArray();
        var first = NotificationPublishPlanner.Plan(PurchaseOrder(), "ผู้ส่ง", candidates).Value!;
        var second = NotificationPublishPlanner.Plan(PurchaseOrder(), "ผู้ส่ง", candidates.Reverse().ToArray()).Value!;
        Assert.Equal(NotificationLimits.MaxRecipientsPerEvent, first.Count);
        Assert.Equal(first.Select(p => p.RecipientUserId), second.Select(p => p.RecipientUserId));
    }

    [Fact]
    public void TheDedupeKey_IsTypePlusTransition_SoAResubmissionNotifiesAgain()
    {
        var checker = Guid.NewGuid();
        var one = NotificationPublishPlanner.Plan(PurchaseOrder(), "ผู้ส่ง", [checker]).Value!.Single();
        Assert.Equal($"purchase-order.approval-requested:{Transition:N}", one.DedupeKey);

        var again = PurchaseOrder() with { TransitionId = Guid.NewGuid() };
        var two = NotificationPublishPlanner.Plan(again, "ผู้ส่ง", [checker]).Value!.Single();
        Assert.NotEqual(one.DedupeKey, two.DedupeKey);
    }

    [Fact]
    public void ThePayload_ContainsExactlyTheAllowedFields_AndTheActorDisplayName()
    {
        var plan = NotificationPublishPlanner.Plan(PurchaseOrder(), "สมชาย ใจดี", [Guid.NewGuid()]).Value!.Single();
        using var document = JsonDocument.Parse(plan.PayloadJson);
        var keys = document.RootElement.EnumerateObject().Select(p => p.Name).OrderBy(k => k, StringComparer.Ordinal).ToArray();
        Assert.Equal(["actorDisplayName", "documentNumber", "resourceId"], keys);
        Assert.Equal("สมชาย ใจดี", document.RootElement.GetProperty("actorDisplayName").GetString());
    }

    [Fact]
    public void AFigureInTheEventFields_FailsThePlan_BeforeAnythingIsStored()
    {
        var result = NotificationPublishPlanner.Plan(PurchaseOrder(extra: new() { ["totalAmount"] = "250000.00" }), "ผู้ส่ง", [Guid.NewGuid()]);
        Assert.Equal("NOTIFICATION_PAYLOAD_INVALID", result.Error.Code);
    }

    [Fact]
    public void ACallerSuppliedActorDisplayName_IsRejected_BecauseThePublisherOwnsIt()
    {
        var result = NotificationPublishPlanner.Plan(PurchaseOrder(extra: new() { [NotificationFields.ActorDisplayName] = "spoof" }), "ผู้ส่ง", [Guid.NewGuid()]);
        Assert.Equal("NOTIFICATION_PAYLOAD_INVALID", result.Error.Code);
    }

    [Fact]
    public void AnUnregisteredType_AndEmptyIds_AreRejected()
    {
        Assert.Equal("NOTIFICATION_TYPE_INVALID", NotificationPublishPlanner.Plan(PurchaseOrder() with { Type = "customer.created" }, "x", [Guid.NewGuid()]).Error.Code);
        Assert.Equal("NOTIFICATION_FIELD_INVALID", NotificationPublishPlanner.Plan(PurchaseOrder() with { TransitionId = Guid.Empty }, "x", [Guid.NewGuid()]).Error.Code);
        Assert.Equal("NOTIFICATION_FIELD_INVALID", NotificationPublishPlanner.Plan(PurchaseOrder() with { OrganizationId = Guid.Empty }, "x", [Guid.NewGuid()]).Error.Code);
    }
}
