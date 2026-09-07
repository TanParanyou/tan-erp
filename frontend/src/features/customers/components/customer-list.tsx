"use client";

import React, { useState } from "react";
import Link from "next/link";
import { useTranslations, useLocale } from "next-intl";
import { useCustomerList } from "../api/customer-queries";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { IconSearch, IconPlus, IconAlertCircle, IconFileText } from "@/components/common/Icons";
import { can } from "@/lib/permissions/can";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";

export function CustomerList() {
  const t = useTranslations("customers");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const { selectedMembership } = useSelectedMembership();

  const [searchInput, setSearchInput] = useState("");
  const [activeSearch, setActiveSearch] = useState("");

  const canCreate = can(selectedMembership, "customers.create");

  const {
    data,
    isLoading,
    isError,
    error,
    refetch,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
  } = useCustomerList({
    search: activeSearch || undefined,
  });

  const handleSearchSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setActiveSearch(searchInput.trim());
  };

  const allCustomers = data?.pages.flatMap((page) => page.items || []) || [];

  return (
    <div style={{ display: "flex", flexDirection: "column", gap: "1.5rem" }}>
      {/* Top Header & Actions */}
      <div
        style={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "flex-start",
          flexWrap: "wrap",
          gap: "1rem",
          borderBottom: "1px solid var(--erp-border)",
          paddingBottom: "1.25rem",
        }}
      >
        <div>
          <h1
            style={{
              fontSize: "1.5rem",
              fontWeight: 700,
              color: "var(--erp-navy)",
              margin: "0 0 0.25rem 0",
              letterSpacing: "-0.01em",
            }}
          >
            {t("title")}
          </h1>
          <p style={{ fontSize: "0.875rem", color: "var(--erp-text-muted)", margin: 0 }}>
            {t("subtitle")}
          </p>
        </div>

        {canCreate && (
          <Link href={`/${locale}/customers/create`} style={{ textDecoration: "none" }}>
            <Button
              variant="primary"
              size="md"
              icon={<IconPlus size={16} />}
              style={{ minHeight: "44px" }}
            >
              {t("createCustomer")}
            </Button>
          </Link>
        )}
      </div>

      {/* Filter / Search Toolbar */}
      <div
        className="erp-card"
        style={{
          padding: "1rem",
          backgroundColor: "var(--erp-surface)",
          display: "flex",
          alignItems: "center",
          gap: "1rem",
        }}
      >
        <form
          onSubmit={handleSearchSubmit}
          style={{ display: "flex", gap: "0.5rem", flex: 1, alignItems: "center" }}
        >
          <div style={{ flex: 1 }}>
            <Input
              id="customer-search-input"
              type="search"
              placeholder={t("searchPlaceholder")}
              value={searchInput}
              onChange={(e) => setSearchInput(e.target.value)}
              leftIcon={<IconSearch size={16} />}
              aria-label={t("searchPlaceholder")}
            />
          </div>
          <Button
            type="submit"
            variant="outline"
            size="md"
            style={{ minHeight: "44px" }}
          >
            {tCommon("actions.confirm")}
          </Button>
        </form>
      </div>

      {/* Main Content Area */}
      {isLoading ? (
        <div
          style={{
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
            minHeight: "300px",
            backgroundColor: "var(--erp-surface)",
            border: "1px solid var(--erp-border)",
          }}
        >
          <MonoSpinner size="lg" label={tCommon("states.loading")} aria-busy="true" />
        </div>
      ) : isError ? (
        <div
          role="alert"
          className="erp-card"
          style={{
            padding: "2rem",
            borderColor: "var(--erp-danger-border)",
            backgroundColor: "var(--erp-danger-bg)",
            display: "flex",
            flexDirection: "column",
            alignItems: "center",
            textAlign: "center",
            gap: "1rem",
          }}
        >
          <IconAlertCircle size={32} style={{ color: "var(--erp-danger)" }} />
          <div>
            <h2 style={{ fontSize: "1.125rem", fontWeight: 700, color: "var(--erp-danger)", margin: "0 0 0.5rem 0" }}>
              {error?.message || "Failed to load customers"}
            </h2>
          </div>
          <Button
            variant="outline"
            size="md"
            onClick={() => refetch()}
            style={{ minHeight: "44px" }}
          >
            {tCommon("actions.cancel")}
          </Button>
        </div>
      ) : allCustomers.length === 0 ? (
        <div
          className="erp-card"
          style={{
            padding: "3rem 1.5rem",
            textAlign: "center",
            display: "flex",
            flexDirection: "column",
            alignItems: "center",
            gap: "1rem",
          }}
        >
          <div
            style={{
              width: "48px",
              height: "48px",
              display: "flex",
              alignItems: "center",
              justifyContent: "center",
              backgroundColor: "var(--erp-surface-muted)",
              border: "1px solid var(--erp-border)",
              color: "var(--erp-text-muted)",
            }}
          >
            <IconFileText size={24} />
          </div>
          <div>
            <h3 style={{ fontSize: "1.125rem", fontWeight: 700, color: "var(--erp-navy)", margin: "0 0 0.25rem 0" }}>
              {t("emptyTitle")}
            </h3>
            <p style={{ fontSize: "0.875rem", color: "var(--erp-text-muted)", margin: 0 }}>
              {t("emptyDetail")}
            </p>
          </div>
          {canCreate && (
            <Link href={`/${locale}/customers/create`} style={{ textDecoration: "none", marginTop: "0.5rem" }}>
              <Button
                variant="primary"
                size="md"
                icon={<IconPlus size={16} />}
                style={{ minHeight: "44px" }}
              >
                {t("createCustomer")}
              </Button>
            </Link>
          )}
        </div>
      ) : (
        <div style={{ display: "flex", flexDirection: "column", gap: "1rem" }}>
          {/* Dense Architectural Table */}
          <div
            tabIndex={0}
            aria-label={t("customerList")}
            style={{
              overflowX: "auto",
              border: "1px solid var(--erp-border)",
              backgroundColor: "var(--erp-surface)",
            }}
          >
            <table
              className="erp-table"
              style={{
                width: "100%",
                borderCollapse: "collapse",
                fontSize: "0.875rem",
                textAlign: "left",
              }}
            >
              <thead>
                <tr
                  style={{
                    backgroundColor: "var(--erp-surface-muted)",
                    borderBottom: "1px solid var(--erp-border)",
                    color: "var(--erp-navy)",
                    fontWeight: 700,
                  }}
                >
                  <th style={{ padding: "0.75rem 1rem", minWidth: "120px" }}>{t("code")}</th>
                  <th style={{ padding: "0.75rem 1rem", minWidth: "200px" }}>{tCommon("fields.name")}</th>
                  <th style={{ padding: "0.75rem 1rem", minWidth: "140px" }}>{t("customerType")}</th>
                  <th style={{ padding: "0.75rem 1rem", minWidth: "220px" }}>{t("primaryContact")}</th>
                  <th style={{ padding: "0.75rem 1rem", minWidth: "100px" }}>{t("status")}</th>
                </tr>
              </thead>
              <tbody>
                {allCustomers.map((customer) => {
                  const customerName =
                    locale === "en" && customer.displayNameEn
                      ? customer.displayNameEn
                      : customer.displayNameTh || customer.displayNameEn || "-";

                  const contact = customer.primaryContact;
                  const contactDetails = [
                    contact?.name,
                    contact?.phone ? `Tel: ${contact.phone}` : null,
                    contact?.email ? `Email: ${contact.email}` : null,
                  ]
                    .filter(Boolean)
                    .join(" • ");

                  return (
                    <tr
                      key={customer.id}
                      style={{
                        borderBottom: "1px solid var(--erp-border-subtle)",
                        cursor: "pointer",
                      }}
                      className="erp-table-row"
                    >
                      <td style={{ padding: "0.75rem 1rem", fontFamily: "monospace", fontWeight: 600 }}>
                        <Link
                          href={`/${locale}/customers/${customer.id}`}
                          style={{
                            color: "var(--erp-navy)",
                            textDecoration: "none",
                            display: "block",
                          }}
                        >
                          {customer.code || "-"}
                        </Link>
                      </td>
                      <td style={{ padding: "0.75rem 1rem", fontWeight: 600 }}>
                        <Link
                          href={`/${locale}/customers/${customer.id}`}
                          style={{
                            color: "var(--erp-text-main)",
                            textDecoration: "none",
                            display: "block",
                          }}
                        >
                          {customerName}
                        </Link>
                      </td>
                      <td style={{ padding: "0.75rem 1rem" }}>
                        <span className="erp-badge erp-badge-neutral">
                          {customer.customerType === "corporate" ? t("corporate") : t("individual")}
                        </span>
                      </td>
                      <td style={{ padding: "0.75rem 1rem", color: "var(--erp-text-muted)", fontSize: "0.8125rem" }}>
                        {contactDetails || "-"}
                      </td>
                      <td style={{ padding: "0.75rem 1rem" }}>
                        <span
                          className={`erp-badge ${
                            customer.status === "active" ? "erp-badge-success" : "erp-badge-neutral"
                          }`}
                        >
                          {customer.status === "active" ? t("active") : t("inactive")}
                        </span>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>

          {/* Keyset Cursor Pagination Button */}
          {hasNextPage && (
            <div style={{ display: "flex", justifyContent: "center", marginTop: "0.5rem" }}>
              <Button
                variant="outline"
                size="md"
                onClick={() => fetchNextPage()}
                disabled={isFetchingNextPage}
                style={{ minHeight: "44px", minWidth: "160px" }}
              >
                {isFetchingNextPage ? t("loadingMore") : t("loadMore")}
              </Button>
            </div>
          )}
        </div>
      )}
    </div>
  );
}
