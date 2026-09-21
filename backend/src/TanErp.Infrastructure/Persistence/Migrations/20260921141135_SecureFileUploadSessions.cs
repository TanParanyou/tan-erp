using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SecureFileUploadSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "file_upload_sessions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    parent_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    parent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    creation_intent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    expires_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    idempotency_key_hash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    request_payload_hash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_file_upload_sessions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "file_upload_slots",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    session_id = table.Column<Guid>(type: "uuid", nullable: false),
                    slot_index = table.Column<int>(type: "integer", nullable: false),
                    filename = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    media_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    file_size_bytes = table.Column<long>(type: "bigint", nullable: false),
                    uploaded_file_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_file_upload_slots", x => x.id);
                    table.ForeignKey(
                        name: "FK_file_upload_slots_file_upload_sessions_session_id",
                        column: x => x.session_id,
                        principalTable: "file_upload_sessions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_file_upload_sessions_creation_intent",
                table: "file_upload_sessions",
                columns: new[] { "organization_id", "creation_intent_id" });

            migrationBuilder.CreateIndex(
                name: "ix_file_upload_sessions_org_idempotency",
                table: "file_upload_sessions",
                columns: new[] { "organization_id", "idempotency_key_hash" });

            migrationBuilder.CreateIndex(
                name: "ix_file_upload_sessions_parent",
                table: "file_upload_sessions",
                columns: new[] { "organization_id", "parent_type", "parent_id" });

            migrationBuilder.CreateIndex(
                name: "ix_file_upload_slots_session_slot_index",
                table: "file_upload_slots",
                columns: new[] { "session_id", "slot_index" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "file_upload_slots");

            migrationBuilder.DropTable(
                name: "file_upload_sessions");
        }
    }
}
