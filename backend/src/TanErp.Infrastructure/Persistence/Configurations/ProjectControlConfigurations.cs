using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Organization;
using TanErp.Domain.Production;
using TanErp.Domain.Projects;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class ProjectWbsNodeConfiguration : IEntityTypeConfiguration<ProjectWbsNode>
{
    public void Configure(EntityTypeBuilder<ProjectWbsNode> builder)
    {
        builder.ToTable("project_wbs_nodes", "projects", t =>
        {
            t.HasCheckConstraint("ck_project_wbs_nodes_level", "level BETWEEN 1 AND 3");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.ProjectId).HasColumnName("project_id").IsRequired();
        builder.Property(x => x.ParentId).HasColumnName("parent_id");
        builder.Property(x => x.Code).HasColumnName("code").HasMaxLength(32).IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
        builder.Property(x => x.Level).HasColumnName("level").IsRequired();
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").IsRequired();
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        builder.HasIndex(x => new { x.OrganizationId, x.ProjectId, x.Code }).IsUnique();
        builder.HasIndex(x => new { x.OrganizationId, x.ProjectId, x.SortOrder });
        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Project>().WithMany().HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<ProjectWbsNode>().WithMany().HasForeignKey(x => x.ParentId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class ProjectBudgetLineConfiguration : IEntityTypeConfiguration<ProjectBudgetLine>
{
    public void Configure(EntityTypeBuilder<ProjectBudgetLine> builder)
    {
        builder.ToTable("project_budget_lines", "projects", t =>
        {
            t.HasCheckConstraint("ck_project_budget_lines_amount", "amount >= 0");
            t.HasCheckConstraint("ck_project_budget_lines_category", "category IN ('material', 'labor', 'subcontract', 'service', 'other')");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.ProjectId).HasColumnName("project_id").IsRequired();
        builder.Property(x => x.WbsNodeId).HasColumnName("wbs_node_id");
        builder.Property(x => x.Category).HasColumnName("category").HasMaxLength(32).IsRequired();
        builder.Property(x => x.Description).HasColumnName("description").HasMaxLength(200).IsRequired();
        builder.Property(x => x.Amount).HasColumnName("amount").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").IsRequired();
        builder.HasIndex(x => new { x.OrganizationId, x.ProjectId, x.SortOrder });
        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Project>().WithMany().HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<ProjectWbsNode>().WithMany().HasForeignKey(x => x.WbsNodeId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class ProjectCostLedgerEntryConfiguration : IEntityTypeConfiguration<ProjectCostLedgerEntry>
{
    public void Configure(EntityTypeBuilder<ProjectCostLedgerEntry> builder)
    {
        builder.ToTable("project_cost_ledger_entries", "projects", t =>
        {
            t.HasCheckConstraint("ck_project_cost_ledger_entries_entry_type", "entry_type IN ('commitment', 'actual')");
            t.HasCheckConstraint("ck_project_cost_ledger_entries_category", "category IN ('material', 'labor', 'subcontract', 'service', 'other')");
            t.HasCheckConstraint("ck_project_cost_ledger_entries_amount", "amount <> 0");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.ProjectId).HasColumnName("project_id").IsRequired();
        builder.Property(x => x.WbsNodeId).HasColumnName("wbs_node_id");
        builder.Property(x => x.Category).HasColumnName("category").HasMaxLength(32).IsRequired();
        builder.Property(x => x.EntryType).HasColumnName("entry_type").HasMaxLength(32).IsRequired();
        builder.Property(x => x.SourceType).HasColumnName("source_type").HasMaxLength(32).IsRequired();
        builder.Property(x => x.SourceId).HasColumnName("source_id").IsRequired();
        builder.Property(x => x.SourceNumber).HasColumnName("source_number").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Description).HasColumnName("description").HasMaxLength(500).IsRequired();
        builder.Property(x => x.Amount).HasColumnName("amount").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.IsReversal).HasColumnName("is_reversal").IsRequired();
        builder.Property(x => x.ActorUserId).HasColumnName("actor_user_id").IsRequired();
        builder.Property(x => x.OccurredAtUtc).HasColumnName("occurred_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.HasIndex(x => new { x.OrganizationId, x.ProjectId, x.OccurredAtUtc });
        builder.HasIndex(x => new { x.OrganizationId, x.SourceType, x.SourceId });
        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Project>().WithMany().HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ProjectWbsNode>().WithMany().HasForeignKey(x => x.WbsNodeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.ActorUserId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class LaborRateConfiguration : IEntityTypeConfiguration<LaborRate>
{
    public void Configure(EntityTypeBuilder<LaborRate> builder)
    {
        builder.ToTable("labor_rates", "projects", t =>
        {
            t.HasCheckConstraint("ck_labor_rates_hourly_rate", "hourly_rate > 0");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.WorkerUserId).HasColumnName("worker_user_id").IsRequired();
        builder.Property(x => x.HourlyRate).HasColumnName("hourly_rate").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.UpdatedByUserId).HasColumnName("updated_by_user_id").IsRequired();
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        builder.HasIndex(x => new { x.OrganizationId, x.WorkerUserId }).IsUnique();
        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.WorkerUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.UpdatedByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class LaborTimesheetEntryConfiguration : IEntityTypeConfiguration<LaborTimesheetEntry>
{
    public void Configure(EntityTypeBuilder<LaborTimesheetEntry> builder)
    {
        builder.ToTable("labor_timesheet_entries", "projects", t =>
        {
            t.HasCheckConstraint("ck_labor_timesheet_entries_hours", "hours >= 0.25 AND hours <= 24");
            t.HasCheckConstraint("ck_labor_timesheet_entries_status", "status IN ('draft', 'submitted', 'approved', 'rejected', 'cancelled')");
            t.HasCheckConstraint("ck_labor_timesheet_entries_decider", "decided_by_user_id IS NULL OR (decided_by_user_id <> created_by_user_id AND decided_by_user_id <> worker_user_id)");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.BranchId).HasColumnName("branch_id").IsRequired();
        builder.Property(x => x.ProjectId).HasColumnName("project_id").IsRequired();
        builder.Property(x => x.WbsNodeId).HasColumnName("wbs_node_id");
        builder.Property(x => x.WorkOrderId).HasColumnName("work_order_id");
        builder.Property(x => x.Number).HasColumnName("number").HasMaxLength(64).IsRequired();
        builder.Property(x => x.WorkerUserId).HasColumnName("worker_user_id").IsRequired();
        builder.Property(x => x.WorkDate).HasColumnName("work_date").IsRequired();
        builder.Property(x => x.Hours).HasColumnName("hours").HasPrecision(6, 2).IsRequired();
        builder.Property(x => x.HourlyRate).HasColumnName("hourly_rate").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.CostAmount).HasColumnName("cost_amount").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.TaskDescription).HasColumnName("task_description").HasMaxLength(500).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(32).IsRequired();
        builder.Property(x => x.IsOvertimeWarning).HasColumnName("is_overtime_warning").IsRequired();
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id").IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.SubmittedAtUtc).HasColumnName("submitted_at_utc").HasColumnType("timestamptz");
        builder.Property(x => x.DecidedByUserId).HasColumnName("decided_by_user_id");
        builder.Property(x => x.DecidedAtUtc).HasColumnName("decided_at_utc").HasColumnType("timestamptz");
        builder.Property(x => x.DecisionNote).HasColumnName("decision_note").HasMaxLength(500);
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        builder.HasIndex(x => new { x.OrganizationId, x.Number }).IsUnique();
        builder.HasIndex(x => new { x.OrganizationId, x.ProjectId, x.Status });
        builder.HasIndex(x => new { x.OrganizationId, x.WorkerUserId, x.WorkDate });
        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Branch>().WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Project>().WithMany().HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ProjectWbsNode>().WithMany().HasForeignKey(x => x.WbsNodeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<WorkOrder>().WithMany().HasForeignKey(x => x.WorkOrderId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.WorkerUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.DecidedByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class ProjectMilestoneConfiguration : IEntityTypeConfiguration<ProjectMilestone>
{
    public void Configure(EntityTypeBuilder<ProjectMilestone> builder)
    {
        builder.ToTable("project_milestones", "projects", t =>
        {
            t.HasCheckConstraint("ck_project_milestones_weight", "weight BETWEEN 1 AND 1000");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.ProjectId).HasColumnName("project_id").IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
        builder.Property(x => x.PlannedDate).HasColumnName("planned_date");
        builder.Property(x => x.Weight).HasColumnName("weight").IsRequired();
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").IsRequired();
        builder.Property(x => x.CompletedAtUtc).HasColumnName("completed_at_utc").HasColumnType("timestamptz");
        builder.Property(x => x.CompletedByUserId).HasColumnName("completed_by_user_id");
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        builder.HasIndex(x => new { x.OrganizationId, x.ProjectId, x.SortOrder });
        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Project>().WithMany().HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.CompletedByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class ProjectChangeOrderConfiguration : IEntityTypeConfiguration<ProjectChangeOrder>
{
    public void Configure(EntityTypeBuilder<ProjectChangeOrder> builder)
    {
        builder.ToTable("project_change_orders", "projects", t =>
        {
            t.HasCheckConstraint("ck_project_change_orders_status", "status IN ('draft', 'submitted', 'approved', 'rejected', 'cancelled')");
            t.HasCheckConstraint("ck_project_change_orders_decider", "decided_by_user_id IS NULL OR decided_by_user_id <> created_by_user_id");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.ProjectId).HasColumnName("project_id").IsRequired();
        builder.Property(x => x.Number).HasColumnName("number").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Title).HasColumnName("title").HasMaxLength(200).IsRequired();
        builder.Property(x => x.Reason).HasColumnName("reason").HasMaxLength(1000).IsRequired();
        builder.Property(x => x.BudgetDelta).HasColumnName("budget_delta").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.ContractDelta).HasColumnName("contract_delta").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(32).IsRequired();
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id").IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.SubmittedAtUtc).HasColumnName("submitted_at_utc").HasColumnType("timestamptz");
        builder.Property(x => x.DecidedByUserId).HasColumnName("decided_by_user_id");
        builder.Property(x => x.DecidedAtUtc).HasColumnName("decided_at_utc").HasColumnType("timestamptz");
        builder.Property(x => x.DecisionNote).HasColumnName("decision_note").HasMaxLength(500);
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        builder.HasIndex(x => new { x.OrganizationId, x.Number }).IsUnique();
        builder.HasIndex(x => new { x.OrganizationId, x.ProjectId, x.Status });
        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Project>().WithMany().HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.DecidedByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class ProjectStatusHistoryConfiguration : IEntityTypeConfiguration<ProjectStatusHistory>
{
    public void Configure(EntityTypeBuilder<ProjectStatusHistory> builder)
    {
        builder.ToTable("project_status_history", "projects");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.ProjectId).HasColumnName("project_id").IsRequired();
        builder.Property(x => x.FromStatus).HasColumnName("from_status").HasMaxLength(32).IsRequired();
        builder.Property(x => x.ToStatus).HasColumnName("to_status").HasMaxLength(32).IsRequired();
        builder.Property(x => x.Reason).HasColumnName("reason").HasMaxLength(500);
        builder.Property(x => x.ActorUserId).HasColumnName("actor_user_id").IsRequired();
        builder.Property(x => x.OccurredAtUtc).HasColumnName("occurred_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.HasIndex(x => new { x.OrganizationId, x.ProjectId, x.OccurredAtUtc });
        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Project>().WithMany().HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.ActorUserId).OnDelete(DeleteBehavior.Restrict);
    }
}
