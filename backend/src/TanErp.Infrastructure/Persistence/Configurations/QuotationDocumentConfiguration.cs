using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.Commercial;
using TanErp.Domain.Files;
using TanErp.Domain.Organization;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class QuotationDocumentConfiguration : IEntityTypeConfiguration<QuotationDocument>
{
    public void Configure(EntityTypeBuilder<QuotationDocument> builder)
    {
        builder.ToTable("quotation_documents", "commercial", t =>
        {
            t.HasCheckConstraint("ck_quotation_documents_sha256", "sha256 ~ '^[0-9a-f]{64}$'");
            t.HasCheckConstraint("ck_quotation_documents_byte_count", "byte_count > 0");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.QuotationId).HasColumnName("quotation_id").IsRequired();
        builder.Property(x => x.FileId).HasColumnName("file_id").IsRequired();
        builder.Property(x => x.Sha256).HasColumnName("sha256").HasMaxLength(64).IsRequired();
        builder.Property(x => x.ByteCount).HasColumnName("byte_count").IsRequired();
        builder.Property(x => x.RendererVersion).HasColumnName("renderer_version").HasMaxLength(64).IsRequired();
        builder.Property(x => x.RenderedAtUtc).HasColumnName("rendered_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.RenderedByUserId).HasColumnName("rendered_by_user_id").IsRequired();

        // One immutable document per quotation (a quotation is bound to exactly one estimate revision); one file backs one document.
        builder.HasIndex(x => x.QuotationId).IsUnique();
        builder.HasIndex(x => x.FileId).IsUnique();
        builder.HasIndex(x => new { x.OrganizationId, x.QuotationId });

        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Quotation>().WithMany().HasForeignKey(x => x.QuotationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<UploadedFile>()
            .WithMany()
            .HasForeignKey(x => new { x.FileId, x.OrganizationId })
            .HasPrincipalKey(x => new { x.Id, x.OrganizationId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
