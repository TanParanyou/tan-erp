using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.Crm.Opportunities;
using TanErp.Domain.Files;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Organization;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class OpportunityWorkImageConfiguration : IEntityTypeConfiguration<OpportunityWorkImage>
{
    public void Configure(EntityTypeBuilder<OpportunityWorkImage> builder)
    {
        builder.ToTable("opportunity_work_images", "crm", t =>
        {
            t.HasCheckConstraint(
                "CK_opportunity_work_images_display_order",
                "display_order >= 0");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.OpportunityId).HasColumnName("opportunity_id").IsRequired();
        builder.Property(x => x.FileId).HasColumnName("file_id").IsRequired();
        builder.Property(x => x.StageAtAttach).HasColumnName("stage_at_attach").HasMaxLength(32).IsRequired();
        builder.Property(x => x.Caption).HasColumnName("caption").HasMaxLength(500);
        builder.Property(x => x.DisplayOrder).HasColumnName("display_order").IsRequired();
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id").IsRequired();

        // Indexes for querying work images by opportunity and stage
        builder.HasIndex(x => new { x.OrganizationId, x.OpportunityId, x.IsDeleted, x.CreatedAtUtc, x.DisplayOrder, x.Id })
            .HasDatabaseName("IX_opportunity_work_images_org_opp_deleted_created_order");
        builder.HasIndex(x => new { x.OrganizationId, x.OpportunityId, x.StageAtAttach, x.IsDeleted })
            .HasDatabaseName("IX_opportunity_work_images_org_opp_stage_deleted");

        // Relationships
        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Opportunity>()
            .WithMany()
            .HasForeignKey(x => new { x.OpportunityId, x.OrganizationId })
            .HasPrincipalKey(x => new { x.Id, x.OrganizationId })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<UploadedFile>()
            .WithMany()
            .HasForeignKey(x => x.FileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
