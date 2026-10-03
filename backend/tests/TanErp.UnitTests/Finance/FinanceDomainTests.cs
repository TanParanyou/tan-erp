using TanErp.Domain.Finance;
using Xunit;

namespace TanErp.UnitTests.Finance;

public class FinanceDomainTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 4, 0, 0, 0, TimeSpan.Zero);
    private static readonly Guid Org = Guid.NewGuid();
    private static readonly Guid User = Guid.NewGuid();

    private static BillingDocument Billing(decimal amount = 1000m) =>
        new(Guid.NewGuid(), Org, Guid.NewGuid(), Guid.NewGuid(), "BIL-1", BillingKind.Milestone, "งวด 1", amount, null, User, Now);

    [Fact]
    public void ReferenceHash_IsStable_AndDetectsTampering()
    {
        var billing = Billing();
        Assert.Equal(billing.ReferenceHash, billing.ComputeReferenceHash());
        var other = new BillingDocument(billing.Id, Org, billing.BranchId, billing.ProjectId, "BIL-1", BillingKind.Milestone, "งวด 1", 1000.01m, null, User, Now);
        Assert.NotEqual(billing.ReferenceHash, other.ReferenceHash);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    [InlineData(10.005)]
    public void Billing_RejectsInvalidAmounts(double amount)
    {
        Assert.Equal("BILLING_FIELD_INVALID", Assert.Throws<FinanceDomainException>(() => Billing((decimal)amount)).Code);
    }

    [Fact]
    public void Payments_MoveTheStatus_AndNeverOverpay()
    {
        var billing = Billing(1000m);
        billing.ApplyPayment(400m, Now);
        Assert.Equal((BillingStatus.PartiallyPaid, 600m), (billing.Status, billing.Outstanding));
        Assert.Equal("PAYMENT_EXCEEDS_OUTSTANDING", Assert.Throws<FinanceDomainException>(() => billing.ApplyPayment(600.01m, Now)).Code);
        billing.ApplyPayment(600m, Now);
        Assert.Equal(BillingStatus.Paid, billing.Status);
        billing.RemovePayment(600m, Now);
        Assert.Equal((BillingStatus.PartiallyPaid, 600m), (billing.Status, billing.Outstanding));
        Assert.Equal("BILLING_HAS_PAYMENTS", Assert.Throws<FinanceDomainException>(() => billing.Void("ผิด", User, Now)).Code);
    }

    [Fact]
    public void VoidedBilling_HasNoOutstanding_AndTakesNoPayments()
    {
        var billing = Billing();
        billing.Void("ผิดงวด", User, Now);
        Assert.Equal((BillingStatus.Voided, 0m), (billing.Status, billing.Outstanding));
        Assert.Equal("BILLING_INVALID_STATE", Assert.Throws<FinanceDomainException>(() => billing.ApplyPayment(10m, Now)).Code);
    }

    [Fact]
    public void Payment_RequiresReferenceMethod_AndNoFutureDate()
    {
        var today = new DateOnly(2026, 10, 4);
        Assert.Equal("PAYMENT_FIELD_INVALID", Assert.Throws<FinanceDomainException>(() => new Payment(Guid.NewGuid(), Org, Guid.NewGuid(), "PAY-1", 10m, "bitcoin", "R1", today, today, User, Now)).Code);
        Assert.Equal("PAYMENT_FIELD_INVALID", Assert.Throws<FinanceDomainException>(() => new Payment(Guid.NewGuid(), Org, Guid.NewGuid(), "PAY-1", 10m, "transfer", " ", today, today, User, Now)).Code);
        Assert.Equal("PAYMENT_FIELD_INVALID", Assert.Throws<FinanceDomainException>(() => new Payment(Guid.NewGuid(), Org, Guid.NewGuid(), "PAY-1", 10m, "transfer", "R1", today.AddDays(1), today, User, Now)).Code);
    }

    private static AccountingOutboxMessage Message() => new(Guid.NewGuid(), Org, "billing.issued:1", OutboxKind.BillingIssued, Guid.NewGuid(), "BIL-1", 1000m, "{}", Now);

    [Fact]
    public void Outbox_BacksOffExponentially_AndDiesAfterTheLastAttempt()
    {
        var message = Message();
        Assert.True(message.IsDue(Now));
        var expectedDelays = new[] { 2, 4, 8, 16 };
        for (var i = 0; i < 4; i++)
        {
            message.MarkFailed("HTTP_503", Now);
            Assert.Equal(OutboxStatus.Failed, message.Status);
            Assert.Equal(Now.AddMinutes(expectedDelays[i]), message.NextAttemptAtUtc);
            Assert.False(message.IsDue(Now));
        }

        message.MarkFailed("HTTP_503", Now);
        Assert.Equal((OutboxStatus.Dead, 5), (message.Status, message.Attempts));
        Assert.False(message.IsDue(Now.AddDays(1)));
        message.Requeue(Now);
        Assert.Equal((OutboxStatus.Pending, 0), (message.Status, message.Attempts));
    }

    [Fact]
    public void Outbox_Confirmation_IsIdempotent_AndRejectsADifferentOutcome()
    {
        var message = Message();
        message.MarkSent("ACC-1", Now);
        message.Confirm("ACC-1", 1000m, Now);
        var confirmedAt = message.ConfirmedAtUtc;
        message.Confirm("ACC-1", 1000m, Now.AddMinutes(5));
        Assert.Equal(confirmedAt, message.ConfirmedAtUtc);
        Assert.Equal("OUTBOX_CONFIRMATION_CONFLICT", Assert.Throws<FinanceDomainException>(() => message.Confirm("ACC-2", 1000m, Now)).Code);
        Assert.Equal("OUTBOX_CONFIRMATION_CONFLICT", Assert.Throws<FinanceDomainException>(() => message.Confirm("ACC-1", 999m, Now)).Code);
    }

    [Fact]
    public void Outbox_LongErrors_AreTruncated()
    {
        var message = Message();
        message.MarkFailed(new string('x', 900), Now);
        Assert.Equal(500, message.LastError!.Length);
    }
}
