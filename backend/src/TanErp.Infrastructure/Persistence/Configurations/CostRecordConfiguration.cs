using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.Files;
using TanErp.Domain.Items;
using TanErp.Domain.Organization;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class CostRecordConfiguration : IEntityTypeConfiguration<CostRecord>
{
    public void Configure(EntityTypeBuilder<CostRecord> builder)
    {
        builder.ToTable("cost_records", "item_master", t =>
        {
            t.HasCheckConstraint("CK_cost_records_scope", "scope IN ('organization', 'branch')");
            t.HasCheckConstraint("CK_cost_records_status", "status IN ('draft', 'submitted', 'returned', 'approved', 'published', 'superseded', 'disabled')");
            t.HasCheckConstraint("CK_cost_records_amount", "amount > 0");
            t.HasCheckConstraint("CK_cost_records_minimum_quantity", "minimum_quantity >= 0");
            t.HasCheckConstraint("CK_cost_records_maximum_quantity", "maximum_quantity IS NULL OR maximum_quantity > minimum_quantity");
            t.HasCheckConstraint("CK_cost_records_effective_period", "effective_to_utc IS NULL OR effective_to_utc >= effective_from_utc");
        });

        builder.HasKey(x => x.Id);
        builder.HasAlternateKey(x => new { x.Id, x.OrganizationId });

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.ItemId).HasColumnName("item_id").IsRequired();
        builder.Property(x => x.Scope).HasColumnName("scope").HasMaxLength(32).IsRequired();
        builder.Property(x => x.BranchId).HasColumnName("branch_id");
        builder.Property(x => x.UnitId).HasColumnName("unit_id").IsRequired();
        builder.Property(x => x.Currency).HasColumnName("currency").HasMaxLength(3).IsRequired();
        builder.Property(x => x.Amount).HasColumnName("amount").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.MinimumQuantity).HasColumnName("minimum_quantity").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.MaximumQuantity).HasColumnName("maximum_quantity").HasPrecision(18, 4);
        builder.Property(x => x.EffectiveFromUtc).HasColumnName("effective_from_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.EffectiveToUtc).HasColumnName("effective_to_utc").HasColumnType("timestamptz");
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(32).IsRequired();
        builder.Property(x => x.Version).HasColumnName("version").IsRequired();
        builder.Property(x => x.CostSourceId).HasColumnName("cost_source_id");
        builder.Property(x => x.SourceReference).HasColumnName("source_reference").HasMaxLength(128);
        builder.Property(x => x.Reason).HasColumnName("reason").HasMaxLength(500);
        builder.Property(x => x.EvidenceFileId).HasColumnName("evidence_file_id");
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id").IsRequired();
        builder.Property(x => x.LastFinancialEditorId).HasColumnName("last_financial_editor_id").IsRequired();
        builder.Property(x => x.ApprovedByUserId).HasColumnName("approved_by_user_id");
        builder.Property(x => x.PublishedByUserId).HasColumnName("published_by_user_id");
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").HasColumnType("timestamptz").IsRequired();

        // Indexes for Cost Resolution
        builder.HasIndex(x => new { x.OrganizationId, x.ItemId, x.Status, x.Currency, x.UnitId, x.EffectiveFromUtc });
        builder.HasIndex(x => new { x.OrganizationId, x.ItemId, x.Scope, x.BranchId, x.Status });

        // Composite Foreign Keys
        builder.HasOne(x => x.Item)
            .WithMany()
            .HasForeignKey(x => new { x.ItemId, x.OrganizationId })
            .HasPrincipalKey(x => new { x.Id, x.OrganizationId })
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Unit)
            .WithMany()
            .HasForeignKey(x => new { x.UnitId, x.OrganizationId })
            .HasPrincipalKey(x => new { x.Id, x.OrganizationId })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CostSource)
            .WithMany()
            .HasForeignKey(x => new { x.CostSourceId, x.OrganizationId })
            .HasPrincipalKey(x => new { x.Id, x.OrganizationId })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Branch>()
            .WithMany()
            .HasForeignKey(x => x.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<UploadedFile>()
            .WithMany()
            .HasForeignKey(x => new { x.EvidenceFileId, x.OrganizationId })
            .HasPrincipalKey(x => new { x.Id, x.OrganizationId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
