using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.IdentityAccess.Authentication;

namespace TanErp.Infrastructure.Persistence.IdentityAccess;

public sealed class SessionAccessValidator : ISessionAccessValidator
{
    private readonly AppDbContext _db;
    private readonly IClock _clock;

    public SessionAccessValidator(AppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<bool> IsSessionActiveAsync(
        Guid sessionId,
        string subjectUid,
        CancellationToken cancellationToken = default)
    {
        var now = _clock.UtcNow;
        return await _db.UserSessions.AsNoTracking()
            .AnyAsync(
                s => s.Id == sessionId
                     && s.RevokedAtUtc == null
                     && s.ExpiresAtUtc > now
                     && s.User != null
                     && s.User.IsActive
                     && s.User.FirebaseUid == subjectUid,
                cancellationToken);
    }
}
