using System.Text.Json;
using TanErp.Application.Notifications;
using TanErp.Domain.Notifications;
using Xunit;

namespace TanErp.UnitTests.Notifications;

public class NotificationEventsTests
{
    private static readonly Guid Org = Guid.NewGuid();
    private static readonly Guid Branch = Guid.NewGuid();
    private static readonly Guid Actor = Guid.NewGuid();
    private static readonly Guid Creator = Guid.NewGuid();
    private static readonly Guid Editor = Guid.NewGuid();
    private static readonly Guid Reviewer = Guid.NewGuid();
    private static readonly Guid Subject = Guid.NewGuid();

    public static TheoryData<NotificationEvent> AllEvents() => new()
    {
        NotificationEvents.EstimateSubmitted(Org, Branch, Guid.NewGuid(), Actor, Guid.NewGuid(), "EST-2026-0001", Reviewer),
        NotificationEvents.CostRecordSubmitted(Org, Branch, Guid.NewGuid(), Actor, Guid.NewGuid(), "ITEM-0001", [Creator, Editor]),
        NotificationEvents.PurchaseOrderSubmitted(Org, Branch, Guid.NewGuid(), Actor, Guid.NewGuid(), "PO-2026-0001", Creator),
        NotificationEvents.ChangeOrderSubmitted(Org, Branch, Guid.NewGuid(), Actor, Guid.NewGuid(), Guid.NewGuid(), "CO-2026-0001", Creator),
        NotificationEvents.MrpRunCreated(Org, Branch, Guid.NewGuid(), Actor, "MRP-2026-0001"),
        NotificationEvents.RoleAssignmentRequested(Org, Guid.NewGuid(), Actor, Subject, "สมหญิง ทดสอบ", "Approver"),
    };

    [Theory]
    [MemberData(nameof(AllEvents))]
    public void EveryEvent_BuildsAPlan_WhosePayloadIsExactlyTheRegisteredFields(NotificationEvent evt)
    {
        var plan = NotificationPublishPlanner.Plan(evt, "ผู้ส่ง ทดสอบ", [Reviewer, Guid.NewGuid()]);
        Assert.True(plan.IsSuccess, plan.Error.Message);

        var descriptor = NotificationTypeRegistry.Find(evt.Type)!;
        using var document = JsonDocument.Parse(plan.Value![0].PayloadJson);
        var keys = document.RootElement.EnumerateObject().Select(p => p.Name).ToHashSet(StringComparer.Ordinal);
        Assert.True(keys.SetEquals(descriptor.RequiredFields));
    }

    [Theory]
    [MemberData(nameof(AllEvents))]
    public void NoEventPayload_CarriesFigures_OrContactDetails(NotificationEvent evt)
    {
        var plan = NotificationPublishPlanner.Plan(evt, "ผู้ส่ง ทดสอบ", [Reviewer, Guid.NewGuid()]).Value![0];
        foreach (var fragment in new[] { "amount", "total", "cost", "price", "margin", "budget", "discount", "email", "phone", "address", "@" })
        {
            Assert.DoesNotContain(fragment, plan.PayloadJson, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public void Estimate_NotifiesOnlyTheRouteReviewer_AndRecordsTheActor()
    {
        var evt = NotificationEvents.EstimateSubmitted(Org, Branch, Guid.NewGuid(), Actor, Guid.NewGuid(), "EST-1", Reviewer);
        Assert.Equal([Reviewer], evt.ExplicitRecipientUserIds!.ToArray());
        Assert.Equal(Actor, evt.ActorUserId);
    }

    [Fact]
    public void CostRecord_ExcludesTheCreatorAndTheLastFinancialEditor_AndMayBeOrganizationWide()
    {
        var evt = NotificationEvents.CostRecordSubmitted(Org, null, Guid.NewGuid(), Actor, Guid.NewGuid(), "ITEM-1", [Creator, Editor]);
        Assert.Null(evt.ExplicitRecipientUserIds);
        Assert.Null(evt.BranchId);
        Assert.True(evt.ExcludedUserIds.ToHashSet().SetEquals([Creator, Editor]));
    }

    [Fact]
    public void PurchaseOrder_AndChangeOrder_ExcludeTheirCreator()
    {
        var po = NotificationEvents.PurchaseOrderSubmitted(Org, Branch, Guid.NewGuid(), Actor, Guid.NewGuid(), "PO-1", Creator);
        var co = NotificationEvents.ChangeOrderSubmitted(Org, Branch, Guid.NewGuid(), Actor, Guid.NewGuid(), Guid.NewGuid(), "CO-1", Creator);
        Assert.Contains(Creator, po.ExcludedUserIds);
        Assert.Contains(Creator, co.ExcludedUserIds);
    }

    [Fact]
    public void RoleAssignment_ExcludesTheSubjectUser_AndIsOrganizationWide()
    {
        var evt = NotificationEvents.RoleAssignmentRequested(Org, Guid.NewGuid(), Actor, Subject, "สมหญิง", "Approver");
        Assert.Contains(Subject, evt.ExcludedUserIds);
        Assert.Null(evt.BranchId);
    }

    [Fact]
    public void ChangeOrder_PutsTheProjectIdInParentId_ForTheDeepLink()
    {
        var projectId = Guid.NewGuid();
        var evt = NotificationEvents.ChangeOrderSubmitted(Org, Branch, Guid.NewGuid(), Actor, Guid.NewGuid(), projectId, "CO-1", Creator);
        Assert.Equal(projectId.ToString("D"), evt.Fields[NotificationFields.ParentId]);
    }

    [Fact]
    public void TheEventTypes_AreAllDistinctAndCoverTheWholeWhitelist()
    {
        var types = new List<string>();
        foreach (NotificationEvent evt in AllEvents())
        {
            types.Add(evt.Type);
        }

        Assert.Equal(types.Count, types.Distinct().Count());
        Assert.True(types.ToHashSet().SetEquals(NotificationTypes.All));
    }
}
