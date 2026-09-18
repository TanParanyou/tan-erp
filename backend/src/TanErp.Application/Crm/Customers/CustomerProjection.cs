namespace TanErp.Application.Crm.Customers;

public sealed record CustomerContactProjection(
    string Name,
    string? RoleTitle,
    string? Phone,
    string? Email,
    string PreferredChannel,
    bool IsMasked,
    string? LineId = null);

public sealed record CustomerProjection(
    Guid Id,
    string Code,
    string CustomerType,
    string DisplayNameTh,
    string? DisplayNameEn,
    string PreferredLocale,
    string Status,
    CustomerContactProjection PrimaryContact,
    Guid RowVersion,
    DateTimeOffset CreatedAtUtc,
    string? LeadSource = null,
    string? LeadSourceNote = null,
    Guid? ImageFileId = null);


public sealed record DuplicateCustomerProjection(
    Guid Id,
    string Code,
    string DisplayNameTh,
    string? MaskedPhone,
    string? MaskedEmail);
