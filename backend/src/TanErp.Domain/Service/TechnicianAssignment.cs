using TanErp.Domain.Common;

namespace TanErp.Domain.Service;

public class TechnicianAssignment : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid TechnicianUserId { get; private set; }
    public DateOnly WorkDate { get; private set; }
    public string AssignmentKind { get; private set; } = string.Empty;
    public Guid? InstallationJobId { get; private set; }
    public Guid? ServiceRequestId { get; private set; }
    public string? Note { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }

    protected TechnicianAssignment() { }

    public TechnicianAssignment(
        Guid id,
        Guid organizationId,
        Guid branchId,
        Guid technicianUserId,
        DateOnly workDate,
        string assignmentKind,
        Guid? installationJobId,
        Guid? serviceRequestId,
        string? note,
        Guid createdByUserId,
        DateTimeOffset now) : base(id)
    {
        if (organizationId == Guid.Empty || branchId == Guid.Empty || technicianUserId == Guid.Empty || createdByUserId == Guid.Empty)
            throw new ArgumentException("Organization, branch, technician and actor are required.");

        var kind = assignmentKind?.Trim() ?? string.Empty;
        if (!TechnicianAssignmentKind.All.Contains(kind))
            throw new ServiceDomainException("SERVICE_FIELD_INVALID", "Assignment kind is invalid.");

        if (kind == TechnicianAssignmentKind.Installation)
        {
            if (!installationJobId.HasValue || serviceRequestId.HasValue)
                throw new ServiceDomainException("SERVICE_FIELD_INVALID", "Installation assignment requires an installation job ID and no service request ID.");
        }
        else if (kind == TechnicianAssignmentKind.ServiceRequest)
        {
            if (!serviceRequestId.HasValue || installationJobId.HasValue)
                throw new ServiceDomainException("SERVICE_FIELD_INVALID", "Service request assignment requires a service request ID and no installation job ID.");
        }

        var text = note?.Trim();
        if (text is { Length: > 500 })
            throw new ServiceDomainException("SERVICE_FIELD_INVALID", "Note cannot exceed 500 characters.");

        OrganizationId = organizationId;
        BranchId = branchId;
        TechnicianUserId = technicianUserId;
        WorkDate = workDate;
        AssignmentKind = kind;
        InstallationJobId = installationJobId;
        ServiceRequestId = serviceRequestId;
        Note = string.IsNullOrWhiteSpace(text) ? null : text;
        CreatedByUserId = createdByUserId;
        CreatedAtUtc = now.ToUniversalTime();
    }
}
