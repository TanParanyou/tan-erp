using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAcceptanceOtpSms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_acceptance_otp_challenges_channel",
                schema: "commercial",
                table: "acceptance_otp_challenges");

            migrationBuilder.AddColumn<string>(
                name: "recipient_phone",
                schema: "commercial",
                table: "quotation_acceptance_links",
                type: "character varying(16)",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "ck_acceptance_otp_challenges_channel",
                schema: "commercial",
                table: "acceptance_otp_challenges",
                sql: "channel IN ('email', 'sms')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_acceptance_otp_challenges_channel",
                schema: "commercial",
                table: "acceptance_otp_challenges");

            migrationBuilder.DropColumn(
                name: "recipient_phone",
                schema: "commercial",
                table: "quotation_acceptance_links");

            migrationBuilder.AddCheckConstraint(
                name: "ck_acceptance_otp_challenges_channel",
                schema: "commercial",
                table: "acceptance_otp_challenges",
                sql: "channel IN ('email')");
        }
    }
}
