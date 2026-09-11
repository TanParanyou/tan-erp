using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class OpportunityQualificationSlice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "opportunity_stage_history",
                schema: "crm",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    opportunity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    from_stage = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    to_stage = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    reason_code = table.Column<string>(type: "text", nullable: true),
                    note = table.Column<string>(type: "text", nullable: true),
                    actor_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    occurred_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    policy_version = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    trace_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_opportunity_stage_history", x => x.id);
                    table.ForeignKey(
                        name: "FK_opportunity_stage_history_opportunities_opportunity_id_orga~",
                        columns: x => new { x.opportunity_id, x.organization_id },
                        principalSchema: "crm",
                        principalTable: "opportunities",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_opportunity_stage_history_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_opportunity_stage_history_users_actor_user_id",
                        column: x => x.actor_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_opportunity_stage_history_actor_user_id",
                schema: "crm",
                table: "opportunity_stage_history",
                column: "actor_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_opportunity_stage_history_opportunity_id_organization_id",
                schema: "crm",
                table: "opportunity_stage_history",
                columns: new[] { "opportunity_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_opportunity_stage_history_organization_id_opportunity_id_oc~",
                schema: "crm",
                table: "opportunity_stage_history",
                columns: new[] { "organization_id", "opportunity_id", "occurred_at_utc", "id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "opportunity_stage_history",
                schema: "crm");
        }
    }
}
