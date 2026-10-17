using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Items.Import;

/// <summary>
/// Maps an import (kind + mode) to the permissions it needs. Permission is checked before the file is parsed, so an unauthorized
/// caller learns nothing about the file or the data. Costs reuse cost-records.create (no new permission); upsert additionally needs items.update.
/// </summary>
public static class ItemImportAccess
{
    public static async Task<Result<RequestAccessContext>> ResolveAsync(
        IRequestAccessResolver resolver,
        string firebaseUid,
        Guid membershipId,
        string kind,
        string mode,
        CancellationToken cancellationToken)
    {
        if (!ItemImportKind.IsValid(kind) || !ItemImportMode.IsValid(mode)
            || (kind == ItemImportKind.Costs && mode != ItemImportMode.Create))
        {
            return Result<RequestAccessContext>.Failure(new Error("ITEM_IMPORT_OPTION_INVALID", "The import kind or mode is not supported."));
        }

        var primary = kind == ItemImportKind.Costs ? "cost-records.create" : "items.create";
        var access = await resolver.ResolveAsync(firebaseUid, membershipId, primary, cancellationToken);
        if (access.IsFailure || mode != ItemImportMode.Upsert)
        {
            return access;
        }

        var update = await resolver.ResolveAsync(firebaseUid, membershipId, "items.update", cancellationToken);
        return update.IsFailure ? Result<RequestAccessContext>.Failure(update.Error) : access;
    }
}
