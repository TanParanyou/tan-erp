using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddItemImportJobs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "item_import_jobs",
                schema: "item_master",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    kind = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    mode = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    content_sha256 = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    content = table.Column<string>(type: "text", nullable: true),
                    total_rows = table.Column<int>(type: "integer", nullable: false),
                    batch_size = table.Column<int>(type: "integer", nullable: false),
                    total_batches = table.Column<int>(type: "integer", nullable: false),
                    next_batch_index = table.Column<int>(type: "integer", nullable: false),
                    processed_rows = table.Column<int>(type: "integer", nullable: false),
                    created_count = table.Column<int>(type: "integer", nullable: false),
                    updated_count = table.Column<int>(type: "integer", nullable: false),
                    unchanged_count = table.Column<int>(type: "integer", nullable: false),
                    rejected_batch_count = table.Column<int>(type: "integer", nullable: false),
                    error_row_count = table.Column<int>(type: "integer", nullable: false),
                    errors_json = table.Column<string>(type: "jsonb", nullable: false),
                    failure_code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    requested_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    requested_by_membership_id = table.Column<Guid>(type: "uuid", nullable: false),
                    requested_by_firebase_uid = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    idempotency_key_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    trace_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    started_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    completed_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    lease_expires_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item_import_jobs", x => x.id);
                    table.CheckConstraint("CK_item_import_jobs_kind", "kind IN ('items', 'costs')");
                    table.CheckConstraint("CK_item_import_jobs_mode", "mode IN ('create', 'upsert')");
                    table.CheckConstraint("CK_item_import_jobs_progress", "processed_rows >= 0 AND processed_rows <= total_rows AND next_batch_index >= 0 AND next_batch_index <= total_batches");
                    table.CheckConstraint("CK_item_import_jobs_status", "status IN ('queued', 'running', 'completed', 'completed_with_errors', 'failed')");
                });

            migrationBuilder.CreateIndex(
                name: "IX_item_import_jobs_organization_id_idempotency_key_hash",
                schema: "item_master",
                table: "item_import_jobs",
                columns: new[] { "organization_id", "idempotency_key_hash" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_item_import_jobs_status_created_at_utc",
                schema: "item_master",
                table: "item_import_jobs",
                columns: new[] { "status", "created_at_utc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "item_import_jobs",
                schema: "item_master");
        }
    }
}
