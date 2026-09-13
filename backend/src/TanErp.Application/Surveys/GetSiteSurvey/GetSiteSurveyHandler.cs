using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Surveys.GetSiteSurvey;

public class GetSiteSurveyHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly ISiteSurveyStore _store;

    public GetSiteSurveyHandler(
        IRequestAccessResolver accessResolver,
        ISiteSurveyStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    public async Task<Result<SiteSurveyProjection?>> Handle(
        GetSiteSurveyQuery query,
        CancellationToken cancellationToken = default)
    {
        var accessResult = await _accessResolver.ResolveAsync(
            query.FirebaseUid,
            query.MembershipId,
            "surveys.read",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return Result<SiteSurveyProjection?>.Failure(accessResult.Error);
        }

        var access = accessResult.Value!;
        var survey = await _store.GetByOpportunityIdAsync(access.OrganizationId, query.OpportunityId, cancellationToken);
        return Result<SiteSurveyProjection?>.Success(survey);
    }
}
