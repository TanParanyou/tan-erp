using TanErp.Application.Notifications;
using TanErp.Domain.Notifications;
using Xunit;

namespace TanErp.UnitTests.Notifications;

public class NotificationPayloadRulesTests
{
    private static readonly NotificationTypeDescriptor Po = NotificationTypeRegistry.Find(NotificationTypes.PurchaseOrderApprovalRequested)!;

    private static Dictionary<string, string> Valid() => new()
    {
        [NotificationFields.ResourceId] = Guid.NewGuid().ToString("D"),
        [NotificationFields.DocumentNumber] = "PO-2026-0001",
        [NotificationFields.ActorDisplayName] = "สมชาย ใจดี"
    };

    [Fact]
    public void ExactAllowedFields_Pass_AndAreTrimmed()
    {
        var payload = Valid();
        payload[NotificationFields.DocumentNumber] = "  PO-2026-0001 ";
        var result = NotificationPayloadRules.Validate(Po, payload);
        Assert.True(result.IsSuccess);
        Assert.Equal("PO-2026-0001", result.Value![NotificationFields.DocumentNumber]);
    }

    [Theory]
    [InlineData("totalAmount")]
    [InlineData("grandTotal")]
    [InlineData("unitCost")]
    [InlineData("sellingPrice")]
    [InlineData("marginRate")]
    [InlineData("budgetRemaining")]
    [InlineData("discount")]
    [InlineData("customerEmail")]
    [InlineData("phone")]
    [InlineData("siteAddress")]
    public void AFigureOrContactField_IsRejected_EvenAsAnExtraKey(string key)
    {
        var payload = Valid();
        payload[key] = "1000";
        var result = NotificationPayloadRules.Validate(Po, payload);
        Assert.Equal("NOTIFICATION_PAYLOAD_INVALID", result.Error.Code);
    }

    [Fact]
    public void AnUnlistedKey_IsRejected_ThatIsNotEvenSuspicious()
    {
        var payload = Valid();
        payload["note"] = "hello";
        Assert.Equal("NOTIFICATION_PAYLOAD_INVALID", NotificationPayloadRules.Validate(Po, payload).Error.Code);
    }

    [Fact]
    public void AMissingField_IsRejected()
    {
        var payload = Valid();
        payload.Remove(NotificationFields.DocumentNumber);
        Assert.Equal("NOTIFICATION_PAYLOAD_INVALID", NotificationPayloadRules.Validate(Po, payload).Error.Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ABlankValue_IsRejected(string value)
    {
        var payload = Valid();
        payload[NotificationFields.ActorDisplayName] = value;
        Assert.Equal("NOTIFICATION_PAYLOAD_INVALID", NotificationPayloadRules.Validate(Po, payload).Error.Code);
    }

    [Fact]
    public void ATooLongValue_IsRejected()
    {
        var payload = Valid();
        payload[NotificationFields.DocumentNumber] = new string('x', NotificationPayloadRules.MaxValueLength + 1);
        Assert.Equal("NOTIFICATION_PAYLOAD_INVALID", NotificationPayloadRules.Validate(Po, payload).Error.Code);
    }

    [Fact]
    public void ANullPayload_IsRejected()
    {
        Assert.Equal("NOTIFICATION_PAYLOAD_INVALID", NotificationPayloadRules.Validate(Po, null).Error.Code);
    }

    [Fact]
    public void TheErrorMessage_NeverEchoesPayloadValues()
    {
        var payload = Valid();
        payload["totalAmount"] = "987654.32";
        var result = NotificationPayloadRules.Validate(Po, payload);
        Assert.DoesNotContain("987654", result.Error.Message);
    }
}
