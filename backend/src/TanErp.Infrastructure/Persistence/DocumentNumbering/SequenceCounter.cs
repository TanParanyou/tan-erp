using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using TanErp.Application.Common.Abstractions;

namespace TanErp.Infrastructure.Persistence.DocumentNumbering;

public class SequenceCounter : ISequenceCounter
{
    private readonly AppDbContext _db;
    private const string AtomicUpsertSql = @"
        INSERT INTO common.sequence_counters (
            organization_id,
            document_type,
            branch_id,
            period_key,
            current_val,
            updated_at_utc
        )
        VALUES (
            @organizationId,
            @documentType,
            @branchId,
            @periodKey,
            1,
            clock_timestamp()
        )
        ON CONFLICT (organization_id, document_type, branch_id, period_key)
        DO UPDATE SET
            current_val = common.sequence_counters.current_val + 1,
            updated_at_utc = clock_timestamp()
        RETURNING current_val;";

    public SequenceCounter(AppDbContext db)
    {
        _db = db;
    }

    public async Task<long> NextValueAsync(
        Guid organizationId,
        string documentType,
        Guid branchId,
        string periodKey,
        CancellationToken cancellationToken = default)
    {
        var connection = _db.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        await using var cmd = connection.CreateCommand();
        cmd.Transaction = _db.Database.CurrentTransaction?.GetDbTransaction();
        cmd.CommandText = AtomicUpsertSql;
        cmd.CommandType = CommandType.Text;

        cmd.Parameters.Add(new NpgsqlParameter("organizationId", organizationId));
        cmd.Parameters.Add(new NpgsqlParameter("documentType", documentType.ToLowerInvariant()));
        cmd.Parameters.Add(new NpgsqlParameter("branchId", branchId));
        cmd.Parameters.Add(new NpgsqlParameter("periodKey", periodKey));

        var result = await cmd.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt64(result);
    }
}
