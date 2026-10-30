using TanErp.Domain.Common;

namespace TanErp.Domain.Finance;

public class CreditLimitOverrideRequest : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid CustomerId { get; private set; }
    public decimal RequestedCeilingAmount { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public DateOnly? ExpiresOn { get; private set; }
    public string Status { get; private set; } = CreditOverrideStatus.PendingApproval;
    public Guid CreatedByUserId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public Guid? DecidedByUserId { get; private set; }
    public DateTimeOffset? DecidedAtUtc { get; private set; }
    public string? DecisionNote { get; private set; }
    public Guid RowVersion { get; private set; }

    protected CreditLimitOverrideRequest() { }

    public CreditLimitOverrideRequest(
        Guid id,
        Guid organizationId,
        Guid branchId,
        Guid customerId,
        decimal requestedCeilingAmount,
        string reason,
        DateOnly? expiresOn,
        Guid createdByUserId,
        DateTimeOffset now) : base(id)
    {
        if (organizationId == Guid.Empty || branchId == Guid.Empty || customerId == Guid.Empty || createdByUserId == Guid.Empty)
            throw new ArgumentException("Organization, branch, customer, and creator are required.");
        if (requestedCeilingAmount <= 0m || requestedCeilingAmount > 999_999_999m || decimal.Round(requestedCeilingAmount, 2) != requestedCeilingAmount)
            throw new FinanceDomainException("CREDIT_OVERRIDE_FIELD_INVALID", "Requested ceiling amount must be positive with at most two decimals.");
        var text = reason?.Trim() ?? string.Empty;
        if (text.Length is 0 or > 500)
            throw new FinanceDomainException("CREDIT_OVERRIDE_FIELD_INVALID", "A reason of 1-500 characters is required.");

        OrganizationId = organizationId;
        BranchId = branchId;
        CustomerId = customerId;
        RequestedCeilingAmount = requestedCeilingAmount;
        Reason = text;
        ExpiresOn = expiresOn;
        Status = CreditOverrideStatus.PendingApproval;
        CreatedByUserId = createdByUserId;
        CreatedAtUtc = now.ToUniversalTime();
        RowVersion = Guid.NewGuid();
    }

    public void Approve(Guid approverUserId, DateTimeOffset now, string? note = null)
    {
        if (Status != CreditOverrideStatus.PendingApproval)
            throw new FinanceDomainException("CREDIT_OVERRIDE_INVALID_STATE", "Only pending requests can be approved.");
        if (approverUserId == CreatedByUserId)
            throw new FinanceDomainException("CREDIT_OVERRIDE_SELF_APPROVAL", "Creator cannot approve their own credit override request.");

        Status = CreditOverrideStatus.Approved;
        DecidedByUserId = approverUserId;
        DecidedAtUtc = now.ToUniversalTime();
        DecisionNote = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        RowVersion = Guid.NewGuid();
    }

    public void Reject(Guid deciderUserId, DateTimeOffset now, string? note = null)
    {
        if (Status != CreditOverrideStatus.PendingApproval)
            throw new FinanceDomainException("CREDIT_OVERRIDE_INVALID_STATE", "Only pending requests can be rejected.");
        if (deciderUserId == CreatedByUserId)
            throw new FinanceDomainException("CREDIT_OVERRIDE_SELF_APPROVAL", "Creator cannot reject their own credit override request.");

        Status = CreditOverrideStatus.Rejected;
        DecidedByUserId = deciderUserId;
        DecidedAtUtc = now.ToUniversalTime();
        DecisionNote = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        RowVersion = Guid.NewGuid();
    }

    public bool IsEffectiveOn(DateOnly date)
    {
        return Status == CreditOverrideStatus.Approved && (!ExpiresOn.HasValue || ExpiresOn.Value >= date);
    }
}

public static class ArAgingBucket
{
    public const string Current = "current";
    public const string Bucket1To30 = "1_30";
    public const string Bucket31To60 = "31_60";
    public const string Bucket61To90 = "61_90";
    public const string BucketOver90 = "over_90";

    public static readonly IReadOnlyList<string> All = new[]
    {
        Current, Bucket1To30, Bucket31To60, Bucket61To90, BucketOver90
    };
}

public static class ArAgingCalculator
{
    public static string ClassifyAgingBucket(int daysOverdue)
    {
        if (daysOverdue <= 0) return ArAgingBucket.Current;
        if (daysOverdue <= 30) return ArAgingBucket.Bucket1To30;
        if (daysOverdue <= 60) return ArAgingBucket.Bucket31To60;
        if (daysOverdue <= 90) return ArAgingBucket.Bucket61To90;
        return ArAgingBucket.BucketOver90;
    }

    public static int CalculateDaysOverdue(DateOnly? dueDate, DateTimeOffset issuedAtUtc, int customerCreditTermDays, DateOnly asOfDate)
    {
        var effectiveDueDate = dueDate ?? DateOnly.FromDateTime(issuedAtUtc.UtcDateTime).AddDays(Math.Max(0, customerCreditTermDays));
        return asOfDate.DayNumber - effectiveDueDate.DayNumber;
    }

    public static decimal? ComputeEffectiveCeiling(
        decimal? customerCreditLimit,
        IEnumerable<CreditLimitOverrideRequest> overrides,
        DateOnly asOfDate)
    {
        if (!customerCreditLimit.HasValue)
        {
            return null;
        }

        var activeOverrides = overrides.Where(o => o.IsEffectiveOn(asOfDate)).ToList();
        if (activeOverrides.Count == 0)
        {
            return customerCreditLimit.Value;
        }

        return Math.Max(customerCreditLimit.Value, activeOverrides.Max(o => o.RequestedCeilingAmount));
    }
}
