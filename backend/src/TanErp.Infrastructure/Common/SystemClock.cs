using TanErp.Application.Common.Abstractions;

namespace TanErp.Infrastructure.Common;

public class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
