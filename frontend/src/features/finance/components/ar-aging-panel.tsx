"use client";

import { useMemo, useRef, useState } from "react";
import { useLocale, useTranslations } from "next-intl";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { ConfirmationModal } from "@/components/ui/ConfirmationModal";
import { DataTable, type Column } from "@/components/ui/DataTable";
import { Input } from "@/components/ui/Input";
import { Select } from "@/components/ui/Select";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import { formatCurrency, formatDate } from "@/lib/formatters/formatters";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import { useCustomerList } from "@/features/customers/api/customer-queries";
import type {
  ArAgingCustomerBucketResponse,
  CreditOverrideResponse,
} from "@/lib/api/api-client";
import {
  useArAgingReport,
  useCreditOverrideMutations,
  useCreditOverrides,
} from "../api/finance-queries";
import { creditOverrideStatusVariant, financeErrorCode } from "../finance-status";

function todayIso(): string {
  return new Date().toISOString().slice(0, 10);
}

export function ArAgingPanel() {
  const t = useTranslations("finance.arAging");
  const tErrors = useTranslations("finance.errors");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const { toast } = useToast();
  const { currentUser, selectedMembership } = useSelectedMembership();
  const currentUserId = currentUser?.user?.id;
  const canManage = can(selectedMembership, PERMISSIONS.BILLINGS_MANAGE);

  const [asOfDate, setAsOfDate] = useState(todayIso);
  const [selectedCustomerId, setSelectedCustomerId] = useState<string>("");

  // Queries
  const reportQuery = useArAgingReport({
    asOfDate: asOfDate || undefined,
    customerId: selectedCustomerId || undefined,
  });
  const overridesQuery = useCreditOverrides();
  const customersQuery = useCustomerList({ page: 1, limit: 100 });
  const mutations = useCreditOverrideMutations();

  // Form state for new override request
  const [overrideCustomer, setOverrideCustomer] = useState("");
  const [overrideAmount, setOverrideAmount] = useState("");
  const [overrideReason, setOverrideReason] = useState("");
  const [overrideExpiresOn, setOverrideExpiresOn] = useState("");
  const [formError, setFormError] = useState<string | null>(null);
  const createKeyRef = useRef(crypto.randomUUID());

  // Modal state for approve override
  const [approvingOverride, setApprovingOverride] = useState<CreditOverrideResponse | null>(null);

  const isBusy = mutations.createOverride.isPending || mutations.approveOverride.isPending;

  function describe(err: unknown): string {
    const code = err instanceof ApiError ? financeErrorCode(err.code) : null;
    return code ? tErrors(code) : tErrors("failed");
  }

  async function handleCreateOverride(e: React.FormEvent): Promise<void> {
    e.preventDefault();
    setFormError(null);
    const amountVal = Number(overrideAmount);
    if (!overrideCustomer || !(amountVal > 0) || !overrideReason.trim()) {
      setFormError(t("requestInvalid"));
      return;
    }

    try {
      await mutations.createOverride.mutateAsync({
        payload: {
          customerId: overrideCustomer,
          requestedCeilingAmount: amountVal,
          reason: overrideReason.trim(),
          expiresOn: overrideExpiresOn || null,
        },
        idempotencyKey: createKeyRef.current,
      });
      createKeyRef.current = crypto.randomUUID();
      toast.success(t("requestCreated"));
      setOverrideCustomer("");
      setOverrideAmount("");
      setOverrideReason("");
      setOverrideExpiresOn("");
    } catch (err: unknown) {
      setFormError(describe(err));
    }
  }

  async function handleApproveOverrideConfirm(): Promise<void> {
    if (!approvingOverride?.id) return;
    try {
      await mutations.approveOverride.mutateAsync({
        id: approvingOverride.id,
        rowVersion: approvingOverride.rowVersion ?? "",
      });
      toast.success(t("overrideApproved"));
      setApprovingOverride(null);
    } catch (err: unknown) {
      toast.error(describe(err));
    }
  }

  const report = reportQuery.data;

  // AR Aging Table Columns
  const customerColumns: Column<ArAgingCustomerBucketResponse>[] = useMemo(
    () => [
      {
        id: "code",
        header: t("customerCode"),
        className: "font-mono font-medium min-w-28",
        cell: (_v, row) => row.customerCode ?? "-",
      },
      {
        id: "name",
        header: t("customerName"),
        className: "min-w-40 font-medium",
        cell: (_v, row) => row.customerName ?? "-",
      },
      {
        id: "current",
        header: t("current"),
        className: "text-right font-mono",
        cell: (_v, row) => formatCurrency(row.current, "THB", locale),
      },
      {
        id: "b1",
        header: t("bucket1To30"),
        className: "text-right font-mono",
        cell: (_v, row) => formatCurrency(row.bucket1To30, "THB", locale),
      },
      {
        id: "b2",
        header: t("bucket31To60"),
        className: "text-right font-mono",
        cell: (_v, row) => formatCurrency(row.bucket31To60, "THB", locale),
      },
      {
        id: "b3",
        header: t("bucket61To90"),
        className: "text-right font-mono text-erp-warning",
        cell: (_v, row) => formatCurrency(row.bucket61To90, "THB", locale),
      },
      {
        id: "b4",
        header: t("bucketOver90"),
        className: "text-right font-mono text-erp-danger font-semibold",
        cell: (_v, row) => formatCurrency(row.bucketOver90, "THB", locale),
      },
      {
        id: "total",
        header: t("totalOutstanding"),
        className: "text-right font-mono font-bold text-erp-navy",
        cell: (_v, row) => formatCurrency(row.totalOutstanding, "THB", locale),
      },
    ],
    [locale, t]
  );

  return (
    <div className="space-y-6">
      {/* Filter Toolbar for AR Aging */}
      <div className="erp-card p-4 flex flex-wrap items-center justify-between gap-4">
        <div>
          <h3 className="text-base font-bold text-erp-navy">{t("title")}</h3>
          <p className="text-xs text-erp-text-muted">{t("subtitle")}</p>
        </div>
        <div className="flex flex-wrap items-center gap-3">
          <Input
            id="ar-aging-as-of-date"
            type="date"
            label={t("asOfDate")}
            placeholder={t("datePlaceholder")}
            value={asOfDate}
            wrapperClassName="!mb-0"
            className="h-11 min-h-[44px]"
            onChange={(e) => setAsOfDate(e.target.value)}
          />
          <Select
            id="ar-aging-filter-customer"
            label={t("customer")}
            placeholder={t("selectCustomer")}
            value={selectedCustomerId}
            wrapperClassName="!mb-0"
            className="h-11 min-h-[44px]"
            options={[
              { value: "", label: tCommon("filters.all") },
              ...(customersQuery.data?.items ?? []).map((c) => {
                const name = (locale === "en" ? c.displayNameEn || c.displayNameTh : c.displayNameTh || c.displayNameEn) ?? "-";
                return {
                  value: c.id ?? "",
                  label: `${name} (${c.code ?? "-"})`,
                };
              }),
            ]}
            onChange={(e) => setSelectedCustomerId(e.target.value)}
          />
        </div>
      </div>

      {/* Summary KPI Cards */}
      <div className="grid grid-cols-2 gap-3 sm:grid-cols-3 lg:grid-cols-6 text-xs">
        <div className="erp-card p-4 space-y-1">
          <div className="text-erp-text-muted">{t("totalCurrent")}</div>
          <div className="font-mono text-base font-bold text-erp-text-main">
            {formatCurrency(report?.totalCurrent ?? 0, "THB", locale)}
          </div>
        </div>
        <div className="erp-card p-4 space-y-1">
          <div className="text-erp-text-muted">{t("totalBucket1To30")}</div>
          <div className="font-mono text-base font-bold text-erp-text-main">
            {formatCurrency(report?.totalBucket1To30 ?? 0, "THB", locale)}
          </div>
        </div>
        <div className="erp-card p-4 space-y-1">
          <div className="text-erp-text-muted">{t("totalBucket31To60")}</div>
          <div className="font-mono text-base font-bold text-erp-text-main">
            {formatCurrency(report?.totalBucket31To60 ?? 0, "THB", locale)}
          </div>
        </div>
        <div className="erp-card p-4 space-y-1">
          <div className="text-erp-text-muted">{t("totalBucket61To90")}</div>
          <div className="font-mono text-base font-bold text-erp-warning">
            {formatCurrency(report?.totalBucket61To90 ?? 0, "THB", locale)}
          </div>
        </div>
        <div className="erp-card p-4 space-y-1">
          <div className="text-erp-text-muted">{t("totalBucketOver90")}</div>
          <div className="font-mono text-base font-bold text-erp-danger">
            {formatCurrency(report?.totalBucketOver90 ?? 0, "THB", locale)}
          </div>
        </div>
        <div className="erp-card p-4 space-y-1 border-l-4 border-erp-navy bg-erp-surface-subtle">
          <div className="text-erp-navy font-semibold">{t("grandTotalOutstanding")}</div>
          <div className="font-mono text-base font-bold text-erp-navy">
            {formatCurrency(report?.grandTotalOutstanding ?? 0, "THB", locale)}
          </div>
        </div>
      </div>

      {/* Customer Aging Table */}
      <div className="erp-card p-5 space-y-3">
        <h4 className="text-sm font-bold text-erp-navy">{t("customersTableTitle")}</h4>
        <DataTable<ArAgingCustomerBucketResponse>
          columns={customerColumns}
          data={report?.customers ?? []}
          isLoading={reportQuery.isLoading}
          isError={reportQuery.isError}
          error={reportQuery.error}
          onRetry={() => { void reportQuery.refetch(); }}
          emptyTitle={t("emptyCustomers")}
          emptyDescription={t("subtitle")}
        />
      </div>

      {/* Credit Limit Overrides Section */}
      <div className="erp-card p-5 space-y-5">
        <div className="flex flex-wrap items-center justify-between gap-3">
          <div>
            <h4 className="text-sm font-bold text-erp-navy">{t("overridesTitle")}</h4>
            <p className="text-xs text-erp-text-muted">{t("subtitle")}</p>
          </div>
        </div>

        {/* Existing Overrides Table */}
        {(overridesQuery.data?.items ?? []).length === 0 ? (
          <p className="text-xs text-erp-text-muted">{t("overridesEmpty")}</p>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-left text-xs">
              <thead>
                <tr className="border-b border-erp-border">
                  <th className="py-2 pr-2">{t("customer")}</th>
                  <th className="py-2 pr-2 text-right">{t("requestedCeilingAmount")}</th>
                  <th className="py-2 pr-2">{t("reason")}</th>
                  <th className="py-2 pr-2">{t("expiresOn")}</th>
                  <th className="py-2 pr-2">{t("status")}</th>
                  <th className="py-2 text-right">
                    <span className="sr-only">{t("approve")}</span>
                  </th>
                </tr>
              </thead>
              <tbody>
                {(overridesQuery.data?.items ?? []).map((row) => {
                  const isPending = row.status === "pending_approval";
                  const isCreator = row.createdBy?.id && row.createdBy.id === currentUserId;
                  const canApprove = isPending && canManage && !isCreator;

                  return (
                    <tr key={row.id} className="border-b border-erp-border/60 align-middle">
                      <td className="py-2 pr-2 font-medium">{row.customerName ?? "-"}</td>
                      <td className="py-2 pr-2 text-right font-mono font-semibold">
                        {formatCurrency(row.requestedCeilingAmount, "THB", locale)}
                      </td>
                      <td className="py-2 pr-2 max-w-xs truncate">{row.reason ?? "-"}</td>
                      <td className="py-2 pr-2">
                        {row.expiresOn ? formatDate(row.expiresOn, locale) : "-"}
                      </td>
                      <td className="py-2 pr-2">
                        <StatusBadge
                          label={t(`statuses.${row.status ?? "pending_approval"}`)}
                          variant={creditOverrideStatusVariant(row.status)}
                        />
                      </td>
                      <td className="py-2 text-right">
                        {canApprove && (
                          <Button
                            type="button"
                            size="sm"
                            variant="primary"
                            className="min-h-11"
                            disabled={isBusy}
                            onClick={() => setApprovingOverride(row)}
                          >
                            {t("approve")}
                          </Button>
                        )}
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
        )}

        {/* Request New Override Form */}
        {canManage && (
          <form
            onSubmit={(e) => void handleCreateOverride(e)}
            className="border border-erp-border bg-erp-surface-subtle p-4 space-y-3"
          >
            <h5 className="text-xs font-bold text-erp-navy uppercase tracking-wider">
              {t("requestOverride")}
            </h5>
            {formError && <Alert variant="danger" onClose={() => setFormError(null)}>{formError}</Alert>}
            <div className="grid grid-cols-1 gap-3 sm:grid-cols-2 lg:grid-cols-4">
              <Select
                id="ar-aging-override-customer"
                label={t("customer")}
                required
                value={overrideCustomer}
                placeholder={t("selectCustomer")}
                disabled={isBusy}
                className="h-11 min-h-[44px]"
                options={(customersQuery.data?.items ?? []).map((c) => {
                  const name = (locale === "en" ? c.displayNameEn || c.displayNameTh : c.displayNameTh || c.displayNameEn) ?? "-";
                  return {
                    value: c.id ?? "",
                    label: `${name} (${c.code ?? "-"})`,
                  };
                })}
                onChange={(e) => setOverrideCustomer(e.target.value)}
              />
              <Input
                id="ar-aging-override-amount"
                type="number"
                min={0}
                step="0.01"
                label={t("requestedCeilingAmount")}
                placeholder={t("requestedCeilingAmountPlaceholder")}
                required
                value={overrideAmount}
                disabled={isBusy}
                className="h-11 min-h-[44px]"
                onChange={(e) => setOverrideAmount(e.target.value)}
              />
              <Input
                id="ar-aging-override-expires-on"
                type="date"
                label={t("expiresOn")}
                placeholder={t("datePlaceholder")}
                value={overrideExpiresOn}
                disabled={isBusy}
                className="h-11 min-h-[44px]"
                onChange={(e) => setOverrideExpiresOn(e.target.value)}
              />
              <Input
                id="ar-aging-override-reason"
                label={t("reason")}
                placeholder={t("reasonPlaceholder")}
                required
                maxLength={500}
                value={overrideReason}
                disabled={isBusy}
                className="h-11 min-h-[44px]"
                onChange={(e) => setOverrideReason(e.target.value)}
              />
            </div>
            <div className="flex justify-end pt-2">
              <Button
                type="submit"
                variant="primary"
                className="min-h-11"
                isLoading={mutations.createOverride.isPending}
                disabled={isBusy}
              >
                {t("submitRequest")}
              </Button>
            </div>
          </form>
        )}
      </div>

      {/* Confirmation Modal for Approve Override */}
      <ConfirmationModal
        isOpen={approvingOverride !== null}
        onClose={() => setApprovingOverride(null)}
        onConfirm={() => void handleApproveOverrideConfirm()}
        title={t("approveTitle")}
        message={
          approvingOverride
            ? t("approveDescription", {
                customer: approvingOverride.customerName ?? "-",
                amount: formatCurrency(approvingOverride.requestedCeilingAmount, "THB", locale),
              })
            : ""
        }
        confirmText={t("approve")}
        variant="info"
        isLoading={mutations.approveOverride.isPending}
      />
    </div>
  );
}
