using TanErp.Application.Common.Results;

namespace TanErp.Application.AppVersion;

/// <summary>Per-platform version bounds read from the "AppVersionPolicy" configuration section.</summary>
public sealed class PlatformVersionOptions
{
    public string MinimumVersion { get; init; } = "0.0.0";
    public string LatestVersion { get; init; } = "0.0.0";
}

public sealed class AppVersionPolicyOptions
{
    public PlatformVersionOptions Ios { get; init; } = new();
    public PlatformVersionOptions Android { get; init; } = new();
}

public sealed record GetAppVersionPolicyQuery(string? Platform);

public sealed record AppVersionPolicyResult(string MinimumVersion, string LatestVersion);

public sealed class GetAppVersionPolicyHandler
{
    private readonly AppVersionPolicyOptions _options;

    public GetAppVersionPolicyHandler(AppVersionPolicyOptions options)
    {
        _options = options;
    }

    public Result<AppVersionPolicyResult> Handle(GetAppVersionPolicyQuery query)
    {
        var platform = query.Platform?.Trim().ToLowerInvariant() switch
        {
            "ios" => _options.Ios,
            "android" => _options.Android,
            _ => null,
        };

        return platform is null
            ? Result<AppVersionPolicyResult>.Failure(new Error("APP_PLATFORM_INVALID", "Platform must be ios or android."))
            : Result<AppVersionPolicyResult>.Success(new AppVersionPolicyResult(platform.MinimumVersion, platform.LatestVersion));
    }
}
