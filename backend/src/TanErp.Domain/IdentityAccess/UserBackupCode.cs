using TanErp.Domain.Common;

namespace TanErp.Domain.IdentityAccess;

public class UserBackupCode : Entity
{
    public Guid UserId { get; private set; }
    public User? User { get; private set; }
    public string CodeHash { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset? UsedAtUtc { get; private set; }

    public bool IsUsed => UsedAtUtc.HasValue;

    protected UserBackupCode() { }

    public UserBackupCode(Guid id, Guid userId, string codeHash, DateTimeOffset createdAtUtc)
        : base(id)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID is required.", nameof(userId));
        if (string.IsNullOrWhiteSpace(codeHash))
            throw new ArgumentException("Backup code hash is required.", nameof(codeHash));

        UserId = userId;
        CodeHash = codeHash.Trim();
        CreatedAtUtc = createdAtUtc;
    }

    public bool TryMarkUsed(DateTimeOffset usedAtUtc)
    {
        if (UsedAtUtc.HasValue)
        {
            return false;
        }

        UsedAtUtc = usedAtUtc;
        return true;
    }
}
