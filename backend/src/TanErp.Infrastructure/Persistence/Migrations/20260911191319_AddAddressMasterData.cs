using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAddressMasterData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "master_data");

            migrationBuilder.CreateTable(
                name: "provinces",
                schema: "master_data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    name_th = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    name_en = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_provinces", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "districts",
                schema: "master_data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    province_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    name_th = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    name_en = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_districts", x => x.id);
                    table.ForeignKey(
                        name: "FK_districts_provinces_province_id",
                        column: x => x.province_id,
                        principalSchema: "master_data",
                        principalTable: "provinces",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "subdistricts",
                schema: "master_data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    district_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    name_th = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    name_en = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    postal_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    default_latitude = table.Column<decimal>(type: "numeric(9,6)", nullable: true),
                    default_longitude = table.Column<decimal>(type: "numeric(10,6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subdistricts", x => x.id);
                    table.ForeignKey(
                        name: "FK_subdistricts_districts_district_id",
                        column: x => x.district_id,
                        principalSchema: "master_data",
                        principalTable: "districts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_districts_code",
                schema: "master_data",
                table: "districts",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_districts_name_th",
                schema: "master_data",
                table: "districts",
                column: "name_th");

            migrationBuilder.CreateIndex(
                name: "IX_districts_province_id_code",
                schema: "master_data",
                table: "districts",
                columns: new[] { "province_id", "code" });

            migrationBuilder.CreateIndex(
                name: "IX_provinces_code",
                schema: "master_data",
                table: "provinces",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_provinces_name_th",
                schema: "master_data",
                table: "provinces",
                column: "name_th");

            migrationBuilder.CreateIndex(
                name: "IX_subdistricts_code",
                schema: "master_data",
                table: "subdistricts",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_subdistricts_district_id_code",
                schema: "master_data",
                table: "subdistricts",
                columns: new[] { "district_id", "code" });

            migrationBuilder.CreateIndex(
                name: "IX_subdistricts_name_th",
                schema: "master_data",
                table: "subdistricts",
                column: "name_th");

            migrationBuilder.CreateIndex(
                name: "IX_subdistricts_postal_code",
                schema: "master_data",
                table: "subdistricts",
                column: "postal_code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "subdistricts",
                schema: "master_data");

            migrationBuilder.DropTable(
                name: "districts",
                schema: "master_data");

            migrationBuilder.DropTable(
                name: "provinces",
                schema: "master_data");
        }
    }
}
