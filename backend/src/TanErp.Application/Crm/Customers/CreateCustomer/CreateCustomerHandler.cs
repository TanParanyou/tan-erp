using System.Security.Cryptography;
using System.Text;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Domain.Common;
using TanErp.Domain.Crm.Customers;

namespace TanErp.Application.Crm.Customers.CreateCustomer;

public class CreateCustomerHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly ICustomerCreationStore _store;
    private readonly IClock _clock;

    public CreateCustomerHandler(
        IRequestAccessResolver accessResolver,
        ICustomerCreationStore store,
        IClock clock)
    {
        _accessResolver = accessResolver;
        _store = store;
        _clock = clock;
    }

    public async Task<Result<CreateCustomerResult>> Handle(
        CreateCustomerCommand command,
        CancellationToken cancellationToken = default)
    {
        // 1. Resolve required customers.create permission
        var accessResult = await _accessResolver.ResolveAsync(
            command.FirebaseUid,
            command.MembershipId,
            "customers.create",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return Result<CreateCustomerResult>.Failure(accessResult.Error);
        }

        var access = accessResult.Value!;

        // 2. Resolve required customer-contacts.manage permission for contact creation (fail closed)
        var manageContactResult = await _accessResolver.ResolveAsync(
            command.FirebaseUid,
            command.MembershipId,
            "customer-contacts.manage",
            cancellationToken);

        if (manageContactResult.IsFailure)
            return Result<CreateCustomerResult>.Failure(manageContactResult.Error);

        const bool includeContactPii = true;

        // 3. Validation
        if (string.IsNullOrWhiteSpace(command.DisplayNameTh))
        {
            return Result<CreateCustomerResult>.Failure(new Error("CUSTOMER_FIELD_REQUIRED", "Customer Thai display name is required."));
        }

        if (!CustomerType.IsValid(command.CustomerType))
        {
            return Result<CreateCustomerResult>.Failure(new Error("CUSTOMER_FIELD_REQUIRED", $"Invalid customer type: '{command.CustomerType}'."));
        }

        if (!PreferredLocale.IsValid(command.PreferredLocale))
        {
            return Result<CreateCustomerResult>.Failure(new Error("CUSTOMER_FIELD_REQUIRED", $"Invalid preferred locale: '{command.PreferredLocale}'."));
        }

        if (command.PrimaryContact == null || string.IsNullOrWhiteSpace(command.PrimaryContact.Name))
        {
            return Result<CreateCustomerResult>.Failure(new Error("CONTACT_FIELD_REQUIRED", "Primary contact name is required."));
        }

        var normalizedPhone = CustomerNormalizer.NormalizePhone(command.PrimaryContact.Phone);
        var normalizedEmail = CustomerNormalizer.NormalizeEmail(command.PrimaryContact.Email);

        if (normalizedEmail is not null && !CustomerNormalizer.IsValidEmail(normalizedEmail))
        {
            return Result<CreateCustomerResult>.Failure(new Error("CONTACT_FIELD_REQUIRED", "Contact email address is invalid."));
        }

        if (string.IsNullOrWhiteSpace(normalizedPhone) && string.IsNullOrWhiteSpace(normalizedEmail))
        {
            return Result<CreateCustomerResult>.Failure(new Error("CONTACT_FIELD_REQUIRED", "Contact must provide at least a phone number or an email address."));
        }

        if (!ContactChannel.IsValid(command.PrimaryContact.PreferredChannel))
        {
            return Result<CreateCustomerResult>.Failure(new Error("CONTACT_FIELD_REQUIRED", $"Invalid preferred channel: '{command.PrimaryContact.PreferredChannel}'."));
        }

        if (!string.IsNullOrWhiteSpace(command.LeadSource) && !CustomerLeadSource.IsValid(command.LeadSource))
        {
            return Result<CreateCustomerResult>.Failure(new Error("CUSTOMER_FIELD_REQUIRED", $"Invalid lead source: '{command.LeadSource}'."));
        }

        if (command.PrimaryContact.LineId?.Trim().Length > 100)
        {
            return Result<CreateCustomerResult>.Failure(new Error("CONTACT_FIELD_REQUIRED", "Line ID cannot exceed 100 characters."));
        }

        var now = _clock.UtcNow;
        var customerId = Guid.NewGuid();

        // 4. Create Customer Aggregate
        var customer = Customer.CreateDraft(
            customerId,
            access.OrganizationId,
            access.ActorUserId,
            command.CustomerType,
            command.DisplayNameTh,
            command.DisplayNameEn,
            command.PreferredLocale,
            new PrimaryContactInput(
                command.PrimaryContact.Name,
                command.PrimaryContact.RoleTitle,
                command.PrimaryContact.Phone,
                command.PrimaryContact.Email,
                command.PrimaryContact.PreferredChannel,
                command.PrimaryContact.LineId),
            now,
            command.LeadSource);

        var primaryContactEntity = customer.Contacts.First();

        // 5. Create deterministic hashes
        var keyHash = ComputeSha256Hex(command.IdempotencyKey);
        var canonicalPayload = $"{command.CustomerType}|{CustomerNormalizer.CollapseWhitespace(command.DisplayNameTh)}|{CustomerNormalizer.CollapseWhitespace(command.DisplayNameEn ?? "")}|{command.PreferredLocale}|{command.LeadSource?.Trim() ?? ""}|{CustomerNormalizer.CollapseWhitespace(command.PrimaryContact.Name)}|{CustomerNormalizer.CollapseWhitespace(command.PrimaryContact.RoleTitle ?? "")}|{normalizedPhone ?? ""}|{normalizedEmail ?? ""}|{command.PrimaryContact.LineId?.Trim() ?? ""}|{command.PrimaryContact.PreferredChannel}";
        var payloadHash = ComputeSha256Hex(canonicalPayload);

        // 6. Audit Events (Changed fields only, never raw PII)
        const string customerChangesJson = "{\"changedFields\":[\"customerType\",\"displayNameTh\",\"displayNameEn\",\"preferredLocale\",\"leadSource\",\"primaryContact\"]}";
        const string contactChangesJson = "{\"changedFields\":[\"name\",\"roleTitle\",\"phone\",\"email\",\"lineId\",\"preferredChannel\"]}";

        var auditEvents = new List<AuditEvent>
        {
            new(
                Guid.NewGuid(),
                access.OrganizationId,
                access.ActorUserId,
                "customer.created",
                "Customer",
                customerId.ToString(),
                now,
                command.TraceId,
                customerChangesJson),
            new(
                Guid.NewGuid(),
                access.OrganizationId,
                access.ActorUserId,
                "contact.created",
                "CustomerContact",
                primaryContactEntity.Id.ToString(),
                now,
                command.TraceId,
                contactChangesJson)
        };

        // 7. Persist via Store
        var persistRequest = new PersistCustomerCreation(
            customer,
            "customers.create",
            keyHash,
            payloadHash,
            auditEvents,
            includeContactPii);

        var persistResult = await _store.CreateAsync(persistRequest, cancellationToken);
        if (persistResult.IsFailure)
        {
            return Result<CreateCustomerResult>.Failure(persistResult.Error);
        }

        return Result<CreateCustomerResult>.Success(new CreateCustomerResult(
            persistResult.Value!.Customer,
            persistResult.Value.DuplicateCandidates,
            persistResult.Value.WasReplayed));
    }

    private static string ComputeSha256Hex(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
