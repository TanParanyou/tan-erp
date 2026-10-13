using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddQuotationDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "document_sha256",
                schema: "commercial",
                table: "quotation_acceptance_evidences",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "quotation_documents",
                schema: "commercial",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quotation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sha256 = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    byte_count = table.Column<long>(type: "bigint", nullable: false),
                    renderer_version = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    rendered_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    rendered_by_user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quotation_documents", x => x.id);
                    table.CheckConstraint("ck_quotation_documents_byte_count", "byte_count > 0");
                    table.CheckConstraint("ck_quotation_documents_sha256", "sha256 ~ '^[0-9a-f]{64}$'");
                    table.ForeignKey(
                        name: "FK_quotation_documents_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_quotation_documents_quotations_quotation_id",
                        column: x => x.quotation_id,
                        principalSchema: "commercial",
                        principalTable: "quotations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_quotation_documents_uploaded_files_file_id_organization_id",
                        columns: x => new { x.file_id, x.organization_id },
                        principalSchema: "files",
                        principalTable: "uploaded_files",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_quotation_documents_file_id",
                schema: "commercial",
                table: "quotation_documents",
                column: "file_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_quotation_documents_file_id_organization_id",
                schema: "commercial",
                table: "quotation_documents",
                columns: new[] { "file_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_quotation_documents_organization_id_quotation_id",
                schema: "commercial",
                table: "quotation_documents",
                columns: new[] { "organization_id", "quotation_id" });

            migrationBuilder.CreateIndex(
                name: "IX_quotation_documents_quotation_id",
                schema: "commercial",
                table: "quotation_documents",
                column: "quotation_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "quotation_documents",
                schema: "commercial");

            migrationBuilder.DropColumn(
                name: "document_sha256",
                schema: "commercial",
                table: "quotation_acceptance_evidences");
        }
    }
}
