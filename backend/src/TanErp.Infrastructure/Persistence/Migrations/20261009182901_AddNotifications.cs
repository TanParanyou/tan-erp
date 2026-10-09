using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "notifications");

            migrationBuilder.CreateTable(
                name: "notifications",
                schema: "notifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    recipient_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    payload = table.Column<string>(type: "jsonb", nullable: false),
                    dedupe_key = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    read_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.id);
                    table.CheckConstraint("ck_notifications_dedupe_key_length", "char_length(btrim(dedupe_key)) BETWEEN 1 AND 160");
                    table.CheckConstraint("ck_notifications_payload_object", "jsonb_typeof(payload) = 'object'");
                    table.CheckConstraint("ck_notifications_type_format", "type ~ '^[a-z][a-z0-9.-]{1,59}$'");
                    table.ForeignKey(
                        name: "FK_notifications_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_notifications_users_recipient_user_id",
                        column: x => x.recipient_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_notifications_recipient_created",
                schema: "notifications",
                table: "notifications",
                columns: new[] { "organization_id", "recipient_user_id", "created_at_utc", "id" },
                descending: new[] { false, false, true, true });

            migrationBuilder.CreateIndex(
                name: "IX_notifications_recipient_user_id",
                schema: "notifications",
                table: "notifications",
                column: "recipient_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_notifications_unread",
                schema: "notifications",
                table: "notifications",
                columns: new[] { "organization_id", "recipient_user_id" },
                filter: "read_at_utc IS NULL");

            migrationBuilder.CreateIndex(
                name: "ux_notifications_recipient_dedupe",
                schema: "notifications",
                table: "notifications",
                columns: new[] { "organization_id", "recipient_user_id", "dedupe_key" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "notifications",
                schema: "notifications");
        }
    }
}
