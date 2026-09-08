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
import { getCustomerStatusLabelKey, getCustomerTypeLabelKey } from "../customer-labels";

export function CustomerList() {
  const t = useTranslations("customers");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const { selectedMembership } = useSelectedMembership();

  const resolveCustomerTypeLabel = (value: string | null | undefined): string => {
    const key = getCustomerTypeLabelKey(value);
    return key ? t(key) : "-";
  };

  const resolveCustomerStatusLabel = (value: string | null | undefined): string => {
    const key = getCustomerStatusLabelKey(value);
    return key ? tCommon(`status.${key}`) : "-";
  };

  const [searchInput, setSearchInput] = useState("");
  const [activeSearch, setActiveSearch] = useState("");

  const canCreate =
    can(selectedMembership, "customers.create") &&
    can(selectedMembership, "customer-contacts.manage");

  const resolveListErrorMessage = (error: Error | null): string => {
    if (error?.message === "No authentication token available") {
      return t("errors.authenticationRequired");
    }
    if (error?.message === "No active membership selected") {
      return t("errors.membershipRequired");
    }
    return t("errors.loadList");
  };

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
    <div className="flex flex-col gap-6">
      {/* Top Header & Actions */}
      <div className="flex justify-between items-start flex-wrap gap-4 border-b border-erp-border pb-5">
        <div>
          <h1 className="text-2xl font-bold text-erp-navy mb-1 tracking-tight">
            {t("title")}
          </h1>
          <p className="text-sm text-erp-text-muted m-0">
            {t("subtitle")}
          </p>
        </div>

        {canCreate && (
          <Button
            href={`/${locale}/customers/create`}
            variant="primary"
            size="md"
            icon={<IconPlus size={16} />}
            className="min-h-[44px]"
          >
            {t("createCustomer")}
          </Button>
        )}
      </div>

      {/* Filter / Search Toolbar */}
      <div className="erp-card p-4 bg-erp-surface flex items-center gap-4">
        <form
          onSubmit={handleSearchSubmit}
          className="flex gap-2 flex-1 items-center"
        >
          <div className="flex-1">
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
            className="min-h-[44px]"
          >
            {tCommon("actions.search")}
          </Button>
        </form>
      </div>

      {/* Main Content Area */}
      {isLoading ? (
        <div className="flex items-center justify-center min-h-[300px] bg-erp-surface border border-erp-border">
          <MonoSpinner size="lg" label={tCommon("states.loading")} aria-busy="true" />
        </div>
      ) : isError ? (
        <div
          role="alert"
          aria-live="polite"
          className="erp-card p-8 border-erp-danger-border bg-erp-danger-bg text-center flex flex-col items-center gap-4"
        >
          <IconAlertCircle size={32} className="text-erp-danger" />
          <div>
            <h2 className="text-lg font-bold text-erp-danger mb-2">
              {resolveListErrorMessage(error)}
            </h2>
          </div>
          <Button
            variant="outline"
            size="md"
            onClick={() => refetch()}
            className="min-h-[44px]"
          >
            {tCommon("actions.retry")}
          </Button>
        </div>
      ) : allCustomers.length === 0 ? (
        <div className="erp-card py-12 px-6 text-center flex flex-col items-center gap-4">
          <div className="w-12 h-12 flex items-center justify-center bg-erp-surface-muted border border-erp-border text-erp-text-muted">
            <IconFileText size={24} />
          </div>
          <div>
            <h3 className="text-lg font-bold text-erp-navy mb-1">
              {t("emptyTitle")}
            </h3>
            <p className="text-sm text-erp-text-muted m-0">
              {t("emptyDetail")}
            </p>
          </div>
          {canCreate && (
            <Button
              href={`/${locale}/customers/create`}
              variant="primary"
              size="md"
              icon={<IconPlus size={16} />}
              className="min-h-[44px] mt-2"
            >
              {t("createCustomer")}
            </Button>
          )}
        </div>
      ) : (
        <div className="flex flex-col gap-4">
          {/* Dense Architectural Table */}
          <div
            tabIndex={0}
            aria-label={t("customerList")}
            className="overflow-x-auto border border-erp-border bg-erp-surface"
          >
            <table className="erp-table w-full border-collapse text-sm text-left">
              <thead>
                <tr className="bg-erp-surface-muted border-b border-erp-border text-erp-navy font-bold">
                  <th className="px-4 py-3 min-w-[120px]">{t("code")}</th>
                  <th className="px-4 py-3 min-w-[200px]">{tCommon("fields.name")}</th>
                  <th className="px-4 py-3 min-w-[140px]">{t("customerType")}</th>
                  <th className="px-4 py-3 min-w-[220px]">{t("primaryContact")}</th>
                  <th className="px-4 py-3 min-w-[100px]">{t("status")}</th>
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
                    contact?.phone ? `${t("telLabel")}: ${contact.phone}` : null,
                    contact?.email ? `${t("emailLabel")}: ${contact.email}` : null,
                  ]
                    .filter(Boolean)
                    .join(" • ");

                  return (
                    <tr
                      key={customer.id}
                      className="erp-table-row border-b border-erp-border-subtle cursor-pointer"
                    >
                      <td className="px-4 py-3 font-mono font-semibold">
                        <Link
                          href={`/${locale}/customers/${customer.id}`}
                          className="text-erp-navy no-underline block hover:underline"
                        >
                          {customer.code || "-"}
                        </Link>
                      </td>
                      <td className="px-4 py-3 font-semibold">
                        <Link
                          href={`/${locale}/customers/${customer.id}`}
                          className="text-erp-text-main no-underline block hover:underline"
                        >
                          {customerName}
                        </Link>
                      </td>
                      <td className="px-4 py-3">
                        <span className="erp-badge erp-badge-neutral">
                          {resolveCustomerTypeLabel(customer.customerType)}
                        </span>
                      </td>
                      <td className="px-4 py-3 text-erp-text-muted text-[0.8125rem]">
                        {contactDetails || "-"}
                      </td>
                      <td className="px-4 py-3">
                        <span
                          className={`erp-badge ${
                            customer.status === "active" ? "erp-badge-success" : "erp-badge-neutral"
                          }`}
                        >
                          {resolveCustomerStatusLabel(customer.status)}
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
            <div className="flex justify-center mt-2">
              <Button
                variant="outline"
                size="md"
                onClick={() => fetchNextPage()}
                disabled={isFetchingNextPage}
                className="min-h-[44px] min-w-[160px]"
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
