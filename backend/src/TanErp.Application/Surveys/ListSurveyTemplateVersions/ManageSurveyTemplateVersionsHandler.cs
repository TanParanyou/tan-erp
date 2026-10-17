using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Domain.Surveys;

namespace TanErp.Application.Surveys.ListSurveyTemplateVersions;

/// <summary>Publishes new survey template versions and ends the effective period of old ones (API-only admin surface).</summary>
public class ManageSurveyTemplateVersionsHandler
{
    private const string ManagePermission = "survey-templates.manage";

    private readonly IRequestAccessResolver _accessResolver;
    private readonly ISurveyTemplateStore _store;

    public ManageSurveyTemplateVersionsHandler(IRequestAccessResolver accessResolver, ISurveyTemplateStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    public async Task<Result<SurveyTemplateVersion>> Publish(
        string firebaseUid, Guid membershipId, PublishSurveyTemplateCommand command, CancellationToken ct = default)
    {
        var access = await _accessResolver.ResolveAsync(firebaseUid, membershipId, ManagePermission, ct);
        if (access.IsFailure) return Result<SurveyTemplateVersion>.Failure(access.Error);
        return await _store.PublishAsync(command, access.Value!.ActorUserId, ct);
    }

    public async Task<Result<SurveyTemplateVersion>> Retire(
        string firebaseUid, Guid membershipId, string code, CancellationToken ct = default)
    {
        var access = await _accessResolver.ResolveAsync(firebaseUid, membershipId, ManagePermission, ct);
        if (access.IsFailure) return Result<SurveyTemplateVersion>.Failure(access.Error);
        return await _store.RetireAsync(code, ct);
    }
}
