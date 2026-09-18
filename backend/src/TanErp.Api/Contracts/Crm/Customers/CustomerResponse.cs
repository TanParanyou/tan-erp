namespace TanErp.Api.Contracts.Crm.Customers;

public sealed record CustomerContactResponse(
    string Name,
    string? RoleTitle,
    string? Phone,
    string? Email,
    string PreferredChannel,
    bool IsMasked,
    string? LineId = null);

public sealed record DuplicateCustomerResponse(
    Guid Id,
    string Code,
    string DisplayNameTh,
    string? MaskedPhone,
    string? MaskedEmail);

public sealed record CustomerResponse(
    Guid Id,
    string Code,
    string CustomerType,
    string DisplayNameTh,
    string? DisplayNameEn,
    string PreferredLocale,
    string Status,
    CustomerContactResponse PrimaryContact,
    IReadOnlyList<DuplicateCustomerResponse>? DuplicateCandidates,
    Guid RowVersion,
    DateTimeOffset CreatedAtUtc,
    string? LeadSource = null,
    string? LeadSourceNote = null,
    Guid? ImageFileId = null);

