using TanErp.Domain.Common;

namespace TanErp.Domain.Inventory;

public class InventoryPeriodClose : Entity
{
    private readonly List<InventoryPeriodCloseLine> _lines = new();

    public Guid OrganizationId { get; private set; }
    public int PeriodYear { get; private set; }
    public int PeriodMonth { get; private set; }
    public DateOnly PeriodEndDate { get; private set; }
    public string? Note { get; private set; }
    public int LineCount { get; private set; }
    public decimal TotalOnHandQuantity { get; private set; }
    public decimal TotalValuationAmount { get; private set; }
    public Guid ClosedByUserId { get; private set; }
    public DateTimeOffset ClosedAtUtc { get; private set; }

    public IReadOnlyCollection<InventoryPeriodCloseLine> Lines => _lines.AsReadOnly();

    protected InventoryPeriodClose() { }

    public InventoryPeriodClose(
        Guid id,
        Guid organizationId,
        int periodYear,
        int periodMonth,
        string? note,
        Guid closedByUserId,
        DateTimeOffset now) : base(id)
    {
        if (organizationId == Guid.Empty || closedByUserId == Guid.Empty)
        {
            throw new ArgumentException("Organization and actor are required.");
        }

        if (periodYear is < 2000 or > 2100 || periodMonth is < 1 or > 12)
        {
            throw new InventoryDomainException("INVENTORY_PERIOD_INVALID", "Period year must be between 2000 and 2100 and period month between 1 and 12.");
        }

        var trimmedNote = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        if (trimmedNote is { Length: > 500 })
        {
            throw new InventoryDomainException("INVENTORY_FIELD_INVALID", "Note cannot exceed 500 characters.");
        }

        OrganizationId = organizationId;
        PeriodYear = periodYear;
        PeriodMonth = periodMonth;
        PeriodEndDate = new DateOnly(periodYear, periodMonth, DateTime.DaysInMonth(periodYear, periodMonth));
        Note = trimmedNote;
        ClosedByUserId = closedByUserId;
        ClosedAtUtc = now.ToUniversalTime();
    }

    public void AddLine(InventoryPeriodCloseLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        _lines.Add(line);
        LineCount = _lines.Count;
        TotalOnHandQuantity = decimal.Round(_lines.Sum(l => l.OnHandQuantity), 4);
        TotalValuationAmount = decimal.Round(_lines.Sum(l => l.TotalValue), 2);
    }
}

public class InventoryPeriodCloseLine : Entity
{
    public Guid PeriodCloseId { get; private set; }
    public Guid WarehouseId { get; private set; }
    public Guid ItemId { get; private set; }
    public Guid UnitId { get; private set; }
    public decimal OnHandQuantity { get; private set; }
    public decimal AverageCost { get; private set; }
    public decimal TotalValue { get; private set; }

    protected InventoryPeriodCloseLine() { }

    public InventoryPeriodCloseLine(
        Guid id,
        Guid periodCloseId,
        Guid warehouseId,
        Guid itemId,
        Guid unitId,
        decimal onHandQuantity,
        decimal averageCost,
        decimal totalValue) : base(id)
    {
        PeriodCloseId = periodCloseId;
        WarehouseId = warehouseId;
        ItemId = itemId;
        UnitId = unitId;
        OnHandQuantity = decimal.Round(onHandQuantity, 4);
        AverageCost = decimal.Round(averageCost, 4);
        TotalValue = decimal.Round(totalValue, 2);
    }
}
