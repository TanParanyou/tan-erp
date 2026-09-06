using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EnforceOrganizationBoundaries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_membership_roles_memberships_membership_id",
                schema: "identity_access",
                table: "membership_roles");

            migrationBuilder.DropForeignKey(
                name: "FK_membership_roles_roles_role_id",
                schema: "identity_access",
                table: "membership_roles");

            migrationBuilder.DropForeignKey(
                name: "FK_memberships_branches_branch_id",
                schema: "organization",
                table: "memberships");

            migrationBuilder.DropIndex(
                name: "IX_memberships_branch_id",
                schema: "organization",
                table: "memberships");

            migrationBuilder.DropIndex(
                name: "IX_membership_roles_role_id",
                schema: "identity_access",
                table: "membership_roles");

            migrationBuilder.AddColumn<Guid>(
                name: "organization_id",
                schema: "identity_access",
                table: "membership_roles",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddUniqueConstraint(
                name: "AK_roles_id_organization_id",
                schema: "identity_access",
                table: "roles",
                columns: new[] { "id", "organization_id" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_memberships_id_organization_id",
                schema: "organization",
                table: "memberships",
                columns: new[] { "id", "organization_id" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_branches_id_organization_id",
                schema: "organization",
                table: "branches",
                columns: new[] { "id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_memberships_branch_id_organization_id",
                schema: "organization",
                table: "memberships",
                columns: new[] { "branch_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_membership_roles_membership_id_organization_id",
                schema: "identity_access",
                table: "membership_roles",
                columns: new[] { "membership_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_membership_roles_role_id_organization_id",
                schema: "identity_access",
                table: "membership_roles",
                columns: new[] { "role_id", "organization_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_membership_roles_memberships_membership_id_organization_id",
                schema: "identity_access",
                table: "membership_roles",
                columns: new[] { "membership_id", "organization_id" },
                principalSchema: "organization",
                principalTable: "memberships",
                principalColumns: new[] { "id", "organization_id" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_membership_roles_roles_role_id_organization_id",
                schema: "identity_access",
                table: "membership_roles",
                columns: new[] { "role_id", "organization_id" },
                principalSchema: "identity_access",
                principalTable: "roles",
                principalColumns: new[] { "id", "organization_id" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_memberships_branches_branch_id_organization_id",
                schema: "organization",
                table: "memberships",
                columns: new[] { "branch_id", "organization_id" },
                principalSchema: "organization",
                principalTable: "branches",
                principalColumns: new[] { "id", "organization_id" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_membership_roles_memberships_membership_id_organization_id",
                schema: "identity_access",
                table: "membership_roles");

            migrationBuilder.DropForeignKey(
                name: "FK_membership_roles_roles_role_id_organization_id",
                schema: "identity_access",
                table: "membership_roles");

            migrationBuilder.DropForeignKey(
                name: "FK_memberships_branches_branch_id_organization_id",
                schema: "organization",
                table: "memberships");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_roles_id_organization_id",
                schema: "identity_access",
                table: "roles");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_memberships_id_organization_id",
                schema: "organization",
                table: "memberships");

            migrationBuilder.DropIndex(
                name: "IX_memberships_branch_id_organization_id",
                schema: "organization",
                table: "memberships");

            migrationBuilder.DropIndex(
                name: "IX_membership_roles_membership_id_organization_id",
                schema: "identity_access",
                table: "membership_roles");

            migrationBuilder.DropIndex(
                name: "IX_membership_roles_role_id_organization_id",
                schema: "identity_access",
                table: "membership_roles");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_branches_id_organization_id",
                schema: "organization",
                table: "branches");

            migrationBuilder.DropColumn(
                name: "organization_id",
                schema: "identity_access",
                table: "membership_roles");

            migrationBuilder.CreateIndex(
                name: "IX_memberships_branch_id",
                schema: "organization",
                table: "memberships",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_membership_roles_role_id",
                schema: "identity_access",
                table: "membership_roles",
                column: "role_id");

            migrationBuilder.AddForeignKey(
                name: "FK_membership_roles_memberships_membership_id",
                schema: "identity_access",
                table: "membership_roles",
                column: "membership_id",
                principalSchema: "organization",
                principalTable: "memberships",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_membership_roles_roles_role_id",
                schema: "identity_access",
                table: "membership_roles",
                column: "role_id",
                principalSchema: "identity_access",
                principalTable: "roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_memberships_branches_branch_id",
                schema: "organization",
                table: "memberships",
                column: "branch_id",
                principalSchema: "organization",
                principalTable: "branches",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
