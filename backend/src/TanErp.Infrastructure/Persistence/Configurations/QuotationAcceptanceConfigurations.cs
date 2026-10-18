using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.Commercial;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Organization;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class QuotationAcceptanceLinkConfiguration : IEntityTypeConfiguration<QuotationAcceptanceLink>
{
    public void Configure(EntityTypeBuilder<QuotationAcceptanceLink> builder)
    {
        builder.ToTable("quotation_acceptance_links", "commercial", t =>
        {
            t.HasCheckConstraint("ck_quotation_acceptance_links_status", "status IN ('active', 'revoked', 'accepted')");
            t.HasCheckConstraint("ck_quotation_acceptance_links_lifetime", "expires_at_utc > created_at_utc");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.QuotationId).HasColumnName("quotation_id").IsRequired();
        builder.Property(x => x.TokenHash).HasColumnName("token_hash").HasMaxLength(64).IsRequired();
        builder.Property(x => x.SignerHint).HasColumnName("signer_hint").HasMaxLength(200);
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(16).IsRequired();
        builder.Property(x => x.ExpiresAtUtc).HasColumnName("expires_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id").IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.RevokedAtUtc).HasColumnName("revoked_at_utc").HasColumnType("timestamptz");
        builder.Property(x => x.RevokedByUserId).HasColumnName("revoked_by_user_id");
        builder.Property(x => x.AcceptedAtUtc).HasColumnName("accepted_at_utc").HasColumnType("timestamptz");
        builder.Property(x => x.RecipientEmail).HasColumnName("recipient_email").HasMaxLength(254);
        builder.Property(x => x.OtpLockedAtUtc).HasColumnName("otp_locked_at_utc").HasColumnType("timestamptz");
        builder.HasIndex(x => x.TokenHash).IsUnique();
        builder.HasIndex(x => new { x.OrganizationId, x.QuotationId });
        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Quotation>().WithMany().HasForeignKey(x => x.QuotationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.RevokedByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class QuotationAcceptanceEvidenceConfiguration : IEntityTypeConfiguration<QuotationAcceptanceEvidence>
{
    public void Configure(EntityTypeBuilder<QuotationAcceptanceEvidence> builder)
    {
        builder.ToTable("quotation_acceptance_evidences", "commercial");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.LinkId).HasColumnName("link_id").IsRequired();
        builder.Property(x => x.QuotationId).HasColumnName("quotation_id").IsRequired();
        builder.Property(x => x.SignerName).HasColumnName("signer_name").HasMaxLength(200).IsRequired();
        builder.Property(x => x.SignerRole).HasColumnName("signer_role").HasMaxLength(100);
        builder.Property(x => x.ConsentVersion).HasColumnName("consent_version").HasMaxLength(32).IsRequired();
        builder.Property(x => x.SignatureImage).HasColumnName("signature_image");
        builder.Property(x => x.SignatureHash).HasColumnName("signature_hash").HasMaxLength(64);
        builder.Property(x => x.ClientAddressHash).HasColumnName("client_address_hash").HasMaxLength(64).IsRequired();
        builder.Property(x => x.UserAgent).HasColumnName("user_agent").HasMaxLength(200);
        builder.Property(x => x.AcceptedAtUtc).HasColumnName("accepted_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.DocumentSha256).HasColumnName("document_sha256").HasMaxLength(64);
        builder.Property(x => x.OtpChannel).HasColumnName("otp_channel").HasMaxLength(16);
        builder.Property(x => x.OtpVerifiedAtUtc).HasColumnName("otp_verified_at_utc").HasColumnType("timestamptz");
        builder.Property(x => x.OtpChallengeId).HasColumnName("otp_challenge_id");
        // One acceptance per link, ever.
        builder.HasIndex(x => x.LinkId).IsUnique();
        builder.HasIndex(x => new { x.OrganizationId, x.QuotationId });
        builder.HasOne<QuotationAcceptanceLink>().WithMany().HasForeignKey(x => x.LinkId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Quotation>().WithMany().HasForeignKey(x => x.QuotationId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class AcceptanceOtpChallengeConfiguration : IEntityTypeConfiguration<AcceptanceOtpChallenge>
{
    public void Configure(EntityTypeBuilder<AcceptanceOtpChallenge> builder)
    {
        builder.ToTable("acceptance_otp_challenges", "commercial", t => t.HasCheckConstraint("ck_acceptance_otp_challenges_channel", "channel IN ('email')"));
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.LinkId).HasColumnName("link_id").IsRequired();
        builder.Property(x => x.Channel).HasColumnName("channel").HasMaxLength(16).IsRequired();
        builder.Property(x => x.CodeHash).HasColumnName("code_hash").HasMaxLength(64).IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.ExpiresAtUtc).HasColumnName("expires_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.FailedAttempts).HasColumnName("failed_attempts").IsRequired();
        builder.Property(x => x.VerifiedAtUtc).HasColumnName("verified_at_utc").HasColumnType("timestamptz");
        builder.Property(x => x.VerificationTokenHash).HasColumnName("verification_token_hash").HasMaxLength(64);
        builder.Property(x => x.ConsumedAtUtc).HasColumnName("consumed_at_utc").HasColumnType("timestamptz");
        builder.HasIndex(x => new { x.LinkId, x.CreatedAtUtc });
        builder.HasOne<QuotationAcceptanceLink>().WithMany().HasForeignKey(x => x.LinkId).OnDelete(DeleteBehavior.Restrict);
    }
}
