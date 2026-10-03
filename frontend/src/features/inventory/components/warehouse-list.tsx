"use client";

import { useMemo, useRef, useState } from "react";
import { useTranslations } from "next-intl";
import { DataTable, type Column } from "@/components/ui/DataTable";
import { ListToolbar } from "@/components/ui/ListToolbar";
import { ListSearchInput } from "@/components/ui/ListSearchInput";
import { ListFilterSelect } from "@/components/ui/ListFilterSelect";
import { ActiveFilterChips, type ActiveFilterChipItem } from "@/components/ui/ActiveFilterChips";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { EmptyState } from "@/components/ui/EmptyState";
import { Input } from "@/components/ui/Input";
import { Modal } from "@/components/ui/Modal";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { PageHeader } from "@/components/layout/PageHeader";
import { IconPlus } from "@/components/common/Icons";
import { useListState, type ListFilterRecord, type ListPageSize } from "@/hooks/useListState";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import type { WarehouseResponse } from "@/lib/api/api-client";
import { useWarehouseList, useWarehouseMutations } from "../api/inventory-queries";
import { inventoryErrorCode, isWarehouseStatus, WAREHOUSE_STATUSES } from "../inventory-status";

interface WarehouseFilters extends ListFilterRecord {
  status?: string;
}

export function WarehouseList() {
  const t = useTranslations("inventory.warehouses");
  const tErrors = useTranslations("inventory.errors");
  const tCommon = useTranslations("common");
  const { toast } = useToast();
  const { selectedMembership } = useSelectedMembership();
  const canManage = can(selectedMembership, PERMISSIONS.WAREHOUSES_MANAGE);
  const state = useListState<WarehouseFilters>({
    schema: { single: ["status"], defaultSort: "code", defaultOrder: "asc", allowedSorts: ["code"] },
    debounceMs: 300,
  });
  const result = useWarehouseList({
    search: state.params.search || undefined,
    status: state.params.filters.status || undefined,
    page: state.params.page,
    pageSize: state.params.limit,
  });
  const { create, update, setActive } = useWarehouseMutations();

  const [editing, setEditing] = useState<WarehouseResponse | "new" | null>(null);
  const [name, setName] = useState("");
  const [address, setAddress] = useState("");
  const [error, setError] = useState<string | null>(null);
  // One key per dialog session so a retried create replays instead of creating a second warehouse.
  const createKeyRef = useRef(crypto.randomUUID());

  const rows = result.data?.items ?? [];
  const pagination = result.data?.pagination;
  const totalCount = pagination?.totalCount ?? 0;
  const isZero = !result.isLoading && !result.isError && totalCount === 0 && !state.params.search && !state.params.filters.status;
  const isBusy = create.isPending || update.isPending;

  function describe(err: unknown): string {
    const code = err instanceof ApiError ? inventoryErrorCode(err.code) : null;
    return code ? tErrors(code) : tErrors("failed");
  }

  function openNew(): void {
    createKeyRef.current = crypto.randomUUID();
    setName("");
    setAddress("");
    setError(null);
    setEditing("new");
  }

  function openEdit(warehouse: WarehouseResponse): void {
    setName(warehouse.name ?? "");
    setAddress(warehouse.address ?? "");
    setError(null);
    setEditing(warehouse);
  }

  async function save(): Promise<void> {
    if (!name.trim()) {
      setError(t("invalid"));
      return;
    }

    const payload = { name: name.trim(), address: address.trim() || null };
    try {
      if (editing === "new") {
        await create.mutateAsync({ payload, idempotencyKey: createKeyRef.current });
        toast.success(t("created"));
      } else if (editing) {
        await update.mutateAsync({ warehouse: editing, payload });
        toast.success(t("updated"));
      }
      setEditing(null);
    } catch (err: unknown) {
      setError(describe(err));
    }
  }

  async function toggle(warehouse: WarehouseResponse): Promise<void> {
    try {
      await setActive.mutateAsync({ warehouse, active: warehouse.status !== "active" });
      toast.success(warehouse.status === "active" ? t("deactivated") : t("activated"));
    } catch (err: unknown) {
      toast.error(describe(err));
    }
  }

  const activeFilters = useMemo<ActiveFilterChipItem[]>(() => {
    const status = state.params.filters.status;
    return status && isWarehouseStatus(status) ? [{ key: "status", value: status, label: `${t("status")}: ${t(`statuses.${status}`)}` }] : [];
  }, [state.params.filters.status, t]);

  const columns: Column<WarehouseResponse>[] = [
    { id: "code", header: t("code"), accessorKey: "code", className: "min-w-28 font-mono" },
    { id: "name", header: t("name"), accessorKey: "name", className: "min-w-48" },
    { id: "address", header: t("address"), cell: (_v, row) => row.address ?? "-" },
    { id: "status", header: t("status"), cell: (_v, row) => <StatusBadge label={isWarehouseStatus(row.status) ? t(`statuses.${row.status}`) : "-"} variant={row.status === "active" ? "success" : "neutral"} /> },
    {
      id: "actions",
      header: t("actions"),
      isAction: true,
      cell: (_v, row) => canManage ? (
        <div className="flex gap-2">
          <Button type="button" variant="outline" size="sm" className="min-h-11" onClick={() => openEdit(row)}>{t("edit")}</Button>
          <Button type="button" variant="outline" size="sm" className="min-h-11" disabled={setActive.isPending} onClick={() => void toggle(row)}>
            {row.status === "active" ? t("deactivate") : t("activate")}
          </Button>
        </div>
      ) : null,
    },
  ];

  return (
    <section className="space-y-5" aria-busy={result.isLoading}>
      <PageHeader
        title={t("title")}
        subtitle={t("subtitle")}
        breadcrumbs={[{ label: t("title") }]}
        actions={canManage ? <Button size="md" icon={<IconPlus size={16} />} onClick={openNew}>{t("create")}</Button> : undefined}
      />
      <ListToolbar activeFilters={<ActiveFilterChips filters={activeFilters} onRemove={() => state.actions.setFilter("status", undefined)} onClear={state.actions.clearFilters} />}>
        <div className="flex w-full flex-wrap items-end gap-3">
          <ListSearchInput
            id="warehouse-search"
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
            id="warehouse-status-filter"
            label={t("status")}
            value={state.params.filters.status ?? ""}
            onChange={(value) => state.actions.setFilter("status", value || undefined)}
            options={WAREHOUSE_STATUSES.map((value) => ({ value, label: t(`statuses.${value}`) }))}
          />
        </div>
      </ListToolbar>
      {isZero ? (
        <EmptyState icon="empty" title={t("empty")} description={t("emptyDetail")} actionLabel={canManage ? t("create") : undefined} onAction={canManage ? openNew : undefined} />
      ) : (
        <DataTable<WarehouseResponse>
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
        isOpen={editing !== null}
        onClose={() => { if (!isBusy) setEditing(null); }}
        title={editing === "new" ? t("create") : t("edit")}
        size="sm"
        closeDisabled={isBusy}
        closeOnOverlayClick={!isBusy}
        closeOnEscape={!isBusy}
        footer={(
          <div className="flex justify-end gap-3">
            <Button type="button" variant="outline" className="min-h-11" disabled={isBusy} onClick={() => setEditing(null)}>{tCommon("actions.cancel")}</Button>
            <Button type="button" variant="primary" className="min-h-11" isLoading={isBusy} disabled={isBusy} onClick={() => void save()}>{tCommon("actions.save")}</Button>
          </div>
        )}
      >
        <div className="space-y-3">
          {error && <Alert variant="danger" onClose={() => setError(null)}>{error}</Alert>}
          <Input label={t("name")} required maxLength={200} value={name} disabled={isBusy} onChange={(event) => setName(event.target.value)} />
          <Input label={t("address")} maxLength={500} value={address} disabled={isBusy} onChange={(event) => setAddress(event.target.value)} />
        </div>
      </Modal>
    </section>
  );
}
