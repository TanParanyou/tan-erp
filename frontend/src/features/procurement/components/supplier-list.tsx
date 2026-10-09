"use client";

import { useMemo, useRef, useState } from "react";
import { useLocale, useTranslations } from "next-intl";
import { DataTable, type Column } from "@/components/ui/DataTable";
import { ListToolbar } from "@/components/ui/ListToolbar";
import { ListSearchInput } from "@/components/ui/ListSearchInput";
import { ListFilterSelect } from "@/components/ui/ListFilterSelect";
import { ActiveFilterChips, type ActiveFilterChipItem } from "@/components/ui/ActiveFilterChips";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { EmptyState } from "@/components/ui/EmptyState";
import { Input } from "@/components/ui/Input";
import { PhoneInput } from "@/components/forms/PhoneInput";
import { TaxIdInput } from "@/components/forms/TaxIdInput";
import { Drawer } from "@/components/ui/Drawer";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { PageHeader } from "@/components/layout/PageHeader";
import { IconPlus } from "@/components/common/Icons";
import { useListState, type ListFilterRecord, type ListPageSize } from "@/hooks/useListState";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import { isValidPhoneNumber } from "@/lib/validation/phone";
import { isValidTaxId } from "@/lib/validation/tax-id";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import type { SupplierRequest, SupplierResponse } from "@/lib/api/api-client";
import { useSupplierList, useSupplierMutations } from "../api/procurement-queries";
import { isSupplierStatus, procurementErrorCode, SUPPLIER_STATUSES } from "../procurement-status";

interface SupplierFilters extends ListFilterRecord {
  status?: string;
}

interface FormState {
  nameTh: string;
  nameEn: string;
  taxId: string;
  contactName: string;
  phone: string;
  email: string;
  paymentTermDays: string;
}

const EMPTY_FORM: FormState = { nameTh: "", nameEn: "", taxId: "", contactName: "", phone: "", email: "", paymentTermDays: "30" };

function toForm(supplier: SupplierResponse): FormState {
  return {
    nameTh: supplier.nameTh ?? "",
    nameEn: supplier.nameEn ?? "",
    taxId: supplier.taxId ?? "",
    contactName: supplier.contactName ?? "",
    phone: supplier.phone ?? "",
    email: supplier.email ?? "",
    paymentTermDays: String(supplier.paymentTermDays ?? 0),
  };
}

export function SupplierList() {
  const t = useTranslations("procurement.suppliers");
  const tErrors = useTranslations("procurement.errors");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const { toast } = useToast();
  const { selectedMembership } = useSelectedMembership();
  const canManage = can(selectedMembership, PERMISSIONS.SUPPLIERS_MANAGE);
  const state = useListState<SupplierFilters>({
    schema: { single: ["status"], defaultSort: "name", defaultOrder: "asc", allowedSorts: ["name"] },
    debounceMs: 300,
  });
  const result = useSupplierList({
    search: state.params.search || undefined,
    status: state.params.filters.status || undefined,
    page: state.params.page,
    pageSize: state.params.limit,
  });
  const { create, update, setActive } = useSupplierMutations();

  const [editing, setEditing] = useState<SupplierResponse | "new" | null>(null);
  const [form, setForm] = useState<FormState>(EMPTY_FORM);
  const [error, setError] = useState<string | null>(null);
  // One key per dialog session so a retried create replays instead of creating a second supplier.
  const createKeyRef = useRef(crypto.randomUUID());

  const rows = result.data?.items ?? [];
  const pagination = result.data?.pagination;
  const totalCount = pagination?.totalCount ?? 0;
  const isZero = !result.isLoading && !result.isError && totalCount === 0 && !state.params.search && !state.params.filters.status;
  const isBusy = create.isPending || update.isPending;

  function describe(err: unknown): string {
    const code = err instanceof ApiError ? procurementErrorCode(err.code) : null;
    return code ? tErrors(code) : tErrors("failed");
  }

  function openNew(): void {
    createKeyRef.current = crypto.randomUUID();
    setForm(EMPTY_FORM);
    setError(null);
    setEditing("new");
  }

  function openEdit(supplier: SupplierResponse): void {
    setForm(toForm(supplier));
    setError(null);
    setEditing(supplier);
  }

  async function save(): Promise<void> {
    const term = Number(form.paymentTermDays);
    if (!form.nameTh.trim() || !Number.isInteger(term) || term < 0 || term > 365) {
      setError(t("invalid"));
      return;
    }

    const trimmedTaxId = form.taxId.trim();
    if (trimmedTaxId && !isValidTaxId(trimmedTaxId)) {
      setError(t("invalidTaxId"));
      return;
    }

    const trimmedPhone = form.phone.trim();
    if (trimmedPhone && !isValidPhoneNumber(trimmedPhone)) {
      setError(t("invalidPhone"));
      return;
    }

    const trimmedEmail = form.email.trim();
    if (trimmedEmail && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(trimmedEmail)) {
      setError(t("invalidEmail"));
      return;
    }

    const payload: SupplierRequest = {
      nameTh: form.nameTh.trim(),
      nameEn: form.nameEn.trim() || null,
      taxId: trimmedTaxId || null,
      contactName: form.contactName.trim() || null,
      phone: trimmedPhone || null,
      email: trimmedEmail || null,
      paymentTermDays: term,
    };

    try {
      if (editing === "new") {
        await create.mutateAsync({ payload, idempotencyKey: createKeyRef.current });
        toast.success(t("created"));
      } else if (editing) {
        await update.mutateAsync({ supplier: editing, payload });
        toast.success(t("updated"));
      }
      setEditing(null);
    } catch (err: unknown) {
      setError(describe(err));
    }
  }

  async function toggle(supplier: SupplierResponse): Promise<void> {
    try {
      await setActive.mutateAsync({ supplier, active: supplier.status !== "active" });
      toast.success(supplier.status === "active" ? t("deactivated") : t("activated"));
    } catch (err: unknown) {
      toast.error(describe(err));
    }
  }

  const activeFilters = useMemo<ActiveFilterChipItem[]>(() => {
    const status = state.params.filters.status;
    return status && isSupplierStatus(status) ? [{ key: "status", value: status, label: `${t("status")}: ${t(`statuses.${status}`)}` }] : [];
  }, [state.params.filters.status, t]);

  const columns: Column<SupplierResponse>[] = [
    { id: "code", header: t("code"), accessorKey: "code", className: "min-w-32 font-mono" },
    { id: "name", header: t("name"), cell: (_v, row) => (locale === "en" ? row.nameEn ?? row.nameTh : row.nameTh) ?? "-", className: "min-w-48" },
    { id: "contact", header: t("contact"), cell: (_v, row) => [row.contactName, row.phone].filter(Boolean).join(" • ") || "-" },
    { id: "terms", header: t("paymentTerm"), cell: (_v, row) => t("days", { count: row.paymentTermDays ?? 0 }) },
    { id: "status", header: t("status"), cell: (_v, row) => <StatusBadge label={isSupplierStatus(row.status) ? t(`statuses.${row.status}`) : "-"} variant={row.status === "active" ? "success" : "neutral"} /> },
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

  const field = (key: keyof FormState) => ({
    value: form[key],
    disabled: isBusy,
    onChange: (event: React.ChangeEvent<HTMLInputElement>) => setForm((current) => ({ ...current, [key]: event.target.value })),
  });

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
            id="supplier-search"
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
            id="supplier-status-filter"
            label={t("status")}
            value={state.params.filters.status ?? ""}
            onChange={(value) => state.actions.setFilter("status", value || undefined)}
            options={SUPPLIER_STATUSES.map((value) => ({ value, label: t(`statuses.${value}`) }))}
          />
        </div>
      </ListToolbar>
      {isZero ? (
        <EmptyState icon="empty" title={t("empty")} description={t("emptyDetail")} actionLabel={canManage ? t("create") : undefined} onAction={canManage ? openNew : undefined} />
      ) : (
        <DataTable<SupplierResponse>
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

      <Drawer
        isOpen={editing !== null}
        onClose={() => { if (!isBusy) setEditing(null); }}
        title={editing === "new" ? t("create") : t("edit")}
        size="lg"
        closeDisabled={isBusy}
        closeOnOverlayClick={!isBusy}
        footer={(
          <div className="flex flex-col-reverse sm:flex-row sm:justify-end gap-2 sm:gap-3">
            <Button
              type="button"
              variant="outline"
              className="w-full sm:w-auto min-h-11"
              disabled={isBusy}
              onClick={() => setEditing(null)}
            >
              {tCommon("actions.cancel")}
            </Button>
            <Button
              type="button"
              variant="primary"
              className="w-full sm:w-auto min-h-11"
              isLoading={isBusy}
              disabled={isBusy}
              onClick={() => void save()}
            >
              {tCommon("actions.save")}
            </Button>
          </div>
        )}
      >
        <div className="space-y-3 py-2">
          {error && <Alert variant="danger" onClose={() => setError(null)}>{error}</Alert>}
          <Input label={t("nameTh")} required maxLength={200} placeholder={t("nameThPlaceholder")} {...field("nameTh")} />
          <Input label={t("nameEn")} maxLength={200} placeholder={t("nameEnPlaceholder")} {...field("nameEn")} />
          <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
            <TaxIdInput
              id="supplier-tax-id"
              label={t("taxId")}
              placeholder={t("taxIdPlaceholder")}
              value={form.taxId}
              onValueChange={(val) => setForm((curr) => ({ ...curr, taxId: val }))}
              disabled={isBusy}
            />
            <Input type="number" min={0} max={365} label={t("paymentTerm")} placeholder={t("paymentTermDaysPlaceholder")} {...field("paymentTermDays")} />
            <Input label={t("contactName")} maxLength={200} placeholder={t("contactNamePlaceholder")} {...field("contactName")} />
            <PhoneInput
              id="supplier-phone"
              label={t("phone")}
              placeholder={t("phonePlaceholder")}
              value={form.phone}
              onValueChange={(val) => setForm((curr) => ({ ...curr, phone: val }))}
              disabled={isBusy}
            />
          </div>
          <Input type="email" label={t("email")} maxLength={200} placeholder={t("emailPlaceholder")} {...field("email")} />
        </div>
      </Drawer>
    </section>
  );
}
