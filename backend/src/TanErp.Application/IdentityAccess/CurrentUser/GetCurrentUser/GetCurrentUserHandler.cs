using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;

namespace TanErp.Application.IdentityAccess.CurrentUser.GetCurrentUser;

public class GetCurrentUserHandler
{
    private readonly ICurrentUserReader _currentUserReader;

    public GetCurrentUserHandler(ICurrentUserReader currentUserReader)
    {
        _currentUserReader = currentUserReader;
    }

    public async Task<Result<GetCurrentUserResult>> Handle(GetCurrentUserQuery query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query.FirebaseUid))
        {
            return Result<GetCurrentUserResult>.Failure(new Error("AUTHENTICATION_REQUIRED", "Authentication is required."));
        }

        return await _currentUserReader.GetAsync(query.FirebaseUid, cancellationToken);
    }
}
