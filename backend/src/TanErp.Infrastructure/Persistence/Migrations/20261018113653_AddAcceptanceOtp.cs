using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAcceptanceOtp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "otp_locked_at_utc",
                schema: "commercial",
                table: "quotation_acceptance_links",
                type: "timestamptz",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "recipient_email",
                schema: "commercial",
                table: "quotation_acceptance_links",
                type: "character varying(254)",
                maxLength: 254,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "otp_challenge_id",
                schema: "commercial",
                table: "quotation_acceptance_evidences",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "otp_channel",
                schema: "commercial",
                table: "quotation_acceptance_evidences",
                type: "character varying(16)",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "otp_verified_at_utc",
                schema: "commercial",
                table: "quotation_acceptance_evidences",
                type: "timestamptz",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "acceptance_otp_challenges",
                schema: "commercial",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    link_id = table.Column<Guid>(type: "uuid", nullable: false),
                    channel = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    code_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    expires_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    failed_attempts = table.Column<int>(type: "integer", nullable: false),
                    verified_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    verification_token_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    consumed_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_acceptance_otp_challenges", x => x.id);
                    table.CheckConstraint("ck_acceptance_otp_challenges_channel", "channel IN ('email')");
                    table.ForeignKey(
                        name: "FK_acceptance_otp_challenges_quotation_acceptance_links_link_id",
                        column: x => x.link_id,
                        principalSchema: "commercial",
                        principalTable: "quotation_acceptance_links",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_acceptance_otp_challenges_link_id_created_at_utc",
                schema: "commercial",
                table: "acceptance_otp_challenges",
                columns: new[] { "link_id", "created_at_utc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "acceptance_otp_challenges",
                schema: "commercial");

            migrationBuilder.DropColumn(
                name: "otp_locked_at_utc",
                schema: "commercial",
                table: "quotation_acceptance_links");

            migrationBuilder.DropColumn(
                name: "recipient_email",
                schema: "commercial",
                table: "quotation_acceptance_links");

            migrationBuilder.DropColumn(
                name: "otp_challenge_id",
                schema: "commercial",
                table: "quotation_acceptance_evidences");

            migrationBuilder.DropColumn(
                name: "otp_channel",
                schema: "commercial",
                table: "quotation_acceptance_evidences");

            migrationBuilder.DropColumn(
                name: "otp_verified_at_utc",
                schema: "commercial",
                table: "quotation_acceptance_evidences");
        }
    }
}
