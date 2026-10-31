namespace TanErp.Domain.Service;

public sealed record ServiceSlaEvaluation(
    int ResponseHours,
    int ResolveHours,
    decimal ElapsedHours,
    long TotalPausedSeconds,
    string SlaStatus);

public static class ServiceSlaCalculator
{
    public static (int ResponseHours, int ResolveHours) GetPolicyHours(string priority) => priority switch
    {
        ServicePriority.Urgent => (2, 8),
        ServicePriority.High => (4, 24),
        ServicePriority.Normal => (8, 48),
        ServicePriority.Low => (24, 72),
        _ => (8, 48)
    };

    public static ServiceSlaEvaluation Evaluate(ServiceRequest request, DateTimeOffset nowUtc, IReadOnlySet<DateOnly> holidays)
    {
        var (responseHours, resolveHours) = GetPolicyHours(request.Priority);
        var endUtc = (request.Status is ServiceRequestStatus.Resolved or ServiceRequestStatus.Closed && request.ResolvedAtUtc.HasValue)
            ? request.ResolvedAtUtc.Value
            : nowUtc;

        if (endUtc < request.CreatedAtUtc)
        {
            endUtc = request.CreatedAtUtc;
        }

        // Subtract holiday hours falling within the period
        var startDay = DateOnly.FromDateTime(request.CreatedAtUtc.UtcDateTime);
        var endDay = DateOnly.FromDateTime(endUtc.UtcDateTime);
        var holidaySeconds = 0.0;

        for (var d = startDay; d <= endDay; d = d.AddDays(1))
        {
            if (!holidays.Contains(d)) continue;
            var holidayStartUtc = new DateTimeOffset(d.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc));
            var holidayEndUtc = holidayStartUtc.AddDays(1);
            var overlapStart = request.CreatedAtUtc > holidayStartUtc ? request.CreatedAtUtc : holidayStartUtc;
            var overlapEnd = endUtc < holidayEndUtc ? endUtc : holidayEndUtc;
            if (overlapEnd > overlapStart)
            {
                holidaySeconds += (overlapEnd - overlapStart).TotalSeconds;
            }
        }

        var totalSeconds = (endUtc - request.CreatedAtUtc).TotalSeconds;
        var activeSeconds = Math.Max(0.0, totalSeconds - holidaySeconds);

        var currentWaitSeconds = (request.Status == ServiceRequestStatus.WaitingCustomer && request.PausedAtUtc.HasValue)
            ? (long)Math.Max(0, (nowUtc - request.PausedAtUtc.Value).TotalSeconds)
            : 0L;
        var totalPausedSeconds = request.PausedSeconds + currentWaitSeconds;

        var elapsedSeconds = Math.Max(0.0, activeSeconds - totalPausedSeconds);
        var elapsedHours = Math.Max(0m, decimal.Round((decimal)elapsedSeconds / 3600m, 2, MidpointRounding.AwayFromZero));

        string slaStatus;
        if (request.Status == ServiceRequestStatus.WaitingCustomer)
        {
            slaStatus = SlaStatus.Paused;
        }
        else if (request.Status is ServiceRequestStatus.Resolved or ServiceRequestStatus.Closed)
        {
            slaStatus = elapsedHours <= resolveHours ? SlaStatus.Met : SlaStatus.Breached;
        }
        else if (elapsedHours > resolveHours)
        {
            slaStatus = SlaStatus.Breached;
        }
        else if (elapsedHours >= resolveHours * 0.80m)
        {
            slaStatus = SlaStatus.Warning;
        }
        else
        {
            slaStatus = SlaStatus.OnTrack;
        }

        return new ServiceSlaEvaluation(responseHours, resolveHours, elapsedHours, totalPausedSeconds, slaStatus);
    }
}
