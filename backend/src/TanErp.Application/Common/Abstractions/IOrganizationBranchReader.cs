namespace TanErp.Application.Common.Abstractions;

public sealed record OrganizationBranchProjection(Guid Id, string Code, string Name);

public interface IOrganizationBranchReader
{
    Task<IReadOnlyList<OrganizationBranchProjection>> ListActiveAsync(Guid organizationId, CancellationToken cancellationToken);
}
