using System.Globalization;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Application.IdentityAccess.Administration;
using TanErp.Domain.IdentityAccess;

namespace TanErp.Application.IdentityAccess.ApprovalLimits;

/// <summary>Every operation resolves approval-limits.manage first; the store is never reached without it.</summary>
public sealed class ApprovalLimitHandler
{
    private readonly IRequestAccessResolver _access;
    private readonly IApprovalLimitStore _store;

    public ApprovalLimitHandler(IRequestAccessResolver access, IApprovalLimitStore store)
    {
        _access = access;
        _store = store;
    }

    private Task<Result<RequestAccessContext>> AccessAsync(AdminCaller caller, CancellationToken ct) =>
        _access.ResolveAsync(caller.FirebaseUid, caller.MembershipId, ApprovalLimitPermissions.Manage, ct);

    private static AdminActor Actor(RequestAccessContext access) => new(access.ActorUserId, access.MembershipId);

    public async Task<Result<ApprovalLimitPage>> ListAsync(
        AdminCaller caller, Guid? roleId, string? documentType, string? status, int page, int limit, CancellationToken ct)
    {
        var access = await AccessAsync(caller, ct);
        if (access.IsFailure) return Result<ApprovalLimitPage>.Failure(access.Error);

        if (documentType is not null && !ApprovalDocumentType.IsEnforced(documentType))
            return AdminHandlerSupport.Validation<ApprovalLimitPage>("Unknown document type.");
        if (status is not (null or "active" or "inactive"))
            return AdminHandlerSupport.Validation<ApprovalLimitPage>("Status must be 'active' or 'inactive'.");

        var query = new ApprovalLimitListQuery(
            roleId, documentType, status, Math.Max(page, 1),
            limit < 1 ? AdministrationPolicy.DefaultPageSize : Math.Min(limit, AdministrationPolicy.MaxPageSize));
        return Result<ApprovalLimitPage>.Success(await _store.ListAsync(access.Value!.OrganizationId, query, ct));
    }

    public async Task<Result<ApprovalLimitOptions>> GetOptionsAsync(AdminCaller caller, CancellationToken ct)
    {
        var access = await AccessAsync(caller, ct);
        return access.IsFailure
            ? Result<ApprovalLimitOptions>.Failure(access.Error)
            : Result<ApprovalLimitOptions>.Success(await _store.GetOptionsAsync(access.Value!.OrganizationId, ct));
    }

    public async Task<Result<ApprovalLimitView>> GetAsync(AdminCaller caller, Guid limitId, CancellationToken ct)
    {
        var access = await AccessAsync(caller, ct);
        return access.IsFailure
            ? Result<ApprovalLimitView>.Failure(access.Error)
            : await _store.GetAsync(access.Value!.OrganizationId, limitId, ct);
    }

    public async Task<Result<ApprovalLimitView>> CreateAsync(
        AdminCaller caller, string idempotencyKey, ApprovalLimitInput input, string traceId, CancellationToken ct)
    {
        var access = await AccessAsync(caller, ct);
        if (access.IsFailure) return Result<ApprovalLimitView>.Failure(access.Error);

        if (input.RoleId == Guid.Empty || input.BranchId == Guid.Empty || input.IsUnlimited is null)
            return AdminHandlerSupport.Validation<ApprovalLimitView>("roleId and isUnlimited are required.");
        var documentType = input.DocumentType?.Trim().ToLowerInvariant() ?? string.Empty;
        var currency = input.Currency?.Trim().ToUpperInvariant() ?? string.Empty;
        try
        {
            ApprovalLimitRules.ValidateCreate(documentType, currency, input.IsUnlimited.Value, input.MaxAmount);
        }
        catch (ApprovalLimitDomainException ex)
        {
            return Result<ApprovalLimitView>.Failure(new Error(ex.Code, ex.Message));
        }

        var payloadHash = Sha256Hex.Compute(string.Join("|",
            input.RoleId.ToString("N"), documentType, input.BranchId?.ToString("N"), currency, input.IsUnlimited.Value,
            input.MaxAmount?.ToString(CultureInfo.InvariantCulture)));
        return await _store.CreateAsync(
            access.Value!.OrganizationId, input.RoleId, documentType, input.BranchId, currency, input.IsUnlimited.Value, input.MaxAmount,
            Actor(access.Value), Sha256Hex.Compute(idempotencyKey), payloadHash, traceId, ct);
    }

    public async Task<Result<ApprovalLimitView>> UpdateAsync(
        AdminCaller caller, Guid limitId, Guid ifMatch, ApprovalLimitUpdateInput input, string traceId, CancellationToken ct)
    {
        var access = await AccessAsync(caller, ct);
        if (access.IsFailure) return Result<ApprovalLimitView>.Failure(access.Error);

        if (input.IsUnlimited is null || input.IsActive is null)
            return AdminHandlerSupport.Validation<ApprovalLimitView>("isUnlimited and isActive are required.");
        try
        {
            ApprovalLimitRules.ValidateAmount(input.IsUnlimited.Value, input.MaxAmount);
        }
        catch (ApprovalLimitDomainException ex)
        {
            return Result<ApprovalLimitView>.Failure(new Error(ex.Code, ex.Message));
        }

        return await _store.UpdateAsync(
            access.Value!.OrganizationId, limitId, ifMatch, input.IsUnlimited.Value, input.MaxAmount, input.IsActive.Value,
            Actor(access.Value), traceId, ct);
    }
}
