using TanErp.Application.Surveys;
using TanErp.Domain.Surveys;
using Xunit;

namespace TanErp.UnitTests.Surveys;

public class SurveyChecklistEvidenceTests
{
    private readonly Guid _orgId = Guid.NewGuid();
    private readonly Guid _userId = Guid.NewGuid();
    private readonly DateTimeOffset _now = DateTimeOffset.UtcNow;

    private SiteSurveyRevision CreateDraft(string templateVersion, bool withChecklist, bool withEvidence, string? failNote = null)
    {
        var revision = SiteSurveyRevision.CreateBaseline(_orgId, Guid.NewGuid(), _userId, _now, templateVersion);
        revision.UpdateDraft(_now, "Built-in bedroom", null, null, null);
        var area = new SiteSurveyArea(Guid.NewGuid(), _orgId, revision.Id, "BEDROOM", "Bedroom", null, 1);
        area.AddMeasurement(new SiteSurveyMeasurement(
            Guid.NewGuid(), _orgId, area.Id, MeasurementType.Width, 3.2m,
            MeasurementUnit.Meter, CaptureMethod.Measured, null, 1));
        revision.AddArea(area);

        if (withChecklist)
        {
            foreach (var item in SurveyTemplates.BaselineV2.RequiredChecklistItems)
            {
                var isLast = item == SurveyTemplates.BaselineV2.RequiredChecklistItems[^1];
                revision.AddChecklistResult(new SiteSurveyChecklistResult(
                    Guid.NewGuid(), _orgId, revision.Id, item,
                    isLast && failNote is not null ? ChecklistResultValue.Fail : ChecklistResultValue.Pass,
                    isLast ? failNote : null));
            }
        }

        if (withEvidence)
        {
            revision.AddEvidence(new SiteSurveyEvidence(
                Guid.NewGuid(), _orgId, revision.Id, Guid.NewGuid(), EvidenceKind.SitePhoto, "Wall", 1));
        }

        return revision;
    }

    [Fact]
    public void MarkReady_BaselineV1_NeedsNoChecklistOrEvidence()
    {
        var revision = CreateDraft(SurveyDefaults.BaselineTemplateVersion, withChecklist: false, withEvidence: false);

        revision.MarkReady(_userId, _now, "v2:test");

        Assert.Equal(SurveyRevisionStatus.Ready, revision.Status);
    }

    [Fact]
    public void MarkReady_BaselineV2_RequiresEveryChecklistItem()
    {
        var revision = CreateDraft(SurveyDefaults.ChecklistTemplateVersion, withChecklist: false, withEvidence: true);

        var ex = Assert.Throws<SurveyReadinessException>(() => revision.MarkReady(_userId, _now, "v3:test"));

        Assert.Equal(nameof(SiteSurveyRevision.ChecklistResults), ex.MissingRequirement);
        Assert.Equal(SurveyRevisionStatus.Draft, revision.Status);
    }

    [Fact]
    public void MarkReady_BaselineV2_NonPassResultNeedsNote()
    {
        var withoutNote = CreateDraft(SurveyDefaults.ChecklistTemplateVersion, true, true, failNote: " ");
        Assert.Throws<SurveyReadinessException>(() => withoutNote.MarkReady(_userId, _now, "v3:test"));

        var withNote = CreateDraft(SurveyDefaults.ChecklistTemplateVersion, true, true, failNote: "Water shut-off unreachable");
        withNote.MarkReady(_userId, _now, "v3:test");
        Assert.Equal(SurveyRevisionStatus.Ready, withNote.Status);
    }

    [Fact]
    public void MarkReady_BaselineV2_RequiresEvidence()
    {
        var revision = CreateDraft(SurveyDefaults.ChecklistTemplateVersion, withChecklist: true, withEvidence: false);

        var ex = Assert.Throws<SurveyReadinessException>(() => revision.MarkReady(_userId, _now, "v3:test"));

        Assert.Equal(nameof(SiteSurveyRevision.Evidence), ex.MissingRequirement);
    }

    [Fact]
    public void MarkReady_UnknownTemplate_FailsClosed()
    {
        var revision = CreateDraft("SURVEY-UNKNOWN-v9", withChecklist: false, withEvidence: false);

        Assert.Throws<SurveyReadinessException>(() => revision.MarkReady(_userId, _now, "v2:test"));
    }

    [Fact]
    public void SnapshotHash_V3CoversChecklistAndEvidenceChecksum_WhileV1KeepsV2Prefix()
    {
        var v1 = CreateDraft(SurveyDefaults.BaselineTemplateVersion, false, false);
        Assert.StartsWith("v2:", SurveySnapshotHasher.Compute("SRV-1", v1));

        var v2 = CreateDraft(SurveyDefaults.ChecklistTemplateVersion, true, true);
        var fileId = v2.Evidence.Single().FileId;
        var first = SurveySnapshotHasher.Compute("SRV-1", v2, new Dictionary<Guid, string?> { [fileId] = "checksum-a" });
        var sameAgain = SurveySnapshotHasher.Compute("SRV-1", v2, new Dictionary<Guid, string?> { [fileId] = "checksum-a" });
        var changedFile = SurveySnapshotHasher.Compute("SRV-1", v2, new Dictionary<Guid, string?> { [fileId] = "checksum-b" });

        Assert.StartsWith("v3:", first);
        Assert.Equal(first, sameAgain);
        Assert.NotEqual(first, changedFile);
    }

    [Fact]
    public void CloneFrom_CopiesChecklistAndEvidenceWithNewIds()
    {
        var source = CreateDraft(SurveyDefaults.ChecklistTemplateVersion, true, true);
        source.MarkReady(_userId, _now, "v3:test");

        var clone = SiteSurveyRevision.CloneFrom(source, 2, "Customer changed plan", _userId, _now);

        Assert.Equal(source.ChecklistResults.Count, clone.ChecklistResults.Count);
        Assert.Equal(source.Evidence.Single().FileId, clone.Evidence.Single().FileId);
        Assert.NotEqual(source.Evidence.Single().Id, clone.Evidence.Single().Id);
        Assert.All(clone.ChecklistResults, c => Assert.Equal(clone.Id, c.SiteSurveyRevisionId));
        Assert.Equal(source.SurveyTemplateVersion, clone.SurveyTemplateVersion);
    }

    [Fact]
    public void SurveyTemplateVersionRow_EnforcesInvariants_AndV4SnapshotHashCoversAreaEvidenceManifest()
    {
        var now = DateTimeOffset.UtcNow;

        // Invalid code, invalid hash version, or area evidence with v3 hash version is rejected
        Assert.Throws<SurveyTemplateInvalidException>(() =>
            new SurveyTemplateVersionRow(Guid.NewGuid(), "x", Array.Empty<string>(), 0, 0, "v3", now, now, _userId));
        Assert.Throws<SurveyTemplateInvalidException>(() =>
            new SurveyTemplateVersionRow(Guid.NewGuid(), "SURVEY-BAD-V2", Array.Empty<string>(), 0, 0, "v2", now, now, _userId));
        Assert.Throws<SurveyTemplateInvalidException>(() =>
            new SurveyTemplateVersionRow(Guid.NewGuid(), "SURVEY-BAD-AREA", Array.Empty<string>(), 0, 1, "v3", now, now, _userId));

        var row = new SurveyTemplateVersionRow(
            Guid.NewGuid(), "SURVEY-V4-AREA", new[] { "site_access_confirmed" }, 1, 1, "v4", now, now, _userId);
        var template = row.ToTemplate();
        Assert.True(template.SupportsAreaEvidence);
        Assert.True(template.IsEffectiveAt(now));

        row.Retire(now.AddHours(1));
        Assert.Throws<SurveyTemplateInvalidException>(() => row.Retire(now.AddHours(2)));

        // v4 hash includes per-area evidence manifest and changes when area evidence checksum or purpose changes
        var revision = CreateDraft("SURVEY-V4-AREA", false, true);
        revision.AddChecklistResult(new SiteSurveyChecklistResult(Guid.NewGuid(), _orgId, revision.Id, "site_access_confirmed", ChecklistResultValue.Pass, null));
        var areaId = revision.Areas.Single().Id;
        var revFileId = revision.Evidence.Single().FileId;
        var areaFileId = Guid.NewGuid();

        var evidenceChecksums = new Dictionary<Guid, string?> { [revFileId] = "rev-sha-1" };
        var areaManifestA = new Dictionary<Guid, IReadOnlyList<AreaEvidenceEntry>>
        {
            [areaId] = new[] { new AreaEvidenceEntry(areaFileId, "area-sha-1", "evidence") }
        };
        var areaManifestB = new Dictionary<Guid, IReadOnlyList<AreaEvidenceEntry>>
        {
            [areaId] = new[] { new AreaEvidenceEntry(areaFileId, "area-sha-2", "evidence") }
        };

        var hashA1 = SurveySnapshotHasher.Compute("SRV-V4", revision, evidenceChecksums, template, areaManifestA);
        var hashA2 = SurveySnapshotHasher.Compute("SRV-V4", revision, evidenceChecksums, template, areaManifestA);
        var hashB = SurveySnapshotHasher.Compute("SRV-V4", revision, evidenceChecksums, template, areaManifestB);

        Assert.StartsWith("v4:", hashA1);
        Assert.Equal(hashA1, hashA2);
        Assert.NotEqual(hashA1, hashB);
    }
}
