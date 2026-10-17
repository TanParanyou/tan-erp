using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using TanErp.Application.Common.Security;
using TanErp.Domain.Surveys;

namespace TanErp.Application.Surveys;

/// <summary>One active attachment of a survey area as it enters the v4 snapshot hash.</summary>
public sealed record AreaEvidenceEntry(Guid FileId, string? ContentSha256, string Purpose);

public static class SurveySnapshotHasher
{
    /// <summary>
    /// Computes the Ready snapshot hash. Revisions on a template whose hash version is <c>v3</c> also cover
    /// checklist results and the evidence manifest (file id, content checksum, kind, caption); older
    /// templates keep the <c>v2</c> payload so previously issued hashes stay reproducible. Hash version <c>v4</c> adds the
    /// per-area evidence manifest on top of the v3 payload. <paramref name="template"/> is the revision's template from the
    /// database; null falls back to the built-in baseline definitions.
    /// </summary>
    public static string Compute(
        string surveyNumber,
        SiteSurveyRevision revision,
        IReadOnlyDictionary<Guid, string?>? evidenceChecksums = null,
        SurveyTemplateVersion? template = null,
        IReadOnlyDictionary<Guid, IReadOnlyList<AreaEvidenceEntry>>? areaEvidence = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(surveyNumber);
        ArgumentNullException.ThrowIfNull(revision);

        var hashVersion = (template ?? SurveyTemplates.Find(revision.SurveyTemplateVersion))?.SnapshotHashVersion ?? "v2";

        if (hashVersion == "v3")
        {
            return ComputeV3(surveyNumber, revision, evidenceChecksums);
        }

        if (hashVersion == SurveyTemplateVersion.AreaEvidenceHashVersion)
        {
            return ComputeV4(surveyNumber, revision, evidenceChecksums, areaEvidence);
        }

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

    private static string ComputeV3(
        string surveyNumber,
        SiteSurveyRevision revision,
        IReadOnlyDictionary<Guid, string?>? evidenceChecksums)
    {
        return $"v3:{Sha256Hex.Compute(BuildV3Payload(surveyNumber, revision, evidenceChecksums))}";
    }

    private static string BuildV3Payload(
        string surveyNumber,
        SiteSurveyRevision revision,
        IReadOnlyDictionary<Guid, string?>? evidenceChecksums)
    {
        return JsonSerializer.Serialize(new
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
                .ToArray(),
            checklist = revision.ChecklistResults
                .OrderBy(result => result.ItemCode, StringComparer.Ordinal)
                .Select(result => new { result.ItemCode, result.Result, result.Note })
                .ToArray(),
            evidence = revision.Evidence
                .OrderBy(item => item.SortOrder)
                .ThenBy(item => item.FileId)
                .Select(item => new
                {
                    fileId = item.FileId.ToString("D"),
                    contentSha256 = evidenceChecksums is not null && evidenceChecksums.TryGetValue(item.FileId, out var checksum) ? checksum : null,
                    item.Kind,
                    item.Caption,
                    item.SortOrder
                })
                .ToArray()
        });
    }

    private static string ComputeV4(
        string surveyNumber,
        SiteSurveyRevision revision,
        IReadOnlyDictionary<Guid, string?>? evidenceChecksums,
        IReadOnlyDictionary<Guid, IReadOnlyList<AreaEvidenceEntry>>? areaEvidence)
    {
        // v4 = the v3 payload (same property order) plus the evidence manifest of every area.
        var payload = JsonNode.Parse(BuildV3Payload(surveyNumber, revision, evidenceChecksums))!.AsObject();
        payload["areaEvidence"] = JsonSerializer.SerializeToNode(revision.Areas
            .OrderBy(area => area.SortOrder)
            .ThenBy(area => area.Code, StringComparer.Ordinal)
            .Select(area => new
            {
                areaCode = area.Code,
                items = (areaEvidence is not null && areaEvidence.TryGetValue(area.Id, out var entries) ? entries : Array.Empty<AreaEvidenceEntry>())
                    .OrderBy(e => e.FileId)
                    .ThenBy(e => e.Purpose, StringComparer.Ordinal)
                    .Select(e => new { fileId = e.FileId.ToString("D"), contentSha256 = e.ContentSha256, e.Purpose })
                    .ToArray()
            })
            .ToArray());

        return $"v4:{Sha256Hex.Compute(payload.ToJsonString())}";
    }
}
