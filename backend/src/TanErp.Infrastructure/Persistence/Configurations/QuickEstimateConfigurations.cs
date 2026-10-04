using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Organization;
using TanErp.Domain.QuickEstimates;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class PricingTemplateConfiguration : IEntityTypeConfiguration<PricingTemplate>
{
    public void Configure(EntityTypeBuilder<PricingTemplate> builder)
    {
        builder.ToTable("pricing_templates", "quick_estimate", t =>
        {
            t.HasCheckConstraint("ck_pricing_templates_status", "status IN ('draft', 'submitted', 'approved', 'calibration', 'active', 'superseded', 'disabled')");
            t.HasCheckConstraint("ck_pricing_templates_work_type", "work_type IN ('built-in', 'curtain', 'wallpaper')");
            t.HasCheckConstraint("ck_pricing_templates_rule", "measurement_rule IN ('area', 'length', 'volume', 'count')");
            t.HasCheckConstraint("ck_pricing_templates_rates", "reference_rate > 0 AND minimum_charge >= 0 AND base_range_rate >= 0 AND max_range_rate >= base_range_rate AND max_range_rate <= 0.9 AND rounding_step > 0");
            t.HasCheckConstraint("ck_pricing_templates_tax", "tax_rate BETWEEN 0 AND 0.3 AND tax_display IN ('exclusive', 'inclusive')");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").HasMaxLength(40).IsRequired();
        builder.Property(x => x.Version).HasColumnName("version").IsRequired();
        builder.Property(x => x.WorkType).HasColumnName("work_type").HasMaxLength(16).IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(16).IsRequired();
        builder.Property(x => x.MeasurementRule).HasColumnName("measurement_rule").HasMaxLength(16).IsRequired();
        builder.Property(x => x.UnitCode).HasColumnName("unit_code").HasMaxLength(20).IsRequired();
        builder.Property(x => x.ReferenceRate).HasColumnName("reference_rate").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.MinimumCharge).HasColumnName("minimum_charge").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.BaseRangeRate).HasColumnName("base_range_rate").HasPrecision(6, 4).IsRequired();
        builder.Property(x => x.MaxRangeRate).HasColumnName("max_range_rate").HasPrecision(6, 4).IsRequired();
        builder.Property(x => x.RoundingStep).HasColumnName("rounding_step").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.ValidityDays).HasColumnName("validity_days").IsRequired();
        builder.Property(x => x.TaxRate).HasColumnName("tax_rate").HasPrecision(6, 4).IsRequired();
        builder.Property(x => x.TaxDisplay).HasColumnName("tax_display").HasMaxLength(16).IsRequired();
        builder.Property(x => x.DirectShareLimit).HasColumnName("direct_share_limit").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.EffectiveFrom).HasColumnName("effective_from");
        builder.Property(x => x.EffectiveTo).HasColumnName("effective_to");
        builder.Property(x => x.ConfigJson).HasColumnName("config").HasColumnType("jsonb").IsRequired();
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id").IsRequired();
        builder.Property(x => x.SubmittedByUserId).HasColumnName("submitted_by_user_id");
        builder.Property(x => x.DecidedByUserId).HasColumnName("decided_by_user_id");
        builder.Property(x => x.DecisionNote).HasColumnName("decision_note").HasMaxLength(500);
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        builder.Ignore(x => x.Config);
        builder.HasIndex(x => new { x.OrganizationId, x.Code, x.Version }).IsUnique();
        // At most one active version per template code.
        builder.HasIndex(x => new { x.OrganizationId, x.Code }).IsUnique().HasFilter("status = 'active'").HasDatabaseName("ux_pricing_templates_one_active");
        builder.HasIndex(x => new { x.OrganizationId, x.Status, x.WorkType });
        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.DecidedByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class QuickEstimateConfiguration : IEntityTypeConfiguration<QuickEstimate>
{
    public void Configure(EntityTypeBuilder<QuickEstimate> builder)
    {
        builder.ToTable("quick_estimates", "quick_estimate", t =>
        {
            t.HasCheckConstraint("ck_quick_estimates_status", "status IN ('draft', 'calculated', 'pending_review', 'approved', 'returned', 'converted')");
            t.HasCheckConstraint("ck_quick_estimates_confidence", "measurement_confidence IN ('low', 'medium', 'high')");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.BranchId).HasColumnName("branch_id").IsRequired();
        builder.Property(x => x.Number).HasColumnName("number").HasMaxLength(64).IsRequired();
        builder.Property(x => x.CustomerId).HasColumnName("customer_id");
        builder.Property(x => x.OpportunityId).HasColumnName("opportunity_id");
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(16).IsRequired();
        builder.Property(x => x.TemplateId).HasColumnName("template_id");
        builder.Property(x => x.PropertyType).HasColumnName("property_type").HasMaxLength(100).IsRequired();
        builder.Property(x => x.RoomOrArea).HasColumnName("room_or_area").HasMaxLength(200).IsRequired();
        builder.Property(x => x.GradeCode).HasColumnName("grade_code").HasMaxLength(40).IsRequired();
        builder.Property(x => x.ComplexityCodesJson).HasColumnName("complexity_codes").HasColumnType("jsonb").IsRequired();
        builder.Property(x => x.AddOnCodesJson).HasColumnName("add_on_codes").HasColumnType("jsonb").IsRequired();
        builder.Property(x => x.MeasurementConfidence).HasColumnName("measurement_confidence").HasMaxLength(8).IsRequired();
        builder.Property(x => x.CustomMaterial).HasColumnName("custom_material").IsRequired();
        builder.Property(x => x.MeasurementsJson).HasColumnName("measurements").HasColumnType("jsonb").IsRequired();
        builder.Property(x => x.CurrentCalculationVersion).HasColumnName("current_calculation_version").IsRequired();
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id").IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        builder.Ignore(x => x.ComplexityCodes);
        builder.Ignore(x => x.AddOnCodes);
        builder.Ignore(x => x.Measurements);
        builder.HasIndex(x => new { x.OrganizationId, x.Number }).IsUnique();
        builder.HasIndex(x => new { x.OrganizationId, x.Status, x.UpdatedAtUtc });
        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Branch>().WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<PricingTemplate>().WithMany().HasForeignKey(x => x.TemplateId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class QuickEstimateCalculationConfiguration : IEntityTypeConfiguration<QuickEstimateCalculation>
{
    public void Configure(EntityTypeBuilder<QuickEstimateCalculation> builder)
    {
        builder.ToTable("quick_estimate_calculations", "quick_estimate", t =>
        {
            t.HasCheckConstraint("ck_quick_estimate_calculations_decision", "share_decision IN ('blocked', 'pending_review', 'shareable')");
            t.HasCheckConstraint("ck_quick_estimate_calculations_range", "displayed_lower <= displayed_upper AND displayed_lower >= 0");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.QuickEstimateId).HasColumnName("quick_estimate_id").IsRequired();
        builder.Property(x => x.Version).HasColumnName("version").IsRequired();
        builder.Property(x => x.TemplateId).HasColumnName("template_id").IsRequired();
        builder.Property(x => x.TemplateCode).HasColumnName("template_code").HasMaxLength(40).IsRequired();
        builder.Property(x => x.TemplateVersion).HasColumnName("template_version").IsRequired();
        builder.Property(x => x.InputHash).HasColumnName("input_hash").HasMaxLength(64).IsRequired();
        builder.Property(x => x.NetAmount).HasColumnName("net_amount").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.TaxAmount).HasColumnName("tax_amount").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.DisplayedLower).HasColumnName("displayed_lower").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.DisplayedUpper).HasColumnName("displayed_upper").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.ValidUntil).HasColumnName("valid_until").IsRequired();
        builder.Property(x => x.ShareDecision).HasColumnName("share_decision").HasMaxLength(16).IsRequired();
        builder.Property(x => x.ReasonCodesJson).HasColumnName("reason_codes").HasColumnType("jsonb").IsRequired();
        builder.Property(x => x.SnapshotJson).HasColumnName("snapshot").HasColumnType("jsonb").IsRequired();
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id").IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.HasIndex(x => new { x.QuickEstimateId, x.Version }).IsUnique();
        builder.HasOne<QuickEstimate>().WithMany().HasForeignKey(x => x.QuickEstimateId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<PricingTemplate>().WithMany().HasForeignKey(x => x.TemplateId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class QuickEstimateReviewConfiguration : IEntityTypeConfiguration<QuickEstimateReview>
{
    public void Configure(EntityTypeBuilder<QuickEstimateReview> builder)
    {
        builder.ToTable("quick_estimate_reviews", "quick_estimate", t =>
        {
            t.HasCheckConstraint("ck_quick_estimate_reviews_status", "status IN ('requested', 'approved', 'returned')");
            t.HasCheckConstraint("ck_quick_estimate_reviews_decider", "decided_by_user_id IS NULL OR decided_by_user_id <> requested_by_user_id");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.QuickEstimateId).HasColumnName("quick_estimate_id").IsRequired();
        builder.Property(x => x.SourceVersion).HasColumnName("source_version").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(16).IsRequired();
        builder.Property(x => x.RequestNote).HasColumnName("request_note").HasMaxLength(500);
        builder.Property(x => x.RequestedByUserId).HasColumnName("requested_by_user_id").IsRequired();
        builder.Property(x => x.RequestedAtUtc).HasColumnName("requested_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.DecidedByUserId).HasColumnName("decided_by_user_id");
        builder.Property(x => x.DecidedAtUtc).HasColumnName("decided_at_utc").HasColumnType("timestamptz");
        builder.Property(x => x.ReasonCode).HasColumnName("reason_code").HasMaxLength(64);
        builder.Property(x => x.DecisionNote).HasColumnName("decision_note").HasMaxLength(500);
        // One review per calculation version: a retried submit cannot create a second request.
        builder.HasIndex(x => new { x.QuickEstimateId, x.SourceVersion }).IsUnique();
        builder.HasOne<QuickEstimate>().WithMany().HasForeignKey(x => x.QuickEstimateId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.RequestedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.DecidedByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class QuickEstimateShareConfiguration : IEntityTypeConfiguration<QuickEstimateShare>
{
    public void Configure(EntityTypeBuilder<QuickEstimateShare> builder)
    {
        builder.ToTable("quick_estimate_shares", "quick_estimate", t =>
        {
            t.HasCheckConstraint("ck_quick_estimate_shares_channel", "channel IN ('onscreen', 'pdf', 'line', 'email')");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.QuickEstimateId).HasColumnName("quick_estimate_id").IsRequired();
        builder.Property(x => x.SourceVersion).HasColumnName("source_version").IsRequired();
        builder.Property(x => x.Channel).HasColumnName("channel").HasMaxLength(16).IsRequired();
        builder.Property(x => x.Recipient).HasColumnName("recipient").HasMaxLength(200);
        builder.Property(x => x.Locale).HasColumnName("locale").HasMaxLength(2).IsRequired();
        builder.Property(x => x.SummaryJson).HasColumnName("summary").HasColumnType("jsonb").IsRequired();
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id").IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.HasIndex(x => new { x.QuickEstimateId, x.SourceVersion });
        builder.HasOne<QuickEstimate>().WithMany().HasForeignKey(x => x.QuickEstimateId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class QuickEstimateConversionConfiguration : IEntityTypeConfiguration<QuickEstimateConversion>
{
    public void Configure(EntityTypeBuilder<QuickEstimateConversion> builder)
    {
        builder.ToTable("quick_estimate_conversions", "quick_estimate");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.QuickEstimateId).HasColumnName("quick_estimate_id").IsRequired();
        builder.Property(x => x.SourceVersion).HasColumnName("source_version").IsRequired();
        builder.Property(x => x.OfficialEstimateId).HasColumnName("official_estimate_id").IsRequired();
        builder.Property(x => x.SourceSnapshotJson).HasColumnName("source_snapshot").HasColumnType("jsonb").IsRequired();
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id").IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        // A calculation version converts once.
        builder.HasIndex(x => new { x.QuickEstimateId, x.SourceVersion }).IsUnique();
        builder.HasIndex(x => x.OfficialEstimateId);
        builder.HasOne<QuickEstimate>().WithMany().HasForeignKey(x => x.QuickEstimateId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}
