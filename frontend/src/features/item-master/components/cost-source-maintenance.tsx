"use client";

import { useMemo, useState } from "react";
import { useLocale, useTranslations } from "next-intl";
import { z } from "zod";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { PageHeader } from "@/components/layout/PageHeader";
import { DataTable, type Column } from "@/components/ui/DataTable";
import { ListToolbar } from "@/components/ui/ListToolbar";
import { ListSearchInput } from "@/components/ui/ListSearchInput";
import { ListFilterSelect } from "@/components/ui/ListFilterSelect";
import { ActiveFilterChips, type ActiveFilterChipItem } from "@/components/ui/ActiveFilterChips";
import { ExportDropdown } from "@/components/ui/ExportDropdown";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Drawer } from "@/components/ui/Drawer";
import { FormSection } from "@/components/forms/FormSection";
import { ConfirmationModal } from "@/components/ui/ConfirmationModal";
import { MultiLangInput } from "@/components/forms/MultiLangInput";
import { GeneratedCodeField } from "@/components/forms/GeneratedCodeField";
import { useToast } from "@/hooks/useToast";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import { LIST_PAGE_SIZES, useListState, type ListFilterRecord, type ListPageSize } from "@/hooks/useListState";
import { useDataExport } from "@/hooks/useDataExport";
import type { ExportColumn } from "@/lib/export/export-types";
import { useCostSources, useItemMasterMutations } from "@/features/item-master/api/item-master-queries";
import type { CostSourceResponse } from "@/lib/api/api-client";

const sourceSchema = z.object({
  code: z.string().trim().max(32),
  codeMode: z.enum(["generated", "manual"]),
  name: z.object({ th: z.string().trim().min(1), en: z.string().optional() }),
}).superRefine((values, context) => {
  if (values.codeMode === "manual" && !values.code) context.addIssue({ code: "custom", message: "CODE_REQUIRED", path: ["code"] });
});
type SourceFormValues = z.infer<typeof sourceSchema>;

interface CostSourceFilters extends ListFilterRecord {
  status?: string;
}

function isListPageSize(value: number): value is ListPageSize {
  return LIST_PAGE_SIZES.some((pageSize) => pageSize === value);
}

export function CostSourceMaintenance() {
  const t = useTranslations("itemMaster");
  const common = useTranslations("common");
  const locale = useLocale();
  const { toast } = useToast();
  const { selectedMembership } = useSelectedMembership();
  const query = useCostSources();
  const mutations = useItemMasterMutations();
  const canManage = can(selectedMembership, "cost-sources.manage");
  const list = useListState<CostSourceFilters>({
    schema: { single: ["status"], defaultSort: "code", defaultOrder: "asc", allowedSorts: ["code", "name", "status"] },
  });
  const [editing, setEditing] = useState<CostSourceResponse | null>(null);
  const [discardOpen, setDiscardOpen] = useState(false);
  const [formOpen, setFormOpen] = useState(false);
  const [deactivating, setDeactivating] = useState<CostSourceResponse | null>(null);
  const form = useForm<SourceFormValues>({ resolver: zodResolver(sourceSchema), defaultValues: { code: "", codeMode: "generated", name: { th: "", en: "" } } });
  const saving = form.formState.isSubmitting || mutations.createCostSource.isPending || mutations.updateCostSource.isPending;
  const isDirty = form.formState.isDirty;
  const requestClose = () => {
    if (saving) return;
    if (isDirty) setDiscardOpen(true);
    else setFormOpen(false);
  };
  const deactivatingNow = mutations.deactivateCostSource.isPending;

  const openCreate = () => {
    setEditing(null);
    form.reset({ code: "", codeMode: "generated", name: { th: "", en: "" } });
    setFormOpen(true);
  };

  const openEdit = (source: CostSourceResponse) => {
    setEditing(source);
    form.reset({ code: source.code ?? "", codeMode: "manual", name: { th: source.name?.thai ?? "", en: source.name?.english ?? "" } });
    setFormOpen(true);
  };

  const onSubmit = form.handleSubmit(async (values) => {
    const payload = { code: !editing && values.codeMode === "generated" ? null : values.code, name: { thai: values.name.th, english: values.name.en?.trim() || null }, sourceType: "manual" as const };
    try {
      if (editing) {
        if (!editing.id || !editing.rowVersion) return;
        await mutations.updateCostSource.mutateAsync({ id: editing.id, rowVersion: editing.rowVersion, payload: { ...payload, code: values.code } });
      } else {
        const created = await mutations.createCostSource.mutateAsync({ payload, idempotencyKey: crypto.randomUUID() });
        setFormOpen(false);
        toast.success(t("sourceSavedWithCode", { code: created.code ?? "" }));
        return;
      }
      setFormOpen(false);
      toast.success(t("sourceSaved"));
    } catch {
      toast.error(t("createFailed"));
    }
  });

  const filteredRows = useMemo(() => {
    const queryText = list.params.search.toLocaleLowerCase(locale);
    const status = list.params.filters.status;
    const collator = new Intl.Collator(locale, { numeric: true, sensitivity: "base" });
    return (query.data ?? []).filter((source) => {
      const name = locale === "en" ? source.name?.english ?? "" : source.name?.thai ?? "";
      return (!queryText || [source.code ?? "", name, source.sourceType ?? ""].some((value) => value.toLocaleLowerCase(locale).includes(queryText)))
        && (!status || (status === "active" ? source.isActive : !source.isActive));
    }).sort((left, right) => {
      const key = list.params.sort ?? "code";
      const leftValue = key === "name" ? (locale === "en" ? left.name?.english ?? "" : left.name?.thai ?? "") : key === "status" ? String(left.isActive) : left.code ?? "";
      const rightValue = key === "name" ? (locale === "en" ? right.name?.english ?? "" : right.name?.thai ?? "") : key === "status" ? String(right.isActive) : right.code ?? "";
      const compared = collator.compare(leftValue, rightValue);
      return (list.params.order === "asc" ? compared : -compared) || collator.compare(left.id ?? "", right.id ?? "");
    });
  }, [list.params.filters.status, list.params.order, list.params.search, list.params.sort, locale, query.data]);
  const pageCount = Math.ceil(filteredRows.length / list.params.limit);
  const pageRows = filteredRows.slice((list.params.page - 1) * list.params.limit, list.params.page * list.params.limit);
  const exportColumns = useMemo<ExportColumn<CostSourceResponse>[]>(() => [
    { header: t("sourceCode"), accessor: (row) => row.code ?? "" },
    { header: t("sourceName"), accessor: (row) => locale === "en" ? row.name?.english ?? "" : row.name?.thai ?? "" },
    { header: t("sourceType"), accessor: (row) => row.sourceType ?? "" },
    { header: t("status"), accessor: (row) => row.isActive ? t("statusActive") : t("statusInactive") },
  ], [locale, t]);
  const exportData = useDataExport<CostSourceResponse>({ filename: "item-master-cost-sources", columns: exportColumns, data: filteredRows });
  const activeFilters = useMemo<ActiveFilterChipItem[]>(() => {
    const status = list.params.filters.status;
    return status ? [{ key: "status", value: status, label: `${t("status")}: ${status === "active" ? t("statusActive") : t("statusInactive")}` }] : [];
  }, [list.params.filters.status, t]);

  const columns = useMemo<Column<CostSourceResponse>[]>(() => [
    { id: "code", header: t("sourceCode"), accessorKey: "code", sortable: true, className: "font-mono" },
    { id: "name", header: t("sourceName"), sortable: true, cell: (_value, row) => locale === "en" ? row.name?.english ?? "-" : row.name?.thai ?? "-" },
    { id: "type", header: t("sourceType"), accessorKey: "sourceType" },
    { id: "status", header: t("status"), sortable: true, cell: (_value, row) => row.isActive ? t("statusActive") : t("statusInactive") },
    { id: "actions", header: t("rowActions"), isAction: true, sticky: "right", cell: (_value, row) => (
      <div className="flex justify-end gap-2">
        {canManage && <Button type="button" size="sm" variant="outline" disabled={!row.isActive || !row.id} onClick={() => openEdit(row)}>{t("edit")}</Button>}
        {canManage && row.isActive && row.id && row.rowVersion && <Button type="button" size="sm" variant="danger" onClick={() => setDeactivating(row)}>{t("deactivate")}</Button>}
      </div>
    ) },
  ], [canManage, locale, t]);

  const confirmDeactivate = async () => {
    if (!deactivating?.id || !deactivating.rowVersion) return;
    try {
      await mutations.deactivateCostSource.mutateAsync({ id: deactivating.id, rowVersion: deactivating.rowVersion });
      setDeactivating(null);
      toast.success(t("deactivated"));
    } catch {
      toast.error(t("versionConflict"));
    }
  };

  return (
    <section className="space-y-5">
      <PageHeader title={t("costSourcesTitle")} subtitle={t("costSourcesHelp")} backHref={`/${locale}/item-master`} breadcrumbs={[{ label: t("title"), href: `/${locale}/item-master` }, { label: t("costSourcesTitle") }]} actions={canManage ? <Button type="button" onClick={openCreate}>{t("createSource")}</Button> : null} />
      <section className="space-y-3" aria-label={t("costSourcesTitle")}>
        <ListToolbar activeFilters={<ActiveFilterChips filters={activeFilters} onRemove={(key) => { if (key === "status") list.actions.setFilter("status", undefined); }} onClear={list.actions.clearFilters} />}>
          <div className="flex w-full flex-wrap items-end gap-3">
            <ListSearchInput id="cost-source-search" label={common("actions.search")} value={list.draftSearch} isDebouncing={list.isDebouncing} onChange={(value) => list.actions.setSearch(value)} onClear={() => list.actions.setSearch("", true)} onSubmit={(value) => list.actions.setSearch(value, true)} placeholder={t("searchCostSources")} widthClassName="w-full max-w-xl" />
            <ListFilterSelect id="cost-source-status-filter" label={t("status")} value={list.params.filters.status ?? ""} onChange={(value) => list.actions.setFilter("status", value || undefined)} options={[{ value: "active", label: t("statusActive") }, { value: "inactive", label: t("statusInactive") }]} />
            <ExportDropdown label={t("exportCostSources")} isLoading={exportData.isExporting} disabled={filteredRows.length === 0} onExport={async (format) => { try { await exportData.exportAll(format); } catch { toast.error(t("exportMaintenanceFailed")); } }} />
          </div>
        </ListToolbar>
        <DataTable<CostSourceResponse>
          columns={columns}
          data={pageRows}
          pagination={{ page: list.params.page, limit: list.params.limit, totalPages: pageCount, total: filteredRows.length }}
          sorting={{ key: list.params.sort ?? "code", order: list.params.order }}
          onPageChange={list.actions.setPage}
          onLimitChange={(limit) => { if (isListPageSize(limit)) list.actions.setLimit(limit); }}
          onSort={list.actions.setSort}
          isLoading={query.isLoading}
          isError={query.isError}
          error={query.error}
          onRetry={() => { void query.refetch(); }}
          emptyTitle={list.params.search || list.params.filters.status ? t("noCostSourceResults") : t("noSources")}
          emptyDescription={filteredRows.length === 0 && (list.params.search || list.params.filters.status) ? t("noCostSourceResultsHelp") : t("noSourcesHelp")}
        />
      </section>
      <Drawer isOpen={formOpen} onClose={requestClose} closeDisabled={saving || discardOpen} closeLabel={common("actions.close")} size="lg" title={editing ? t("editSource") : t("createSource")} description={t("sourceFormHelp")}
        footer={<div className="flex flex-wrap justify-end gap-3"><Button type="button" variant="outline" disabled={saving} onClick={requestClose}>{common("actions.cancel")}</Button><Button type="submit" form="cost-source-form" isLoading={saving} disabled={!canManage || saving}>{t("saveSource")}</Button></div>}
      >
        <form id="cost-source-form" onSubmit={(event) => { void onSubmit(event); }} noValidate className="space-y-4">
          <FormSection title={t("sourceIdentity")} description={t("thaiNameRequiredEnglishOptional")} className="p-4">
          {editing ? <Input label={t("sourceCode")} placeholder={t("sourceCodePlaceholder")} required {...form.register("code")} error={form.formState.errors.code ? t("codeRequired") : undefined} disabled={saving || !editing.isActive} /> : <GeneratedCodeField id="cost-source-code" label={t("sourceCode")} value={form.watch("code")} mode={form.watch("codeMode")} maxLength={32} disabled={saving} error={form.formState.errors.code ? t("codeRequired") : undefined} onChange={(value) => form.setValue("code", value, { shouldDirty: true, shouldValidate: true })} onModeChange={(mode) => { form.setValue("codeMode", mode, { shouldDirty: true, shouldValidate: true }); if (mode === "generated") form.setValue("code", "", { shouldDirty: true, shouldValidate: true }); }} />}
          <MultiLangInput label={t("sourceName")} required disabled={saving} value={form.watch("name")} onChange={(value) => form.setValue("name", { th: value.th ?? "", en: value.en }, { shouldDirty: true, shouldValidate: true })} error={form.formState.errors.name ? t("validationRequired") : undefined} />
          {(mutations.createCostSource.isError || mutations.updateCostSource.isError) && <p role="alert" className="text-sm text-erp-danger">{t("createFailed")}</p>}
          </FormSection>
        </form>
      </Drawer>
      <ConfirmationModal isOpen={discardOpen} onClose={() => setDiscardOpen(false)} onConfirm={() => { setDiscardOpen(false); setFormOpen(false); }} title={common("dialog.confirmCancelTitle")} message={common("dialog.confirmCancelDesc")} confirmText={common("actions.discard")} cancelText={common("actions.cancel")} variant="warning" isLoading={saving} />
      <ConfirmationModal
        isOpen={Boolean(deactivating)}
        onClose={() => { if (!deactivatingNow) setDeactivating(null); }}
        onConfirm={confirmDeactivate}
        title={t("deactivateTitle")}
        message={t("deactivateMessage")}
        confirmText={t("deactivate")}
        cancelText={common("actions.cancel")}
        isLoading={deactivatingNow}
      />
    </section>
  );
}
