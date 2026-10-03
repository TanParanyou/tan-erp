using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddQuotationAcceptance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "quotation_acceptance_links",
                schema: "commercial",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quotation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    token_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    signer_hint = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    expires_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    revoked_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    revoked_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    accepted_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quotation_acceptance_links", x => x.id);
                    table.CheckConstraint("ck_quotation_acceptance_links_lifetime", "expires_at_utc > created_at_utc");
                    table.CheckConstraint("ck_quotation_acceptance_links_status", "status IN ('active', 'revoked', 'accepted')");
                    table.ForeignKey(
                        name: "FK_quotation_acceptance_links_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_quotation_acceptance_links_quotations_quotation_id",
                        column: x => x.quotation_id,
                        principalSchema: "commercial",
                        principalTable: "quotations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_quotation_acceptance_links_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_quotation_acceptance_links_users_revoked_by_user_id",
                        column: x => x.revoked_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "quotation_acceptance_evidences",
                schema: "commercial",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    link_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quotation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    signer_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    signer_role = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    consent_version = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    signature_image = table.Column<string>(type: "text", nullable: true),
                    signature_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    client_address_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    user_agent = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    accepted_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quotation_acceptance_evidences", x => x.id);
                    table.ForeignKey(
                        name: "FK_quotation_acceptance_evidences_quotation_acceptance_links_l~",
                        column: x => x.link_id,
                        principalSchema: "commercial",
                        principalTable: "quotation_acceptance_links",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_quotation_acceptance_evidences_quotations_quotation_id",
                        column: x => x.quotation_id,
                        principalSchema: "commercial",
                        principalTable: "quotations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_quotation_acceptance_evidences_link_id",
                schema: "commercial",
                table: "quotation_acceptance_evidences",
                column: "link_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_quotation_acceptance_evidences_organization_id_quotation_id",
                schema: "commercial",
                table: "quotation_acceptance_evidences",
                columns: new[] { "organization_id", "quotation_id" });

            migrationBuilder.CreateIndex(
                name: "IX_quotation_acceptance_evidences_quotation_id",
                schema: "commercial",
                table: "quotation_acceptance_evidences",
                column: "quotation_id");

            migrationBuilder.CreateIndex(
                name: "IX_quotation_acceptance_links_created_by_user_id",
                schema: "commercial",
                table: "quotation_acceptance_links",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_quotation_acceptance_links_organization_id_quotation_id",
                schema: "commercial",
                table: "quotation_acceptance_links",
                columns: new[] { "organization_id", "quotation_id" });

            migrationBuilder.CreateIndex(
                name: "IX_quotation_acceptance_links_quotation_id",
                schema: "commercial",
                table: "quotation_acceptance_links",
                column: "quotation_id");

            migrationBuilder.CreateIndex(
                name: "IX_quotation_acceptance_links_revoked_by_user_id",
                schema: "commercial",
                table: "quotation_acceptance_links",
                column: "revoked_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_quotation_acceptance_links_token_hash",
                schema: "commercial",
                table: "quotation_acceptance_links",
                column: "token_hash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "quotation_acceptance_evidences",
                schema: "commercial");

            migrationBuilder.DropTable(
                name: "quotation_acceptance_links",
                schema: "commercial");
        }
    }
}
