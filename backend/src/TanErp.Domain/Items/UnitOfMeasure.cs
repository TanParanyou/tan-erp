using TanErp.Domain.Common;

namespace TanErp.Domain.Items;

public class UnitOfMeasure : Entity
{
    public Guid OrganizationId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string NormalizedCode { get; private set; } = string.Empty;
    public LocalizedText Name { get; private set; } = null!;
    public string Symbol { get; private set; } = string.Empty;
    public string Dimension { get; private set; } = "count";
    public int DecimalScale { get; private set; } = 4;
    public string RoundingMode { get; private set; } = "half_up";
    public string Status { get; private set; } = ItemStatus.Active;
    public Guid RowVersion { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public Guid UpdatedByUserId { get; private set; }

    protected UnitOfMeasure() : base() { }

    public UnitOfMeasure(
        Guid id,
        Guid organizationId,
        string code,
        LocalizedText name,
        string symbol,
        string dimension,
        int decimalScale,
        string roundingMode,
        Guid createdByUserId,
        DateTimeOffset createdAtUtc) : base(id)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ItemValidationException("ITEM_FIELD_REQUIRED", "Unit code is required.");
        }

        if (string.IsNullOrWhiteSpace(symbol))
        {
            throw new ItemValidationException("ITEM_FIELD_REQUIRED", "Unit symbol is required.");
        }

        OrganizationId = organizationId;
        Code = code.Trim();
        NormalizedCode = code.Trim().ToUpperInvariant();
        Name = name;
        Symbol = symbol.Trim();
        Dimension = string.IsNullOrWhiteSpace(dimension) ? "count" : dimension.Trim().ToLowerInvariant();
        DecimalScale = Math.Clamp(decimalScale, 0, 6);
        RoundingMode = string.IsNullOrWhiteSpace(roundingMode) ? "half_up" : roundingMode.Trim().ToLowerInvariant();
        Status = ItemStatus.Active;
        RowVersion = Guid.NewGuid();
        CreatedAtUtc = createdAtUtc;
        CreatedByUserId = createdByUserId;
        UpdatedAtUtc = createdAtUtc;
        UpdatedByUserId = createdByUserId;
    }

    public void Update(
        string code,
        LocalizedText name,
        string symbol,
        string dimension,
        int decimalScale,
        string roundingMode,
        Guid updatedByUserId,
        DateTimeOffset updatedAtUtc)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ItemValidationException("ITEM_FIELD_REQUIRED", "Unit code is required.");
        }

        if (string.IsNullOrWhiteSpace(symbol))
        {
            throw new ItemValidationException("ITEM_FIELD_REQUIRED", "Unit symbol is required.");
        }

        Code = code.Trim();
        NormalizedCode = code.Trim().ToUpperInvariant();
        Name = name;
        Symbol = symbol.Trim();
        Dimension = string.IsNullOrWhiteSpace(dimension) ? "count" : dimension.Trim().ToLowerInvariant();
        DecimalScale = Math.Clamp(decimalScale, 0, 6);
        RoundingMode = string.IsNullOrWhiteSpace(roundingMode) ? "half_up" : roundingMode.Trim().ToLowerInvariant();
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = updatedAtUtc;
        UpdatedByUserId = updatedByUserId;
    }
}
