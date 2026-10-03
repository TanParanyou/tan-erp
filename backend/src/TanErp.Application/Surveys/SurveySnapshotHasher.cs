using System.Globalization;
using System.Text.Json;
using TanErp.Application.Common.Security;
using TanErp.Domain.Surveys;

namespace TanErp.Application.Surveys;

public static class SurveySnapshotHasher
{
    public static string Compute(string surveyNumber, SiteSurveyRevision revision)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(surveyNumber);
        ArgumentNullException.ThrowIfNull(revision);

        var canonicalPayload = JsonSerializer.Serialize(new
        {
            surveyNumber,
            revision.RevisionNumber,
            revision.SurveyTemplateVersion,
            visitedAtUtc = revision.VisitedAtUtc?.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture),
            revision.ScopeSummary,
            assumptions = revision.Assumptions.OrderBy(value => value, StringComparer.Ordinal).ToArray(),
            constraints = revision.Constraints.OrderBy(value => value, StringComparer.Ordinal).ToArray(),
            missingDetails = revision.MissingDetails.OrderBy(value => value, StringComparer.Ordinal).ToArray(),
            areas = revision.Areas
                .OrderBy(area => area.SortOrder)
                .ThenBy(area => area.Code, StringComparer.Ordinal)
                .Select(area => new
                {
                    area.Code,
                    area.Name,
                    area.Description,
                    area.SortOrder,
                    measurements = area.Measurements
                        .OrderBy(measurement => measurement.SortOrder)
                        .ThenBy(measurement => measurement.MeasurementType, StringComparer.Ordinal)
                        .ThenBy(measurement => measurement.UnitCode, StringComparer.Ordinal)
                        .ThenBy(measurement => measurement.Value)
                        .ThenBy(measurement => measurement.CaptureMethod, StringComparer.Ordinal)
                        .ThenBy(measurement => measurement.Notes, StringComparer.Ordinal)
                        .Select(measurement => new
                        {
                            measurement.MeasurementType,
                            value = measurement.Value.ToString("G29", CultureInfo.InvariantCulture),
                            measurement.UnitCode,
                            measurement.CaptureMethod,
                            measurement.Notes,
                            measurement.SortOrder
                        })
                        .ToArray()
                })
                .ToArray()
        });

        return $"v2:{Sha256Hex.Compute(canonicalPayload)}";
    }
}
