"use client";

import { useMemo, useState } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { useLocale, useTranslations } from "next-intl";
import { DataTable, type Column } from "@/components/ui/DataTable";
import { ListToolbar } from "@/components/ui/ListToolbar";
import { ListSearchInput } from "@/components/ui/ListSearchInput";
import { ListFilterSelect } from "@/components/ui/ListFilterSelect";
import { ActiveFilterChips, type ActiveFilterChipItem } from "@/components/ui/ActiveFilterChips";
import { Button } from "@/components/ui/Button";
import { Checkbox } from "@/components/ui/Checkbox";
import { EmptyState } from "@/components/ui/EmptyState";
import { Input } from "@/components/ui/Input";
import { Modal } from "@/components/ui/Modal";
import { Select } from "@/components/ui/Select";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { Alert } from "@/components/ui/Alert";
import { PageHeader } from "@/components/layout/PageHeader";
import { IconPlus } from "@/components/common/Icons";
import { useListState, type ListFilterRecord, type ListPageSize } from "@/hooks/useListState";
import { useRowSelection } from "@/hooks/useRowSelection";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import { formatCurrency, formatDate } from "@/lib/formatters/formatters";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import type { PurchaseRequestListItemResponse } from "@/lib/api/api-client";
import {
  usePurchaseRequestList,
  usePurchaseRequestMutations,
  useSupplierList,
} from "../api/procurement-queries";
import {
  isPurchaseRequestStatus,
  procurementErrorCode,
  PURCHASE_REQUEST_STATUSES,
  purchaseRequestStatusVariant,
} from "../procurement-status";

interface RequestFilters extends ListFilterRecord {
  status?: string;
}

export function PurchaseRequestList() {
  const t = useTranslations("procurement.requests");
  const tErrors = useTranslations("procurement.errors");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const router = useRouter();
  const { toast } = useToast();
  const { selectedMembership } = useSelectedMembership();
  const canCreate = can(selectedMembership, PERMISSIONS.PURCHASE_ORDERS_CREATE);

  const state = useListState<RequestFilters>({
    schema: { single: ["status"], defaultSort: "createdAt", defaultOrder: "desc", allowedSorts: ["createdAt"] },
    debounceMs: 300,
  });

  const {
    selectedIds,
    selectedArray,
    selectedCount,
    toggleSelection,
    selectAll,
    clearSelection,
  } = useRowSelection<string>();

  const result = usePurchaseRequestList({
    search: state.params.search || undefined,
    status: state.params.filters.status || undefined,
    page: state.params.page,
    pageSize: state.params.limit,
  });

  const mutations = usePurchaseRequestMutations();
  const suppliers = useSupplierList({ status: "active", page: 1, pageSize: 100 }, canCreate);

  // Convert Modal state
  const [isConvertOpen, setIsConvertOpen] = useState(false);
  const [supplierId, setSupplierId] = useState("");
  const [expectedDeliveryDate, setExpectedDeliveryDate] = useState("");
  const [convertNote, setConvertNote] = useState("");
  const [convertError, setConvertError] = useState<string | null>(null);

  const rows = result.data?.items ?? [];
  const pagination = result.data?.pagination;
  const totalCount = pagination?.totalCount ?? 0;
  const isZero = !result.isLoading && !result.isError && totalCount === 0 && !state.params.search && !state.params.filters.status;

  const approvedRows = useMemo(() => rows.filter((r) => r.status === "approved" && Boolean(r.id)), [rows]);

  const activeFilters = useMemo<ActiveFilterChipItem[]>(() => {
    const status = state.params.filters.status;
    return status && isPurchaseRequestStatus(status) ? [{ key: "status", value: status, label: `${t("status")}: ${t(`statuses.${status}`)}` }] : [];
  }, [state.params.filters.status, t]);

  async function handleConvert(): Promise<void> {
    setConvertError(null);
    if (!supplierId) {
      setConvertError(t("supplierRequired"));
      return;
    }

    try {
      const order = await mutations.convert.mutateAsync({
        payload: {
          purchaseRequestIds: selectedArray,
          supplierId,
          expectedDeliveryDate: expectedDeliveryDate || null,
          note: convertNote.trim() || null,
        },
      });
      toast.success(t("convertSuccess"));
      setIsConvertOpen(false);
      clearSelection();
      router.push(`/${locale}/procurement/purchase-orders/${order.id}`);
    } catch (err: unknown) {
      const code = err instanceof ApiError ? procurementErrorCode(err.code) : null;
      setConvertError(code ? tErrors(code) : tErrors("failed"));
    }
  }

  const columns: Column<PurchaseRequestListItemResponse>[] = [
    {
      id: "select",
      header: (
        <Checkbox
          checked={approvedRows.length > 0 && approvedRows.every((r) => r.id && selectedIds.has(r.id))}
          aria-label={t("selectApprovedOnly")}
          onChange={() => {
            const approvedIds = approvedRows.map((r) => r.id).filter((id): id is string => Boolean(id));
            const allSelected = approvedIds.length > 0 && approvedIds.every((id) => selectedIds.has(id));
            if (allSelected) {
              clearSelection();
            } else {
              selectAll(approvedIds);
            }
          }}
        />
      ),
      cell: (_v, row) =>
        row.status === "approved" && row.id ? (
          <Checkbox
            checked={selectedIds.has(row.id)}
            aria-label={`${t("number")} ${row.number ?? "-"}`}
            onChange={() => toggleSelection(row.id!)}
          />
        ) : null,
      className: "w-10 text-center",
    },
    {
      id: "number",
      header: t("number"),
      className: "min-w-32 font-mono",
      cell: (_v, row) => (
        <Link href={`/${locale}/procurement/purchase-requests/${row.id}`} className="font-mono font-semibold text-erp-navy underline">
          {row.number ?? "-"}
        </Link>
      ),
    },
    { id: "project", header: t("project"), cell: (_v, row) => row.projectCode ?? "-" },
    {
      id: "status",
      header: t("status"),
      cell: (_v, row) => (
        <StatusBadge
          label={isPurchaseRequestStatus(row.status) ? t(`statuses.${row.status}`) : "-"}
          variant={purchaseRequestStatusVariant(row.status)}
        />
      ),
    },
    {
      id: "total",
      header: t("totalEstimatedAmount"),
      className: "text-right font-mono",
      cell: (_v, row) => formatCurrency(row.totalEstimatedAmount, "THB", locale),
    },
    {
      id: "requiredBy",
      header: t("requiredByDate"),
      cell: (_v, row) => (row.requiredByDate ? formatDate(row.requiredByDate, locale) : "-"),
    },
    {
      id: "convertedPo",
      header: t("convertedToPo", { number: "" }).replace(":", "").trim(),
      cell: (_v, row) =>
        row.convertedPurchaseOrderId && row.convertedPurchaseOrderNumber ? (
          <Link
            href={`/${locale}/procurement/purchase-orders/${row.convertedPurchaseOrderId}`}
            className="font-mono text-erp-navy underline"
          >
            {row.convertedPurchaseOrderNumber}
          </Link>
        ) : (
          "-"
        ),
    },
  ];

  return (
    <section className="space-y-5" aria-busy={result.isLoading}>
      <PageHeader
        title={t("title")}
        subtitle={t("subtitle")}
        breadcrumbs={[{ label: t("title") }]}
        actions={
          <div className="flex flex-wrap items-center gap-3">
            {canCreate && selectedCount > 0 && (
              <Button
                variant="primary"
                size="md"
                onClick={() => {
                  setConvertError(null);
                  setIsConvertOpen(true);
                }}
              >
                {t("convertTitle")} ({selectedCount})
              </Button>
            )}
            {canCreate && (
              <Button size="md" icon={<IconPlus size={16} />} href={`/${locale}/procurement/purchase-requests/create`}>
                {t("create")}
              </Button>
            )}
          </div>
        }
      />
      <ListToolbar activeFilters={<ActiveFilterChips filters={activeFilters} onRemove={() => state.actions.setFilter("status", undefined)} onClear={state.actions.clearFilters} />}>
        <div className="flex w-full flex-wrap items-end gap-3">
          <ListSearchInput
            id="purchase-request-search"
            label={tCommon("actions.search")}
            value={state.draftSearch}
            isDebouncing={state.isDebouncing}
            onChange={(value) => state.actions.setSearch(value)}
            onClear={() => state.actions.setSearch("", true)}
            onSubmit={(value) => state.actions.setSearch(value, true)}
            placeholder={t("search")}
            widthClassName="w-full max-w-xl"
          />
          <ListFilterSelect
            id="purchase-request-status-filter"
            label={t("status")}
            value={state.params.filters.status ?? ""}
            onChange={(value) => state.actions.setFilter("status", value || undefined)}
            options={PURCHASE_REQUEST_STATUSES.map((value) => ({ value, label: t(`statuses.${value}`) }))}
          />
        </div>
      </ListToolbar>
      {isZero ? (
        <EmptyState icon="empty" title={t("empty")} description={t("emptyDetail")} />
      ) : (
        <DataTable<PurchaseRequestListItemResponse>
          columns={columns}
          data={rows}
          isLoading={result.isLoading}
          isError={result.isError}
          error={result.error}
          onRetry={() => { void result.refetch(); }}
          emptyTitle={t("noResults")}
          emptyDescription={t("search")}
          pagination={{ page: state.params.page, limit: state.params.limit, totalPages: pagination?.totalPages ?? 0, total: totalCount }}
          onPageChange={state.actions.setPage}
          onLimitChange={(limit) => state.actions.setLimit(limit as ListPageSize)}
        />
      )}

      <Modal
        isOpen={isConvertOpen}
        onClose={() => { if (!mutations.convert.isPending) setIsConvertOpen(false); }}
        title={t("convertTitle")}
        description={t("convertDescription")}
        size="md"
        closeDisabled={mutations.convert.isPending}
        closeOnOverlayClick={!mutations.convert.isPending}
        closeOnEscape={!mutations.convert.isPending}
        footer={
          <div className="flex justify-end gap-3">
            <Button
              type="button"
              variant="outline"
              className="min-h-11"
              disabled={mutations.convert.isPending}
              onClick={() => setIsConvertOpen(false)}
            >
              {tCommon("actions.cancel")}
            </Button>
            <Button
              type="button"
              variant="primary"
              className="min-h-11"
              isLoading={mutations.convert.isPending}
              disabled={mutations.convert.isPending || !supplierId}
              onClick={() => void handleConvert()}
            >
              {t("convertSubmit")}
            </Button>
          </div>
        }
      >
        <div className="space-y-4">
          {convertError && <Alert variant="danger">{convertError}</Alert>}
          <p className="text-sm font-semibold text-erp-navy">{t("selectionSummary", { count: selectedCount })}</p>
          <Select
            label={t("supplier")}
            required
            value={supplierId}
            placeholder={t("selectSupplier")}
            disabled={mutations.convert.isPending}
            options={(suppliers.data?.items ?? []).map((s) => ({
              value: s.id ?? "",
              label: `${s.code ?? "-"} · ${s.nameTh ?? "-"}`,
            }))}
            onChange={(event) => setSupplierId(event.target.value)}
          />
          <Input
            type="date"
            label={t("expectedDelivery")}
            placeholder={t("expectedDeliveryPlaceholder")}
            value={expectedDeliveryDate}
            disabled={mutations.convert.isPending}
            onChange={(event) => setExpectedDeliveryDate(event.target.value)}
          />
          <Input
            label={t("convertNote")}
            placeholder={t("convertNotePlaceholder")}
            value={convertNote}
            maxLength={500}
            disabled={mutations.convert.isPending}
            onChange={(event) => setConvertNote(event.target.value)}
          />
        </div>
      </Modal>
    </section>
  );
}
