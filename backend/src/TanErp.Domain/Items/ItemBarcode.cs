using TanErp.Domain.Common;

namespace TanErp.Domain.Items;

public sealed class ItemBarcode : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid ItemId { get; private set; }
    public string IdentifierType { get; private set; } = string.Empty;
    public string Value { get; private set; } = string.Empty;
    public string NormalizedValue { get; private set; } = string.Empty;
    public Guid UnitId { get; private set; }
    public decimal QuantityInBaseUnit { get; private set; }
    public string PackagingLevel { get; private set; } = string.Empty;
    public bool IsPrimary { get; private set; }
    public string Status { get; private set; } = ItemStatus.Active;
    public Guid RowVersion { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public Guid UpdatedByUserId { get; private set; }

    public Item? Item { get; private set; }
    public UnitOfMeasure? Unit { get; private set; }

    private ItemBarcode() : base() { }

    public ItemBarcode(
        Guid id,
        Guid organizationId,
        Guid itemId,
        string identifierType,
        string value,
        Guid unitId,
        decimal quantityInBaseUnit,
        string packagingLevel,
        bool isPrimary,
        Guid actorUserId,
        DateTimeOffset now) : base(id)
    {
        if (organizationId == Guid.Empty || itemId == Guid.Empty || unitId == Guid.Empty)
            throw new ItemValidationException("ITEM_BARCODE_INVALID", "Organization, item and unit are required.");
        if (identifierType is not ("gtin" or "internal"))
            throw new ItemValidationException("ITEM_BARCODE_INVALID", "Barcode identifier type is invalid.");
        if (packagingLevel is not ("each" or "inner" or "case" or "pallet"))
            throw new ItemValidationException("ITEM_BARCODE_INVALID", "Barcode packaging level is invalid.");
        if (quantityInBaseUnit <= 0 || decimal.Round(quantityInBaseUnit, 4) != quantityInBaseUnit)
            throw new ItemValidationException("ITEM_BARCODE_INVALID", "Barcode base quantity must be positive with at most four decimal places.");

        var trimmedValue = value?.Trim() ?? string.Empty;
        if (identifierType == "gtin" && !HasValidGtinCheckDigit(trimmedValue))
            throw new ItemValidationException("ITEM_BARCODE_INVALID", "GTIN length or check digit is invalid.");
        if (identifierType == "internal" && (trimmedValue.Length is < 1 or > 64 ||
            !trimmedValue.All(c => char.IsAsciiLetterOrDigit(c) || c is '-' or '_' or '.')))
            throw new ItemValidationException("ITEM_BARCODE_INVALID", "Internal barcode must use 1–64 letters, digits, dash, underscore or period.");

        OrganizationId = organizationId;
        ItemId = itemId;
        IdentifierType = identifierType;
        Value = trimmedValue;
        NormalizedValue = NormalizeForLookup(trimmedValue);
        UnitId = unitId;
        QuantityInBaseUnit = quantityInBaseUnit;
        PackagingLevel = packagingLevel;
        IsPrimary = isPrimary;
        Status = ItemStatus.Active;
        RowVersion = Guid.NewGuid();
        CreatedAtUtc = now;
        CreatedByUserId = actorUserId;
        UpdatedAtUtc = now;
        UpdatedByUserId = actorUserId;
    }

    public void SetPrimary(Guid actorUserId, DateTimeOffset now)
    {
        if (Status != ItemStatus.Active)
            throw new ItemValidationException("ITEM_BARCODE_INACTIVE", "Inactive barcode cannot be primary.");
        IsPrimary = true;
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = now;
        UpdatedByUserId = actorUserId;
    }

    public void ClearPrimary(Guid actorUserId, DateTimeOffset now)
    {
        if (!IsPrimary) return;
        IsPrimary = false;
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = now;
        UpdatedByUserId = actorUserId;
    }

    public void Deactivate(Guid actorUserId, DateTimeOffset now)
    {
        if (Status != ItemStatus.Active)
            throw new ItemValidationException("ITEM_BARCODE_INACTIVE", "Barcode is already inactive.");
        Status = ItemStatus.Inactive;
        IsPrimary = false;
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = now;
        UpdatedByUserId = actorUserId;
    }

    private static bool HasValidGtinCheckDigit(string value)
    {
        if (value.Length is not (8 or 12 or 13 or 14) || !value.All(char.IsAsciiDigit)) return false;
        var sum = 0;
        for (var index = value.Length - 2; index >= 0; index--)
        {
            var offsetFromRight = value.Length - 2 - index;
            sum += (value[index] - '0') * (offsetFromRight % 2 == 0 ? 3 : 1);
        }
        return (10 - sum % 10) % 10 == value[^1] - '0';
    }

    public static string NormalizeForLookup(string value)
    {
        var trimmed = value.Trim();
        return trimmed.Length is 8 or 12 or 13 or 14 && trimmed.All(char.IsAsciiDigit)
            ? trimmed.PadLeft(14, '0')
            : trimmed.ToUpperInvariant();
    }
}
