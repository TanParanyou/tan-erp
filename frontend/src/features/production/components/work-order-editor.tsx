"use client";

import { useRef, useState } from "react";
import { useRouter } from "next/navigation";
import { useLocale, useTranslations } from "next-intl";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Select } from "@/components/ui/Select";
import { PageHeader } from "@/components/layout/PageHeader";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import { ItemAutocomplete } from "@/features/item-master/components/item-autocomplete";
import { useProjectList } from "@/features/projects/api/project-queries";
import { useWarehouseList } from "@/features/inventory/api/inventory-queries";
import { useWorkOrderMutations } from "../api/production-queries";
import { productionErrorCode } from "../production-status";

const PAGE_LIMIT = 100;

export function WorkOrderEditor() {
  const t = useTranslations("production.workOrders");
  const tErrors = useTranslations("production.errors");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const router = useRouter();
  const { toast } = useToast();
  const { selectedMembership } = useSelectedMembership();
  const canReadProjects = can(selectedMembership, PERMISSIONS.PROJECTS_READ);
  const mutations = useWorkOrderMutations();
  const warehouses = useWarehouseList({ status: "active", page: 1, pageSize: PAGE_LIMIT });
  const projects = useProjectList({ page: 1, pageSize: PAGE_LIMIT }, canReadProjects);

  const [itemId, setItemId] = useState("");
  const [warehouseId, setWarehouseId] = useState("");
  const [projectId, setProjectId] = useState("");
  const [quantity, setQuantity] = useState("1");
  const [note, setNote] = useState("");
  const [error, setError] = useState<string | null>(null);
  // One key per form session so a retried create replays instead of creating a second order.
  const createKeyRef = useRef(crypto.randomUUID());
  const isBusy = mutations.create.isPending;

  async function save(): Promise<void> {
    setError(null);
    const planned = Number(quantity);
    if (!itemId || !warehouseId || !(planned > 0)) {
      setError(t("fieldsInvalid"));
      return;
    }

    try {
      const saved = await mutations.create.mutateAsync({
        payload: { itemId, warehouseId, projectId: projectId || null, plannedQuantity: planned, note: note.trim() || null },
        idempotencyKey: createKeyRef.current,
      });
      toast.success(t("created"));
      router.push(`/${locale}/production/work-orders/${saved.id}`);
    } catch (err: unknown) {
      const code = err instanceof ApiError ? productionErrorCode(err.code) : null;
      setError(code ? tErrors(code) : tErrors("failed"));
    }
  }

  return (
    <section className="space-y-5">
      <PageHeader
        title={t("createTitle")}
        subtitle={t("editorSubtitle")}
        breadcrumbs={[{ label: t("title"), href: `/${locale}/production/work-orders` }, { label: t("create") }]}
      />
      {error && <Alert variant="danger" onClose={() => setError(null)}>{error}</Alert>}
      <div className="erp-card p-5 grid grid-cols-1 gap-4 md:grid-cols-2">
        <ItemAutocomplete label={t("item")} required producibleOnly stockableOnly disabled={isBusy} value={itemId} onChange={(id) => setItemId(id)} />
        <Select
          label={t("warehouse")}
          required
          value={warehouseId}
          placeholder={t("selectWarehouse")}
          disabled={isBusy}
          options={(warehouses.data?.items ?? []).map((w) => ({ value: w.id ?? "", label: `${w.code ?? "-"} · ${w.name ?? "-"}` }))}
          onChange={(event) => setWarehouseId(event.target.value)}
        />
        {canReadProjects && (
          <Select
            label={t("project")}
            value={projectId}
            placeholder={t("noProject")}
            disabled={isBusy}
            options={(projects.data?.items ?? []).map((p) => ({ value: p.id ?? "", label: `${p.code ?? "-"} · ${p.name ?? "-"}` }))}
            onChange={(event) => setProjectId(event.target.value)}
          />
        )}
        <Input type="number" min={0} step="0.0001" label={t("plannedQuantity")} value={quantity} disabled={isBusy} placeholder={t("plannedQuantityPlaceholder")} onChange={(event) => setQuantity(event.target.value)} />
        <Input label={t("note")} value={note} maxLength={500} disabled={isBusy} placeholder={t("notePlaceholder")} onChange={(event) => setNote(event.target.value)} />
      </div>
      <div className="flex justify-end gap-3">
        <Button type="button" variant="outline" className="min-h-11" disabled={isBusy} onClick={() => router.back()}>{tCommon("actions.cancel")}</Button>
        <Button type="button" variant="primary" className="min-h-11" isLoading={isBusy} disabled={isBusy} onClick={() => void save()}>{tCommon("actions.save")}</Button>
      </div>
    </section>
  );
}
