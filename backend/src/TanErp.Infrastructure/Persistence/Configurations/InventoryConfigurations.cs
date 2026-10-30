using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Inventory;
using TanErp.Domain.Items;
using TanErp.Domain.Organization;
using TanErp.Domain.Projects;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.ToTable("warehouses", "inventory", t =>
        {
            t.HasCheckConstraint("ck_warehouses_status", "status IN ('active', 'inactive')");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.BranchId).HasColumnName("branch_id").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").HasMaxLength(64).IsRequired();
        builder.Property(x => x.NormalizedCode).HasColumnName("normalized_code").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
        builder.Property(x => x.NormalizedName).HasColumnName("normalized_name").HasMaxLength(200).IsRequired();
        builder.Property(x => x.Address).HasColumnName("address").HasMaxLength(500);
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(16).IsRequired();
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id").IsRequired();
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.HasIndex(x => new { x.OrganizationId, x.NormalizedCode }).IsUnique();
        builder.HasIndex(x => new { x.OrganizationId, x.BranchId, x.Status });
        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Branch>().WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class StockBalanceConfiguration : IEntityTypeConfiguration<StockBalance>
{
    public void Configure(EntityTypeBuilder<StockBalance> builder)
    {
        builder.ToTable("stock_balances", "inventory", t =>
        {
            // Last line of defence: negative stock and over-reservation are impossible even if application checks are bypassed.
            t.HasCheckConstraint("ck_stock_balances_on_hand", "on_hand >= 0");
            t.HasCheckConstraint("ck_stock_balances_reserved", "reserved >= 0 AND reserved <= on_hand");
            t.HasCheckConstraint("ck_stock_balances_total_value", "total_value >= 0");
        });
        builder.HasKey(x => x.Id);
        builder.Ignore(x => x.Available);
        builder.Ignore(x => x.AverageCost);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.WarehouseId).HasColumnName("warehouse_id").IsRequired();
        builder.Property(x => x.ItemId).HasColumnName("item_id").IsRequired();
        builder.Property(x => x.UnitId).HasColumnName("unit_id").IsRequired();
        builder.Property(x => x.OnHand).HasColumnName("on_hand").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.Reserved).HasColumnName("reserved").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.TotalValue).HasColumnName("total_value").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.HasIndex(x => new { x.OrganizationId, x.WarehouseId, x.ItemId }).IsUnique();
        builder.HasIndex(x => new { x.OrganizationId, x.ItemId });
        builder.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Item>().WithMany().HasForeignKey(x => x.ItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<UnitOfMeasure>().WithMany().HasForeignKey(x => x.UnitId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class StockDocumentConfiguration : IEntityTypeConfiguration<StockDocument>
{
    public void Configure(EntityTypeBuilder<StockDocument> builder)
    {
        builder.ToTable("stock_documents", "inventory", t =>
        {
            t.HasCheckConstraint("ck_stock_documents_type", "document_type IN ('receipt', 'issue', 'transfer', 'adjustment', 'return')");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.BranchId).HasColumnName("branch_id").IsRequired();
        builder.Property(x => x.DocumentType).HasColumnName("document_type").HasMaxLength(16).IsRequired();
        builder.Property(x => x.Number).HasColumnName("number").HasMaxLength(64).IsRequired();
        builder.Property(x => x.WarehouseId).HasColumnName("warehouse_id").IsRequired();
        builder.Property(x => x.ToWarehouseId).HasColumnName("to_warehouse_id");
        builder.Property(x => x.ProjectId).HasColumnName("project_id");
        builder.Property(x => x.SourceType).HasColumnName("source_type").HasMaxLength(32);
        builder.Property(x => x.SourceId).HasColumnName("source_id");
        builder.Property(x => x.Reason).HasColumnName("reason").HasMaxLength(500);
        builder.Property(x => x.OccurredAtUtc).HasColumnName("occurred_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.PostedByUserId).HasColumnName("posted_by_user_id").IsRequired();
        builder.Property(x => x.PostedAtUtc).HasColumnName("posted_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.HasIndex(x => new { x.OrganizationId, x.Number }).IsUnique();
        // A source document (e.g. a goods receipt) can be put into stock only once.
        builder.HasIndex(x => new { x.OrganizationId, x.SourceType, x.SourceId }).IsUnique().HasFilter("source_id IS NOT NULL AND source_type <> 'project_return'");
        builder.HasIndex(x => new { x.OrganizationId, x.WarehouseId, x.PostedAtUtc });
        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Branch>().WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.ToWarehouseId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Project>().WithMany().HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.PostedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Movements).WithOne().HasForeignKey(m => m.StockDocumentId).OnDelete(DeleteBehavior.Restrict);
        builder.Navigation(x => x.Movements).HasField("_movements").UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
{
    public void Configure(EntityTypeBuilder<StockMovement> builder)
    {
        builder.ToTable("stock_movements", "inventory", t =>
        {
            t.HasCheckConstraint("ck_stock_movements_quantity", "quantity_delta <> 0");
            t.HasCheckConstraint("ck_stock_movements_kind", "kind IN ('receipt', 'issue', 'transfer_out', 'transfer_in', 'adjustment_in', 'adjustment_out', 'return_in', 'return_out')");
            t.HasCheckConstraint("ck_stock_movements_on_hand_after", "on_hand_after >= 0");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.StockDocumentId).HasColumnName("stock_document_id").IsRequired();
        builder.Property(x => x.WarehouseId).HasColumnName("warehouse_id").IsRequired();
        builder.Property(x => x.LocationId).HasColumnName("location_id");
        builder.Property(x => x.ItemId).HasColumnName("item_id").IsRequired();
        builder.Property(x => x.UnitId).HasColumnName("unit_id").IsRequired();
        builder.Property(x => x.Kind).HasColumnName("kind").HasMaxLength(16).IsRequired();
        builder.Property(x => x.QuantityDelta).HasColumnName("quantity_delta").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.UnitCost).HasColumnName("unit_cost").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.ValueDelta).HasColumnName("value_delta").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.OnHandAfter).HasColumnName("on_hand_after").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.LotNumber).HasColumnName("lot_number").HasMaxLength(64).HasDefaultValue(string.Empty).IsRequired();
        builder.Property(x => x.SerialNumbersJson).HasColumnName("serial_numbers_json").HasColumnType("jsonb");
        builder.Property(x => x.OccurredAtUtc).HasColumnName("occurred_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.PostedAtUtc).HasColumnName("posted_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.HasIndex(x => new { x.OrganizationId, x.WarehouseId, x.ItemId, x.PostedAtUtc });
        builder.HasIndex(x => x.StockDocumentId);
        builder.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<WarehouseLocation>().WithMany().HasForeignKey(x => x.LocationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Item>().WithMany().HasForeignKey(x => x.ItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<UnitOfMeasure>().WithMany().HasForeignKey(x => x.UnitId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class WarehouseLocationConfiguration : IEntityTypeConfiguration<WarehouseLocation>
{
    public void Configure(EntityTypeBuilder<WarehouseLocation> builder)
    {
        builder.ToTable("warehouse_locations", "inventory", t =>
        {
            t.HasCheckConstraint("ck_warehouse_locations_status", "status IN ('active', 'inactive')");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.WarehouseId).HasColumnName("warehouse_id").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").HasMaxLength(32).IsRequired();
        builder.Property(x => x.NormalizedCode).HasColumnName("normalized_code").HasMaxLength(32).IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(120).IsRequired();
        builder.Property(x => x.IsDefault).HasColumnName("is_default").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(16).IsRequired();
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.HasIndex(x => new { x.OrganizationId, x.WarehouseId, x.NormalizedCode }).IsUnique();
        builder.HasIndex(x => new { x.OrganizationId, x.WarehouseId, x.Status });
        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class StockLocationBalanceConfiguration : IEntityTypeConfiguration<StockLocationBalance>
{
    public void Configure(EntityTypeBuilder<StockLocationBalance> builder)
    {
        builder.ToTable("stock_location_balances", "inventory", t =>
        {
            t.HasCheckConstraint("ck_stock_location_balances_on_hand", "on_hand >= 0");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.WarehouseId).HasColumnName("warehouse_id").IsRequired();
        builder.Property(x => x.LocationId).HasColumnName("location_id").IsRequired();
        builder.Property(x => x.ItemId).HasColumnName("item_id").IsRequired();
        builder.Property(x => x.UnitId).HasColumnName("unit_id").IsRequired();
        builder.Property(x => x.LotNumber).HasColumnName("lot_number").HasMaxLength(64).HasDefaultValue(string.Empty).IsRequired();
        builder.Property(x => x.OnHand).HasColumnName("on_hand").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.HasIndex(x => new { x.OrganizationId, x.WarehouseId, x.LocationId, x.ItemId, x.LotNumber }).IsUnique();
        builder.HasIndex(x => new { x.OrganizationId, x.WarehouseId, x.ItemId });
        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<WarehouseLocation>().WithMany().HasForeignKey(x => x.LocationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Item>().WithMany().HasForeignKey(x => x.ItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<UnitOfMeasure>().WithMany().HasForeignKey(x => x.UnitId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class StockSerialConfiguration : IEntityTypeConfiguration<StockSerial>
{
    public void Configure(EntityTypeBuilder<StockSerial> builder)
    {
        builder.ToTable("stock_serials", "inventory", t =>
        {
            t.HasCheckConstraint("ck_stock_serials_status", "status IN ('in_stock', 'issued', 'returned_out')");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.ItemId).HasColumnName("item_id").IsRequired();
        builder.Property(x => x.SerialNumber).HasColumnName("serial_number").HasMaxLength(100).IsRequired();
        builder.Property(x => x.NormalizedSerialNumber).HasColumnName("normalized_serial_number").HasMaxLength(100).IsRequired();
        builder.Property(x => x.WarehouseId).HasColumnName("warehouse_id");
        builder.Property(x => x.LocationId).HasColumnName("location_id");
        builder.Property(x => x.LotNumber).HasColumnName("lot_number").HasMaxLength(64).HasDefaultValue(string.Empty).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(16).IsRequired();
        builder.Property(x => x.LastStockDocumentId).HasColumnName("last_stock_document_id").IsRequired();
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        builder.HasIndex(x => new { x.OrganizationId, x.ItemId, x.NormalizedSerialNumber }).IsUnique();
        builder.HasIndex(x => new { x.OrganizationId, x.WarehouseId, x.ItemId, x.Status });
        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Item>().WithMany().HasForeignKey(x => x.ItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<WarehouseLocation>().WithMany().HasForeignKey(x => x.LocationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<StockDocument>().WithMany().HasForeignKey(x => x.LastStockDocumentId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class InventoryPeriodCloseConfiguration : IEntityTypeConfiguration<InventoryPeriodClose>
{
    public void Configure(EntityTypeBuilder<InventoryPeriodClose> builder)
    {
        builder.ToTable("inventory_period_closes", "inventory", t =>
        {
            t.HasCheckConstraint("ck_inventory_period_closes_month", "period_month >= 1 AND period_month <= 12");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.PeriodYear).HasColumnName("period_year").IsRequired();
        builder.Property(x => x.PeriodMonth).HasColumnName("period_month").IsRequired();
        builder.Property(x => x.PeriodEndDate).HasColumnName("period_end_date").IsRequired();
        builder.Property(x => x.Note).HasColumnName("note").HasMaxLength(500);
        builder.Property(x => x.LineCount).HasColumnName("line_count").IsRequired();
        builder.Property(x => x.TotalOnHandQuantity).HasColumnName("total_on_hand_quantity").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.TotalValuationAmount).HasColumnName("total_valuation_amount").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.ClosedByUserId).HasColumnName("closed_by_user_id").IsRequired();
        builder.Property(x => x.ClosedAtUtc).HasColumnName("closed_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.HasIndex(x => new { x.OrganizationId, x.PeriodYear, x.PeriodMonth }).IsUnique();
        builder.HasIndex(x => new { x.OrganizationId, x.PeriodEndDate });
        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.ClosedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Lines).WithOne().HasForeignKey(l => l.PeriodCloseId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(x => x.Lines).HasField("_lines").UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class InventoryPeriodCloseLineConfiguration : IEntityTypeConfiguration<InventoryPeriodCloseLine>
{
    public void Configure(EntityTypeBuilder<InventoryPeriodCloseLine> builder)
    {
        builder.ToTable("inventory_period_close_lines", "inventory");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.PeriodCloseId).HasColumnName("period_close_id").IsRequired();
        builder.Property(x => x.WarehouseId).HasColumnName("warehouse_id").IsRequired();
        builder.Property(x => x.ItemId).HasColumnName("item_id").IsRequired();
        builder.Property(x => x.UnitId).HasColumnName("unit_id").IsRequired();
        builder.Property(x => x.OnHandQuantity).HasColumnName("on_hand_quantity").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.AverageCost).HasColumnName("average_cost").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.TotalValue).HasColumnName("total_value").HasPrecision(18, 2).IsRequired();
        builder.HasIndex(x => x.PeriodCloseId);
        builder.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Item>().WithMany().HasForeignKey(x => x.ItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<UnitOfMeasure>().WithMany().HasForeignKey(x => x.UnitId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class StockReservationConfiguration : IEntityTypeConfiguration<StockReservation>
{
    public void Configure(EntityTypeBuilder<StockReservation> builder)
    {
        builder.ToTable("stock_reservations", "inventory", t =>
        {
            t.HasCheckConstraint("ck_stock_reservations_status", "status IN ('active', 'released', 'consumed')");
            t.HasCheckConstraint("ck_stock_reservations_quantity", "quantity >= 0");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.WarehouseId).HasColumnName("warehouse_id").IsRequired();
        builder.Property(x => x.ItemId).HasColumnName("item_id").IsRequired();
        builder.Property(x => x.ProjectId).HasColumnName("project_id").IsRequired();
        builder.Property(x => x.Quantity).HasColumnName("quantity").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(16).IsRequired();
        builder.Property(x => x.Note).HasColumnName("note").HasMaxLength(500);
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id").IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        builder.HasIndex(x => new { x.OrganizationId, x.WarehouseId, x.ItemId, x.Status });
        builder.HasIndex(x => new { x.OrganizationId, x.ProjectId, x.Status });
        builder.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Item>().WithMany().HasForeignKey(x => x.ItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Project>().WithMany().HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}
