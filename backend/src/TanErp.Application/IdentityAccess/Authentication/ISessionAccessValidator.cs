namespace TanErp.Application.IdentityAccess.Authentication;

public interface ISessionAccessValidator
{
    Task<bool> IsSessionActiveAsync(Guid sessionId, string subjectUid, CancellationToken cancellationToken = default);
}
