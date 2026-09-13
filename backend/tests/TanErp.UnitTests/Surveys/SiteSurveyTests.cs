using TanErp.Domain.Crm.Opportunities;
using TanErp.Domain.Surveys;
using Xunit;

namespace TanErp.UnitTests.Surveys;

public class SiteSurveyTests
{
    private readonly Guid _orgId = Guid.NewGuid();
    private readonly Guid _branchId = Guid.NewGuid();
    private readonly Guid _opportunityId = Guid.NewGuid();
    private readonly Guid _siteId = Guid.NewGuid();
    private readonly Guid _surveyorId = Guid.NewGuid();
    private readonly Guid _userId = Guid.NewGuid();
    private readonly DateTimeOffset _now = DateTimeOffset.UtcNow;

    [Fact]
    public void CreateAppointment_ValidInputs_InitializesScheduledStatusAndSurveyNumber()
    {
        var start = _now.AddDays(1);
        var end = start.AddHours(2);

        var survey = SiteSurvey.CreateAppointment(
            _orgId,
            _branchId,
            _opportunityId,
            _siteId,
            _surveyorId,
            _userId,
            start,
            end,
            _now);

        Assert.NotEqual(Guid.Empty, survey.Id);
        Assert.Equal(_orgId, survey.OrganizationId);
        Assert.Equal(_branchId, survey.BranchId);
        Assert.Equal(_opportunityId, survey.OpportunityId);
        Assert.Equal(_siteId, survey.SiteId);
        Assert.Equal(_surveyorId, survey.AssignedSurveyorId);
        Assert.Equal(SiteSurveyStatus.Scheduled, survey.Status);
        Assert.StartsWith("SRV-", survey.SurveyNumber);
        Assert.Equal(start.ToUniversalTime(), survey.ScheduledStartUtc);
        Assert.Equal(end.ToUniversalTime(), survey.ScheduledEndUtc);
    }

    [Fact]
    public void CreateAppointment_EndBeforeOrEqualStart_ThrowsArgumentException()
    {
        var start = _now.AddDays(1);
        var end = start.AddHours(-1);

        Assert.Throws<ArgumentException>(() =>
            SiteSurvey.CreateAppointment(
                _orgId,
                _branchId,
                _opportunityId,
                _siteId,
                _surveyorId,
                _userId,
                start,
                end,
                _now));
    }

    [Fact]
    public void CreateBaselineRevision_ValidInputs_InitializesDraftRevision1()
    {
        var surveyId = Guid.NewGuid();
        var revision = SiteSurveyRevision.CreateBaseline(
            _orgId,
            surveyId,
            _userId,
            _now);

        Assert.NotEqual(Guid.Empty, revision.Id);
        Assert.Equal(_orgId, revision.OrganizationId);
        Assert.Equal(surveyId, revision.SiteSurveyId);
        Assert.Equal(1, revision.RevisionNumber);
        Assert.Equal(SurveyDefaults.BaselineTemplateVersion, revision.SurveyTemplateVersion);
        Assert.Equal(SurveyRevisionStatus.Draft, revision.Status);
        Assert.Equal(SurveyReadiness.Incomplete, revision.Readiness);
    }

    [Fact]
    public void EnterSurveying_OpportunityQualified_TransitionsToSurveying()
    {
        var opp = Opportunity.CreateDraft(
            Guid.NewGuid(), _orgId, _branchId, Guid.NewGuid(), null, _userId, _userId,
            "Built-in Closet", "Full bedroom closet", new[] { "built-in" }, null, null, null, null,
            _now.AddDays(1), "Follow up", _now);

        opp.Qualify(opp.RowVersion);
        Assert.Equal(OpportunityStage.Qualified, opp.Stage);

        var previousVersion = opp.RowVersion;
        opp.EnterSurveying(previousVersion, _siteId);

        Assert.Equal(OpportunityStage.Surveying, opp.Stage);
        Assert.Equal(_siteId, opp.PrimarySiteId);
        Assert.NotEqual(previousVersion, opp.RowVersion);
    }

    [Fact]
    public void EnterSurveying_OpportunityDraft_ThrowsOpportunityTransitionException()
    {
        var opp = Opportunity.CreateDraft(
            Guid.NewGuid(), _orgId, _branchId, Guid.NewGuid(), null, _userId, _userId,
            "Built-in Closet", "Full bedroom closet", new[] { "built-in" }, null, null, null, null,
            _now.AddDays(1), "Follow up", _now);

        Assert.Throws<OpportunityTransitionException>(() => opp.EnterSurveying(opp.RowVersion, _siteId));
    }

    [Fact]
    public void EnterSurveying_StaleVersion_ThrowsOpportunityVersionException()
    {
        var opp = Opportunity.CreateDraft(
            Guid.NewGuid(), _orgId, _branchId, Guid.NewGuid(), null, _userId, _userId,
            "Built-in Closet", "Full bedroom closet", new[] { "built-in" }, null, null, null, null,
            _now.AddDays(1), "Follow up", _now);

        opp.Qualify(opp.RowVersion);

        Assert.Throws<OpportunityVersionException>(() => opp.EnterSurveying(Guid.NewGuid(), _siteId));
    }
}
