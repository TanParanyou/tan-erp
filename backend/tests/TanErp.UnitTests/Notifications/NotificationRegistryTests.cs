using TanErp.Application.Notifications;
using TanErp.Domain.Notifications;
using Xunit;

namespace TanErp.UnitTests.Notifications;

public class NotificationRegistryTests
{
    [Fact]
    public void Registry_CoversExactlyTheDomainWhitelist()
    {
        Assert.True(NotificationTypeRegistry.Types.ToHashSet().SetEquals(NotificationTypes.All));
    }

    [Fact]
    public void Find_IsExactMatchOnly()
    {
        Assert.NotNull(NotificationTypeRegistry.Find("purchase-order.approval-requested"));
        Assert.Null(NotificationTypeRegistry.Find("Purchase-Order.Approval-Requested"));
        Assert.Null(NotificationTypeRegistry.Find(null));
        Assert.Null(NotificationTypeRegistry.Find("customer.created"));
    }

    [Fact]
    public void EveryDescriptor_DeclaresItsContract()
    {
        foreach (var type in NotificationTypeRegistry.Types)
        {
            var d = NotificationTypeRegistry.Find(type)!;
            Assert.False(string.IsNullOrWhiteSpace(d.TargetPermission), type);
            Assert.StartsWith("/", d.DeepLinkTemplate);
            Assert.Contains(NotificationFields.ActorDisplayName, d.RequiredFields);
            Assert.Contains(d.ReferenceField, d.RequiredFields);
            Assert.Contains(NotificationFields.ResourceId, d.RequiredFields);
        }
    }

    [Fact]
    public void DeepLinkTokens_AreAlwaysDeclaredPayloadFields()
    {
        foreach (var type in NotificationTypeRegistry.Types)
        {
            var d = NotificationTypeRegistry.Find(type)!;
            foreach (var token in NotificationTypeRegistry.TemplateTokens(d.DeepLinkTemplate))
            {
                Assert.Contains(token, d.RequiredFields);
            }
        }
    }

    [Fact]
    public void NoRequiredFieldName_LooksLikeAFigureOrContactDetail()
    {
        foreach (var type in NotificationTypeRegistry.Types)
        {
            foreach (var field in NotificationTypeRegistry.Find(type)!.RequiredFields)
            {
                Assert.False(NotificationPayloadRules.IsForbiddenKey(field), $"{type}:{field}");
            }
        }
    }

    [Fact]
    public void TargetPermissions_MatchTheApprovalPermissionsOfEachSource()
    {
        Assert.Equal("estimates.approve", NotificationTypeRegistry.Find(NotificationTypes.EstimateApprovalRequested)!.TargetPermission);
        Assert.Equal("cost-records.approve", NotificationTypeRegistry.Find(NotificationTypes.CostRecordApprovalRequested)!.TargetPermission);
        Assert.Equal("purchase-orders.approve", NotificationTypeRegistry.Find(NotificationTypes.PurchaseOrderApprovalRequested)!.TargetPermission);
        Assert.Equal("projects.change-orders.approve", NotificationTypeRegistry.Find(NotificationTypes.ChangeOrderApprovalRequested)!.TargetPermission);
        Assert.Equal("mrp.approve", NotificationTypeRegistry.Find(NotificationTypes.MrpRunApprovalRequested)!.TargetPermission);
        Assert.Equal("roles.assign-approval", NotificationTypeRegistry.Find(NotificationTypes.RoleAssignmentApprovalRequested)!.TargetPermission);
    }

    [Fact]
    public void RenderDeepLink_FillsGuidTokens_AndRefusesAnythingElse()
    {
        var d = NotificationTypeRegistry.Find(NotificationTypes.PurchaseOrderApprovalRequested)!;
        var id = Guid.NewGuid();
        var payload = new Dictionary<string, string> { [NotificationFields.ResourceId] = id.ToString("D") };
        Assert.Equal($"/procurement/purchase-orders/{id:D}", NotificationTypeRegistry.RenderDeepLink(d, payload));

        payload[NotificationFields.ResourceId] = "../../admin";
        Assert.Null(NotificationTypeRegistry.RenderDeepLink(d, payload));
        Assert.Null(NotificationTypeRegistry.RenderDeepLink(d, new Dictionary<string, string>()));
    }

    [Fact]
    public void RenderDeepLink_ForAStaticTemplate_NeedsNoPayload()
    {
        var d = NotificationTypeRegistry.Find(NotificationTypes.RoleAssignmentApprovalRequested)!;
        Assert.Equal("/settings/role-requests", NotificationTypeRegistry.RenderDeepLink(d, new Dictionary<string, string>()));
    }
}
