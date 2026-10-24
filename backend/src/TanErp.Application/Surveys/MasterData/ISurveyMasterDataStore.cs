using TanErp.Domain.Surveys;

namespace TanErp.Application.Surveys.MasterData;

public interface ISurveyMasterDataStore
{
    Task<IReadOnlyList<SurveyDesignStyle>> ListDesignStylesAsync(Guid organizationId, bool onlyActive, CancellationToken cancellationToken);
    Task<SurveyDesignStyle?> GetDesignStyleByIdAsync(Guid organizationId, Guid id, CancellationToken cancellationToken);
    Task<SurveyDesignStyle?> GetDesignStyleByCodeAsync(Guid organizationId, string code, CancellationToken cancellationToken);
    Task AddDesignStyleAsync(SurveyDesignStyle style, CancellationToken cancellationToken);
    Task UpdateDesignStyleAsync(SurveyDesignStyle style, CancellationToken cancellationToken);

    Task<IReadOnlyList<SurveyMoodTone>> ListMoodTonesAsync(Guid organizationId, bool onlyActive, CancellationToken cancellationToken);
    Task<SurveyMoodTone?> GetMoodToneByIdAsync(Guid organizationId, Guid id, CancellationToken cancellationToken);
    Task<SurveyMoodTone?> GetMoodToneByCodeAsync(Guid organizationId, string code, CancellationToken cancellationToken);
    Task AddMoodToneAsync(SurveyMoodTone moodTone, CancellationToken cancellationToken);
    Task UpdateMoodToneAsync(SurveyMoodTone moodTone, CancellationToken cancellationToken);

    Task EnsureDefaultMasterDataAsync(Guid organizationId, CancellationToken cancellationToken);
}
