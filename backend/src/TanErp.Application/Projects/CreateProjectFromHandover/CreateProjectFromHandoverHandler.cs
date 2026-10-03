using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;

namespace TanErp.Application.Projects.CreateProjectFromHandover;

public class CreateProjectFromHandoverHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IProjectStore _store;

    public CreateProjectFromHandoverHandler(IRequestAccessResolver accessResolver, IProjectStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    public async Task<Result<ProjectDetailProjection>> Handle(
        CreateProjectFromHandoverCommand command,
        CancellationToken cancellationToken = default)
    {
        var accessResult = await _accessResolver.ResolveAsync(
            command.FirebaseUid, command.MembershipId, "projects.create", cancellationToken);
        if (accessResult.IsFailure)
        {
            return Result<ProjectDetailProjection>.Failure(accessResult.Error);
        }

        if (command.QuotationId == Guid.Empty || command.OwnerUserId == Guid.Empty || command.ExpectedQuotationVersion == Guid.Empty)
        {
            return Result<ProjectDetailProjection>.Failure(
                new Error("PROJECT_FIELD_REQUIRED", "Quotation, expected quotation version and project owner are required."));
        }

        var name = string.IsNullOrWhiteSpace(command.Name) ? null : command.Name.Trim();
        if (name is { Length: > 200 })
        {
            return Result<ProjectDetailProjection>.Failure(
                new Error("PROJECT_FIELD_INVALID", "Project name cannot exceed 200 characters."));
        }

        var normalized = command with { Name = name };
        var keyHash = Sha256Hex.Compute(command.IdempotencyKey);
        var payloadHash = Sha256Hex.Compute(
            $"{command.QuotationId}|{command.ExpectedQuotationVersion}|{command.OwnerUserId}|{command.PlannedStartDate:O}|{name}");

        return await _store.CreateFromHandoverAsync(accessResult.Value!, normalized, keyHash, payloadHash, cancellationToken);
    }
}
