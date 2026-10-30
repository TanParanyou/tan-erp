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
import { useStockMutations, useWarehouseList, useWarehouseLocations } from "../api/inventory-queries";
import { inventoryErrorCode } from "../inventory-status";

export type StockDocumentKind = "issue" | "transfer" | "adjustment" | "return";

interface DraftLine {
  key: string;
  itemId: string;
  label: string;
  value: string;
  unitCost: string;
  lotNumber: string;
  serialNumbers: string;
}

interface StockDocumentModalProps {
  kind: StockDocumentKind | null;
  onClose: () => void;
  defaultWarehouseId?: string;
}

const PAGE_LIMIT = 100;

/** One dialog for the multi-line stock postings: issue, transfer, adjustment, and return. */
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

  const [warehouseId, setWarehouseId] = useState(defaultWarehouseId ?? "");
  const [locationId, setLocationId] = useState("");
  const [toWarehouseId, setToWarehouseId] = useState("");
  const [toLocationId, setToLocationId] = useState("");
  const [projectId, setProjectId] = useState("");
  const [reason, setReason] = useState("");
  const [lines, setLines] = useState<DraftLine[]>([]);
  const [pickerKey, setPickerKey] = useState(0);
  const [error, setError] = useState<string | null>(null);
  // One key per dialog session so a retried submit replays instead of posting twice.
  const keyRef = useRef(crypto.randomUUID());

  const isReturn = kind === "return";
  const isAdjustment = kind === "adjustment";
  const isTransfer = kind === "transfer";
  const isIssue = kind === "issue";

  const projects = useProjectList(
    { page: 1, pageSize: PAGE_LIMIT },
    canReadProjects && (isIssue || isReturn)
  );

  const fromLocations = useWarehouseLocations(warehouseId, Boolean(warehouseId));
  const targetWarehouseId = toWarehouseId || (isTransfer ? warehouseId : "");
  const toLocations = useWarehouseLocations(targetWarehouseId, Boolean(targetWarehouseId));

  const isBusy =
    mutations.issue.isPending ||
    mutations.transfer.isPending ||
    mutations.adjust.isPending ||
    mutations.returnStock.isPending;

  const warehouseOptions = (warehouses.data?.items ?? []).map((w) => ({
    value: w.id ?? "",
    label: `${w.code ?? "-"} · ${w.name ?? "-"}`,
  }));

  const fromLocationOptions = [
    { value: "", label: t("defaultLocation") },
    ...(fromLocations.data?.items ?? []).map((loc) => ({
      value: loc.id ?? "",
      label: `${loc.code ?? "-"} · ${loc.name ?? "-"}${loc.isDefault ? ` (${t("defaultBadge")})` : ""}`,
    })),
  ];

  const toLocationOptions = [
    { value: "", label: t("defaultLocation") },
    ...(toLocations.data?.items ?? []).map((loc) => ({
      value: loc.id ?? "",
      label: `${loc.code ?? "-"} · ${loc.name ?? "-"}${loc.isDefault ? ` (${t("defaultBadge")})` : ""}`,
    })),
  ];

  function describe(err: unknown): string {
    const code = err instanceof ApiError ? inventoryErrorCode(err.code) : null;
    return code ? tErrors(code) : tErrors("failed");
  }

  function parseSerialNumbers(raw: string): string[] | null {
    const parts = raw
      .split(/[\n,]+/)
      .map((s) => s.trim())
      .filter(Boolean);
    return parts.length > 0 ? parts : null;
  }

  async function submit(): Promise<void> {
    setError(null);
    if (!kind) return;
    if (!warehouseId || (isTransfer && !toWarehouseId && !toLocationId)) {
      setError(t("warehouseRequired"));
      return;
    }

    if (isTransfer) {
      const effectiveToWh = toWarehouseId || warehouseId;
      if (warehouseId === effectiveToWh && (!locationId || !toLocationId || locationId === toLocationId)) {
        setError(tErrors("INVENTORY_TRANSFER_SAME_WAREHOUSE"));
        return;
      }
    }

    if ((isAdjustment || isReturn) && !reason.trim()) {
      setError(t("reasonRequired"));
      return;
    }

    const parsed = lines.map((line) => ({
      line,
      value: Number(line.value),
      cost: line.unitCost.trim() === "" ? null : Number(line.unitCost),
      lot: line.lotNumber.trim() || null,
      serials: parseSerialNumbers(line.serialNumbers),
    }));

    const valueOk = (value: number): boolean =>
      Number.isFinite(value) && (isAdjustment ? value >= 0 : value > 0);
    if (
      parsed.length === 0 ||
      parsed.some((p) => !valueOk(p.value) || (p.cost !== null && !(p.cost >= 0)))
    ) {
      setError(t("linesInvalid"));
      return;
    }

    try {
      if (isIssue) {
        await mutations.issue.mutateAsync({
          payload: {
            warehouseId,
            ...(locationId ? { locationId } : {}),
            projectId: projectId || null,
            reason: reason.trim() || null,
            lines: parsed.map((p) => ({
              itemId: p.line.itemId,
              quantity: p.value,
              ...(locationId ? { locationId } : {}),
              ...(p.lot ? { lotNumber: p.lot } : {}),
              ...(p.serials ? { serialNumbers: p.serials } : {}),
            })),
          },
          idempotencyKey: keyRef.current,
        });
      } else if (isTransfer) {
        await mutations.transfer.mutateAsync({
          payload: {
            fromWarehouseId: warehouseId,
            toWarehouseId: toWarehouseId || warehouseId,
            ...(locationId ? { fromLocationId: locationId } : {}),
            ...(toLocationId ? { toLocationId } : {}),
            reason: reason.trim() || null,
            lines: parsed.map((p) => ({
              itemId: p.line.itemId,
              quantity: p.value,
              ...(locationId ? { locationId } : {}),
              ...(toLocationId ? { toLocationId } : {}),
              ...(p.lot ? { lotNumber: p.lot } : {}),
              ...(p.serials ? { serialNumbers: p.serials } : {}),
            })),
          },
          idempotencyKey: keyRef.current,
        });
      } else if (isAdjustment) {
        await mutations.adjust.mutateAsync({
          payload: {
            warehouseId,
            ...(locationId ? { locationId } : {}),
            reason: reason.trim(),
            lines: parsed.map((p) => ({
              itemId: p.line.itemId,
              countedQuantity: p.value,
              ...(p.cost !== null ? { unitCost: p.cost } : {}),
              ...(locationId ? { locationId } : {}),
              ...(p.lot ? { lotNumber: p.lot } : {}),
              ...(p.serials ? { serialNumbers: p.serials } : {}),
            })),
          },
          idempotencyKey: keyRef.current,
        });
      } else if (isReturn) {
        await mutations.returnStock.mutateAsync({
          payload: {
            warehouseId,
            ...(locationId ? { locationId } : {}),
            projectId: projectId || null,
            reason: reason.trim(),
            lines: parsed.map((p) => ({
              itemId: p.line.itemId,
              quantity: p.value,
              ...(p.cost !== null ? { unitCost: p.cost } : {}),
              ...(locationId ? { locationId } : {}),
              ...(p.lot ? { lotNumber: p.lot } : {}),
              ...(p.serials ? { serialNumbers: p.serials } : {}),
            })),
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
    setLocationId("");
    setToWarehouseId("");
    setToLocationId("");
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
      size="xl"
      closeDisabled={isBusy}
      closeOnOverlayClick={!isBusy}
      closeOnEscape={!isBusy}
      footer={(
        <div className="flex justify-end gap-3">
          <Button
            type="button"
            variant="outline"
            className="min-h-11"
            disabled={isBusy}
            onClick={close}
          >
            {tCommon("actions.cancel")}
          </Button>
          <Button
            type="button"
            variant="primary"
            className="min-h-11"
            isLoading={isBusy}
            disabled={isBusy}
            onClick={() => void submit()}
          >
            {t("post")}
          </Button>
        </div>
      )}
    >
      <div className="space-y-4">
        {error && (
          <Alert variant="danger" onClose={() => setError(null)}>
            {error}
          </Alert>
        )}

        <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
          <Select
            label={isTransfer ? t("fromWarehouse") : t("warehouse")}
            required
            value={warehouseId}
            placeholder={t("selectWarehouse")}
            disabled={isBusy}
            options={warehouseOptions}
            onChange={(event) => {
              setWarehouseId(event.target.value);
              setLocationId("");
            }}
          />

          <Select
            label={isTransfer ? t("fromLocation") : t("location")}
            value={locationId}
            placeholder={t("selectLocation")}
            disabled={isBusy || !warehouseId}
            options={fromLocationOptions}
            onChange={(event) => setLocationId(event.target.value)}
          />

          {isTransfer && (
            <>
              <Select
                label={t("toWarehouse")}
                value={toWarehouseId}
                placeholder={t("selectWarehouse")}
                disabled={isBusy}
                options={warehouseOptions}
                onChange={(event) => {
                  setToWarehouseId(event.target.value);
                  setToLocationId("");
                }}
              />
              <Select
                label={t("toLocation")}
                value={toLocationId}
                placeholder={t("selectLocation")}
                disabled={isBusy || (!toWarehouseId && !warehouseId)}
                options={toLocationOptions}
                onChange={(event) => setToLocationId(event.target.value)}
              />
            </>
          )}

          {(isIssue || isReturn) && canReadProjects && (
            <Select
              label={t("project")}
              value={projectId}
              placeholder={t("noProject")}
              disabled={isBusy}
              options={(projects.data?.items ?? []).map((p) => ({
                value: p.id ?? "",
                label: `${p.code ?? "-"} · ${p.name ?? "-"}`,
              }))}
              onChange={(event) => setProjectId(event.target.value)}
            />
          )}

          <div className={(isIssue || isReturn) && canReadProjects ? "sm:col-span-2" : isTransfer ? "sm:col-span-2" : ""}>
            <Input
              label={t("reason")}
              placeholder={t("reasonPlaceholder")}
              required={isAdjustment || isReturn}
              maxLength={500}
              value={reason}
              disabled={isBusy}
              onChange={(event) => setReason(event.target.value)}
            />
          </div>
        </div>

        <div className="space-y-2">
          <label className="text-sm font-semibold text-erp-navy">{t("linesTitle")}</label>
          <ul className="space-y-2 max-h-80 overflow-y-auto">
            {lines.map((line) => (
              <li
                key={line.key}
                className="grid grid-cols-1 gap-2 border border-erp-border bg-erp-surface-subtle p-3 sm:grid-cols-[1fr_120px_130px_130px_auto] sm:items-end"
              >
                <div>
                  <div className="text-sm font-medium text-erp-text-main">{line.label}</div>
                  <Input
                    className="mt-1"
                    label={t("serialNumbers")}
                    placeholder={t("serialNumbersPlaceholder")}
                    value={line.serialNumbers}
                    disabled={isBusy}
                    onChange={(event) =>
                      setLines((current) =>
                        current.map((l) =>
                          l.key === line.key ? { ...l, serialNumbers: event.target.value } : l
                        )
                      )
                    }
                  />
                </div>
                <Input
                  type="number"
                  min={0}
                  step="0.0001"
                  label={isAdjustment ? t("countedQuantity") : t("quantity")}
                  placeholder={isAdjustment ? t("countedQuantity") : t("quantity")}
                  value={line.value}
                  disabled={isBusy}
                  onChange={(event) =>
                    setLines((current) =>
                      current.map((l) => (l.key === line.key ? { ...l, value: event.target.value } : l))
                    )
                  }
                />
                <Input
                  type="number"
                  min={0}
                  step="0.01"
                  label={t("unitCost")}
                  placeholder={t("unitCost")}
                  value={line.unitCost}
                  disabled={isBusy || (!isAdjustment && !isReturn)}
                  onChange={(event) =>
                    setLines((current) =>
                      current.map((l) =>
                        l.key === line.key ? { ...l, unitCost: event.target.value } : l
                      )
                    )
                  }
                />
                <Input
                  label={t("lotNumber")}
                  placeholder={t("lotNumberPlaceholder")}
                  value={line.lotNumber}
                  disabled={isBusy}
                  maxLength={100}
                  onChange={(event) =>
                    setLines((current) =>
                      current.map((l) =>
                        l.key === line.key ? { ...l, lotNumber: event.target.value } : l
                      )
                    )
                  }
                />
                <Button
                  type="button"
                  variant="outline"
                  size="sm"
                  className="min-h-11"
                  disabled={isBusy}
                  onClick={() => setLines((current) => current.filter((l) => l.key !== line.key))}
                >
                  {t("removeLine")}
                </Button>
              </li>
            ))}
            {lines.length === 0 && (
              <li className="text-xs text-erp-text-muted py-2">{t("linesEmpty")}</li>
            )}
          </ul>
        </div>

        <ItemAutocomplete
          key={pickerKey}
          label={t("addItem")}
          disabled={isBusy}
          onChange={(itemId, item) => {
            if (itemId && !lines.some((l) => l.itemId === itemId)) {
              const name = (locale === "en" ? item?.name?.english : item?.name?.thai) ?? "-";
              setLines((current) => [
                ...current,
                {
                  key: crypto.randomUUID(),
                  itemId,
                  label: `${item?.code ?? "-"} · ${name} (${item?.baseUnit?.code ?? "-"})`,
                  value: isAdjustment ? "0" : "1",
                  unitCost: "",
                  lotNumber: "",
                  serialNumbers: "",
                },
              ]);
            }
            setPickerKey((k) => k + 1);
          }}
        />
        {isAdjustment && <p className="text-xs text-erp-text-muted">{t("adjustmentHint")}</p>}
      </div>
    </Modal>
  );
}
