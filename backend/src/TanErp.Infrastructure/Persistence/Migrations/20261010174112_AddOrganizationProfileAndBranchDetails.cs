using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationProfileAndBranchDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "address_en",
                schema: "organization",
                table: "organizations",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "address_th",
                schema: "organization",
                table: "organizations",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "name_en",
                schema: "organization",
                table: "organizations",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "phone",
                schema: "organization",
                table: "organizations",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "row_version",
                schema: "organization",
                table: "organizations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "tax_identifier",
                schema: "organization",
                table: "organizations",
                type: "character varying(13)",
                maxLength: 13,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "address_en",
                schema: "organization",
                table: "branches",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "address_th",
                schema: "organization",
                table: "branches",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "name_en",
                schema: "organization",
                table: "branches",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "phone",
                schema: "organization",
                table: "branches",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "row_version",
                schema: "organization",
                table: "branches",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "tax_branch_code",
                schema: "organization",
                table: "branches",
                type: "character(5)",
                fixedLength: true,
                maxLength: 5,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_branches_organization_id_tax_branch_code",
                schema: "organization",
                table: "branches",
                columns: new[] { "organization_id", "tax_branch_code" },
                unique: true,
                filter: "tax_branch_code IS NOT NULL");

            migrationBuilder.Sql("UPDATE organization.organizations SET row_version = gen_random_uuid();");
            migrationBuilder.Sql("UPDATE organization.branches SET row_version = gen_random_uuid();");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_branches_organization_id_tax_branch_code",
                schema: "organization",
                table: "branches");

            migrationBuilder.DropColumn(
                name: "address_en",
                schema: "organization",
                table: "organizations");

            migrationBuilder.DropColumn(
                name: "address_th",
                schema: "organization",
                table: "organizations");

            migrationBuilder.DropColumn(
                name: "name_en",
                schema: "organization",
                table: "organizations");

            migrationBuilder.DropColumn(
                name: "phone",
                schema: "organization",
                table: "organizations");

            migrationBuilder.DropColumn(
                name: "row_version",
                schema: "organization",
                table: "organizations");

            migrationBuilder.DropColumn(
                name: "tax_identifier",
                schema: "organization",
                table: "organizations");

            migrationBuilder.DropColumn(
                name: "address_en",
                schema: "organization",
                table: "branches");

            migrationBuilder.DropColumn(
                name: "address_th",
                schema: "organization",
                table: "branches");

            migrationBuilder.DropColumn(
                name: "name_en",
                schema: "organization",
                table: "branches");

            migrationBuilder.DropColumn(
                name: "phone",
                schema: "organization",
                table: "branches");

            migrationBuilder.DropColumn(
                name: "row_version",
                schema: "organization",
                table: "branches");

            migrationBuilder.DropColumn(
                name: "tax_branch_code",
                schema: "organization",
                table: "branches");
        }
    }
}
