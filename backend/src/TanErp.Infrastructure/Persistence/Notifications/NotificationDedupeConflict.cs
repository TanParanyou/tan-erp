using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace TanErp.Infrastructure.Persistence.Notifications;

/// <summary>
/// Detects the race where two identical transitions both stage the same (recipient, transition) notification and the second commit
/// violates ux_notifications_recipient_dedupe. Stores treat it like their existing lost-race outcome. Any other database error is not matched.
/// </summary>
public static class NotificationDedupeConflict
{
    public const string ConstraintName = "ux_notifications_recipient_dedupe";

    private const string UniqueViolationSqlState = "23505";

    public static bool Is(DbUpdateException exception) =>
        exception.InnerException is PostgresException { SqlState: UniqueViolationSqlState } postgres
        && string.Equals(postgres.ConstraintName, ConstraintName, StringComparison.Ordinal);
}
