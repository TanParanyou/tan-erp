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
}
