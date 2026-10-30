using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Inventory;
using TanErp.Domain.Items;
using TanErp.Domain.Mrp;
using TanErp.Domain.Organization;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class MrpRunConfiguration : IEntityTypeConfiguration<MrpRun>
{
    public void Configure(EntityTypeBuilder<MrpRun> builder)
    {
        builder.ToTable("runs", "mrp", t =>
        {
            t.HasCheckConstraint("ck_mrp_runs_lead_times", "purchase_lead_time_days BETWEEN 0 AND 365 AND production_lead_time_days BETWEEN 0 AND 365");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.BranchId).HasColumnName("branch_id").IsRequired();
        builder.Property(x => x.Number).HasColumnName("number").HasMaxLength(64).IsRequired();
        builder.Property(x => x.AsOfDate).HasColumnName("as_of_date").IsRequired();
        builder.Property(x => x.PurchaseLeadTimeDays).HasColumnName("purchase_lead_time_days").IsRequired();
        builder.Property(x => x.ProductionLeadTimeDays).HasColumnName("production_lead_time_days").IsRequired();
        builder.Property(x => x.InputHash).HasColumnName("input_hash").HasMaxLength(64).IsRequired();
        builder.Property(x => x.SnapshotJson).HasColumnName("snapshot").HasColumnType("jsonb").IsRequired();
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id").IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.HasIndex(x => new { x.OrganizationId, x.Number }).IsUnique();
        builder.HasIndex(x => new { x.OrganizationId, x.CreatedAtUtc });
        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Branch>().WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Recommendations).WithOne().HasForeignKey(r => r.RunId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(x => x.Recommendations).HasField("_recommendations").UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class MrpRecommendationConfiguration : IEntityTypeConfiguration<MrpRecommendation>
{
    public void Configure(EntityTypeBuilder<MrpRecommendation> builder)
    {
        builder.ToTable("recommendations", "mrp", t =>
        {
            t.HasCheckConstraint("ck_mrp_recommendations_action", "action IN ('buy', 'make', 'shortage')");
            t.HasCheckConstraint("ck_mrp_recommendations_status", "status IN ('proposed', 'approved', 'rejected', 'converted')");
            t.HasCheckConstraint("ck_mrp_recommendations_quantity", "quantity > 0");
            t.HasCheckConstraint("ck_mrp_recommendations_converted", "(status = 'converted') = (converted_id IS NOT NULL)");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.RunId).HasColumnName("run_id").IsRequired();
        builder.Property(x => x.LineNo).HasColumnName("line_no").IsRequired();
        builder.Property(x => x.ItemId).HasColumnName("item_id").IsRequired();
        builder.Property(x => x.Action).HasColumnName("action").HasMaxLength(16).IsRequired();
        builder.Property(x => x.Quantity).HasColumnName("quantity").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.NeedBy).HasColumnName("need_by").IsRequired();
        builder.Property(x => x.OrderBy).HasColumnName("order_by").IsRequired();
        builder.Property(x => x.Level).HasColumnName("level").IsRequired();
        builder.Property(x => x.GrossRequirement).HasColumnName("gross_requirement").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.StockUsed).HasColumnName("stock_used").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.ScheduledReceiptsUsed).HasColumnName("scheduled_receipts_used").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.ReasonsJson).HasColumnName("reasons").HasColumnType("jsonb").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(16).IsRequired();
        builder.Property(x => x.DecidedByUserId).HasColumnName("decided_by_user_id");
        builder.Property(x => x.DecidedAtUtc).HasColumnName("decided_at_utc").HasColumnType("timestamptz");
        builder.Property(x => x.ConvertedType).HasColumnName("converted_type").HasMaxLength(32);
        builder.Property(x => x.ConvertedId).HasColumnName("converted_id");
        builder.Property(x => x.ConvertedNumber).HasColumnName("converted_number").HasMaxLength(64);
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        builder.HasIndex(x => new { x.RunId, x.LineNo }).IsUnique();
        builder.HasIndex(x => new { x.OrganizationId, x.Status });
        builder.HasOne<Item>().WithMany().HasForeignKey(x => x.ItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.DecidedByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class MrpItemPlanningParameterConfiguration : IEntityTypeConfiguration<MrpItemPlanningParameter>
{
    public void Configure(EntityTypeBuilder<MrpItemPlanningParameter> builder)
    {
        builder.ToTable("item_planning_parameters", "mrp", t =>
        {
            t.HasCheckConstraint(
                "ck_item_planning_parameters_values",
                "safety_stock >= 0 AND min_order_quantity >= 0 AND lot_multiple >= 0 AND (lead_time_days IS NULL OR (lead_time_days >= 0 AND lead_time_days <= 365))");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.ItemId).HasColumnName("item_id").IsRequired();
        builder.Property(x => x.WarehouseId).HasColumnName("warehouse_id");
        builder.Property(x => x.SafetyStock).HasColumnName("safety_stock").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.MinOrderQuantity).HasColumnName("min_order_quantity").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.LotMultiple).HasColumnName("lot_multiple").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.LeadTimeDays).HasColumnName("lead_time_days");
        builder.Property(x => x.UpdatedByUserId).HasColumnName("updated_by_user_id").IsRequired();
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();

        builder.HasIndex(x => new { x.OrganizationId, x.ItemId })
            .HasDatabaseName("ux_mrp_item_planning_parameters_org_item_default")
            .HasFilter("warehouse_id IS NULL")
            .IsUnique();
        builder.HasIndex(x => new { x.OrganizationId, x.ItemId, x.WarehouseId })
            .HasDatabaseName("ux_mrp_item_planning_parameters_org_item_warehouse")
            .HasFilter("warehouse_id IS NOT NULL")
            .IsUnique();

        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Item>().WithMany().HasForeignKey(x => x.ItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.UpdatedByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class WorkingCalendarHolidayConfiguration : IEntityTypeConfiguration<WorkingCalendarHoliday>
{
    public void Configure(EntityTypeBuilder<WorkingCalendarHoliday> builder)
    {
        builder.ToTable("working_calendar_holidays", "mrp");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.BranchId).HasColumnName("branch_id");
        builder.Property(x => x.HolidayDate).HasColumnName("holiday_date").HasColumnType("date").IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id").IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();

        builder.HasIndex(x => new { x.OrganizationId, x.HolidayDate })
            .HasDatabaseName("ux_mrp_working_calendar_holidays_org_date")
            .HasFilter("branch_id IS NULL")
            .IsUnique();
        builder.HasIndex(x => new { x.OrganizationId, x.BranchId, x.HolidayDate })
            .HasDatabaseName("ux_mrp_working_calendar_holidays_org_branch_date")
            .HasFilter("branch_id IS NOT NULL")
            .IsUnique();

        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Branch>().WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}
