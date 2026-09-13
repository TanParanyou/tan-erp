namespace TanErp.Domain.Estimates;

public class EstimateSection
{
    private readonly List<EstimateWorkItem> _workItems = new();

    public Guid Id { get; private set; }
    public Guid OrganizationId { get; private set; }
    public Guid EstimateRevisionId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string NameTh { get; private set; } = string.Empty;
    public string? NameEn { get; private set; }
    public int SortOrder { get; private set; }
    public decimal SubtotalCost { get; private set; }
    public decimal SubtotalSellingPrice { get; private set; }

    public IReadOnlyCollection<EstimateWorkItem> WorkItems => _workItems.AsReadOnly();

    private EstimateSection() { }

    public EstimateSection(
        Guid id,
        Guid organizationId,
        Guid estimateRevisionId,
        string code,
        string nameTh,
        string? nameEn,
        int sortOrder = 1)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Section ID cannot be empty.", nameof(id));
        if (organizationId == Guid.Empty)
            throw new ArgumentException("Organization ID cannot be empty.", nameof(organizationId));
        if (estimateRevisionId == Guid.Empty)
            throw new ArgumentException("Revision ID cannot be empty.", nameof(estimateRevisionId));
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Section code cannot be empty.", nameof(code));
        if (string.IsNullOrWhiteSpace(nameTh))
            throw new ArgumentException("Thai section name cannot be empty.", nameof(nameTh));

        Id = id;
        OrganizationId = organizationId;
        EstimateRevisionId = estimateRevisionId;
        Code = code.Trim();
        NameTh = nameTh.Trim();
        NameEn = string.IsNullOrWhiteSpace(nameEn) ? null : nameEn.Trim();
        SortOrder = sortOrder;
    }

    public void AddWorkItem(EstimateWorkItem workItem)
    {
        ArgumentNullException.ThrowIfNull(workItem);
        _workItems.Add(workItem);
        Recalculate();
    }

    public void ClearWorkItems()
    {
        _workItems.Clear();
        Recalculate();
    }

    public void Update(string code, string nameTh, string? nameEn, int sortOrder)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Section code cannot be empty.", nameof(code));
        if (string.IsNullOrWhiteSpace(nameTh))
            throw new ArgumentException("Thai section name cannot be empty.", nameof(nameTh));

        Code = code.Trim();
        NameTh = nameTh.Trim();
        NameEn = string.IsNullOrWhiteSpace(nameEn) ? null : nameEn.Trim();
        SortOrder = sortOrder;
    }

    public void Recalculate()
    {
        foreach (var item in _workItems)
        {
            item.Recalculate();
        }

        SubtotalCost = decimal.Round(_workItems.Sum(w => w.TotalCost), 2, MidpointRounding.AwayFromZero);
        SubtotalSellingPrice = decimal.Round(_workItems.Sum(w => w.TotalSellingPrice), 2, MidpointRounding.AwayFromZero);
    }
}
