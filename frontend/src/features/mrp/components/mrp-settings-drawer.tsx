"use client";

import { useMemo, useRef, useState } from "react";
import { useLocale, useTranslations } from "next-intl";
import { Drawer } from "@/components/ui/Drawer";
import { DataTable, type Column } from "@/components/ui/DataTable";
import { Input } from "@/components/ui/Input";
import { Select } from "@/components/ui/Select";
import { Button } from "@/components/ui/Button";
import { Alert } from "@/components/ui/Alert";
import { EmptyState } from "@/components/ui/EmptyState";
import { ConfirmationModal } from "@/components/ui/ConfirmationModal";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import { formatDate, formatDateTime, formatNumber } from "@/lib/formatters/formatters";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import { ItemAutocomplete } from "@/features/item-master/components/item-autocomplete";
import { useWarehouseList } from "@/features/inventory/api/inventory-queries";
import type {
  MrpPlanningParameterResponse,
  WorkingCalendarHolidayResponse,
} from "@/lib/api/api-client";
import {
  useMrpCalendarHolidays,
  useMrpPlanningParameters,
  useMrpSettingsMutations,
} from "../api/mrp-queries";
import { mrpErrorCode } from "../mrp-status";

interface MrpSettingsDrawerProps {
  isOpen: boolean;
  onClose: () => void;
  canManage?: boolean;
}

type SettingsTab = "parameters" | "holidays";

export function MrpSettingsDrawer({ isOpen, onClose, canManage: propCanManage }: MrpSettingsDrawerProps) {
  const t = useTranslations("mrp.settings");
  const tErrors = useTranslations("mrp.errors");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const { toast } = useToast();
  const { selectedMembership } = useSelectedMembership();
  const canManage = propCanManage ?? can(selectedMembership, PERMISSIONS.MRP_RUN);

  const [activeTab, setActiveTab] = useState<SettingsTab>("parameters");

  // Queries
  const paramsQuery = useMrpPlanningParameters({ page: 1, pageSize: 100 }, isOpen && activeTab === "parameters");
  const holidaysQuery = useMrpCalendarHolidays(undefined, isOpen && activeTab === "holidays");
  const warehousesQuery = useWarehouseList({ status: "active", page: 1, pageSize: 100 }, isOpen && activeTab === "parameters");
  const mutations = useMrpSettingsMutations();

  // Tab 1: Parameter form state
  const [editingParamId, setEditingParamId] = useState<string | null>(null);
  const [paramItemId, setParamItemId] = useState("");
  const [paramWarehouseId, setParamWarehouseId] = useState("");
  const [safetyStock, setSafetyStock] = useState("0");
  const [minOrderQty, setMinOrderQty] = useState("0");
  const [lotMultiple, setLotMultiple] = useState("0");
  const [leadTimeDays, setLeadTimeDays] = useState("");
  const [paramError, setParamError] = useState<string | null>(null);
  const [deletingParam, setDeletingParam] = useState<MrpPlanningParameterResponse | null>(null);

  // Tab 2: Holiday form state
  const [holidayDate, setHolidayDate] = useState("");
  const [holidayName, setHolidayName] = useState("");
  const [holidayError, setHolidayError] = useState<string | null>(null);
  const [deletingHoliday, setDeletingHoliday] = useState<WorkingCalendarHolidayResponse | null>(null);
  const holidayKeyRef = useRef(crypto.randomUUID());

  const isBusy =
    mutations.upsertParameter.isPending ||
    mutations.deleteParameter.isPending ||
    mutations.createHoliday.isPending ||
    mutations.deleteHoliday.isPending;

  function describe(err: unknown): string {
    const code = err instanceof ApiError ? mrpErrorCode(err.code) : null;
    return code ? tErrors(code) : tErrors("failed");
  }

  function resetParamForm(): void {
    setEditingParamId(null);
    setParamItemId("");
    setParamWarehouseId("");
    setSafetyStock("0");
    setMinOrderQty("0");
    setLotMultiple("0");
    setLeadTimeDays("");
    setParamError(null);
  }

  function openEditParam(param: MrpPlanningParameterResponse): void {
    setEditingParamId(param.id ?? null);
    setParamItemId(param.item?.id ?? "");
    setParamWarehouseId(param.warehouse?.id ?? "");
    setSafetyStock(String(param.safetyStock ?? 0));
    setMinOrderQty(String(param.minOrderQuantity ?? 0));
    setLotMultiple(String(param.lotMultiple ?? 0));
    setLeadTimeDays(param.leadTimeDays != null ? String(param.leadTimeDays) : "");
    setParamError(null);
  }

  async function handleSaveParameter(e: React.FormEvent): Promise<void> {
    e.preventDefault();
    if (!paramItemId) {
      setParamError(t("itemRequired"));
      return;
    }

    const ss = Number(safetyStock);
    const moq = Number(minOrderQty);
    const lm = Number(lotMultiple);
    const lt = leadTimeDays ? Number(leadTimeDays) : null;

    if (Number.isNaN(ss) || ss < 0 || Number.isNaN(moq) || moq < 0 || Number.isNaN(lm) || lm < 0) {
      setParamError(t("quantitiesInvalid"));
      return;
    }

    if (lt != null && (Number.isNaN(lt) || !Number.isInteger(lt) || lt < 0 || lt > 365)) {
      setParamError(t("leadTimeInvalid"));
      return;
    }

    setParamError(null);
    try {
      await mutations.upsertParameter.mutateAsync({
        payload: {
          itemId: paramItemId,
          warehouseId: paramWarehouseId || null,
          safetyStock: ss,
          minOrderQuantity: moq,
          lotMultiple: lm,
          leadTimeDays: lt,
        },
      });
      toast.success(editingParamId ? t("paramUpdated") : t("paramCreated"));
      resetParamForm();
    } catch (err: unknown) {
      setParamError(describe(err));
    }
  }

  async function handleDeleteParameterConfirm(): Promise<void> {
    if (!deletingParam?.id) return;
    try {
      await mutations.deleteParameter.mutateAsync(deletingParam.id);
      toast.success(t("paramDeleted"));
      setDeletingParam(null);
      if (editingParamId === deletingParam.id) {
        resetParamForm();
      }
    } catch (err: unknown) {
      toast.error(describe(err));
    }
  }

  async function handleCreateHoliday(e: React.FormEvent): Promise<void> {
    e.preventDefault();
    if (!holidayDate) {
      setHolidayError(t("holidayDateRequired"));
      return;
    }
    if (!holidayName.trim()) {
      setHolidayError(t("holidayNameRequired"));
      return;
    }

    setHolidayError(null);
    try {
      await mutations.createHoliday.mutateAsync({
        payload: {
          holidayDate,
          name: holidayName.trim(),
        },
        idempotencyKey: holidayKeyRef.current,
      });
      holidayKeyRef.current = crypto.randomUUID();
      toast.success(t("holidayCreated"));
      setHolidayDate("");
      setHolidayName("");
    } catch (err: unknown) {
      setHolidayError(describe(err));
    }
  }

  async function handleDeleteHolidayConfirm(): Promise<void> {
    if (!deletingHoliday?.id) return;
    try {
      await mutations.deleteHoliday.mutateAsync(deletingHoliday.id);
      toast.success(t("holidayDeleted"));
      setDeletingHoliday(null);
    } catch (err: unknown) {
      toast.error(describe(err));
    }
  }

  const paramColumns: Column<MrpPlanningParameterResponse>[] = useMemo(
    () => [
      {
        id: "item",
        header: t("item"),
        className: "min-w-40",
        cell: (_v, row) => (
          <div>
            <span className="font-mono font-medium">{row.item?.code ?? "-"}</span>
            <div className="text-xs text-erp-text-muted">{row.item?.nameTh ?? "-"}</div>
          </div>
        ),
      },
      {
        id: "warehouse",
        header: t("warehouse"),
        className: "min-w-32",
        cell: (_v, row) =>
          row.warehouse ? (
            <span>{row.warehouse.code ?? "-"} · {row.warehouse.name ?? "-"}</span>
          ) : (
            <span className="text-erp-text-muted font-medium">{t("allWarehouses")}</span>
          ),
      },
      {
        id: "safetyStock",
        header: t("safetyStock"),
        className: "text-right font-mono",
        cell: (_v, row) => formatNumber(row.safetyStock),
      },
      {
        id: "moq",
        header: t("minOrderQuantity"),
        className: "text-right font-mono",
        cell: (_v, row) => formatNumber(row.minOrderQuantity),
      },
      {
        id: "lotMultiple",
        header: t("lotMultiple"),
        className: "text-right font-mono",
        cell: (_v, row) => formatNumber(row.lotMultiple),
      },
      {
        id: "leadTime",
        header: t("leadTimeDays"),
        className: "text-right font-mono",
        cell: (_v, row) => (row.leadTimeDays != null ? `${row.leadTimeDays} ${t("daysUnit")}` : "-"),
      },
      {
        id: "actions",
        header: t("actions"),
        className: "text-right",
        cell: (_v, row) =>
          canManage ? (
            <div className="flex items-center justify-end gap-1">
              <Button
                type="button"
                variant="outline"
                size="sm"
                className="min-h-11"
                disabled={isBusy}
                onClick={() => openEditParam(row)}
              >
                {t("edit")}
              </Button>
              <Button
                type="button"
                variant="outline"
                size="sm"
                className="min-h-11 text-erp-danger hover:bg-erp-danger/10"
                disabled={isBusy}
                onClick={() => setDeletingParam(row)}
              >
                {t("delete")}
              </Button>
            </div>
          ) : (
            <span className="text-erp-text-muted">-</span>
          ),
      },
    ],
    [canManage, isBusy, t]
  );

  const holidayColumns: Column<WorkingCalendarHolidayResponse>[] = useMemo(
    () => [
      {
        id: "date",
        header: t("holidayDate"),
        className: "min-w-32 font-mono font-medium",
        cell: (_v, row) => (row.holidayDate ? formatDate(row.holidayDate, locale) : "-"),
      },
      {
        id: "name",
        header: t("holidayName"),
        className: "min-w-48 font-medium",
        cell: (_v, row) => row.name ?? "-",
      },
      {
        id: "created",
        header: t("createdAt"),
        className: "text-erp-text-muted font-mono",
        cell: (_v, row) => (row.createdAtUtc ? formatDateTime(row.createdAtUtc, locale) : "-"),
      },
      {
        id: "actions",
        header: t("actions"),
        className: "text-right",
        cell: (_v, row) =>
          canManage ? (
            <Button
              type="button"
              variant="outline"
              size="sm"
              className="min-h-11 text-erp-danger hover:bg-erp-danger/10"
              disabled={isBusy}
              onClick={() => setDeletingHoliday(row)}
            >
              {t("delete")}
            </Button>
          ) : (
            <span className="text-erp-text-muted">-</span>
          ),
      },
    ],
    [canManage, isBusy, locale, t]
  );

  return (
    <Drawer
      isOpen={isOpen}
      onClose={onClose}
      title={t("title")}
      description={t("subtitle")}
      size="xl"
    >
      <div className="space-y-5 py-2">
        {/* Tab navigation */}
        <div className="flex border-b border-erp-border gap-2">
          <button
            type="button"
            className={`px-4 py-2 text-sm font-medium border-b-2 -mb-px transition-colors ${
              activeTab === "parameters"
                ? "border-erp-navy text-erp-navy font-semibold"
                : "border-transparent text-erp-text-muted hover:text-erp-text-main"
            }`}
            onClick={() => setActiveTab("parameters")}
          >
            {t("tabs.parameters")}
          </button>
          <button
            type="button"
            className={`px-4 py-2 text-sm font-medium border-b-2 -mb-px transition-colors ${
              activeTab === "holidays"
                ? "border-erp-navy text-erp-navy font-semibold"
                : "border-transparent text-erp-text-muted hover:text-erp-text-main"
            }`}
            onClick={() => setActiveTab("holidays")}
          >
            {t("tabs.holidays")}
          </button>
        </div>

        {/* Tab 1: Planning Parameters */}
        {activeTab === "parameters" && (
          <div className="space-y-5">
            {canManage && (
              <form onSubmit={(e) => void handleSaveParameter(e)} className="border border-erp-border bg-erp-surface-subtle p-4 space-y-4">
                <h4 className="text-sm font-semibold text-erp-navy">
                  {editingParamId ? t("editParameterTitle") : t("addParameterTitle")}
                </h4>
                {paramError && <Alert variant="danger" onClose={() => setParamError(null)}>{paramError}</Alert>}
                <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
                  <div className="sm:col-span-2">
                    <ItemAutocomplete
                      label={t("item")}
                      required
                      stockableOnly
                      disabled={isBusy || Boolean(editingParamId)}
                      value={paramItemId}
                      onChange={(id) => setParamItemId(id)}
                    />
                  </div>
                  <Select
                    label={t("warehouse")}
                    value={paramWarehouseId}
                    disabled={isBusy || Boolean(editingParamId)}
                    placeholder={t("allWarehouses")}
                    options={(warehousesQuery.data?.items ?? []).map((w) => ({
                      value: w.id ?? "",
                      label: `${w.code ?? "-"} · ${w.name ?? "-"}`,
                    }))}
                    onChange={(e) => setParamWarehouseId(e.target.value)}
                  />
                  <Input
                    type="number"
                    min={0}
                    step="0.0001"
                    label={t("safetyStock")}
                    required
                    value={safetyStock}
                    disabled={isBusy}
                    placeholder={t("safetyStockPlaceholder")}
                    onChange={(e) => setSafetyStock(e.target.value)}
                  />
                  <Input
                    type="number"
                    min={0}
                    step="0.0001"
                    label={t("minOrderQuantity")}
                    required
                    value={minOrderQty}
                    disabled={isBusy}
                    placeholder={t("minOrderQuantityPlaceholder")}
                    onChange={(e) => setMinOrderQty(e.target.value)}
                  />
                  <Input
                    type="number"
                    min={0}
                    step="0.0001"
                    label={t("lotMultiple")}
                    required
                    value={lotMultiple}
                    disabled={isBusy}
                    placeholder={t("lotMultiplePlaceholder")}
                    onChange={(e) => setLotMultiple(e.target.value)}
                  />
                  <Input
                    type="number"
                    min={0}
                    max={365}
                    step={1}
                    label={t("leadTimeDays")}
                    value={leadTimeDays}
                    disabled={isBusy}
                    placeholder={t("leadTimeDaysPlaceholder")}
                    onChange={(e) => setLeadTimeDays(e.target.value)}
                  />
                </div>
                <div className="flex justify-end gap-2 pt-2">
                  {editingParamId && (
                    <Button type="button" variant="outline" className="min-h-11" disabled={isBusy} onClick={resetParamForm}>
                      {tCommon("actions.cancel")}
                    </Button>
                  )}
                  <Button type="submit" variant="primary" className="min-h-11" isLoading={mutations.upsertParameter.isPending} disabled={isBusy}>
                    {editingParamId ? t("saveParameter") : t("addParameter")}
                  </Button>
                </div>
              </form>
            )}

            <div className="space-y-3">
              <h4 className="text-sm font-semibold text-erp-navy">{t("parametersListTitle")}</h4>
              <DataTable<MrpPlanningParameterResponse>
                columns={paramColumns}
                data={paramsQuery.data?.items ?? []}
                isLoading={paramsQuery.isLoading}
                isError={paramsQuery.isError}
                error={paramsQuery.error}
                onRetry={() => { void paramsQuery.refetch(); }}
                emptyTitle={t("emptyParamsTitle")}
                emptyDescription={t("emptyParamsDescription")}
              />
            </div>
          </div>
        )}

        {/* Tab 2: Working Calendar Holidays */}
        {activeTab === "holidays" && (
          <div className="space-y-5">
            {canManage && (
              <form onSubmit={(e) => void handleCreateHoliday(e)} className="border border-erp-border bg-erp-surface-subtle p-4 space-y-4">
                <h4 className="text-sm font-semibold text-erp-navy">{t("addHolidayTitle")}</h4>
                {holidayError && <Alert variant="danger" onClose={() => setHolidayError(null)}>{holidayError}</Alert>}
                <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
                  <Input
                    type="date"
                    label={t("holidayDate")}
                    required
                    value={holidayDate}
                    disabled={isBusy}
                    placeholder={t("datePlaceholder")}
                    onChange={(e) => setHolidayDate(e.target.value)}
                  />
                  <Input
                    label={t("holidayName")}
                    required
                    maxLength={200}
                    value={holidayName}
                    disabled={isBusy}
                    placeholder={t("holidayNamePlaceholder")}
                    onChange={(e) => setHolidayName(e.target.value)}
                  />
                </div>
                <div className="flex justify-end pt-2">
                  <Button type="submit" variant="primary" className="min-h-11" isLoading={mutations.createHoliday.isPending} disabled={isBusy}>
                    {t("addHoliday")}
                  </Button>
                </div>
              </form>
            )}

            <div className="space-y-3">
              <h4 className="text-sm font-semibold text-erp-navy">{t("holidaysListTitle")}</h4>
              <DataTable<WorkingCalendarHolidayResponse>
                columns={holidayColumns}
                data={holidaysQuery.data?.items ?? []}
                isLoading={holidaysQuery.isLoading}
                isError={holidaysQuery.isError}
                error={holidaysQuery.error}
                onRetry={() => { void holidaysQuery.refetch(); }}
                emptyTitle={t("emptyHolidaysTitle")}
                emptyDescription={t("emptyHolidaysDescription")}
              />
            </div>
          </div>
        )}
      </div>

      {/* Delete Parameter Confirmation Modal */}
      <ConfirmationModal
        isOpen={deletingParam !== null}
        onClose={() => setDeletingParam(null)}
        onConfirm={() => void handleDeleteParameterConfirm()}
        title={t("deleteParameterTitle")}
        message={t("deleteParameterConfirmMessage", {
          item: deletingParam?.item?.code ?? deletingParam?.item?.nameTh ?? "",
        })}
        confirmText={t("delete")}
        variant="danger"
        isLoading={mutations.deleteParameter.isPending}
      />

      {/* Delete Holiday Confirmation Modal */}
      <ConfirmationModal
        isOpen={deletingHoliday !== null}
        onClose={() => setDeletingHoliday(null)}
        onConfirm={() => void handleDeleteHolidayConfirm()}
        title={t("deleteHolidayTitle")}
        message={t("deleteHolidayConfirmMessage", {
          name: deletingHoliday?.name ?? deletingHoliday?.holidayDate ?? "",
        })}
        confirmText={t("delete")}
        variant="danger"
        isLoading={mutations.deleteHoliday.isPending}
      />
    </Drawer>
  );
}
