using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Domain.Items;
using TanErp.Domain.Surveys;

namespace TanErp.Application.Surveys.MasterData;

public sealed record SurveyCaller(string FirebaseUid, Guid MembershipId);

public class SurveyMasterDataHandler
{
    private readonly IRequestAccessResolver _access;
    private readonly ISurveyMasterDataStore _store;

    public SurveyMasterDataHandler(IRequestAccessResolver access, ISurveyMasterDataStore store)
    {
        _access = access;
        _store = store;
    }

    public async Task<Result<IReadOnlyList<SurveyDesignStyleDto>>> ListDesignStylesAsync(
        SurveyCaller caller, bool onlyActive, CancellationToken ct)
    {
        var access = await ResolveReadAccessAsync(caller, ct);
        if (access.IsFailure) return Result<IReadOnlyList<SurveyDesignStyleDto>>.Failure(access.Error);

        await _store.EnsureDefaultMasterDataAsync(access.Value!.OrganizationId, ct);
        var entities = await _store.ListDesignStylesAsync(access.Value.OrganizationId, onlyActive, ct);

        var dtos = entities.Select(ToStyleDto).ToList();
        return Result<IReadOnlyList<SurveyDesignStyleDto>>.Success(dtos);
    }

    public async Task<Result<SurveyDesignStyleDto>> CreateDesignStyleAsync(
        SurveyCaller caller, CreateSurveyDesignStyleInput input, CancellationToken ct)
    {
        var access = await ResolveManageAccessAsync(caller, ct);
        if (access.IsFailure) return Result<SurveyDesignStyleDto>.Failure(access.Error);

        if (string.IsNullOrWhiteSpace(input.Code))
            return Result<SurveyDesignStyleDto>.Failure(new Error("REQUEST_VALIDATION_FAILED", "Code is required."));
        if (input.Name == null || string.IsNullOrWhiteSpace(input.Name.Thai))
            return Result<SurveyDesignStyleDto>.Failure(new Error("REQUEST_VALIDATION_FAILED", "Thai name is required."));

        var normalizedCode = input.Code.Trim().ToLowerInvariant();
        var existing = await _store.GetDesignStyleByCodeAsync(access.Value!.OrganizationId, normalizedCode, ct);
        if (existing != null)
            return Result<SurveyDesignStyleDto>.Failure(new Error("DUPLICATE_CODE", $"Design style with code '{normalizedCode}' already exists."));

        var name = LocalizedText.Create(input.Name.Thai, input.Name.English);
        var description = input.Description != null ? LocalizedText.CreateOptional(input.Description.Thai, input.Description.English) : null;

        var style = new SurveyDesignStyle(
            Guid.NewGuid(),
            access.Value.OrganizationId,
            normalizedCode,
            name,
            description,
            input.SortOrder);

        await _store.AddDesignStyleAsync(style, ct);
        return Result<SurveyDesignStyleDto>.Success(ToStyleDto(style));
    }

    public async Task<Result<SurveyDesignStyleDto>> UpdateDesignStyleAsync(
        SurveyCaller caller, Guid id, UpdateSurveyDesignStyleInput input, CancellationToken ct)
    {
        var access = await ResolveManageAccessAsync(caller, ct);
        if (access.IsFailure) return Result<SurveyDesignStyleDto>.Failure(access.Error);

        if (input.Name == null || string.IsNullOrWhiteSpace(input.Name.Thai))
            return Result<SurveyDesignStyleDto>.Failure(new Error("REQUEST_VALIDATION_FAILED", "Thai name is required."));

        var existing = await _store.GetDesignStyleByIdAsync(access.Value!.OrganizationId, id, ct);
        if (existing == null)
            return Result<SurveyDesignStyleDto>.Failure(new Error("NOT_FOUND", $"Design style with id '{id}' was not found."));

        var name = LocalizedText.Create(input.Name.Thai, input.Name.English);
        var description = input.Description != null ? LocalizedText.CreateOptional(input.Description.Thai, input.Description.English) : null;

        existing.Update(name, description, input.SortOrder, input.IsActive);
        await _store.UpdateDesignStyleAsync(existing, ct);

        return Result<SurveyDesignStyleDto>.Success(ToStyleDto(existing));
    }

    public async Task<Result<IReadOnlyList<SurveyMoodToneDto>>> ListMoodTonesAsync(
        SurveyCaller caller, bool onlyActive, CancellationToken ct)
    {
        var access = await ResolveReadAccessAsync(caller, ct);
        if (access.IsFailure) return Result<IReadOnlyList<SurveyMoodToneDto>>.Failure(access.Error);

        await _store.EnsureDefaultMasterDataAsync(access.Value!.OrganizationId, ct);
        var entities = await _store.ListMoodTonesAsync(access.Value.OrganizationId, onlyActive, ct);

        var dtos = entities.Select(ToMoodToneDto).ToList();
        return Result<IReadOnlyList<SurveyMoodToneDto>>.Success(dtos);
    }

    public async Task<Result<SurveyMoodToneDto>> CreateMoodToneAsync(
        SurveyCaller caller, CreateSurveyMoodToneInput input, CancellationToken ct)
    {
        var access = await ResolveManageAccessAsync(caller, ct);
        if (access.IsFailure) return Result<SurveyMoodToneDto>.Failure(access.Error);

        if (string.IsNullOrWhiteSpace(input.Code))
            return Result<SurveyMoodToneDto>.Failure(new Error("REQUEST_VALIDATION_FAILED", "Code is required."));
        if (input.Name == null || string.IsNullOrWhiteSpace(input.Name.Thai))
            return Result<SurveyMoodToneDto>.Failure(new Error("REQUEST_VALIDATION_FAILED", "Thai name is required."));

        var normalizedCode = input.Code.Trim().ToLowerInvariant();
        var existing = await _store.GetMoodToneByCodeAsync(access.Value!.OrganizationId, normalizedCode, ct);
        if (existing != null)
            return Result<SurveyMoodToneDto>.Failure(new Error("DUPLICATE_CODE", $"Mood & tone with code '{normalizedCode}' already exists."));

        var name = LocalizedText.Create(input.Name.Thai, input.Name.English);
        var description = input.Description != null ? LocalizedText.CreateOptional(input.Description.Thai, input.Description.English) : null;

        var moodTone = new SurveyMoodTone(
            Guid.NewGuid(),
            access.Value.OrganizationId,
            normalizedCode,
            name,
            description,
            input.ColorPalette,
            input.SortOrder);

        await _store.AddMoodToneAsync(moodTone, ct);
        return Result<SurveyMoodToneDto>.Success(ToMoodToneDto(moodTone));
    }

    public async Task<Result<SurveyMoodToneDto>> UpdateMoodToneAsync(
        SurveyCaller caller, Guid id, UpdateSurveyMoodToneInput input, CancellationToken ct)
    {
        var access = await ResolveManageAccessAsync(caller, ct);
        if (access.IsFailure) return Result<SurveyMoodToneDto>.Failure(access.Error);

        if (input.Name == null || string.IsNullOrWhiteSpace(input.Name.Thai))
            return Result<SurveyMoodToneDto>.Failure(new Error("REQUEST_VALIDATION_FAILED", "Thai name is required."));

        var existing = await _store.GetMoodToneByIdAsync(access.Value!.OrganizationId, id, ct);
        if (existing == null)
            return Result<SurveyMoodToneDto>.Failure(new Error("NOT_FOUND", $"Mood & tone with id '{id}' was not found."));

        var name = LocalizedText.Create(input.Name.Thai, input.Name.English);
        var description = input.Description != null ? LocalizedText.CreateOptional(input.Description.Thai, input.Description.English) : null;

        existing.Update(name, description, input.ColorPalette, input.SortOrder, input.IsActive);
        await _store.UpdateMoodToneAsync(existing, ct);

        return Result<SurveyMoodToneDto>.Success(ToMoodToneDto(existing));
    }

    private async Task<Result<Common.Models.RequestAccessContext>> ResolveReadAccessAsync(SurveyCaller caller, CancellationToken ct)
    {
        var access = await _access.ResolveAsync(caller.FirebaseUid, caller.MembershipId, "surveys.read", ct);
        if (access.IsSuccess) return access;
        access = await _access.ResolveAsync(caller.FirebaseUid, caller.MembershipId, "settings.manage", ct);
        if (access.IsSuccess) return access;
        return await _access.ResolveAsync(caller.FirebaseUid, caller.MembershipId, "organizations.read", ct);
    }

    private async Task<Result<Common.Models.RequestAccessContext>> ResolveManageAccessAsync(SurveyCaller caller, CancellationToken ct)
    {
        var access = await _access.ResolveAsync(caller.FirebaseUid, caller.MembershipId, "surveys.update", ct);
        if (access.IsSuccess) return access;
        access = await _access.ResolveAsync(caller.FirebaseUid, caller.MembershipId, "survey-templates.manage", ct);
        if (access.IsSuccess) return access;
        access = await _access.ResolveAsync(caller.FirebaseUid, caller.MembershipId, "settings.manage", ct);
        if (access.IsSuccess) return access;
        return await _access.ResolveAsync(caller.FirebaseUid, caller.MembershipId, "organizations.manage", ct);
    }

    private static SurveyDesignStyleDto ToStyleDto(SurveyDesignStyle s) =>
        new(s.Id, s.Code, new LocalizedNameDto(s.Name.Thai, s.Name.English), s.Description != null ? new LocalizedNameDto(s.Description.Thai, s.Description.English) : null, s.SortOrder, s.IsActive, s.CreatedAtUtc);

    private static SurveyMoodToneDto ToMoodToneDto(SurveyMoodTone m) =>
        new(m.Id, m.Code, new LocalizedNameDto(m.Name.Thai, m.Name.English), m.Description != null ? new LocalizedNameDto(m.Description.Thai, m.Description.English) : null, m.ColorPalette, m.SortOrder, m.IsActive, m.CreatedAtUtc);
}
