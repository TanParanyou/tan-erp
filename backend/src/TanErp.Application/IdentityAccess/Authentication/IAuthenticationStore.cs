using TanErp.Domain.IdentityAccess;

namespace TanErp.Application.IdentityAccess.Authentication;

public interface IAuthenticationStore
{
    Task<User?> FindUserByNormalizedEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default);

    Task<User?> FindUserByGoogleSubjectIdAsync(string googleSubjectId, CancellationToken cancellationToken = default);

    Task<User?> FindUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<User?> FindUserBySubjectUidAsync(string subjectUid, CancellationToken cancellationToken = default);

    Task<int> CountRemainingBackupCodesAsync(Guid userId, CancellationToken cancellationToken = default);

    Task ReplaceBackupCodesAsync(Guid userId, IReadOnlyCollection<UserBackupCode> backupCodes, CancellationToken cancellationToken = default);

    Task DeleteAllBackupCodesAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<bool> TryConsumeBackupCodeAsync(Guid userId, string codeHash, DateTimeOffset usedAtUtc, CancellationToken cancellationToken = default);

    void AddSession(UserSession session);

    Task<UserSession?> FindSessionWithUserAsync(Guid sessionId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<UserSession>> ListActiveSessionsAsync(Guid userId, DateTimeOffset now, CancellationToken cancellationToken = default);

    Task<UserSession?> FindUserSessionAsync(Guid userId, Guid sessionId, CancellationToken cancellationToken = default);

    Task<int> RevokeOtherActiveSessionsAsync(Guid userId, Guid? excludeSessionId, string reason, DateTimeOffset now, CancellationToken cancellationToken = default);

    Task RecordGoogleLinkedAuditAsync(Guid userId, string traceId, DateTimeOffset now, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
