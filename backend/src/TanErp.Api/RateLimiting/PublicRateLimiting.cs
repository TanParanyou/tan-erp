using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace TanErp.Api;

/// <summary>Per-client fixed-window limit for the unauthenticated customer endpoints, to blunt token guessing and abuse.</summary>
public static class PublicRateLimiting
{
    public const string PolicyName = "public-acceptance";
    public const string PermitConfigKey = "PublicAcceptance:PermitsPerMinute";
    public const int DefaultPermitsPerMinute = 30;

    public static IServiceCollection AddPublicRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        var permits = configuration.GetValue<int?>(PermitConfigKey) ?? DefaultPermitsPerMinute;
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddPolicy(PolicyName, httpContext => RateLimitPartition.GetFixedWindowLimiter(
                httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                _ => new FixedWindowRateLimiterOptions { PermitLimit = permits, Window = TimeSpan.FromMinutes(1), QueueLimit = 0, AutoReplenishment = true }));
        });
        return services;
    }
}
