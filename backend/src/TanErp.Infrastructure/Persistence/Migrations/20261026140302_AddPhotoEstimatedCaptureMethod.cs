using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPhotoEstimatedCaptureMethod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_site_survey_measurements_capture_method",
                schema: "crm",
                table: "site_survey_measurements");

            migrationBuilder.AddCheckConstraint(
                name: "CK_site_survey_measurements_capture_method",
                schema: "crm",
                table: "site_survey_measurements",
                sql: "capture_method IN ('measured', 'customer_provided', 'derived', 'photo_estimated')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_site_survey_measurements_capture_method",
                schema: "crm",
                table: "site_survey_measurements");

            migrationBuilder.AddCheckConstraint(
                name: "CK_site_survey_measurements_capture_method",
                schema: "crm",
                table: "site_survey_measurements",
                sql: "capture_method IN ('measured', 'customer_provided', 'derived')");
        }
    }
}
