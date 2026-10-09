using TanErp.Domain.Notifications;
using Xunit;

namespace TanErp.UnitTests.Notifications;

public class NotificationDomainTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 8, 3, 0, 0, TimeSpan.Zero);
    private static readonly Guid Org = Guid.NewGuid();
    private static readonly Guid Recipient = Guid.NewGuid();
    private const string Payload = "{\"documentNumber\":\"PO-2026-0001\"}";

    private static Notification Create(string type = NotificationTypes.PurchaseOrderApprovalRequested, string payload = Payload, string key = "purchase-order.approval-requested:abc") =>
        new(Guid.NewGuid(), Org, Recipient, type, payload, key, Now);

    [Fact]
    public void RegisteredTypes_AreExactlyTheSixApprovalRequests_AndMatchTheFrontendList()
    {
        // Keep in step with NOTIFICATION_TYPES in frontend/src/lib/notifications/notification-types.ts (checked there too).
        var expected = new[]
        {
            "change-order.approval-requested",
            "cost-record.approval-requested",
            "estimate.approval-requested",
            "mrp-run.approval-requested",
            "purchase-order.approval-requested",
            "role-assignment.approval-requested"
        };
        Assert.Equal(expected, NotificationTypes.All.OrderBy(t => t, StringComparer.Ordinal).ToArray());
    }

    [Fact]
    public void Normalize_AcceptsOnlyRegisteredTypes()
    {
        Assert.Equal("estimate.approval-requested", NotificationTypes.Normalize("  Estimate.Approval-Requested "));
        Assert.Null(NotificationTypes.Normalize("estimate.deleted"));
        Assert.Null(NotificationTypes.Normalize(null));
        Assert.False(NotificationTypes.IsRegistered("customer"));
    }

    [Fact]
    public void NewNotification_IsUnreadAndKeepsItsFields()
    {
        var notification = Create();
        Assert.Equal((Org, Recipient, NotificationTypes.PurchaseOrderApprovalRequested), (notification.OrganizationId, notification.RecipientUserId, notification.Type));
        Assert.Equal(Now, notification.CreatedAtUtc);
        Assert.False(notification.IsRead);
        Assert.Null(notification.ReadAtUtc);
    }

    [Fact]
    public void UnregisteredType_IsRejected()
    {
        Assert.Equal("NOTIFICATION_TYPE_INVALID", Assert.Throws<NotificationDomainException>(() => Create(type: "estimate.deleted")).Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not json")]
    [InlineData("[1,2]")]
    [InlineData("\"text\"")]
    public void Payload_MustBeAJsonObject(string payload)
    {
        Assert.Equal("NOTIFICATION_PAYLOAD_INVALID", Assert.Throws<NotificationDomainException>(() => Create(payload: payload)).Code);
    }

    [Fact]
    public void Payload_IsLimitedInLength()
    {
        var tooLong = "{\"documentNumber\":\"" + new string('x', Notification.MaxPayloadLength) + "\"}";
        Assert.Equal("NOTIFICATION_PAYLOAD_INVALID", Assert.Throws<NotificationDomainException>(() => Create(payload: tooLong)).Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void DedupeKey_IsRequired(string key)
    {
        Assert.Equal("NOTIFICATION_FIELD_INVALID", Assert.Throws<NotificationDomainException>(() => Create(key: key)).Code);
    }

    [Fact]
    public void OrganizationAndRecipient_AreRequired()
    {
        Assert.Equal("NOTIFICATION_FIELD_INVALID", Assert.Throws<NotificationDomainException>(() =>
            new Notification(Guid.NewGuid(), Guid.Empty, Recipient, NotificationTypes.EstimateApprovalRequested, Payload, "k", Now)).Code);
        Assert.Equal("NOTIFICATION_FIELD_INVALID", Assert.Throws<NotificationDomainException>(() =>
            new Notification(Guid.NewGuid(), Org, Guid.Empty, NotificationTypes.EstimateApprovalRequested, Payload, "k", Now)).Code);
    }

    [Fact]
    public void MarkRead_IsIdempotent_AndKeepsTheFirstReadTime()
    {
        var notification = Create();
        Assert.True(notification.MarkRead(Now.AddMinutes(5)));
        Assert.False(notification.MarkRead(Now.AddMinutes(10)));
        Assert.Equal(Now.AddMinutes(5), notification.ReadAtUtc);
        Assert.True(notification.IsRead);
    }
}
