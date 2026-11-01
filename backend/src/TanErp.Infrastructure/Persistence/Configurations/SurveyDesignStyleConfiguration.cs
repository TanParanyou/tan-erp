using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TanErp.Domain.Items;
using TanErp.Domain.Surveys;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class SurveyDesignStyleConfiguration : IEntityTypeConfiguration<SurveyDesignStyle>
{
    public void Configure(EntityTypeBuilder<SurveyDesignStyle> builder)
    {
        builder.ToTable("survey_design_styles", "crm");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").HasMaxLength(64).IsRequired();

        var localizedConverter = new ValueConverter<LocalizedText, string>(
            v => JsonSerializer.Serialize(new { th = v.Thai, en = v.English }, (JsonSerializerOptions?)null),
            v => ItemConfiguration.DeserializeLocalizedText(v));

        var optionalLocalizedConverter = new ValueConverter<LocalizedText?, string?>(
            v => v == null ? null : JsonSerializer.Serialize(new { th = v.Thai, en = v.English }, (JsonSerializerOptions?)null),
            v => v == null ? null : ItemConfiguration.DeserializeLocalizedText(v));

        var imageFileIdsConverter = new ValueConverter<IReadOnlyList<string>, string>(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>());

        var imageFileIdsComparer = new ValueComparer<IReadOnlyList<string>>(
            (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList());

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasColumnType("jsonb")
            .HasConversion(localizedConverter)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasColumnType("jsonb")
            .HasConversion(optionalLocalizedConverter);

        builder.Property(x => x.SortOrder).HasColumnName("sort_order").IsRequired();
        builder.Property(x => x.IsActive).HasColumnName("is_active").IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        builder.Property(x => x.ImageFileId).HasColumnName("image_file_id");
        builder.Property(x => x.ImageFileIds)
            .HasColumnName("image_file_ids")
            .HasColumnType("jsonb")
            .HasConversion(imageFileIdsConverter, imageFileIdsComparer)
            .IsRequired();

        builder.HasIndex(x => new { x.OrganizationId, x.Code })
            .IsUnique()
            .HasDatabaseName("ix_survey_design_styles_org_code");

        builder.HasIndex(x => new { x.OrganizationId, x.SortOrder })
            .HasDatabaseName("ix_survey_design_styles_org_sort");

        builder.HasIndex(x => x.ImageFileId);

        builder.HasOne<TanErp.Domain.Files.UploadedFile>()
            .WithMany()
            .HasForeignKey(x => x.ImageFileId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
