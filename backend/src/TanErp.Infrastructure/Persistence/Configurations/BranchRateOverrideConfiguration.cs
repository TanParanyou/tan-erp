using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.QuickEstimates;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class BranchRateOverrideConfiguration : IEntityTypeConfiguration<BranchRateOverride>
{
    public void Configure(EntityTypeBuilder<BranchRateOverride> builder)
    {
        builder.ToTable("branch_rate_overrides", "quick_estimate", t =>
        {
            t.HasCheckConstraint("ck_branch_rate_overrides_status", "status IN ('requested', 'approved', 'returned', 'revoked')");
            t.HasCheckConstraint("ck_branch_rate_overrides_rates", "reference_rate > 0 AND minimum_charge >= 0");
            t.HasCheckConstraint("ck_branch_rate_overrides_period", "effective_to >= effective_from");
            t.HasCheckConstraint("ck_branch_rate_overrides_decider", "decided_by_user_id IS NULL OR decided_by_user_id <> requested_by_user_id");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id"); builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.BranchId).HasColumnName("branch_id").IsRequired(); builder.Property(x => x.TemplateId).HasColumnName("template_id").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(16).IsRequired(); builder.Property(x => x.ReferenceRate).HasColumnName("reference_rate").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.MinimumCharge).HasColumnName("minimum_charge").HasPrecision(18, 2).IsRequired(); builder.Property(x => x.EffectiveFrom).HasColumnName("effective_from").IsRequired();
        builder.Property(x => x.EffectiveTo).HasColumnName("effective_to").IsRequired(); builder.Property(x => x.ReasonCode).HasColumnName("reason_code").HasMaxLength(64).IsRequired();
        builder.Property(x => x.ReasonDetail).HasColumnName("reason_detail").HasMaxLength(500).IsRequired(); builder.Property(x => x.SourceReference).HasColumnName("source_reference").HasMaxLength(200);
        builder.Property(x => x.RequestedByUserId).HasColumnName("requested_by_user_id").IsRequired(); builder.Property(x => x.RequestedAtUtc).HasColumnName("requested_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.DecidedByUserId).HasColumnName("decided_by_user_id"); builder.Property(x => x.DecidedAtUtc).HasColumnName("decided_at_utc").HasColumnType("timestamptz");
        builder.Property(x => x.DecisionNote).HasColumnName("decision_note").HasMaxLength(500); builder.Property(x => x.ClosedByUserId).HasColumnName("closed_by_user_id");
        builder.Property(x => x.ClosedAtUtc).HasColumnName("closed_at_utc").HasColumnType("timestamptz"); builder.Property(x => x.CloseNote).HasColumnName("close_note").HasMaxLength(500);
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        builder.HasIndex(x => new { x.OrganizationId, x.TemplateId, x.BranchId, x.Status });
        builder.HasOne<TanErp.Domain.Organization.Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<TanErp.Domain.Organization.Branch>().WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<PricingTemplate>().WithMany().HasForeignKey(x => x.TemplateId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.RequestedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.DecidedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.ClosedByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}
