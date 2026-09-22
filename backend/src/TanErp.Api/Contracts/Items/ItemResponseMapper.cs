using TanErp.Application.Items;

namespace TanErp.Api.Contracts.Items;

public static class ItemResponseMapper
{
    public static LocalizedTextResponse ToResponse(LocalizedTextDto dto) => new()
    {
        Thai = dto.Thai,
        English = dto.English
    };

    public static CategorySummaryResponse ToResponse(CategorySummaryDto dto) => new()
    {
        Id = dto.Id,
        Code = dto.Code,
        Name = ToResponse(dto.Name),
        ParentCategoryId = dto.ParentCategoryId
    };

    public static BrandSummaryResponse ToResponse(BrandSummaryDto dto) => new()
    {
        Id = dto.Id,
        Code = dto.Code,
        Name = ToResponse(dto.Name)
    };

    public static UnitSummaryResponse ToResponse(UnitSummaryDto dto) => new()
    {
        Id = dto.Id,
        Code = dto.Code,
        Symbol = dto.Symbol,
        Name = ToResponse(dto.Name)
    };

    public static ItemResponse ToResponse(ItemDetailProjection p) => new()
    {
        Id = p.Id,
        OrganizationId = p.OrganizationId,
        Code = p.Code,
        ItemType = p.ItemType,
        Category = ToResponse(p.Category),
        Brand = p.Brand != null ? ToResponse(p.Brand) : null,
        BaseUnit = ToResponse(p.BaseUnit),
        Name = ToResponse(p.Name),
        Description = p.Description != null ? ToResponse(p.Description) : null,
        TaxCategoryCode = p.TaxCategoryCode,
        AvailabilityMode = p.AvailabilityMode,
        Capabilities = new ItemCapabilitiesResponse
        {
            CanSell = p.Capabilities.CanSell,
            CanCost = p.Capabilities.CanCost,
            CanPurchase = p.Capabilities.CanPurchase,
            CanStock = p.Capabilities.CanStock,
            CanProduce = p.Capabilities.CanProduce
        },
        Attributes = p.Attributes,
        AttributesSchemaVersion = p.AttributesSchemaVersion,
        Status = p.Status,
        ActivatedOnce = p.ActivatedOnce,
        ActivatedAtUtc = p.ActivatedAtUtc,
        ActivatedByUserId = p.ActivatedByUserId,
        InactiveAtUtc = p.InactiveAtUtc,
        InactiveByUserId = p.InactiveByUserId,
        InactiveReasonCode = p.InactiveReasonCode,
        InactiveReason = p.InactiveReason,
        Aliases = p.Aliases.Select(a => new ItemAliasResponse
        {
            Id = a.Id,
            Alias = ToResponse(a.Alias),
            Status = a.Status
        }).ToList(),
        BranchAvailabilities = p.BranchAvailabilities.Select(ba => new ItemBranchAvailabilityResponse
        {
            Id = ba.Id,
            BranchId = ba.BranchId,
            BranchCode = ba.BranchCode,
            BranchName = ba.BranchName,
            Status = ba.Status,
            EffectiveFromUtc = ba.EffectiveFromUtc,
            EffectiveToUtc = ba.EffectiveToUtc
        }).ToList(),
        PrimaryImage = p.PrimaryImage != null ? new ItemImageSummaryResponse
        {
            Id = p.PrimaryImage.Id,
            FileId = p.PrimaryImage.FileId,
            Role = p.PrimaryImage.Role,
            IsPrimary = p.PrimaryImage.IsPrimary,
            DisplayOrder = p.PrimaryImage.DisplayOrder,
            AltText = ToResponse(p.PrimaryImage.AltText),
            Caption = p.PrimaryImage.Caption != null ? ToResponse(p.PrimaryImage.Caption) : null
        } : null,
        RowVersion = p.RowVersion,
        CreatedAtUtc = p.CreatedAtUtc,
        CreatedByUserId = p.CreatedByUserId,
        UpdatedAtUtc = p.UpdatedAtUtc,
        UpdatedByUserId = p.UpdatedByUserId
    };

    public static ItemCategoryDetailResponse ToResponse(ItemCategoryDetailProjection p) => new()
    {
        Id = p.Id,
        OrganizationId = p.OrganizationId,
        Code = p.Code,
        Name = ToResponse(p.Name),
        Description = p.Description != null ? ToResponse(p.Description) : null,
        ParentCategoryId = p.ParentCategoryId,
        ParentCategory = p.ParentCategory != null ? ToResponse(p.ParentCategory) : null,
        AllowedItemTypes = p.AllowedItemTypes,
        SortOrder = p.SortOrder,
        Status = p.Status,
        RowVersion = p.RowVersion,
        CreatedAtUtc = p.CreatedAtUtc,
        UpdatedAtUtc = p.UpdatedAtUtc
    };

    public static ItemBrandDetailResponse ToResponse(ItemBrandDetailProjection p) => new()
    {
        Id = p.Id,
        OrganizationId = p.OrganizationId,
        Code = p.Code,
        Name = ToResponse(p.Name),
        Description = p.Description != null ? ToResponse(p.Description) : null,
        SortOrder = p.SortOrder,
        Status = p.Status,
        RowVersion = p.RowVersion,
        CreatedAtUtc = p.CreatedAtUtc,
        UpdatedAtUtc = p.UpdatedAtUtc
    };

    public static UnitOfMeasureDetailResponse ToResponse(UnitOfMeasureDetailProjection p) => new()
    {
        Id = p.Id,
        OrganizationId = p.OrganizationId,
        Code = p.Code,
        Name = ToResponse(p.Name),
        Symbol = p.Symbol,
        Dimension = p.Dimension,
        DecimalScale = p.DecimalScale,
        RoundingMode = p.RoundingMode,
        Status = p.Status,
        RowVersion = p.RowVersion,
        CreatedAtUtc = p.CreatedAtUtc,
        UpdatedAtUtc = p.UpdatedAtUtc
    };

    public static ItemImageDetailResponse ToResponse(ItemImageDetailProjection p) => new()
    {
        Id = p.Id,
        OrganizationId = p.OrganizationId,
        ItemId = p.ItemId,
        FileId = p.FileId,
        Role = p.Role,
        IsPrimary = p.IsPrimary,
        DisplayOrder = p.DisplayOrder,
        AltText = ToResponse(p.AltText),
        Caption = p.Caption != null ? ToResponse(p.Caption) : null,
        Status = p.Status,
        RowVersion = p.RowVersion,
        CreatedAtUtc = p.CreatedAtUtc,
        CreatedByUserId = p.CreatedByUserId,
        FileName = p.FileName,
        ContentType = p.ContentType,
        ByteSize = p.ByteSize,
        Width = p.Width,
        Height = p.Height
    };

    public static CostRecordResponse ToResponse(CostRecordDetailProjection p) => new(
        p.Id,
        p.OrganizationId,
        p.ItemId,
        p.Scope,
        p.BranchId,
        p.UnitId,
        new LocalizedTextDto(p.UnitName.Thai, p.UnitName.English),
        p.Currency,
        p.Amount,
        p.MinimumQuantity,
        p.MaximumQuantity,
        p.EffectiveFromUtc,
        p.EffectiveToUtc,
        p.Status,
        p.Version,
        p.CostSourceId,
        p.CostSourceName != null ? new LocalizedTextDto(p.CostSourceName.Thai, p.CostSourceName.English) : null,
        p.SourceReference,
        p.Reason,
        p.EvidenceFileId,
        p.CreatedByUserId,
        p.LastFinancialEditorId,
        p.ApprovedByUserId,
        p.PublishedByUserId,
        p.RowVersion,
        p.CreatedAtUtc,
        p.UpdatedAtUtc);
}
