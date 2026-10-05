using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSharedAttachmentsAndSignatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "attachment_links",
                schema: "files",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_type = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_id = table.Column<Guid>(type: "uuid", nullable: false),
                    purpose = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    removed_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    removed_by_user_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attachment_links", x => x.id);
                    table.CheckConstraint("ck_attachment_links_owner_type_format", "owner_type ~ '^[a-z][a-z0-9-]{1,39}$'");
                    table.CheckConstraint("ck_attachment_links_purpose_format", "purpose ~ '^[a-z][a-z-]{1,31}$'");
                    table.CheckConstraint("ck_attachment_links_removed_pair", "(removed_at_utc IS NULL) = (removed_by_user_id IS NULL)");
                    table.ForeignKey(
                        name: "FK_attachment_links_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_attachment_links_uploaded_files_file_id_organization_id",
                        columns: x => new { x.file_id, x.organization_id },
                        principalSchema: "files",
                        principalTable: "uploaded_files",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_attachment_links_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_attachment_links_users_removed_by_user_id",
                        column: x => x.removed_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "signature_captures",
                schema: "files",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_type = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    purpose = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    signer_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    signer_role = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    signed_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    image_file_id = table.Column<Guid>(type: "uuid", nullable: false),
                    consent_text_version = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    content_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    captured_by_user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_signature_captures", x => x.id);
                    table.CheckConstraint("ck_signature_captures_content_hash", "content_hash ~ '^[0-9a-f]{64}$'");
                    table.CheckConstraint("ck_signature_captures_owner_type_format", "owner_type ~ '^[a-z][a-z0-9-]{1,39}$'");
                    table.CheckConstraint("ck_signature_captures_purpose_format", "purpose ~ '^[a-z][a-z-]{1,31}$'");
                    table.CheckConstraint("ck_signature_captures_signer_name", "char_length(btrim(signer_name)) BETWEEN 2 AND 200");
                    table.ForeignKey(
                        name: "FK_signature_captures_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_signature_captures_uploaded_files_image_file_id_organizatio~",
                        columns: x => new { x.image_file_id, x.organization_id },
                        principalSchema: "files",
                        principalTable: "uploaded_files",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_signature_captures_users_captured_by_user_id",
                        column: x => x.captured_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_attachment_links_created_by_user_id",
                schema: "files",
                table: "attachment_links",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_attachment_links_file",
                schema: "files",
                table: "attachment_links",
                column: "file_id");

            migrationBuilder.CreateIndex(
                name: "IX_attachment_links_file_id_organization_id",
                schema: "files",
                table: "attachment_links",
                columns: new[] { "file_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "ix_attachment_links_owner",
                schema: "files",
                table: "attachment_links",
                columns: new[] { "organization_id", "owner_type", "owner_id" });

            migrationBuilder.CreateIndex(
                name: "IX_attachment_links_removed_by_user_id",
                schema: "files",
                table: "attachment_links",
                column: "removed_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ux_attachment_links_active_owner_file_purpose",
                schema: "files",
                table: "attachment_links",
                columns: new[] { "organization_id", "owner_type", "owner_id", "file_id", "purpose" },
                unique: true,
                filter: "removed_at_utc IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_signature_captures_captured_by_user_id",
                schema: "files",
                table: "signature_captures",
                column: "captured_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_signature_captures_image_file_id_organization_id",
                schema: "files",
                table: "signature_captures",
                columns: new[] { "image_file_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "ix_signature_captures_owner",
                schema: "files",
                table: "signature_captures",
                columns: new[] { "organization_id", "owner_type", "owner_id", "signed_at_utc" });

            migrationBuilder.CreateIndex(
                name: "ux_signature_captures_image_file",
                schema: "files",
                table: "signature_captures",
                column: "image_file_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "attachment_links",
                schema: "files");

            migrationBuilder.DropTable(
                name: "signature_captures",
                schema: "files");
        }
    }
}
