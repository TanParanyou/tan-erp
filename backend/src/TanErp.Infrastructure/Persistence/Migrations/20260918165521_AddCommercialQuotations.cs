using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCommercialQuotations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "commercial");

            migrationBuilder.CreateTable(
                name: "quotations",
                schema: "commercial",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    opportunity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    estimate_id = table.Column<Guid>(type: "uuid", nullable: false),
                    estimate_revision_id = table.Column<Guid>(type: "uuid", nullable: false),
                    number = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    snapshot_hash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    issued_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    accepted_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quotations", x => x.id);
                    table.CheckConstraint("ck_quotations_status", "status IN ('draft', 'issued', 'accepted', 'rejected', 'expired')");
                    table.CheckConstraint("ck_quotations_total_amount", "total_amount >= 0");
                    table.ForeignKey(
                        name: "FK_quotations_branches_branch_id",
                        column: x => x.branch_id,
                        principalSchema: "organization",
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_quotations_customers_customer_id",
                        column: x => x.customer_id,
                        principalSchema: "crm",
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_quotations_estimate_revisions_estimate_revision_id",
                        column: x => x.estimate_revision_id,
                        principalSchema: "estimates",
                        principalTable: "estimate_revisions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_quotations_estimates_estimate_id",
                        column: x => x.estimate_id,
                        principalSchema: "estimates",
                        principalTable: "estimates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_quotations_opportunities_opportunity_id",
                        column: x => x.opportunity_id,
                        principalSchema: "crm",
                        principalTable: "opportunities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_quotations_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_quotations_branch_id",
                schema: "commercial",
                table: "quotations",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_quotations_customer_id",
                schema: "commercial",
                table: "quotations",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_quotations_estimate_id",
                schema: "commercial",
                table: "quotations",
                column: "estimate_id");

            migrationBuilder.CreateIndex(
                name: "IX_quotations_estimate_revision_id",
                schema: "commercial",
                table: "quotations",
                column: "estimate_revision_id");

            migrationBuilder.CreateIndex(
                name: "IX_quotations_opportunity_id",
                schema: "commercial",
                table: "quotations",
                column: "opportunity_id");

            migrationBuilder.CreateIndex(
                name: "IX_quotations_organization_id_estimate_id",
                schema: "commercial",
                table: "quotations",
                columns: new[] { "organization_id", "estimate_id" });

            migrationBuilder.CreateIndex(
                name: "IX_quotations_organization_id_number",
                schema: "commercial",
                table: "quotations",
                columns: new[] { "organization_id", "number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_quotations_organization_id_opportunity_id",
                schema: "commercial",
                table: "quotations",
                columns: new[] { "organization_id", "opportunity_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "quotations",
                schema: "commercial");
        }
    }
}
