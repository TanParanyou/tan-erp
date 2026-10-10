"use client";

import { useRef, useState } from "react";
import { useRouter } from "next/navigation";
import { useLocale, useTranslations } from "next-intl";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { Select } from "@/components/ui/Select";
import { PageHeader } from "@/components/layout/PageHeader";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import { formatCurrency } from "@/lib/formatters/formatters";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import { ItemAutocomplete } from "@/features/item-master/components/item-autocomplete";
import { useProjectList } from "@/features/projects/api/project-queries";
import type { PurchaseOrderRequest, PurchaseOrderResponse } from "@/lib/api/api-client";
import { usePurchaseOrder, usePurchaseOrderMutations, useSupplierList } from "../api/procurement-queries";
import { procurementErrorCode } from "../procurement-status";

interface DraftLine {
  key: string;
  itemId: string;
  label: string;
  quantity: string;
  unitPrice: string;
}

interface PurchaseOrderEditorProps {
  /** Existing draft to edit; omit to create a new order. */
  purchaseOrderId?: string;
}

const PAGE_LIMIT = 100;

function fromOrder(order: PurchaseOrderResponse): DraftLine[] {
  return (order.lines ?? []).map((line) => ({
    key: line.id ?? crypto.randomUUID(),
    itemId: line.itemId ?? "",
    label: `${line.itemCode ?? "-"} · ${line.itemNameTh ?? "-"} (${line.unitCode ?? "-"})`,
    quantity: String(line.quantity ?? 1),
    unitPrice: String(line.unitPrice ?? 0),
  }));
}

export function PurchaseOrderEditor({ purchaseOrderId }: PurchaseOrderEditorProps) {
  const orderQuery = usePurchaseOrder(purchaseOrderId);
  const t = useTranslations("procurement.orders");
  if (purchaseOrderId && orderQuery.isLoading) {
    return <div className="py-16 flex justify-center" role="status"><MonoSpinner size="md" /></div>;
  }

  if (purchaseOrderId && (orderQuery.isError || !orderQuery.data)) {
    return <Alert variant="danger">{t("loadError")}</Alert>;
  }

  if (purchaseOrderId && orderQuery.data?.status !== "draft") {
    return <Alert variant="warning">{t("onlyDraftEditable")}</Alert>;
  }

  // Key by the loaded version so the form state is rebuilt from the server data, never patched in an effect.
  return <EditorForm key={orderQuery.data?.rowVersion ?? "new"} order={orderQuery.data} />;
}

function EditorForm({ order }: { order?: PurchaseOrderResponse }) {
  const t = useTranslations("procurement.orders");
  const tErrors = useTranslations("procurement.errors");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const router = useRouter();
  const { toast } = useToast();
  const { selectedMembership } = useSelectedMembership();
  const canReadProjects = can(selectedMembership, PERMISSIONS.PROJECTS_READ);
  const mutations = usePurchaseOrderMutations();
  const suppliers = useSupplierList({ status: "active", page: 1, pageSize: PAGE_LIMIT });
  const projects = useProjectList({ page: 1, pageSize: PAGE_LIMIT }, canReadProjects);

  const [supplierId, setSupplierId] = useState(order?.supplier?.id ?? "");
  const [projectId, setProjectId] = useState(order?.project?.id ?? "");
  const [expectedDelivery, setExpectedDelivery] = useState(order?.expectedDeliveryDate ?? "");
  const [note, setNote] = useState(order?.note ?? "");
  const [lines, setLines] = useState<DraftLine[]>(order ? fromOrder(order) : []);
  const [pickerKey, setPickerKey] = useState(0);
  const [error, setError] = useState<string | null>(null);
  // One key per form session so a retried create replays instead of creating a second order.
  const createKeyRef = useRef(crypto.randomUUID());

  const isBusy = mutations.create.isPending || mutations.update.isPending;
  const total = lines.reduce((sum, line) => sum + (Number(line.quantity) || 0) * (Number(line.unitPrice) || 0), 0);

  function updateLine(key: string, patch: Partial<DraftLine>): void {
    setLines((current) => current.map((line) => (line.key === key ? { ...line, ...patch } : line)));
  }

  async function save(): Promise<void> {
    setError(null);
    if (!supplierId) {
      setError(t("supplierRequired"));
      return;
    }

    const parsed = lines.map((line) => ({ itemId: line.itemId, quantity: Number(line.quantity), unitPrice: Number(line.unitPrice) }));
    if (parsed.length === 0 || parsed.some((l) => !(l.quantity > 0) || !(l.unitPrice >= 0) || !Number.isFinite(l.quantity) || !Number.isFinite(l.unitPrice))) {
      setError(t("linesInvalid"));
      return;
    }

    const payload: PurchaseOrderRequest = {
      supplierId,
      projectId: projectId || null,
      expectedDeliveryDate: expectedDelivery || null,
      note: note.trim() || null,
      lines: parsed,
    };

    try {
      const saved = order
        ? await mutations.update.mutateAsync({ id: order.id ?? "", rowVersion: order.rowVersion ?? "", payload })
        : await mutations.create.mutateAsync({ payload, idempotencyKey: createKeyRef.current });
      toast.success(order ? t("updated") : t("created"));
      router.push(`/${locale}/procurement/purchase-orders/${saved.id}`);
    } catch (err: unknown) {
      const code = err instanceof ApiError ? procurementErrorCode(err.code) : null;
      setError(code ? tErrors(code) : tErrors("failed"));
    }
  }

  return (
    <section className="space-y-5">
      <PageHeader
        title={order ? t("editTitle", { number: order.number ?? "-" }) : t("createTitle")}
        subtitle={t("editorSubtitle")}
        breadcrumbs={[{ label: t("title"), href: `/${locale}/procurement/purchase-orders` }, { label: order?.number ?? t("create") }]}
      />
      {error && <Alert variant="danger" onClose={() => setError(null)}>{error}</Alert>}

      <div className="erp-card p-5 grid grid-cols-1 gap-4 md:grid-cols-2">
        <Select
          label={t("supplier")}
          required
          value={supplierId}
          placeholder={t("selectSupplier")}
          disabled={isBusy || Boolean(order)}
          options={(suppliers.data?.items ?? []).map((s) => ({ value: s.id ?? "", label: `${s.code ?? "-"} · ${s.nameTh ?? "-"}` }))}
          onChange={(event) => setSupplierId(event.target.value)}
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
        <Input type="date" label={t("expectedDelivery")} value={expectedDelivery} disabled={isBusy} onChange={(event) => setExpectedDelivery(event.target.value)} />
        <Input label={t("note")} value={note} maxLength={500} disabled={isBusy} placeholder={t("notePlaceholder")} onChange={(event) => setNote(event.target.value)} />
      </div>

      <div className="erp-card p-5 space-y-4">
        <h3 className="text-sm font-bold text-erp-navy">{t("linesTitle")}</h3>
        <ul className="space-y-2">
          {lines.map((line) => (
            <li key={line.key} className="grid grid-cols-1 gap-2 border border-erp-border bg-erp-surface-subtle p-3 sm:grid-cols-[1fr_130px_150px_auto] sm:items-end">
              <div className="text-sm font-medium text-erp-text-main">{line.label}</div>
              <Input type="number" min={0} step="0.0001" aria-label={t("quantity")} placeholder={t("quantity")} value={line.quantity} disabled={isBusy} onChange={(event) => updateLine(line.key, { quantity: event.target.value })} />
              <Input type="number" min={0} step="0.01" aria-label={t("unitPrice")} placeholder={t("unitPrice")} value={line.unitPrice} disabled={isBusy} onChange={(event) => updateLine(line.key, { unitPrice: event.target.value })} />
              <Button type="button" variant="outline" size="sm" className="min-h-11" disabled={isBusy} onClick={() => setLines((current) => current.filter((l) => l.key !== line.key))}>{t("removeLine")}</Button>
            </li>
          ))}
          {lines.length === 0 && <li className="text-xs text-erp-text-muted">{t("linesEmpty")}</li>}
        </ul>

        <ItemAutocomplete
          key={pickerKey}
          label={t("addItem")}
          purchasableOnly
          disabled={isBusy}
          onChange={(itemId, item) => {
            if (!itemId || lines.some((l) => l.itemId === itemId)) {
              setPickerKey((k) => k + 1);
              return;
            }

            const name = (locale === "en" ? item?.name?.english : item?.name?.thai) ?? "-";
            setLines((current) => [...current, {
              key: crypto.randomUUID(),
              itemId,
              label: `${item?.code ?? "-"} · ${name} (${item?.baseUnit?.code ?? "-"})`,
              quantity: "1",
              unitPrice: "0",
            }]);
            setPickerKey((k) => k + 1);
          }}
        />
      </div>

      <div className="flex flex-wrap items-center justify-between gap-3">
        <span className="font-mono text-sm font-semibold text-erp-navy">{t("draftTotal", { amount: formatCurrency(total, "THB", locale) })}</span>
        <div className="flex gap-3">
          <Button type="button" variant="outline" className="min-h-11" disabled={isBusy} onClick={() => router.back()}>{tCommon("actions.cancel")}</Button>
          <Button type="button" variant="primary" className="min-h-11" isLoading={isBusy} disabled={isBusy} onClick={() => void save()}>{tCommon("actions.save")}</Button>
        </div>
      </div>
    </section>
  );
}
