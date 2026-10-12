using TanErp.Domain.Common;

namespace TanErp.Domain.IdentityAccess;

public sealed class ApprovalLimitDomainException : Exception
{
    public ApprovalLimitDomainException(string code, string message) : base(message) => Code = code;
    public string Code { get; }
}

/// <summary>Document types an approval limit can be defined for. <c>credit_note</c> is reserved for G-15 and is not enforced yet.</summary>
public static class ApprovalDocumentType
{
    public const string Estimate = "estimate";
    public const string PurchaseOrder = "purchase_order";
    public const string ChangeOrder = "change_order";
    public const string CreditNote = "credit_note";

    public static readonly IReadOnlyList<string> Enforced = new[] { Estimate, PurchaseOrder, ChangeOrder };

    public static bool IsEnforced(string? value) => value is not null && Enforced.Contains(value);
}

public static class ApprovalLimitRules
{
    public const string Currency = "THB";
    public const decimal MaxAmountCeiling = 9_999_999_999_999.99m;

    public static void ValidateCreate(string? documentType, string? currency, bool isUnlimited, decimal? maxAmount)
    {
        if (!ApprovalDocumentType.IsEnforced(documentType))
            throw Invalid("Document type must be estimate, purchase_order or change_order (credit_note is reserved).");
        if (!string.Equals(currency, Currency, StringComparison.Ordinal))
            throw Invalid($"Currency must be {Currency}.");
        ValidateAmount(isUnlimited, maxAmount);
    }

    public static void ValidateAmount(bool isUnlimited, decimal? maxAmount)
    {
        if (isUnlimited)
        {
            if (maxAmount is not null) throw Invalid("An unlimited approval limit cannot carry a maximum amount.");
            return;
        }

        if (maxAmount is null) throw Invalid("A maximum amount is required unless the limit is unlimited.");
        if (maxAmount < 0 || maxAmount > MaxAmountCeiling) throw Invalid($"The maximum amount must be between 0 and {MaxAmountCeiling}.");
        if (decimal.Round(maxAmount.Value, 2) != maxAmount.Value) throw Invalid("The maximum amount allows at most 2 decimal places.");
    }

    private static ApprovalLimitDomainException Invalid(string message) => new("REQUEST_VALIDATION_FAILED", message);
}

/// <summary>
/// The most a role may approve for one document type, branch (or organization-wide) and currency.
/// An inactive row is treated as if it did not exist (fail-closed).
/// </summary>
public class ApprovalLimit : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid RoleId { get; private set; }
    public string DocumentType { get; private set; } = string.Empty;
    public Guid? BranchId { get; private set; }
    public string Currency { get; private set; } = ApprovalLimitRules.Currency;
    public bool IsUnlimited { get; private set; }
    public decimal? MaxAmount { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public Guid RowVersion { get; private set; } = Guid.NewGuid();

    protected ApprovalLimit() { }

    private ApprovalLimit(Guid id) : base(id) { }

    public static ApprovalLimit Create(
        Guid id, Guid organizationId, Guid roleId, string documentType, Guid? branchId, string currency,
        bool isUnlimited, decimal? maxAmount, DateTimeOffset now)
    {
        if (organizationId == Guid.Empty) throw new ArgumentException("Organization ID cannot be empty.", nameof(organizationId));
        if (roleId == Guid.Empty) throw new ArgumentException("Role ID cannot be empty.", nameof(roleId));
        if (branchId == Guid.Empty) throw new ArgumentException("Branch ID cannot be empty.", nameof(branchId));
        ApprovalLimitRules.ValidateCreate(documentType, currency, isUnlimited, maxAmount);

        return new ApprovalLimit(id)
        {
            OrganizationId = organizationId,
            RoleId = roleId,
            DocumentType = documentType,
            BranchId = branchId,
            Currency = currency,
            IsUnlimited = isUnlimited,
            MaxAmount = maxAmount,
            IsActive = true,
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };
    }

    /// <summary>Role, document type, branch and currency form the row's identity and never change.</summary>
    public void Update(bool isUnlimited, decimal? maxAmount, bool isActive, DateTimeOffset now)
    {
        ApprovalLimitRules.ValidateAmount(isUnlimited, maxAmount);
        IsUnlimited = isUnlimited;
        MaxAmount = maxAmount;
        IsActive = isActive;
        UpdatedAtUtc = now;
        RowVersion = Guid.NewGuid();
    }
}
