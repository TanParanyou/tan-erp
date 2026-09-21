using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Files;
using TanErp.Domain.Files;
using TanErp.Domain.IdentityAccess;
using TanErp.Infrastructure.Persistence;

namespace TanErp.Infrastructure.Files;

public class FileParentAccessResolver : IFileParentAccessResolver
{
    private readonly AppDbContext _db;
    private readonly IClock _clock;

    public FileParentAccessResolver(AppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<Result<FileParentAccess>> ResolveAsync(
        RequestAccessContext access,
        string parentType,
        Guid? parentId,
        Guid? creationIntentId,
        FileAccessOperation operation,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(parentType) || !FileParentTypes.IsValid(parentType))
        {
            return Result<FileParentAccess>.Failure(
                new Error("FILE_PARENT_TYPE_INVALID", $"Parent type '{parentType}' is invalid. Supported: opportunity, customer, site."));
        }

        var normalizedParentType = parentType.Trim().ToLowerInvariant();

        if (parentId.HasValue && creationIntentId.HasValue)
        {
            return Result<FileParentAccess>.Failure(
                new Error("FILE_PARENT_TYPE_INVALID", "Cannot specify both parentId and creationIntentId."));
        }

        if (!parentId.HasValue && !creationIntentId.HasValue)
        {
            return Result<FileParentAccess>.Failure(
                new Error("FILE_PARENT_TYPE_INVALID", "Exactly one of parentId or creationIntentId must be provided."));
        }

        switch (normalizedParentType)
        {
            case FileParentTypes.Opportunity:
            {
                if (!parentId.HasValue)
                {
                    return Result<FileParentAccess>.Failure(
                        new Error("FILE_PARENT_TYPE_INVALID", "Opportunity upload requires an existing parentId."));
                }

                var requiredPerm = operation switch
                {
                    FileAccessOperation.Read => "opportunities.read",
                    _ => "opportunities.update"
                };

                var hasPerm = await HasPermissionAsync(access.MembershipId, requiredPerm, cancellationToken);
                if (!hasPerm && operation == FileAccessOperation.Read)
                {
                    hasPerm = await HasPermissionAsync(access.MembershipId, "opportunities.update", cancellationToken);
                }

                if (!hasPerm)
                {
                    return Result<FileParentAccess>.Failure(
                        new Error("PERMISSION_DENIED", "Access is denied for the requested operation."));
                }

                var exists = await _db.Opportunities
                    .AsNoTracking()
                    .AnyAsync(o => o.Id == parentId.Value && o.OrganizationId == access.OrganizationId, cancellationToken);

                if (!exists)
                {
                    return Result<FileParentAccess>.Failure(
                        new Error("RESOURCE_NOT_FOUND", "Opportunity not found."));
                }

                return Result<FileParentAccess>.Success(
                    new FileParentAccess(normalizedParentType, parentId, null, access.OrganizationId));
            }

            case FileParentTypes.Customer:
            {
                if (parentId.HasValue)
                {
                    var requiredPerm = operation switch
                    {
                        FileAccessOperation.Read => "customers.read",
                        _ => "customers.update"
                    };

                    var hasPerm = await HasPermissionAsync(access.MembershipId, requiredPerm, cancellationToken);
                    if (!hasPerm && operation == FileAccessOperation.Read)
                    {
                        hasPerm = await HasPermissionAsync(access.MembershipId, "customers.update", cancellationToken);
                    }

                    if (!hasPerm)
                    {
                        return Result<FileParentAccess>.Failure(
                            new Error("PERMISSION_DENIED", "Access is denied for the requested operation."));
                    }

                    var exists = await _db.Customers
                        .AsNoTracking()
                        .AnyAsync(c => c.Id == parentId.Value && c.OrganizationId == access.OrganizationId, cancellationToken);

                    if (!exists)
                    {
                        return Result<FileParentAccess>.Failure(
                            new Error("RESOURCE_NOT_FOUND", "Customer not found."));
                    }

                    return Result<FileParentAccess>.Success(
                        new FileParentAccess(normalizedParentType, parentId, null, access.OrganizationId));
                }
                else
                {
                    var requiredPerm = operation switch
                    {
                        FileAccessOperation.Read => "customers.read",
                        _ => "customers.create"
                    };

                    var hasPerm = await HasPermissionAsync(access.MembershipId, requiredPerm, cancellationToken);
                    if (!hasPerm && operation == FileAccessOperation.Read)
                    {
                        hasPerm = await HasPermissionAsync(access.MembershipId, "customers.create", cancellationToken);
                    }

                    if (!hasPerm)
                    {
                        return Result<FileParentAccess>.Failure(
                            new Error("PERMISSION_DENIED", "Access is denied for the requested operation."));
                    }

                    return Result<FileParentAccess>.Success(
                        new FileParentAccess(normalizedParentType, null, creationIntentId, access.OrganizationId));
                }
            }

            case FileParentTypes.Site:
            {
                if (parentId.HasValue)
                {
                    var requiredPerm = operation switch
                    {
                        FileAccessOperation.Read => "sites.read",
                        _ => "sites.update"
                    };

                    var hasPerm = await HasPermissionAsync(access.MembershipId, requiredPerm, cancellationToken);
                    if (!hasPerm && operation == FileAccessOperation.Read)
                    {
                        hasPerm = await HasPermissionAsync(access.MembershipId, "sites.update", cancellationToken);
                    }

                    if (!hasPerm)
                    {
                        return Result<FileParentAccess>.Failure(
                            new Error("PERMISSION_DENIED", "Access is denied for the requested operation."));
                    }

                    var exists = await _db.Sites
                        .AsNoTracking()
                        .AnyAsync(s => s.Id == parentId.Value && s.OrganizationId == access.OrganizationId, cancellationToken);

                    if (!exists)
                    {
                        return Result<FileParentAccess>.Failure(
                            new Error("RESOURCE_NOT_FOUND", "Site not found."));
                    }

                    return Result<FileParentAccess>.Success(
                        new FileParentAccess(normalizedParentType, parentId, null, access.OrganizationId));
                }
                else
                {
                    var requiredPerm = operation switch
                    {
                        FileAccessOperation.Read => "sites.read",
                        _ => "sites.create"
                    };

                    var hasPerm = await HasPermissionAsync(access.MembershipId, requiredPerm, cancellationToken);
                    if (!hasPerm && operation == FileAccessOperation.Read)
                    {
                        hasPerm = await HasPermissionAsync(access.MembershipId, "sites.create", cancellationToken);
                    }

                    if (!hasPerm)
                    {
                        return Result<FileParentAccess>.Failure(
                            new Error("PERMISSION_DENIED", "Access is denied for the requested operation."));
                    }

                    return Result<FileParentAccess>.Success(
                        new FileParentAccess(normalizedParentType, null, creationIntentId, access.OrganizationId));
                }
            }

            default:
                return Result<FileParentAccess>.Failure(
                    new Error("FILE_PARENT_TYPE_INVALID", $"Parent type '{parentType}' is invalid. Supported: opportunity, customer, site."));
        }
    }

    private async Task<bool> HasPermissionAsync(
        Guid membershipId,
        string permissionKey,
        CancellationToken cancellationToken)
    {
        var now = _clock.UtcNow;

        return await _db.Memberships
            .AsNoTracking()
            .Where(m => m.Id == membershipId)
            .Where(m => m.IsActive && m.User!.IsActive && m.Organization!.IsActive)
            .Where(m => m.StartsAtUtc == null || m.StartsAtUtc <= now)
            .Where(m => m.ExpiresAtUtc == null || m.ExpiresAtUtc > now)
            .SelectMany(m => m.MembershipRoles
                .Where(mr => mr.Role!.IsActive)
                .SelectMany(mr => mr.Role!.RolePermissions
                    .Where(rp => rp.Permission!.IsActive
                        && rp.Permission.Key == permissionKey
                        && rp.Scope == PermissionScope.Organization
                        && rp.ScopeId == m.OrganizationId)))
            .AnyAsync(cancellationToken);
    }
}
