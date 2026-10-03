using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;

namespace TanErp.Application.IdentityAccess.CurrentUser.GetCurrentUser;

public class GetCurrentUserHandler
{
    private readonly ICurrentUserReader _currentUserReader;
    private readonly IFirebaseIdentityLinker? _identityLinker;

    public GetCurrentUserHandler(ICurrentUserReader currentUserReader, IFirebaseIdentityLinker? identityLinker = null)
    {
        _currentUserReader = currentUserReader;
        _identityLinker = identityLinker;
    }

    public async Task<Result<GetCurrentUserResult>> Handle(GetCurrentUserQuery query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query.FirebaseUid))
        {
            return Result<GetCurrentUserResult>.Failure(new Error("AUTHENTICATION_REQUIRED", "Authentication is required."));
        }

        if (_identityLinker is not null && !string.IsNullOrWhiteSpace(query.VerifiedEmail))
        {
            await _identityLinker.TryLinkAsync(query.FirebaseUid, query.VerifiedEmail, cancellationToken);
        }

        return await _currentUserReader.GetAsync(query.FirebaseUid, cancellationToken);
    }
}
