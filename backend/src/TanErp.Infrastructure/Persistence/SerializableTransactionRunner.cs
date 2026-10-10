using System.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TanErp.Application.Common.Results;

namespace TanErp.Infrastructure.Persistence;

/// <summary>Runs a unit of work in a Serializable transaction and maps concurrency/serialization failures to a conflict error.</summary>
public static class SerializableTransactionRunner
{
    public static async Task<Result<T>> RunAsync<T>(
        AppDbContext db, Func<Task<Result<T>>> work, string conflictCode, CancellationToken cancellationToken)
    {
        var strategy = db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            db.ChangeTracker.Clear();
            await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
            try
            {
                var result = await work();
                if (result.IsSuccess) await transaction.CommitAsync(cancellationToken);
                else await transaction.RollbackAsync(cancellationToken);
                return result;
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result<T>.Failure(new Error(conflictCode, "The record was modified by another user."));
            }
            catch (Exception ex) when (IsSerializationFailure(ex))
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result<T>.Failure(new Error(conflictCode, "The change conflicted with a concurrent update; reload and retry."));
            }
        });
    }

    public static bool IsSerializationFailure(Exception ex)
    {
        for (var current = ex; current is not null; current = current.InnerException!)
        {
            if (current is PostgresException { SqlState: PostgresErrorCodes.SerializationFailure or PostgresErrorCodes.DeadlockDetected })
                return true;
            if (current.InnerException is null) break;
        }

        return false;
    }

    /// <summary>True only for a unique violation of the named constraint/index, never for any 23505.</summary>
    public static bool IsUniqueViolation(DbUpdateException ex, string constraintName) =>
        ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation } pg && pg.ConstraintName == constraintName;
}
