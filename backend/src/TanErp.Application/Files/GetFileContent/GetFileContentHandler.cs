using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Files.GetFileContent;

public class GetFileContentHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IFileStore _fileStore;

    public GetFileContentHandler(
        IRequestAccessResolver accessResolver,
        IFileStore fileStore)
    {
        _accessResolver = accessResolver;
        _fileStore = fileStore;
    }

    public async Task<Result<FileContentResult>> Handle(
        GetFileContentQuery query,
        CancellationToken cancellationToken = default)
    {
        // Resolve membership and verify user belongs to an organization
        var accessResult = await _accessResolver.ResolveAnyAsync(
            query.FirebaseUid,
            query.MembershipId,
            [
                "opportunities.read",
                "opportunities.update",
                "customers.read",
                "customers.update",
                "customers.create",
                "sites.read",
                "sites.update",
                "sites.create",
                "items.read",
                "items.manage-images",
                "items.update"
            ],
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return Result<FileContentResult>.Failure(accessResult.Error);
        }

        return await _fileStore.GetAuthorizedFileContentAsync(
            accessResult.Value!,
            query.FileId,
            cancellationToken);
    }
}
