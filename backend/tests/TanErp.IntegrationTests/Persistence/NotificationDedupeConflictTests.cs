using Microsoft.EntityFrameworkCore;
using Npgsql;
using TanErp.Infrastructure.Persistence.Notifications;
using Xunit;

namespace TanErp.IntegrationTests.Persistence;

public sealed class NotificationDedupeConflictTests
{
    [Fact]
    public void Is_ReturnsTrue_ForUniqueViolationOnNotificationDedupeConstraint()
    {
        var ex = new DbUpdateException("save failed", CreatePostgresException("23505", NotificationDedupeConflict.ConstraintName));

        Assert.True(NotificationDedupeConflict.Is(ex));
    }

    [Fact]
    public void Is_ReturnsFalse_ForUniqueViolationOnAnotherConstraint()
    {
        var ex = new DbUpdateException("save failed", CreatePostgresException("23505", "ux_estimates_idempotency"));

        Assert.False(NotificationDedupeConflict.Is(ex));
    }

    [Fact]
    public void Is_ReturnsFalse_ForOtherSqlStateOnNotificationDedupeConstraint()
    {
        var ex = new DbUpdateException("save failed", CreatePostgresException("23503", NotificationDedupeConflict.ConstraintName));

        Assert.False(NotificationDedupeConflict.Is(ex));
    }

    [Fact]
    public void Is_ReturnsFalse_WhenInnerExceptionIsNotPostgres()
    {
        var ex = new DbUpdateException("save failed", new InvalidOperationException("boom"));

        Assert.False(NotificationDedupeConflict.Is(ex));
    }

    [Fact]
    public void Is_ReturnsFalse_WithoutInnerException()
    {
        var ex = new DbUpdateException("save failed");

        Assert.False(NotificationDedupeConflict.Is(ex));
    }

    // Npgsql 9 positional order: messageText, severity, invariantSeverity, sqlState, messageDetail, messageHint, position,
    // internalPosition, internalQuery, where, schemaName, tableName, columnName, dataTypeName, constraintName, file, line, routine.
    private static PostgresException CreatePostgresException(string sqlState, string constraintName) =>
        new(
            "duplicate key value violates unique constraint",
            "ERROR",
            "ERROR",
            sqlState,
            null!,
            null!,
            0,
            0,
            null!,
            null!,
            "notifications",
            "notifications",
            null!,
            null!,
            constraintName,
            null!,
            null!,
            null!);
}
