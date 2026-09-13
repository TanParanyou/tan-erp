/**
 * Unified Permissions Catalog for tan-erp
 * Matches docs/03-contracts/permission-catalog.md and backend RBAC policy
 */
export const PERMISSIONS = {
  // Organization & Access
  ORGANIZATIONS_READ: "organizations.read",
  BRANCHES_MANAGE: "branches.manage",
  USERS_MANAGE: "users.manage",
  ROLES_MANAGE: "roles.manage",

  // CRM - Customers
  CUSTOMERS_READ: "customers.read",
  CUSTOMERS_CREATE: "customers.create",
  CUSTOMERS_UPDATE: "customers.update",
  CUSTOMERS_ACTIVATE: "customers.activate",
  CUSTOMERS_DEACTIVATE: "customers.deactivate",
  CUSTOMER_CONTACTS_MANAGE: "customer-contacts.manage",

  // CRM - Sites
  SITES_READ: "sites.read",
  SITES_MANAGE: "sites.manage",

  // CRM - Opportunities
  OPPORTUNITIES_READ: "opportunities.read",
  OPPORTUNITIES_CREATE: "opportunities.create",
  OPPORTUNITIES_UPDATE: "opportunities.update",
  OPPORTUNITIES_TRANSITION: "opportunities.transition",

  // Surveys
  SURVEYS_READ: "surveys.read",
  SURVEYS_CREATE: "surveys.create",
} as const;

export type PermissionKey = (typeof PERMISSIONS)[keyof typeof PERMISSIONS];
