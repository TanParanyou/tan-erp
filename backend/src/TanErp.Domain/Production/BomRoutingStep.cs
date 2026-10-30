using TanErp.Domain.Common;

namespace TanErp.Domain.Production;

public class BomRoutingStep : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid BomRevisionId { get; private set; }
    public int Sequence { get; private set; }
    public string OperationCode { get; private set; } = string.Empty;
    public string OperationName { get; private set; } = string.Empty;
    public Guid? WorkCenterId { get; private set; }
    public decimal SetupHours { get; private set; }
    public decimal RunHours { get; private set; }
    public bool IsSubcontracted { get; private set; }
    public decimal StdSubcontractCost { get; private set; }
    public string? Note { get; private set; }

    protected BomRoutingStep() { }

    public BomRoutingStep(
        Guid id,
        Guid organizationId,
        Guid bomRevisionId,
        int sequence,
        string operationCode,
        string operationName,
        Guid? workCenterId,
        decimal setupHours,
        decimal runHours,
        bool isSubcontracted,
        decimal stdSubcontractCost,
        string? note) : base(id)
    {
        if (organizationId == Guid.Empty || bomRevisionId == Guid.Empty)
        {
            throw new ArgumentException("Organization and BOM revision are required.");
        }

        if (sequence <= 0)
        {
            throw new ProductionDomainException("BOM_ROUTING_INVALID", "Routing step sequence must be greater than zero.");
        }

        var trimmedCode = operationCode?.Trim() ?? string.Empty;
        if (trimmedCode.Length == 0 || trimmedCode.Length > 64)
        {
            throw new ProductionDomainException("BOM_ROUTING_INVALID", "Operation code is required and cannot exceed 64 characters.");
        }

        var trimmedName = operationName?.Trim() ?? string.Empty;
        if (trimmedName.Length == 0 || trimmedName.Length > 200)
        {
            throw new ProductionDomainException("BOM_ROUTING_INVALID", "Operation name is required and cannot exceed 200 characters.");
        }

        if (setupHours < 0m || runHours < 0m || stdSubcontractCost < 0m)
        {
            throw new ProductionDomainException("BOM_ROUTING_INVALID", "Routing hours and standard subcontract cost cannot be negative.");
        }

        if (!isSubcontracted && (!workCenterId.HasValue || workCenterId.Value == Guid.Empty))
        {
            throw new ProductionDomainException("BOM_ROUTING_INVALID", "An internal routing step requires a work center.");
        }

        if (isSubcontracted && workCenterId.HasValue && workCenterId.Value != Guid.Empty)
        {
            throw new ProductionDomainException("BOM_ROUTING_INVALID", "A subcontracted routing step cannot be assigned to an internal work center.");
        }

        if (setupHours <= 0m && runHours <= 0m && !isSubcontracted)
        {
            throw new ProductionDomainException("BOM_ROUTING_INVALID", "A routing step must specify positive setup/run hours or be subcontracted.");
        }

        var trimmedNote = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        if (trimmedNote is { Length: > 500 })
        {
            throw new ProductionDomainException("BOM_ROUTING_INVALID", "Routing step note cannot exceed 500 characters.");
        }

        OrganizationId = organizationId;
        BomRevisionId = bomRevisionId;
        Sequence = sequence;
        OperationCode = trimmedCode.ToUpperInvariant();
        OperationName = trimmedName;
        WorkCenterId = workCenterId == Guid.Empty ? null : workCenterId;
        SetupHours = decimal.Round(setupHours, 4);
        RunHours = decimal.Round(runHours, 4);
        IsSubcontracted = isSubcontracted;
        StdSubcontractCost = decimal.Round(stdSubcontractCost, 2);
        Note = trimmedNote;
    }
}
