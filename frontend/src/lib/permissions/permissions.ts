/**
 * Unified Permissions Catalog for tan-erp
 * Matches docs/03-contracts/permission-catalog.md and backend RBAC policy
 */
export const PERMISSIONS = {
  // Organization & Access
  ORGANIZATIONS_READ: "organizations.read",
  BRANCHES_MANAGE: "branches.manage",
  USERS_READ: "users.read",
  USERS_MANAGE: "users.manage",
  MEMBERSHIPS_MANAGE: "memberships.manage",
  ROLES_ASSIGN: "roles.assign",
  ROLES_ASSIGN_APPROVAL: "roles.assign-approval",
  ROLES_MANAGE: "roles.manage",

  // Item Master
  ITEMS_READ: "items.read",
  ITEMS_CREATE: "items.create",

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
  SURVEYS_CREATE_REVISION: "surveys.create-revision",
  SURVEYS_VOID: "surveys.void",

  // Procurement
  SUPPLIERS_READ: "suppliers.read",
  SUPPLIERS_MANAGE: "suppliers.manage",
  PURCHASE_ORDERS_READ: "purchase-orders.read",
  PURCHASE_ORDERS_CREATE: "purchase-orders.create",
  PURCHASE_ORDERS_APPROVE: "purchase-orders.approve",
  GOODS_RECEIPTS_CREATE: "goods-receipts.create",

  // Projects
  PROJECTS_READ: "projects.read",
  PROJECTS_CREATE: "projects.create",
  PROJECTS_UPDATE: "projects.update",
  PROJECTS_TRANSITION: "projects.transition",
  PROJECTS_CHANGE_ORDERS_MANAGE: "projects.change-orders.manage",
  PROJECTS_CHANGE_ORDERS_APPROVE: "projects.change-orders.approve",

  // Commercial - Quotations
  QUOTATIONS_READ: "quotations.read",
  QUOTATIONS_ISSUE: "quotations.issue",
  QUOTATIONS_ACCEPT: "quotations.accept",
} as const;

export type PermissionKey = (typeof PERMISSIONS)[keyof typeof PERMISSIONS];
