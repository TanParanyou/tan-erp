using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Inventory;
using TanErp.Domain.Items;
using TanErp.Domain.Organization;
using TanErp.Domain.Production;
using TanErp.Domain.Projects;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class BomConfiguration : IEntityTypeConfiguration<Bom>
{
    public void Configure(EntityTypeBuilder<Bom> builder)
    {
        builder.ToTable("boms", "production");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.ItemId).HasColumnName("item_id").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").HasMaxLength(64).IsRequired();
        builder.Property(x => x.NormalizedCode).HasColumnName("normalized_code").HasMaxLength(64).IsRequired();
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id").IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.HasIndex(x => new { x.OrganizationId, x.NormalizedCode }).IsUnique();
        builder.HasIndex(x => new { x.OrganizationId, x.ItemId }).IsUnique();
        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Item>().WithMany().HasForeignKey(x => x.ItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Revisions).WithOne().HasForeignKey(r => r.BomId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(x => x.Revisions).HasField("_revisions").UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class BomRevisionConfiguration : IEntityTypeConfiguration<BomRevision>
{
    public void Configure(EntityTypeBuilder<BomRevision> builder)
    {
        builder.ToTable("bom_revisions", "production", t =>
        {
            t.HasCheckConstraint("ck_bom_revisions_status", "status IN ('draft', 'approved', 'obsolete')");
            t.HasCheckConstraint("ck_bom_revisions_output_quantity", "output_quantity > 0");
            t.HasCheckConstraint("ck_bom_revisions_approver", "approved_by_user_id IS NULL OR approved_by_user_id <> created_by_user_id");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.BomId).HasColumnName("bom_id").IsRequired();
        builder.Property(x => x.RevisionNo).HasColumnName("revision_no").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(16).IsRequired();
        builder.Property(x => x.OutputQuantity).HasColumnName("output_quantity").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.Note).HasColumnName("note").HasMaxLength(500);
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id").IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.ApprovedByUserId).HasColumnName("approved_by_user_id");
        builder.Property(x => x.ApprovedAtUtc).HasColumnName("approved_at_utc").HasColumnType("timestamptz");
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        builder.HasIndex(x => new { x.BomId, x.RevisionNo }).IsUnique();
        // At most one approved revision per BOM.
        builder.HasIndex(x => x.BomId).IsUnique().HasFilter("status = 'approved'").HasDatabaseName("ux_bom_revisions_one_approved");
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.ApprovedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Lines).WithOne().HasForeignKey(l => l.BomRevisionId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(x => x.Lines).HasField("_lines").UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class BomLineConfiguration : IEntityTypeConfiguration<BomLine>
{
    public void Configure(EntityTypeBuilder<BomLine> builder)
    {
        builder.ToTable("bom_lines", "production", t =>
        {
            t.HasCheckConstraint("ck_bom_lines_quantity", "quantity > 0");
            t.HasCheckConstraint("ck_bom_lines_scrap_percent", "scrap_percent >= 0 AND scrap_percent <= 50");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.BomRevisionId).HasColumnName("bom_revision_id").IsRequired();
        builder.Property(x => x.ComponentItemId).HasColumnName("component_item_id").IsRequired();
        builder.Property(x => x.Quantity).HasColumnName("quantity").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.ScrapPercent).HasColumnName("scrap_percent").HasPrecision(5, 2).IsRequired();
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").IsRequired();
        builder.HasIndex(x => new { x.BomRevisionId, x.ComponentItemId }).IsUnique();
        builder.HasIndex(x => x.ComponentItemId);
        builder.HasOne<Item>().WithMany().HasForeignKey(x => x.ComponentItemId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class WorkOrderConfiguration : IEntityTypeConfiguration<WorkOrder>
{
    public void Configure(EntityTypeBuilder<WorkOrder> builder)
    {
        builder.ToTable("work_orders", "production", t =>
        {
            t.HasCheckConstraint("ck_work_orders_status", "status IN ('draft', 'released', 'in_progress', 'completed', 'cancelled')");
            t.HasCheckConstraint("ck_work_orders_quantities", "planned_quantity > 0 AND completed_quantity >= 0 AND completed_quantity <= planned_quantity");
            t.HasCheckConstraint("ck_work_orders_cost_allocated", "cost_allocated >= 0");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.BranchId).HasColumnName("branch_id").IsRequired();
        builder.Property(x => x.Number).HasColumnName("number").HasMaxLength(64).IsRequired();
        builder.Property(x => x.ItemId).HasColumnName("item_id").IsRequired();
        builder.Property(x => x.BomRevisionId).HasColumnName("bom_revision_id").IsRequired();
        builder.Property(x => x.WarehouseId).HasColumnName("warehouse_id").IsRequired();
        builder.Property(x => x.ProjectId).HasColumnName("project_id");
        builder.Property(x => x.PlannedQuantity).HasColumnName("planned_quantity").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.CompletedQuantity).HasColumnName("completed_quantity").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.CostAllocated).HasColumnName("cost_allocated").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(16).IsRequired();
        builder.Property(x => x.Note).HasColumnName("note").HasMaxLength(500);
        builder.Property(x => x.CancelReason).HasColumnName("cancel_reason").HasMaxLength(500);
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id").IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        builder.HasIndex(x => new { x.OrganizationId, x.Number }).IsUnique();
        builder.HasIndex(x => new { x.OrganizationId, x.Status, x.CreatedAtUtc });
        builder.HasIndex(x => new { x.OrganizationId, x.ProjectId });
        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Branch>().WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Item>().WithMany().HasForeignKey(x => x.ItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<BomRevision>().WithMany().HasForeignKey(x => x.BomRevisionId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Project>().WithMany().HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Materials).WithOne().HasForeignKey(m => m.WorkOrderId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(x => x.Materials).HasField("_materials").UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class WorkOrderMaterialConfiguration : IEntityTypeConfiguration<WorkOrderMaterial>
{
    public void Configure(EntityTypeBuilder<WorkOrderMaterial> builder)
    {
        builder.ToTable("work_order_materials", "production", t =>
        {
            t.HasCheckConstraint("ck_work_order_materials_quantities", "required_quantity > 0 AND issued_quantity >= 0 AND returned_quantity >= 0 AND returned_quantity <= issued_quantity");
            t.HasCheckConstraint("ck_work_order_materials_values", "issued_value >= 0 AND returned_value >= 0");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.WorkOrderId).HasColumnName("work_order_id").IsRequired();
        builder.Property(x => x.ItemId).HasColumnName("item_id").IsRequired();
        builder.Property(x => x.RequiredQuantity).HasColumnName("required_quantity").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.IssuedQuantity).HasColumnName("issued_quantity").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.ReturnedQuantity).HasColumnName("returned_quantity").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.IssuedValue).HasColumnName("issued_value").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.ReturnedValue).HasColumnName("returned_value").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").IsRequired();
        builder.HasIndex(x => new { x.WorkOrderId, x.ItemId }).IsUnique();
        builder.HasOne<Item>().WithMany().HasForeignKey(x => x.ItemId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class WorkOrderTransactionConfiguration : IEntityTypeConfiguration<WorkOrderTransaction>
{
    public void Configure(EntityTypeBuilder<WorkOrderTransaction> builder)
    {
        builder.ToTable("work_order_transactions", "production", t =>
        {
            t.HasCheckConstraint("ck_work_order_transactions_kind", "kind IN ('issue', 'return', 'completion')");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.WorkOrderId).HasColumnName("work_order_id").IsRequired();
        builder.Property(x => x.Kind).HasColumnName("kind").HasMaxLength(16).IsRequired();
        builder.Property(x => x.StockDocumentId).HasColumnName("stock_document_id").IsRequired();
        builder.Property(x => x.StockDocumentNumber).HasColumnName("stock_document_number").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Quantity).HasColumnName("quantity").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.Value).HasColumnName("value").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.ActorUserId).HasColumnName("actor_user_id").IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.HasIndex(x => new { x.WorkOrderId, x.CreatedAtUtc });
        builder.HasOne<WorkOrder>().WithMany().HasForeignKey(x => x.WorkOrderId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.ActorUserId).OnDelete(DeleteBehavior.Restrict);
    }
}
