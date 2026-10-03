namespace TanErp.Domain.Finance;

public class FinanceDomainException : Exception
{
    public string Code { get; }

    public FinanceDomainException(string code, string message) : base(message)
    {
        Code = code;
    }
}

public static class BillingKind
{
    public const string Deposit = "deposit";
    public const string Milestone = "milestone";
    public const string Final = "final";
    public const string Other = "other";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal) { Deposit, Milestone, Final, Other };
}

public static class BillingStatus
{
    public const string Issued = "issued";
    public const string PartiallyPaid = "partially_paid";
    public const string Paid = "paid";
    public const string Voided = "voided";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal) { Issued, PartiallyPaid, Paid, Voided };
}

public static class PaymentMethod
{
    public const string Transfer = "transfer";
    public const string Cheque = "cheque";
    public const string Cash = "cash";
    public const string Card = "card";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal) { Transfer, Cheque, Cash, Card };
}

public static class PaymentStatus
{
    public const string Recorded = "recorded";
    public const string Reversed = "reversed";
}

public static class OutboxStatus
{
    public const string Pending = "pending";
    public const string Sent = "sent";
    public const string Failed = "failed";
    public const string Dead = "dead";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal) { Pending, Sent, Failed, Dead };
}

public static class OutboxKind
{
    public const string BillingIssued = "billing.issued";
    public const string BillingVoided = "billing.voided";
    public const string PaymentRecorded = "payment.recorded";
    public const string PaymentReversed = "payment.reversed";
}
