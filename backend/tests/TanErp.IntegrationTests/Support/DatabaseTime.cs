namespace TanErp.IntegrationTests.Support;

/// <summary>
/// PostgreSQL stores timestamps with microsecond precision while .NET keeps 100ns ticks (Linux clocks use them,
/// macOS clocks do not). Use this to build the value a round trip through the database is expected to return.
/// </summary>
public static class DatabaseTime
{
    private const long TicksPerMicrosecond = 10;

    public static DateTimeOffset ToMicroseconds(this DateTimeOffset value) =>
        value.AddTicks(-(value.Ticks % TicksPerMicrosecond));
}
