namespace TanErp.Application.Files.GetFileContent;

public sealed record GetFileContentQuery(
    string FirebaseUid,
    Guid MembershipId,
    Guid FileId);

public sealed record FileContentResult(
    Stream ContentStream,
    string MediaType,
    string Filename);
