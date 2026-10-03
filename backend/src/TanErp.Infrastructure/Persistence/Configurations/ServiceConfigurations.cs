using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Organization;
using TanErp.Domain.Projects;
using TanErp.Domain.Service;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class InstallationJobConfiguration : IEntityTypeConfiguration<InstallationJob>
{
    public void Configure(EntityTypeBuilder<InstallationJob> builder)
    {
        builder.ToTable("installation_jobs", "service", t =>
        {
            t.HasCheckConstraint("ck_installation_jobs_status", "status IN ('planned', 'in_progress', 'ready_for_handover', 'handed_over', 'cancelled')");
            t.HasCheckConstraint("ck_installation_jobs_dates", "scheduled_end >= scheduled_start");
            t.HasCheckConstraint("ck_installation_jobs_warranty_months", "warranty_months IS NULL OR warranty_months BETWEEN 0 AND 120");
            t.HasCheckConstraint("ck_installation_jobs_handover", "(status = 'handed_over') = (handover_date IS NOT NULL)");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.BranchId).HasColumnName("branch_id").IsRequired();
        builder.Property(x => x.ProjectId).HasColumnName("project_id").IsRequired();
        builder.Property(x => x.Number).HasColumnName("number").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(24).IsRequired();
        builder.Property(x => x.ScheduledStart).HasColumnName("scheduled_start").IsRequired();
        builder.Property(x => x.ScheduledEnd).HasColumnName("scheduled_end").IsRequired();
        builder.Property(x => x.CrewName).HasColumnName("crew_name").HasMaxLength(200);
        builder.Property(x => x.Note).HasColumnName("note").HasMaxLength(500);
        builder.Property(x => x.CancelReason).HasColumnName("cancel_reason").HasMaxLength(500);
        builder.Property(x => x.ReadyAtUtc).HasColumnName("ready_at_utc").HasColumnType("timestamptz");
        builder.Property(x => x.HandoverDate).HasColumnName("handover_date");
        builder.Property(x => x.HandoverSignerName).HasColumnName("handover_signer_name").HasMaxLength(200);
        builder.Property(x => x.HandoverNote).HasColumnName("handover_note").HasMaxLength(500);
        builder.Property(x => x.DisputeCount).HasColumnName("dispute_count").IsRequired();
        builder.Property(x => x.LastDisputeNote).HasColumnName("last_dispute_note").HasMaxLength(500);
        builder.Property(x => x.WarrantyMonths).HasColumnName("warranty_months");
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id").IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        builder.HasIndex(x => new { x.OrganizationId, x.Number }).IsUnique();
        builder.HasIndex(x => new { x.OrganizationId, x.ProjectId, x.Status });
        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Branch>().WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Project>().WithMany().HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Checklist).WithOne().HasForeignKey(i => i.InstallationJobId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(x => x.Checklist).HasField("_checklist").UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.HasMany(x => x.Defects).WithOne().HasForeignKey(i => i.InstallationJobId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(x => x.Defects).HasField("_defects").UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class InstallationChecklistItemConfiguration : IEntityTypeConfiguration<InstallationChecklistItem>
{
    public void Configure(EntityTypeBuilder<InstallationChecklistItem> builder)
    {
        builder.ToTable("installation_checklist_items", "service");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.InstallationJobId).HasColumnName("installation_job_id").IsRequired();
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").IsRequired();
        builder.Property(x => x.Title).HasColumnName("title").HasMaxLength(200).IsRequired();
        builder.Property(x => x.Required).HasColumnName("required").IsRequired();
        builder.Property(x => x.Done).HasColumnName("done").IsRequired();
        builder.Property(x => x.DoneByUserId).HasColumnName("done_by_user_id");
        builder.Property(x => x.DoneAtUtc).HasColumnName("done_at_utc").HasColumnType("timestamptz");
        builder.HasIndex(x => new { x.InstallationJobId, x.SortOrder });
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.DoneByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class InstallationDefectConfiguration : IEntityTypeConfiguration<InstallationDefect>
{
    public void Configure(EntityTypeBuilder<InstallationDefect> builder)
    {
        builder.ToTable("installation_defects", "service", t =>
        {
            t.HasCheckConstraint("ck_installation_defects_severity", "severity IN ('minor', 'major', 'critical')");
            t.HasCheckConstraint("ck_installation_defects_status", "status IN ('open', 'resolved', 'verified', 'reopened')");
            t.HasCheckConstraint("ck_installation_defects_verifier", "verified_by_user_id IS NULL OR verified_by_user_id <> resolved_by_user_id");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.InstallationJobId).HasColumnName("installation_job_id").IsRequired();
        builder.Property(x => x.No).HasColumnName("no").IsRequired();
        builder.Property(x => x.Description).HasColumnName("description").HasMaxLength(500).IsRequired();
        builder.Property(x => x.Severity).HasColumnName("severity").HasMaxLength(16).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(16).IsRequired();
        builder.Property(x => x.ReportedByUserId).HasColumnName("reported_by_user_id").IsRequired();
        builder.Property(x => x.ReportedAtUtc).HasColumnName("reported_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.ResolvedByUserId).HasColumnName("resolved_by_user_id");
        builder.Property(x => x.ResolvedAtUtc).HasColumnName("resolved_at_utc").HasColumnType("timestamptz");
        builder.Property(x => x.ResolutionNote).HasColumnName("resolution_note").HasMaxLength(500);
        builder.Property(x => x.VerifiedByUserId).HasColumnName("verified_by_user_id");
        builder.Property(x => x.VerifiedAtUtc).HasColumnName("verified_at_utc").HasColumnType("timestamptz");
        builder.Property(x => x.ReopenCount).HasColumnName("reopen_count").IsRequired();
        builder.Property(x => x.LastReopenReason).HasColumnName("last_reopen_reason").HasMaxLength(500);
        builder.HasIndex(x => new { x.InstallationJobId, x.No }).IsUnique();
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.ReportedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.ResolvedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.VerifiedByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class WarrantyConfiguration : IEntityTypeConfiguration<Warranty>
{
    public void Configure(EntityTypeBuilder<Warranty> builder)
    {
        builder.ToTable("warranties", "service", t =>
        {
            t.HasCheckConstraint("ck_warranties_term", "months BETWEEN 1 AND 120 AND end_date >= start_date");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.ProjectId).HasColumnName("project_id").IsRequired();
        builder.Property(x => x.InstallationJobId).HasColumnName("installation_job_id").IsRequired();
        builder.Property(x => x.Number).HasColumnName("number").HasMaxLength(64).IsRequired();
        builder.Property(x => x.StartDate).HasColumnName("start_date").IsRequired();
        builder.Property(x => x.EndDate).HasColumnName("end_date").IsRequired();
        builder.Property(x => x.Months).HasColumnName("months").IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.HasIndex(x => new { x.OrganizationId, x.Number }).IsUnique();
        // A handover creates exactly one warranty.
        builder.HasIndex(x => x.InstallationJobId).IsUnique();
        builder.HasIndex(x => new { x.OrganizationId, x.ProjectId, x.EndDate });
        builder.HasOne<Project>().WithMany().HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<InstallationJob>().WithMany().HasForeignKey(x => x.InstallationJobId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class ServiceRequestConfiguration : IEntityTypeConfiguration<ServiceRequest>
{
    public void Configure(EntityTypeBuilder<ServiceRequest> builder)
    {
        builder.ToTable("service_requests", "service", t =>
        {
            t.HasCheckConstraint("ck_service_requests_status", "status IN ('open', 'scheduled', 'in_progress', 'resolved', 'closed')");
            t.HasCheckConstraint("ck_service_requests_priority", "priority IN ('low', 'normal', 'high', 'urgent')");
            t.HasCheckConstraint("ck_service_requests_warranty", "in_warranty = (warranty_id IS NOT NULL)");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.BranchId).HasColumnName("branch_id").IsRequired();
        builder.Property(x => x.ProjectId).HasColumnName("project_id").IsRequired();
        builder.Property(x => x.WarrantyId).HasColumnName("warranty_id");
        builder.Property(x => x.Number).HasColumnName("number").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Title).HasColumnName("title").HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasColumnName("description").HasMaxLength(1000).IsRequired();
        builder.Property(x => x.Priority).HasColumnName("priority").HasMaxLength(16).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(16).IsRequired();
        builder.Property(x => x.InWarranty).HasColumnName("in_warranty").IsRequired();
        builder.Property(x => x.ScheduledDate).HasColumnName("scheduled_date");
        builder.Property(x => x.ResolutionNote).HasColumnName("resolution_note").HasMaxLength(500);
        builder.Property(x => x.ReopenCount).HasColumnName("reopen_count").IsRequired();
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id").IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        builder.HasIndex(x => new { x.OrganizationId, x.Number }).IsUnique();
        builder.HasIndex(x => new { x.OrganizationId, x.Status, x.CreatedAtUtc });
        builder.HasIndex(x => new { x.OrganizationId, x.ProjectId });
        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Branch>().WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Project>().WithMany().HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Warranty>().WithMany().HasForeignKey(x => x.WarrantyId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Events).WithOne().HasForeignKey(e => e.ServiceRequestId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(x => x.Events).HasField("_events").UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class ServiceRequestEventConfiguration : IEntityTypeConfiguration<ServiceRequestEvent>
{
    public void Configure(EntityTypeBuilder<ServiceRequestEvent> builder)
    {
        builder.ToTable("service_request_events", "service");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.ServiceRequestId).HasColumnName("service_request_id").IsRequired();
        builder.Property(x => x.FromStatus).HasColumnName("from_status").HasMaxLength(16);
        builder.Property(x => x.ToStatus).HasColumnName("to_status").HasMaxLength(16).IsRequired();
        builder.Property(x => x.Note).HasColumnName("note").HasMaxLength(500);
        builder.Property(x => x.ActorUserId).HasColumnName("actor_user_id").IsRequired();
        builder.Property(x => x.OccurredAtUtc).HasColumnName("occurred_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.HasIndex(x => new { x.ServiceRequestId, x.OccurredAtUtc });
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.ActorUserId).OnDelete(DeleteBehavior.Restrict);
    }
}
