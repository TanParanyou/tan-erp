using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EnforceRolePermissionOrganizationBoundaries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_role_permissions_roles_role_id",
                schema: "identity_access",
                table: "role_permissions");

            migrationBuilder.DropIndex(
                name: "IX_role_permissions_role_id",
                schema: "identity_access",
                table: "role_permissions");

            migrationBuilder.AddColumn<Guid>(
                name: "branch_id",
                schema: "identity_access",
                table: "role_permissions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "organization_id",
                schema: "identity_access",
                table: "role_permissions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.Sql(
                @"UPDATE identity_access.role_permissions rp
                  SET organization_id = r.organization_id
                  FROM identity_access.roles r
                  WHERE rp.role_id = r.id;");

            migrationBuilder.Sql(
                @"UPDATE identity_access.role_permissions
                  SET branch_id = scope_id
                  WHERE scope = 'branch' AND scope_id IS NOT NULL;");

            migrationBuilder.CreateIndex(
                name: "ix_role_permissions_branch_id_organization_id",
                schema: "identity_access",
                table: "role_permissions",
                columns: new[] { "branch_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "ix_role_permissions_role_id_organization_id",
                schema: "identity_access",
                table: "role_permissions",
                columns: new[] { "role_id", "organization_id" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_role_permissions_branch_scope",
                schema: "identity_access",
                table: "role_permissions",
                sql: "scope <> 'branch' OR (branch_id IS NOT NULL AND scope_id = branch_id)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_role_permissions_org_scope",
                schema: "identity_access",
                table: "role_permissions",
                sql: "scope <> 'organization' OR (scope_id = organization_id AND branch_id IS NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_role_permissions_own_scope",
                schema: "identity_access",
                table: "role_permissions",
                sql: "scope <> 'own' OR (scope_id IS NULL AND branch_id IS NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_role_permissions_scope",
                schema: "identity_access",
                table: "role_permissions",
                sql: "scope IN ('organization', 'branch', 'own')");

            migrationBuilder.AddForeignKey(
                name: "FK_role_permissions_branches_branch_id_organization_id",
                schema: "identity_access",
                table: "role_permissions",
                columns: new[] { "branch_id", "organization_id" },
                principalSchema: "organization",
                principalTable: "branches",
                principalColumns: new[] { "id", "organization_id" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_role_permissions_roles_role_id_organization_id",
                schema: "identity_access",
                table: "role_permissions",
                columns: new[] { "role_id", "organization_id" },
                principalSchema: "identity_access",
                principalTable: "roles",
                principalColumns: new[] { "id", "organization_id" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_role_permissions_branches_branch_id_organization_id",
                schema: "identity_access",
                table: "role_permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_role_permissions_roles_role_id_organization_id",
                schema: "identity_access",
                table: "role_permissions");

            migrationBuilder.DropIndex(
                name: "ix_role_permissions_branch_id_organization_id",
                schema: "identity_access",
                table: "role_permissions");

            migrationBuilder.DropIndex(
                name: "ix_role_permissions_role_id_organization_id",
                schema: "identity_access",
                table: "role_permissions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_role_permissions_branch_scope",
                schema: "identity_access",
                table: "role_permissions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_role_permissions_org_scope",
                schema: "identity_access",
                table: "role_permissions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_role_permissions_own_scope",
                schema: "identity_access",
                table: "role_permissions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_role_permissions_scope",
                schema: "identity_access",
                table: "role_permissions");

            migrationBuilder.DropColumn(
                name: "branch_id",
                schema: "identity_access",
                table: "role_permissions");

            migrationBuilder.DropColumn(
                name: "organization_id",
                schema: "identity_access",
                table: "role_permissions");

            migrationBuilder.CreateIndex(
                name: "IX_role_permissions_role_id",
                schema: "identity_access",
                table: "role_permissions",
                column: "role_id");

            migrationBuilder.AddForeignKey(
                name: "FK_role_permissions_roles_role_id",
                schema: "identity_access",
                table: "role_permissions",
                column: "role_id",
                principalSchema: "identity_access",
                principalTable: "roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
