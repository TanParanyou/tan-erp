"use client";

import { useRef, useState } from "react";
import { useLocale, useTranslations } from "next-intl";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Modal } from "@/components/ui/Modal";
import { Select } from "@/components/ui/Select";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import { ItemAutocomplete } from "@/features/item-master/components/item-autocomplete";
import { useProjectList } from "@/features/projects/api/project-queries";
import { useStockMutations, useWarehouseList } from "../api/inventory-queries";
import { inventoryErrorCode } from "../inventory-status";

export type StockDocumentKind = "issue" | "transfer" | "adjustment";

interface DraftLine {
  key: string;
  itemId: string;
  label: string;
  value: string;
  unitCost: string;
}

interface StockDocumentModalProps {
  kind: StockDocumentKind | null;
  onClose: () => void;
  defaultWarehouseId?: string;
}

const PAGE_LIMIT = 100;

/** One dialog for the three multi-line stock postings; the lines mean quantity (issue/transfer) or counted quantity (adjustment). */
export function StockDocumentModal({ kind, onClose, defaultWarehouseId }: StockDocumentModalProps) {
  const t = useTranslations("inventory.documents");
  const tErrors = useTranslations("inventory.errors");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const { toast } = useToast();
  const { selectedMembership } = useSelectedMembership();
  const canReadProjects = can(selectedMembership, PERMISSIONS.PROJECTS_READ);
  const mutations = useStockMutations();
  const warehouses = useWarehouseList({ status: "active", page: 1, pageSize: PAGE_LIMIT }, kind !== null);
  const projects = useProjectList({ page: 1, pageSize: PAGE_LIMIT }, canReadProjects && kind === "issue");

  const [warehouseId, setWarehouseId] = useState(defaultWarehouseId ?? "");
  const [toWarehouseId, setToWarehouseId] = useState("");
  const [projectId, setProjectId] = useState("");
  const [reason, setReason] = useState("");
  const [lines, setLines] = useState<DraftLine[]>([]);
  const [pickerKey, setPickerKey] = useState(0);
  const [error, setError] = useState<string | null>(null);
  // One key per dialog session so a retried submit replays instead of posting twice.
  const keyRef = useRef(crypto.randomUUID());

  const isBusy = mutations.issue.isPending || mutations.transfer.isPending || mutations.adjust.isPending;
  const warehouseOptions = (warehouses.data?.items ?? []).map((w) => ({ value: w.id ?? "", label: `${w.code ?? "-"} · ${w.name ?? "-"}` }));
  const isAdjustment = kind === "adjustment";

  function describe(err: unknown): string {
    const code = err instanceof ApiError ? inventoryErrorCode(err.code) : null;
    return code ? tErrors(code) : tErrors("failed");
  }

  async function submit(): Promise<void> {
    setError(null);
    if (!kind) return;
    if (!warehouseId || (kind === "transfer" && !toWarehouseId)) {
      setError(t("warehouseRequired"));
      return;
    }

    if (kind === "transfer" && warehouseId === toWarehouseId) {
      setError(tErrors("INVENTORY_TRANSFER_SAME_WAREHOUSE"));
      return;
    }

    if (isAdjustment && !reason.trim()) {
      setError(t("reasonRequired"));
      return;
    }

    const parsed = lines.map((line) => ({ line, value: Number(line.value), cost: line.unitCost.trim() === "" ? null : Number(line.unitCost) }));
    const valueOk = (value: number): boolean => Number.isFinite(value) && (isAdjustment ? value >= 0 : value > 0);
    if (parsed.length === 0 || parsed.some((p) => !valueOk(p.value) || (p.cost !== null && !(p.cost >= 0)))) {
      setError(t("linesInvalid"));
      return;
    }

    try {
      if (kind === "issue") {
        await mutations.issue.mutateAsync({
          payload: {
            warehouseId,
            projectId: projectId || null,
            reason: reason.trim() || null,
            lines: parsed.map((p) => ({ itemId: p.line.itemId, quantity: p.value })),
          },
          idempotencyKey: keyRef.current,
        });
      } else if (kind === "transfer") {
        await mutations.transfer.mutateAsync({
          payload: {
            fromWarehouseId: warehouseId,
            toWarehouseId,
            reason: reason.trim() || null,
            lines: parsed.map((p) => ({ itemId: p.line.itemId, quantity: p.value })),
          },
          idempotencyKey: keyRef.current,
        });
      } else {
        await mutations.adjust.mutateAsync({
          payload: {
            warehouseId,
            reason: reason.trim(),
            lines: parsed.map((p) => ({ itemId: p.line.itemId, countedQuantity: p.value, unitCost: p.cost })),
          },
          idempotencyKey: keyRef.current,
        });
      }

      toast.success(t(`posted.${kind}`));
      reset();
      onClose();
    } catch (err: unknown) {
      setError(describe(err));
    }
  }

  function reset(): void {
    keyRef.current = crypto.randomUUID();
    setToWarehouseId("");
    setProjectId("");
    setReason("");
    setLines([]);
    setPickerKey((k) => k + 1);
    setError(null);
  }

  function close(): void {
    if (isBusy) return;
    reset();
    onClose();
  }

  return (
    <Modal
      isOpen={kind !== null}
      onClose={close}
      title={kind ? t(`titles.${kind}`) : undefined}
      description={kind ? t(`descriptions.${kind}`) : undefined}
      size="lg"
      closeDisabled={isBusy}
      closeOnOverlayClick={!isBusy}
      closeOnEscape={!isBusy}
      footer={(
        <div className="flex justify-end gap-3">
          <Button type="button" variant="outline" className="min-h-11" disabled={isBusy} onClick={close}>{tCommon("actions.cancel")}</Button>
          <Button type="button" variant="primary" className="min-h-11" isLoading={isBusy} disabled={isBusy} onClick={() => void submit()}>{t("post")}</Button>
        </div>
      )}
    >
      <div className="space-y-4">
        {error && <Alert variant="danger" onClose={() => setError(null)}>{error}</Alert>}
        <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
          <Select label={kind === "transfer" ? t("fromWarehouse") : t("warehouse")} required value={warehouseId} placeholder={t("selectWarehouse")} disabled={isBusy} options={warehouseOptions} onChange={(event) => setWarehouseId(event.target.value)} />
          {kind === "transfer" && (
            <Select label={t("toWarehouse")} required value={toWarehouseId} placeholder={t("selectWarehouse")} disabled={isBusy} options={warehouseOptions} onChange={(event) => setToWarehouseId(event.target.value)} />
          )}
          {kind === "issue" && canReadProjects && (
            <Select
              label={t("project")}
              value={projectId}
              placeholder={t("noProject")}
              disabled={isBusy}
              options={(projects.data?.items ?? []).map((p) => ({ value: p.id ?? "", label: `${p.code ?? "-"} · ${p.name ?? "-"}` }))}
              onChange={(event) => setProjectId(event.target.value)}
            />
          )}
          <Input label={t("reason")} required={isAdjustment} maxLength={500} value={reason} disabled={isBusy} onChange={(event) => setReason(event.target.value)} />
        </div>

        <ul className="space-y-2">
          {lines.map((line) => (
            <li key={line.key} className={`grid grid-cols-1 gap-2 border border-erp-border bg-erp-surface-subtle p-3 ${isAdjustment ? "sm:grid-cols-[1fr_130px_130px_auto]" : "sm:grid-cols-[1fr_140px_auto]"} sm:items-end`}>
              <div className="text-sm font-medium text-erp-text-main">{line.label}</div>
              <Input type="number" min={0} step="0.0001" aria-label={isAdjustment ? t("countedQuantity") : t("quantity")} placeholder={isAdjustment ? t("countedQuantity") : t("quantity")} value={line.value} disabled={isBusy} onChange={(event) => setLines((current) => current.map((l) => (l.key === line.key ? { ...l, value: event.target.value } : l)))} />
              {isAdjustment && (
                <Input type="number" min={0} step="0.01" aria-label={t("unitCost")} placeholder={t("unitCost")} value={line.unitCost} disabled={isBusy} onChange={(event) => setLines((current) => current.map((l) => (l.key === line.key ? { ...l, unitCost: event.target.value } : l)))} />
              )}
              <Button type="button" variant="outline" size="sm" className="min-h-11" disabled={isBusy} onClick={() => setLines((current) => current.filter((l) => l.key !== line.key))}>{t("removeLine")}</Button>
            </li>
          ))}
          {lines.length === 0 && <li className="text-xs text-erp-text-muted">{t("linesEmpty")}</li>}
        </ul>

        <ItemAutocomplete
          key={pickerKey}
          label={t("addItem")}
          disabled={isBusy}
          onChange={(itemId, item) => {
            if (itemId && !lines.some((l) => l.itemId === itemId)) {
              const name = (locale === "en" ? item?.name?.english : item?.name?.thai) ?? "-";
              setLines((current) => [...current, { key: crypto.randomUUID(), itemId, label: `${item?.code ?? "-"} · ${name} (${item?.baseUnit?.code ?? "-"})`, value: isAdjustment ? "0" : "1", unitCost: "" }]);
            }
            setPickerKey((k) => k + 1);
          }}
        />
        {isAdjustment && <p className="text-xs text-erp-text-muted">{t("adjustmentHint")}</p>}
      </div>
    </Modal>
  );
}
