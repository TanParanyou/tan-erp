using TanErp.Domain.Common;

namespace TanErp.Domain.Finance;

public class AccountMapping : Entity
{
    public Guid OrganizationId { get; private set; }
    public string Kind { get; private set; } = string.Empty;
    public string DebitAccountCode { get; private set; } = string.Empty;
    public string DebitAccountName { get; private set; } = string.Empty;
    public string CreditAccountCode { get; private set; } = string.Empty;
    public string CreditAccountName { get; private set; } = string.Empty;
    public Guid UpdatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public Guid RowVersion { get; private set; }

    protected AccountMapping() { }

    public AccountMapping(
        Guid id,
        Guid organizationId,
        string kind,
        string debitAccountCode,
        string debitAccountName,
        string creditAccountCode,
        string creditAccountName,
        Guid updatedByUserId,
        DateTimeOffset now) : base(id)
    {
        if (organizationId == Guid.Empty || updatedByUserId == Guid.Empty)
            throw new ArgumentException("Organization and actor are required.");
        if (!OutboxKind.All.Contains(kind))
            throw new FinanceDomainException("ACCOUNT_MAPPING_FIELD_INVALID", "Invalid outbox kind.");

        ValidateFields(debitAccountCode, debitAccountName, creditAccountCode, creditAccountName);

        OrganizationId = organizationId;
        Kind = kind;
        DebitAccountCode = debitAccountCode.Trim();
        DebitAccountName = debitAccountName.Trim();
        CreditAccountCode = creditAccountCode.Trim();
        CreditAccountName = creditAccountName.Trim();
        UpdatedByUserId = updatedByUserId;
        UpdatedAtUtc = now.ToUniversalTime();
        RowVersion = Guid.NewGuid();
    }

    public void Update(
        string debitAccountCode,
        string debitAccountName,
        string creditAccountCode,
        string creditAccountName,
        Guid actorUserId,
        DateTimeOffset now)
    {
        if (actorUserId == Guid.Empty) throw new ArgumentException("Actor is required.");
        ValidateFields(debitAccountCode, debitAccountName, creditAccountCode, creditAccountName);

        DebitAccountCode = debitAccountCode.Trim();
        DebitAccountName = debitAccountName.Trim();
        CreditAccountCode = creditAccountCode.Trim();
        CreditAccountName = creditAccountName.Trim();
        UpdatedByUserId = actorUserId;
        UpdatedAtUtc = now.ToUniversalTime();
        RowVersion = Guid.NewGuid();
    }

    private static void ValidateFields(string debitCode, string debitName, string creditCode, string creditName)
    {
        var dCode = debitCode?.Trim() ?? string.Empty;
        var dName = debitName?.Trim() ?? string.Empty;
        var cCode = creditCode?.Trim() ?? string.Empty;
        var cName = creditName?.Trim() ?? string.Empty;

        if (dCode.Length is 0 or > 32)
            throw new FinanceDomainException("ACCOUNT_MAPPING_FIELD_INVALID", "Debit account code must be 1-32 characters.");
        if (dName.Length is 0 or > 200)
            throw new FinanceDomainException("ACCOUNT_MAPPING_FIELD_INVALID", "Debit account name must be 1-200 characters.");
        if (cCode.Length is 0 or > 32)
            throw new FinanceDomainException("ACCOUNT_MAPPING_FIELD_INVALID", "Credit account code must be 1-32 characters.");
        if (cName.Length is 0 or > 200)
            throw new FinanceDomainException("ACCOUNT_MAPPING_FIELD_INVALID", "Credit account name must be 1-200 characters.");
    }
}
