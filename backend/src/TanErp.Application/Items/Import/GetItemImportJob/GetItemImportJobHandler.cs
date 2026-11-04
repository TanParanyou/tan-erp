using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Items.Import.GetItemImportJob;

public sealed record GetItemImportJobQuery(string FirebaseUid, Guid MembershipId, Guid JobId);

public sealed record CancelItemImportJobCommand(string FirebaseUid, Guid MembershipId, Guid JobId, string TraceId = "");

/// <summary>Progress/status of one job. The permission is checked before the job is looked up, and a job of another organization or another requester is a 404.</summary>
public class GetItemImportJobHandler
{
    // A cost importer may not hold items.read, so either read permission may view job progress.
    private static readonly string[] ReadPermissions = ["items.read", "cost-records.read"];
    private static readonly string[] WritePermissions = ["items.create", "cost-records.create"];

    private readonly IRequestAccessResolver _accessResolver;
    private readonly IItemImportJobStore _jobStore;

    public GetItemImportJobHandler(IRequestAccessResolver accessResolver, IItemImportJobStore jobStore)
    {
        _accessResolver = accessResolver;
        _jobStore = jobStore;
    }

    public async Task<Result<ItemImportJobSnapshot>> Handle(GetItemImportJobQuery query, CancellationToken cancellationToken = default)
    {
        var accessResult = await _accessResolver.ResolveAnyAsync(query.FirebaseUid, query.MembershipId, ReadPermissions, cancellationToken);
        if (accessResult.IsFailure)
        {
            return Result<ItemImportJobSnapshot>.Failure(accessResult.Error);
        }

        var job = await _jobStore.GetAsync(accessResult.Value!.OrganizationId, accessResult.Value.ActorUserId, query.JobId, cancellationToken);
        return job is null
            ? Result<ItemImportJobSnapshot>.Failure(new Error("RESOURCE_NOT_FOUND", "Import job not found."))
            : Result<ItemImportJobSnapshot>.Success(job);
    }

    public async Task<Result<ItemImportJobSnapshot>> Cancel(CancelItemImportJobCommand command, CancellationToken cancellationToken = default)
    {
        var accessResult = await _accessResolver.ResolveAnyAsync(command.FirebaseUid, command.MembershipId, WritePermissions, cancellationToken);
        if (accessResult.IsFailure)
        {
            return Result<ItemImportJobSnapshot>.Failure(accessResult.Error);
        }

        var job = await _jobStore.GetAsync(accessResult.Value!.OrganizationId, accessResult.Value.ActorUserId, command.JobId, cancellationToken);
        if (job is null)
        {
            return Result<ItemImportJobSnapshot>.Failure(new Error("RESOURCE_NOT_FOUND", "Import job not found."));
        }

        var kindAccess = await ItemImportAccess.ResolveAsync(
            _accessResolver, command.FirebaseUid, command.MembershipId, job.Kind, job.Mode, cancellationToken);
        if (kindAccess.IsFailure)
        {
            return Result<ItemImportJobSnapshot>.Failure(kindAccess.Error);
        }

        return await _jobStore.CancelAsync(
            accessResult.Value.OrganizationId, accessResult.Value.ActorUserId, command.JobId, command.TraceId, cancellationToken);
    }
}
