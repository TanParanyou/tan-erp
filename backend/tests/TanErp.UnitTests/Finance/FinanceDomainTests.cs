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
    public void Outbox_Confirmation_TruncatesTimestampToDatabaseMicrosecondPrecision()
    {
        var message = Message();
        message.MarkSent("ACC-1", Now);
        message.Confirm("ACC-1", 1000m, Now.AddTicks(7));
        Assert.Equal(0, message.ConfirmedAtUtc!.Value.Ticks % (TimeSpan.TicksPerMillisecond / 1000));
        Assert.Equal(Now, message.ConfirmedAtUtc);
    }

    [Fact]
    public void Outbox_LongErrors_AreTruncated()
    {
        var message = Message();
        message.MarkFailed(new string('x', 900), Now);
        Assert.Equal(500, message.LastError!.Length);
    }

    [Fact]
    public void VatAndLines_CalculatesAndRoundsAwayFromZero_AndReferenceHashIncludesTax()
    {
        var branchId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var billingId = Guid.NewGuid();

        // 1. Direct VAT without lines
        var billingVatOnly = new BillingDocument(
            billingId, Org, branchId, projectId, "BIL-100", BillingKind.Milestone, "Direct VAT",
            100m, null, User, Now, vatRate: 0.07m, retentionRate: 0m, taxInvoiceNumber: "TIV-2026-0001");
        Assert.Equal(7.00m, billingVatOnly.VatAmount);
        Assert.Equal(107.00m, billingVatOnly.GrossTotal);
        Assert.Equal("TIV-2026-0001", billingVatOnly.TaxInvoiceNumber);

        // 2. VAT with lines
        var line1 = new BillingLine(Guid.NewGuid(), Org, billingId, 1, "Line 1", 33.33m, 0.07m);
        // 33.33 * 0.07 = 2.3331 => AwayFromZero => 2.33
        Assert.Equal(2.33m, line1.VatAmount);
        Assert.Equal(35.66m, line1.TotalAmount);

        var line2 = new BillingLine(Guid.NewGuid(), Org, billingId, 2, "Line 2", 66.67m, 0.07m);
        // 66.67 * 0.07 = 4.6669 => AwayFromZero => 4.67
        Assert.Equal(4.67m, line2.VatAmount);
        Assert.Equal(71.34m, line2.TotalAmount);

        var billingWithLines = new BillingDocument(
            billingId, Org, branchId, projectId, "BIL-101", BillingKind.Milestone, "With Lines",
            100m, null, User, Now, vatRate: 0.07m, retentionRate: 0m, taxInvoiceNumber: "TIV-2026-0002",
            lines: new[] { line1, line2 });
        Assert.Equal(7.00m, billingWithLines.VatAmount);
        Assert.Equal(107.00m, billingWithLines.GrossTotal);
        Assert.Equal(2, billingWithLines.Lines.Count);

        // Line total mismatch
        var badLines = new[] { line1 };
        Assert.Equal("BILLING_FIELD_INVALID", Assert.Throws<FinanceDomainException>(() =>
            new BillingDocument(billingId, Org, branchId, projectId, "BIL-BAD", BillingKind.Milestone, "Mismatch",
                100m, null, User, Now, vatRate: 0.07m, lines: badLines)).Code);

        // Legacy reference hash stability
        var legacy = Billing(1000m);
        var expectedLegacyHash = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes($"{Org}|{legacy.ProjectId}|BIL-1|milestone|1000.00|THB|{Now:O}"))).ToLowerInvariant();
        Assert.Equal(expectedLegacyHash, legacy.ReferenceHash);
        Assert.Equal(legacy.ReferenceHash, legacy.ComputeReferenceHash());

        // Tax-enabled reference hash
        var expectedTaxHash = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes($"{Org}|{projectId}|BIL-100|milestone|100.00|THB|{Now:O}|0.0700|7.00|0.0000|0.00|TIV-2026-0001"))).ToLowerInvariant();
        Assert.Equal(expectedTaxHash, billingVatOnly.ReferenceHash);
    }

    [Fact]
    public void Payment_WhtCalculation_Requires50BisCertificate_WhenWhtApplied()
    {
        var today = new DateOnly(2026, 10, 4);
        var billingId = Guid.NewGuid();

        // WHT > 0 without certificate throws
        Assert.Equal("PAYMENT_WHT_CERTIFICATE_REQUIRED", Assert.Throws<FinanceDomainException>(() =>
            new Payment(Guid.NewGuid(), Org, billingId, "PAY-1", 1000m, PaymentMethod.Transfer, "REF-1",
                today, today, User, Now, whtRate: 0.03m, whtType: "service_3", whtCertificateNumber: null)).Code);

        Assert.Equal("PAYMENT_WHT_CERTIFICATE_REQUIRED", Assert.Throws<FinanceDomainException>(() =>
            new Payment(Guid.NewGuid(), Org, billingId, "PAY-1", 1000m, PaymentMethod.Transfer, "REF-1",
                today, today, User, Now, whtRate: 0.03m, whtType: "service_3", whtCertificateNumber: "   ")).Code);

        // Valid WHT payment
        var payment = new Payment(Guid.NewGuid(), Org, billingId, "PAY-1", 1000m, PaymentMethod.Transfer, "REF-1",
            today, today, User, Now, whtRate: 0.03m, whtType: "service_3", whtCertificateNumber: "50BIS-2026-0001", receiptNumber: "RCT-2026-0001");
        Assert.Equal(30.00m, payment.WhtAmount);
        Assert.Equal(970.00m, payment.NetCashReceived);
        Assert.Equal("50BIS-2026-0001", payment.WhtCertificateNumber);
        Assert.Equal("RCT-2026-0001", payment.ReceiptNumber);

        // Invalid WHT rate
        Assert.Equal("PAYMENT_FIELD_INVALID", Assert.Throws<FinanceDomainException>(() =>
            new Payment(Guid.NewGuid(), Org, billingId, "PAY-1", 1000m, PaymentMethod.Transfer, "REF-1",
                today, today, User, Now, whtRate: 0.35m, whtCertificateNumber: "50BIS-1")).Code);
    }

    [Fact]
    public void Retention_Held_ReducesOutstanding_AndOpensWhenReleased()
    {
        var billing = new BillingDocument(
            Guid.NewGuid(), Org, Guid.NewGuid(), Guid.NewGuid(), "BIL-RET", BillingKind.Milestone, "Retention Test",
            1000m, null, User, Now, vatRate: 0.07m, retentionRate: 0.10m);

        Assert.Equal(1070m, billing.GrossTotal);
        Assert.Equal(100m, billing.RetentionAmount);
        Assert.Equal(RetentionHoldingStatus.Held, billing.RetentionStatus);
        Assert.Equal(970m, billing.EffectiveReceivable);
        Assert.Equal(970m, billing.Outstanding);

        // Overpaying before retention release is rejected
        Assert.Equal("PAYMENT_EXCEEDS_OUTSTANDING", Assert.Throws<FinanceDomainException>(() =>
            billing.ApplyPayment(970.01m, Now)).Code);

        // Pay exactly the effective receivable
        billing.ApplyPayment(970m, Now);
        Assert.Equal(BillingStatus.PartiallyPaid, billing.Status);
        Assert.Equal(0m, billing.Outstanding);

        // Release retention
        billing.ReleaseRetention(User, Now);
        Assert.Equal(RetentionHoldingStatus.Released, billing.RetentionStatus);
        Assert.Equal(1070m, billing.EffectiveReceivable);
        Assert.Equal(100m, billing.Outstanding);

        // Pay remaining retention
        billing.ApplyPayment(100m, Now);
        Assert.Equal(1070m, billing.PaidAmount);
        Assert.Equal(BillingStatus.Paid, billing.Status);
        Assert.Equal(0m, billing.Outstanding);

        // Releasing again throws
        Assert.Equal("BILLING_INVALID_STATE", Assert.Throws<FinanceDomainException>(() =>
            billing.ReleaseRetention(User, Now)).Code);
    }

    [Fact]
    public void CreditAndDebitNotes_MakerChecker_AndInvariantEnforcement()
    {
        var branchId = Guid.NewGuid();
        var billing = new BillingDocument(
            Guid.NewGuid(), Org, branchId, Guid.NewGuid(), "BIL-NOTES", BillingKind.Milestone, "Notes Test",
            1000m, null, User, Now, vatRate: 0.07m);
        Assert.Equal(1070m, billing.GrossTotal);

        var approver = Guid.NewGuid();

        // 1. Credit Note with Maker-Checker
        var cn = new BillingAdjustmentNote(
            Guid.NewGuid(), Org, branchId, billing.Id, "CN-2026-0001",
            BillingNoteType.Credit, "Discount agreed", 100m, 0.07m, User, Now);
        Assert.Equal(7m, cn.VatDelta);
        Assert.Equal(107m, cn.TotalDelta);
        Assert.Equal(BillingNoteStatus.PendingApproval, cn.Status);

        // Self-approval rejected
        Assert.Equal("BILLING_NOTE_SELF_APPROVAL", Assert.Throws<FinanceDomainException>(() =>
            cn.Approve(User, Now)).Code);

        // Independent checker approves
        cn.Approve(approver, Now);
        Assert.Equal(BillingNoteStatus.Approved, cn.Status);
        Assert.Equal(approver, cn.DecidedByUserId);

        billing.ApplyApprovedNote(cn, Now);
        Assert.Equal(100m, billing.CreditNoteBaseAmount);
        Assert.Equal(7m, billing.CreditNoteVatAmount);
        Assert.Equal(963m, billing.GrossTotal);
        Assert.Equal(963m, billing.Outstanding);

        // 2. Pay 900m
        billing.ApplyPayment(900m, Now);
        Assert.Equal(63m, billing.Outstanding);

        // Credit note that would reduce gross total below paid amount (900m) is rejected
        // remaining gross is 963, if credit note total is 107 (tax base 100), new gross would be 856 < 900
        var excessiveCn = new BillingAdjustmentNote(
            Guid.NewGuid(), Org, branchId, billing.Id, "CN-2026-0002",
            BillingNoteType.Credit, "Too big", 100m, 0.07m, User, Now);
        excessiveCn.Approve(approver, Now);
        Assert.Equal("BILLING_NOTE_EXCEEDS_ORIGINAL", Assert.Throws<FinanceDomainException>(() =>
            billing.ApplyApprovedNote(excessiveCn, Now)).Code);

        // 3. Debit note increases amount
        var dn = new BillingAdjustmentNote(
            Guid.NewGuid(), Org, branchId, billing.Id, "DN-2026-0001",
            BillingNoteType.Debit, "Additional work", 200m, 0.07m, User, Now);
        dn.Approve(approver, Now);
        billing.ApplyApprovedNote(dn, Now);

        Assert.Equal(200m, billing.DebitNoteBaseAmount);
        Assert.Equal(14m, billing.DebitNoteVatAmount);
        Assert.Equal(963m + 214m, billing.GrossTotal); // 1177m
        Assert.Equal(1177m - 900m, billing.Outstanding); // 277m
    }

    [Theory]
    [InlineData(-5, ArAgingBucket.Current)]
    [InlineData(0, ArAgingBucket.Current)]
    [InlineData(1, ArAgingBucket.Bucket1To30)]
    [InlineData(30, ArAgingBucket.Bucket1To30)]
    [InlineData(31, ArAgingBucket.Bucket31To60)]
    [InlineData(60, ArAgingBucket.Bucket31To60)]
    [InlineData(61, ArAgingBucket.Bucket61To90)]
    [InlineData(90, ArAgingBucket.Bucket61To90)]
    [InlineData(91, ArAgingBucket.BucketOver90)]
    [InlineData(150, ArAgingBucket.BucketOver90)]
    public void ArAgingCalculator_ClassifiesBucketsCorrectly(int daysOverdue, string expectedBucket)
    {
        Assert.Equal(expectedBucket, ArAgingCalculator.ClassifyAgingBucket(daysOverdue));
    }

    [Fact]
    public void ArAgingCalculator_CalculatesDaysOverdue_WithOrWithoutDueDate()
    {
        var asOf = new DateOnly(2026, 10, 30);
        var issued = new DateTimeOffset(2026, 10, 1, 0, 0, 0, TimeSpan.Zero);

        // Case 1: explicit DueDate
        var withDueDate = ArAgingCalculator.CalculateDaysOverdue(new DateOnly(2026, 10, 20), issued, 30, asOf);
        Assert.Equal(10, withDueDate); // 30 - 20 = 10 days overdue

        // Case 2: fallback to credit terms: issued 2026-10-01 + 30 days = 2026-10-31
        var fallbackTerm = ArAgingCalculator.CalculateDaysOverdue(null, issued, 30, asOf);
        Assert.Equal(-1, fallbackTerm); // 30 - 31 = -1 (current / not yet due)
    }

    [Fact]
    public void CreditLimitOverride_MakerChecker_AndExpiration()
    {
        var branchId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var creator = Guid.NewGuid();
        var approver = Guid.NewGuid();
        var today = new DateOnly(2026, 10, 30);

        // Invalid fields rejected
        Assert.Equal("CREDIT_OVERRIDE_FIELD_INVALID", Assert.Throws<FinanceDomainException>(() =>
            new CreditLimitOverrideRequest(Guid.NewGuid(), Org, branchId, customerId, -100m, "Reason", null, creator, Now)).Code);
        Assert.Equal("CREDIT_OVERRIDE_FIELD_INVALID", Assert.Throws<FinanceDomainException>(() =>
            new CreditLimitOverrideRequest(Guid.NewGuid(), Org, branchId, customerId, 500_000m, " ", null, creator, Now)).Code);

        // Valid request created
        var req = new CreditLimitOverrideRequest(
            Guid.NewGuid(), Org, branchId, customerId, 500_000m, "Special project credit", today.AddDays(7), creator, Now);
        Assert.Equal(CreditOverrideStatus.PendingApproval, req.Status);
        Assert.False(req.IsEffectiveOn(today));

        // Self approval rejected
        Assert.Equal("CREDIT_OVERRIDE_SELF_APPROVAL", Assert.Throws<FinanceDomainException>(() =>
            req.Approve(creator, Now)).Code);

        // Independent checker approves
        req.Approve(approver, Now, "Approved by CFO");
        Assert.Equal(CreditOverrideStatus.Approved, req.Status);
        Assert.Equal(approver, req.DecidedByUserId);
        Assert.Equal("Approved by CFO", req.DecisionNote);

        // Effective during valid window
        Assert.True(req.IsEffectiveOn(today));
        Assert.True(req.IsEffectiveOn(today.AddDays(7)));
        Assert.False(req.IsEffectiveOn(today.AddDays(8))); // expired

        // Cannot approve again
        Assert.Equal("CREDIT_OVERRIDE_INVALID_STATE", Assert.Throws<FinanceDomainException>(() =>
            req.Approve(approver, Now)).Code);
    }

    [Fact]
    public void ComputeEffectiveCeiling_RespectsActiveOverrides_AndCustomerLimit()
    {
        var branchId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var creator = Guid.NewGuid();
        var approver = Guid.NewGuid();
        var today = new DateOnly(2026, 10, 30);

        // 1. Customer has no credit limit
        Assert.Null(ArAgingCalculator.ComputeEffectiveCeiling(null, Array.Empty<CreditLimitOverrideRequest>(), today));

        // 2. Customer has limit 200,000, no active overrides
        var limit = 200_000m;
        Assert.Equal(200_000m, ArAgingCalculator.ComputeEffectiveCeiling(limit, Array.Empty<CreditLimitOverrideRequest>(), today));

        // 3. Pending override does not increase ceiling
        var pending = new CreditLimitOverrideRequest(Guid.NewGuid(), Org, branchId, customerId, 600_000m, "Pending", null, creator, Now);
        Assert.Equal(200_000m, ArAgingCalculator.ComputeEffectiveCeiling(limit, new[] { pending }, today));

        // 4. Approved active override increases ceiling
        var approved = new CreditLimitOverrideRequest(Guid.NewGuid(), Org, branchId, customerId, 600_000m, "Approved", today.AddDays(5), creator, Now);
        approved.Approve(approver, Now);
        Assert.Equal(600_000m, ArAgingCalculator.ComputeEffectiveCeiling(limit, new[] { approved }, today));

        // 5. Expired override falls back to customer limit
        Assert.Equal(200_000m, ArAgingCalculator.ComputeEffectiveCeiling(limit, new[] { approved }, today.AddDays(6)));
    }

    // ===== Wave D G-17 Journal Export & Account Mapping Tests =========================================

    [Fact]
    public void AccountMapping_ValidatesFields_AndUpdatesCorrectly()
    {
        // 1. Invalid kind rejected
        Assert.Equal("ACCOUNT_MAPPING_FIELD_INVALID", Assert.Throws<FinanceDomainException>(() =>
            new AccountMapping(Guid.NewGuid(), Org, "invalid.kind", "1130", "AR", "4100", "Sales", User, Now)).Code);

        // 2. Empty code/name rejected
        Assert.Equal("ACCOUNT_MAPPING_FIELD_INVALID", Assert.Throws<FinanceDomainException>(() =>
            new AccountMapping(Guid.NewGuid(), Org, OutboxKind.BillingIssued, "", "AR", "4100", "Sales", User, Now)).Code);
        Assert.Equal("ACCOUNT_MAPPING_FIELD_INVALID", Assert.Throws<FinanceDomainException>(() =>
            new AccountMapping(Guid.NewGuid(), Org, OutboxKind.BillingIssued, "1130", "", "4100", "Sales", User, Now)).Code);
        Assert.Equal("ACCOUNT_MAPPING_FIELD_INVALID", Assert.Throws<FinanceDomainException>(() =>
            new AccountMapping(Guid.NewGuid(), Org, OutboxKind.BillingIssued, "1130", "AR", "", "Sales", User, Now)).Code);
        Assert.Equal("ACCOUNT_MAPPING_FIELD_INVALID", Assert.Throws<FinanceDomainException>(() =>
            new AccountMapping(Guid.NewGuid(), Org, OutboxKind.BillingIssued, "1130", "AR", "4100", "", User, Now)).Code);

        // 3. Valid creation
        var mapping = new AccountMapping(
            Guid.NewGuid(), Org, OutboxKind.BillingIssued, "1130", "Accounts Receivable", "4100", "Service Revenue", User, Now);
        Assert.Equal(OutboxKind.BillingIssued, mapping.Kind);
        Assert.Equal("1130", mapping.DebitAccountCode);
        Assert.Equal("Accounts Receivable", mapping.DebitAccountName);
        Assert.Equal("4100", mapping.CreditAccountCode);
        Assert.Equal("Service Revenue", mapping.CreditAccountName);

        // 4. Update
        var updatedActor = Guid.NewGuid();
        var updateTime = Now.AddHours(2);
        mapping.Update("1131", "AR Branch", "4101", "Revenue Branch", updatedActor, updateTime);
        Assert.Equal("1131", mapping.DebitAccountCode);
        Assert.Equal("AR Branch", mapping.DebitAccountName);
        Assert.Equal("4101", mapping.CreditAccountCode);
        Assert.Equal("Revenue Branch", mapping.CreditAccountName);
        Assert.Equal(updatedActor, mapping.UpdatedByUserId);
        Assert.Equal(updateTime.ToUniversalTime(), mapping.UpdatedAtUtc);
    }

    [Fact]
    public void AccountingOutboxMessage_MarkExported_AndMarkDeadMissingMapping()
    {
        var message = Message();
        Assert.True(message.IsDue(Now));

        // 1. MarkExported transitions status to exported, externalRef = batchRef, not due
        message.MarkExported("JRN-20261030-ABC123", Now);
        Assert.Equal(OutboxStatus.Exported, message.Status);
        Assert.Equal("JRN-20261030-ABC123", message.ExternalRef);
        Assert.Equal(1, message.Attempts);
        Assert.False(message.IsDue(Now)); // Not due, so won't be exported again!
        Assert.False(message.IsDue(Now.AddDays(30)));

        // 2. Confirmation of an exported message updates status to sent
        message.Confirm("ACC-EXT-01", 1000m, Now.AddMinutes(10));
        Assert.Equal(OutboxStatus.Sent, message.Status);
        Assert.Equal("ACC-EXT-01", message.ExternalRef);

        // 3. Missing mapping marks as dead immediately
        var msg2 = Message();
        msg2.MarkDeadMissingMapping(OutboxKind.PaymentRecorded, Now);
        Assert.Equal(OutboxStatus.Dead, msg2.Status);
        Assert.Equal("ACCOUNT_MAPPING_MISSING:payment.recorded", msg2.LastError);
        Assert.Equal(AccountingOutboxMessage.MaxAttempts, msg2.Attempts);
        Assert.False(msg2.IsDue(Now));

        // 4. Can be requeued once mapping is configured
        msg2.Requeue(Now.AddMinutes(5));
        Assert.Equal(OutboxStatus.Pending, msg2.Status);
        Assert.Equal(0, msg2.Attempts);
        Assert.True(msg2.IsDue(Now.AddMinutes(5)));
    }
}
