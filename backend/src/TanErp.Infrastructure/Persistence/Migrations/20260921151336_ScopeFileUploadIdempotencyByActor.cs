using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ScopeFileUploadIdempotencyByActor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_file_upload_sessions_org_idempotency",
                table: "file_upload_sessions");

            migrationBuilder.CreateIndex(
                name: "ux_file_upload_sessions_org_actor_idempotency",
                table: "file_upload_sessions",
                columns: new[] { "organization_id", "created_by_user_id", "idempotency_key_hash" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ux_file_upload_sessions_org_actor_idempotency",
                table: "file_upload_sessions");

            migrationBuilder.CreateIndex(
                name: "ix_file_upload_sessions_org_idempotency",
                table: "file_upload_sessions",
                columns: new[] { "organization_id", "idempotency_key_hash" });
        }
    }
}
