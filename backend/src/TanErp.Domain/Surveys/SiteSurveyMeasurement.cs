using TanErp.Domain.Common;

namespace TanErp.Domain.Surveys;

public class SiteSurveyMeasurement : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid SiteSurveyAreaId { get; private set; }
    public string MeasurementType { get; private set; } = string.Empty;
    public decimal Value { get; private set; }
    public string UnitCode { get; private set; } = string.Empty;
    public string CaptureMethod { get; private set; } = string.Empty;
    public string? Notes { get; private set; }
    public int SortOrder { get; private set; }

    protected SiteSurveyMeasurement() { }

    public SiteSurveyMeasurement(
        Guid id,
        Guid organizationId,
        Guid siteSurveyAreaId,
        string measurementType,
        decimal value,
        string unitCode,
        string captureMethod,
        string? notes,
        int sortOrder) : base(id)
    {
        if (organizationId == Guid.Empty) throw new ArgumentException("Organization ID cannot be empty.", nameof(organizationId));
        if (siteSurveyAreaId == Guid.Empty) throw new ArgumentException("Site survey area ID cannot be empty.", nameof(siteSurveyAreaId));
        if (!Surveys.MeasurementType.IsValid(measurementType)) throw new ArgumentException($"Invalid measurement type: '{measurementType}'.", nameof(measurementType));
        if (value <= 0) throw new ArgumentException("Measurement value must be strictly positive.", nameof(value));
        if (!MeasurementUnit.IsValid(unitCode)) throw new ArgumentException($"Invalid measurement unit: '{unitCode}'.", nameof(unitCode));
        if (!Surveys.CaptureMethod.IsValid(captureMethod)) throw new ArgumentException($"Invalid capture method: '{captureMethod}'.", nameof(captureMethod));

        OrganizationId = organizationId;
        SiteSurveyAreaId = siteSurveyAreaId;
        MeasurementType = measurementType.Trim().ToLowerInvariant();
        Value = value;
        UnitCode = unitCode.Trim().ToLowerInvariant();
        CaptureMethod = captureMethod.Trim().ToLowerInvariant();
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        SortOrder = sortOrder;
    }
}
