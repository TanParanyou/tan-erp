using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TanErp.Infrastructure.Persistence;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260927125106_AddEstimateDiscountModes")]
public partial class AddEstimateDiscountModes : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "discount_type",
            schema: "estimates",
            table: "estimate_revisions",
            type: "character varying(32)",
            maxLength: 32,
            nullable: true);

        migrationBuilder.AddColumn<decimal>(
            name: "discount_value",
            schema: "estimates",
            table: "estimate_revisions",
            type: "numeric(19,6)",
            precision: 19,
            scale: 6,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "discount_reason_code",
            schema: "estimates",
            table: "estimate_revisions",
            type: "character varying(64)",
            maxLength: 64,
            nullable: true);

        migrationBuilder.Sql("""
            UPDATE estimates.estimate_revisions
            SET discount_type = CASE WHEN discount_amount > 0 THEN 'fixed-amount' ELSE 'none' END,
                discount_value = discount_amount;
            """);

        migrationBuilder.AlterColumn<string>(
            name: "discount_type",
            schema: "estimates",
            table: "estimate_revisions",
            type: "character varying(32)",
            maxLength: 32,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(32)",
            oldMaxLength: 32,
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "discount_value",
            schema: "estimates",
            table: "estimate_revisions",
            type: "numeric(19,6)",
            precision: 19,
            scale: 6,
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "numeric(19,6)",
            oldPrecision: 19,
            oldScale: 6,
            oldNullable: true);

        migrationBuilder.AddCheckConstraint(
            name: "ck_estimate_revisions_discount_type",
            schema: "estimates",
            table: "estimate_revisions",
            sql: "discount_type IN ('none', 'percent', 'fixed-amount')");

        migrationBuilder.AddCheckConstraint(
            name: "ck_estimate_revisions_discount_value",
            schema: "estimates",
            table: "estimate_revisions",
            sql: "discount_value >= 0 AND (discount_type <> 'percent' OR discount_value <= 1) AND (discount_type <> 'none' OR discount_value = 0)");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropCheckConstraint(name: "ck_estimate_revisions_discount_value", schema: "estimates", table: "estimate_revisions");
        migrationBuilder.DropCheckConstraint(name: "ck_estimate_revisions_discount_type", schema: "estimates", table: "estimate_revisions");
        migrationBuilder.DropColumn(name: "discount_reason_code", schema: "estimates", table: "estimate_revisions");
        migrationBuilder.DropColumn(name: "discount_type", schema: "estimates", table: "estimate_revisions");
        migrationBuilder.DropColumn(name: "discount_value", schema: "estimates", table: "estimate_revisions");
    }
}
