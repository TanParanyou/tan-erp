using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddQuotationLifecycle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_quotations_estimate_id",
                schema: "commercial",
                table: "quotations");

            migrationBuilder.DropCheckConstraint(
                name: "ck_quotations_status",
                schema: "commercial",
                table: "quotations");

            migrationBuilder.AddColumn<string>(
                name: "amendment_reason",
                schema: "commercial",
                table: "quotations",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "superseded_by_quotation_id",
                schema: "commercial",
                table: "quotations",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "supersedes_quotation_id",
                schema: "commercial",
                table: "quotations",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "void_reason",
                schema: "commercial",
                table: "quotations",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "voided_at_utc",
                schema: "commercial",
                table: "quotations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "voided_by_user_id",
                schema: "commercial",
                table: "quotations",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ux_quotations_one_live_per_estimate",
                schema: "commercial",
                table: "quotations",
                column: "estimate_id",
                unique: true,
                filter: "status IN ('issued', 'accepted')");

            migrationBuilder.AddCheckConstraint(
                name: "ck_quotations_status",
                schema: "commercial",
                table: "quotations",
                sql: "status IN ('draft', 'issued', 'accepted', 'rejected', 'expired', 'superseded', 'voided')");

            migrationBuilder.AddCheckConstraint(
                name: "ck_quotations_superseded_fields",
                schema: "commercial",
                table: "quotations",
                sql: "(status = 'superseded') = (superseded_by_quotation_id IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "ck_quotations_void_fields",
                schema: "commercial",
                table: "quotations",
                sql: "(status = 'voided') = (void_reason IS NOT NULL)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ux_quotations_one_live_per_estimate",
                schema: "commercial",
                table: "quotations");

            migrationBuilder.DropCheckConstraint(
                name: "ck_quotations_status",
                schema: "commercial",
                table: "quotations");

            migrationBuilder.DropCheckConstraint(
                name: "ck_quotations_superseded_fields",
                schema: "commercial",
                table: "quotations");

            migrationBuilder.DropCheckConstraint(
                name: "ck_quotations_void_fields",
                schema: "commercial",
                table: "quotations");

            migrationBuilder.DropColumn(
                name: "amendment_reason",
                schema: "commercial",
                table: "quotations");

            migrationBuilder.DropColumn(
                name: "superseded_by_quotation_id",
                schema: "commercial",
                table: "quotations");

            migrationBuilder.DropColumn(
                name: "supersedes_quotation_id",
                schema: "commercial",
                table: "quotations");

            migrationBuilder.DropColumn(
                name: "void_reason",
                schema: "commercial",
                table: "quotations");

            migrationBuilder.DropColumn(
                name: "voided_at_utc",
                schema: "commercial",
                table: "quotations");

            migrationBuilder.DropColumn(
                name: "voided_by_user_id",
                schema: "commercial",
                table: "quotations");

            migrationBuilder.CreateIndex(
                name: "IX_quotations_estimate_id",
                schema: "commercial",
                table: "quotations",
                column: "estimate_id");

            migrationBuilder.AddCheckConstraint(
                name: "ck_quotations_status",
                schema: "commercial",
                table: "quotations",
                sql: "status IN ('draft', 'issued', 'accepted', 'rejected', 'expired')");
        }
    }
}
