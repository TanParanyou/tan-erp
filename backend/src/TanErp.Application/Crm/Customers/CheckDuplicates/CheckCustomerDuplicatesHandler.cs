using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Domain.Common;
using TanErp.Domain.Crm.Customers;

namespace TanErp.Application.Crm.Customers.CheckDuplicates;

public class CheckCustomerDuplicatesHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly ICustomerReadStore _store;

    public CheckCustomerDuplicatesHandler(
        IRequestAccessResolver accessResolver,
        ICustomerReadStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    public async Task<Result<CheckCustomerDuplicatesResult>> Handle(
        CheckCustomerDuplicatesQuery query,
        CancellationToken cancellationToken = default)
    {
        // 1. Resolve required customers.create or customers.read permission
        var accessResult = await _accessResolver.ResolveAsync(
            query.FirebaseUid,
            query.MembershipId,
            "customers.create",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            accessResult = await _accessResolver.ResolveAsync(
                query.FirebaseUid,
                query.MembershipId,
                "customers.read",
                cancellationToken);

            if (accessResult.IsFailure)
            {
                return Result<CheckCustomerDuplicatesResult>.Failure(accessResult.Error);
            }
        }

        var access = accessResult.Value!;

        // 2. Resolve optional customer-contacts.manage permission for PII visibility
        var manageContactResult = await _accessResolver.ResolveAsync(
            query.FirebaseUid,
            query.MembershipId,
            "customer-contacts.manage",
            cancellationToken);

        var includeContactPii = manageContactResult.IsSuccess;

        // 3. Normalize inputs
        var normName = !string.IsNullOrWhiteSpace(query.Name) ? CustomerNormalizer.NormalizeName(query.Name) : null;
        var normPhone = !string.IsNullOrWhiteSpace(query.Phone) ? CustomerNormalizer.NormalizePhone(query.Phone) : null;
        var normEmail = !string.IsNullOrWhiteSpace(query.Email) ? CustomerNormalizer.NormalizeEmail(query.Email) : null;

        if (string.IsNullOrWhiteSpace(normName) && string.IsNullOrWhiteSpace(normPhone) && string.IsNullOrWhiteSpace(normEmail))
        {
            return Result<CheckCustomerDuplicatesResult>.Success(
                new CheckCustomerDuplicatesResult(Array.Empty<DuplicateCustomerProjection>()));
        }

        // 4. Query store
        var candidates = await _store.FindDuplicatesAsync(
            access.OrganizationId,
            normName,
            normPhone,
            normEmail,
            includeContactPii,
            cancellationToken);

        return Result<CheckCustomerDuplicatesResult>.Success(new CheckCustomerDuplicatesResult(candidates));
    }
}
