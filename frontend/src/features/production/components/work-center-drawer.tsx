"use client";

import { useMemo, useRef, useState } from "react";
import { useLocale, useTranslations } from "next-intl";
import { Drawer } from "@/components/ui/Drawer";
import { DataTable, type Column } from "@/components/ui/DataTable";
import { Input } from "@/components/ui/Input";
import { Button } from "@/components/ui/Button";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { Alert } from "@/components/ui/Alert";
import { EmptyState } from "@/components/ui/EmptyState";
import { ConfirmationModal } from "@/components/ui/ConfirmationModal";
import { IconPlus } from "@/components/common/Icons";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import { formatCurrency, formatNumber } from "@/lib/formatters/formatters";
import type {
  CreateWorkCenterRequest,
  UpdateWorkCenterRequest,
  WorkCenterCapacityRowResponse,
  WorkCenterResponse,
} from "@/lib/api/api-client";
import {
  useWorkCenterCapacity,
  useWorkCenterList,
  useWorkCenterMutations,
} from "../api/production-queries";
import { productionErrorCode } from "../production-status";

interface WorkCenterDrawerProps {
  isOpen: boolean;
  onClose: () => void;
  canManage: boolean;
}

type WorkCenterTab = "workCenters" | "capacity";

function toDateString(d: Date): string {
  const year = d.getFullYear();
  const month = String(d.getMonth() + 1).padStart(2, "0");
  const day = String(d.getDate()).padStart(2, "0");
  return `${year}-${month}-${day}`;
}

export function WorkCenterDrawer({ isOpen, onClose, canManage }: WorkCenterDrawerProps) {
  const t = useTranslations("production.workCenters");
  const tErrors = useTranslations("production.errors");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const { toast } = useToast();

  const [activeTab, setActiveTab] = useState<WorkCenterTab>("workCenters");

  // Work Centers list & mutations
  const workCentersQuery = useWorkCenterList({ page: 1, pageSize: 100 }, isOpen);
  const mutations = useWorkCenterMutations();

  // Create / Edit state
  const [editingCenter, setEditingCenter] = useState<WorkCenterResponse | "new" | null>(null);
  const [code, setCode] = useState("");
  const [name, setName] = useState("");
  const [dailyCapacityHours, setDailyCapacityHours] = useState("8");
  const [hourlyRate, setHourlyRate] = useState("0");
  const [formError, setFormError] = useState<string | null>(null);
  const createKeyRef = useRef(crypto.randomUUID());

  // Deactivate confirmation state
  const [deactivatingCenter, setDeactivatingCenter] = useState<WorkCenterResponse | null>(null);

  // Capacity date range (default today to +14 days)
  const defaultDates = useMemo(() => {
    const today = new Date();
    const future = new Date();
    future.setDate(today.getDate() + 14);
    return {
      from: toDateString(today),
      to: toDateString(future),
    };
  }, []);

  const [fromDate, setFromDate] = useState(defaultDates.from);
  const [toDate, setToDate] = useState(defaultDates.to);
  const [expandedCenterId, setExpandedCenterId] = useState<string | null>(null);

  const capacityQuery = useWorkCenterCapacity(
    { fromDate: fromDate || undefined, toDate: toDate || undefined },
    isOpen && activeTab === "capacity"
  );

  const isBusy =
    mutations.create.isPending ||
    mutations.update.isPending ||
    mutations.setActive.isPending;

  function describe(err: unknown): string {
    const errorCode = err instanceof ApiError ? productionErrorCode(err.code) : null;
    return errorCode ? tErrors(errorCode) : tErrors("failed");
  }

  function openCreate(): void {
    createKeyRef.current = crypto.randomUUID();
    setCode("");
    setName("");
    setDailyCapacityHours("8");
    setHourlyRate("0");
    setFormError(null);
    setEditingCenter("new");
  }

  function openEdit(center: WorkCenterResponse): void {
    setCode(center.code ?? "");
    setName(center.name ?? "");
    setDailyCapacityHours(String(center.dailyCapacityHours ?? 8));
    setHourlyRate(String(center.hourlyRate ?? 0));
    setFormError(null);
    setEditingCenter(center);
  }

  async function handleSave(event: React.FormEvent): Promise<void> {
    event.preventDefault();
    if (!name.trim()) {
      setFormError(t("validationNameRequired"));
      return;
    }

    const capacityNum = Number(dailyCapacityHours);
    if (!Number.isFinite(capacityNum) || capacityNum < 0.25 || capacityNum > 24) {
      setFormError(t("validationCapacityHours"));
      return;
    }

    const rateNum = Number(hourlyRate);
    if (!Number.isFinite(rateNum) || rateNum < 0) {
      setFormError(t("validationHourlyRate"));
      return;
    }

    setFormError(null);
    try {
      if (editingCenter === "new") {
        const payload: CreateWorkCenterRequest = {
          code: code.trim().toUpperCase() || null,
          name: name.trim(),
          dailyCapacityHours: capacityNum,
          hourlyRate: rateNum,
        };
        await mutations.create.mutateAsync({
          payload,
          idempotencyKey: createKeyRef.current,
        });
        toast.success(t("created"));
      } else if (editingCenter) {
        const payload: UpdateWorkCenterRequest = {
          name: name.trim(),
          dailyCapacityHours: capacityNum,
          hourlyRate: rateNum,
        };
        await mutations.update.mutateAsync({
          id: editingCenter.id ?? "",
          rowVersion: editingCenter.rowVersion ?? "",
          payload,
        });
        toast.success(t("updated"));
      }
      setEditingCenter(null);
    } catch (err: unknown) {
      setFormError(describe(err));
    }
  }

  async function handleDeactivateConfirm(): Promise<void> {
    if (!deactivatingCenter) return;
    try {
      await mutations.setActive.mutateAsync({
        id: deactivatingCenter.id ?? "",
        rowVersion: deactivatingCenter.rowVersion ?? "",
        active: false,
      });
      toast.success(t("deactivated"));
      setDeactivatingCenter(null);
    } catch (err: unknown) {
      toast.error(describe(err));
    }
  }

  async function handleActivate(center: WorkCenterResponse): Promise<void> {
    try {
      await mutations.setActive.mutateAsync({
        id: center.id ?? "",
        rowVersion: center.rowVersion ?? "",
        active: true,
      });
      toast.success(t("activated"));
    } catch (err: unknown) {
      toast.error(describe(err));
    }
  }

  const workCenters = workCentersQuery.data?.items ?? [];

  const workCenterColumns: Column<WorkCenterResponse>[] = [
    {
      id: "code",
      header: t("code"),
      accessorKey: "code",
      className: "min-w-28 font-mono font-medium",
      cell: (_v, row) => row.code ?? "-",
    },
    {
      id: "name",
      header: t("name"),
      accessorKey: "name",
      className: "min-w-40",
      cell: (_v, row) => row.name ?? "-",
    },
    {
      id: "dailyCapacityHours",
      header: t("dailyCapacityHours"),
      className: "text-right font-mono min-w-28",
      cell: (_v, row) => `${formatNumber(row.dailyCapacityHours)} ${t("hoursUnit")}`,
    },
    {
      id: "hourlyRate",
      header: t("hourlyRate"),
      className: "text-right font-mono min-w-28",
      cell: (_v, row) => formatCurrency(row.hourlyRate, "THB", locale),
    },
    {
      id: "status",
      header: t("status"),
      className: "min-w-28",
      cell: (_v, row) => (
        <StatusBadge
          label={row.status === "active" ? t("statusActive") : t("statusInactive")}
          variant={row.status === "active" ? "success" : "neutral"}
        />
      ),
    },
    {
      id: "actions",
      header: t("actions"),
      isAction: true,
      cell: (_v, row) =>
        canManage ? (
          <div className="flex gap-2">
            <Button
              type="button"
              variant="outline"
              size="sm"
              className="min-h-11"
              disabled={isBusy}
              onClick={() => openEdit(row)}
            >
              {t("edit")}
            </Button>
            {row.status === "active" ? (
              <Button
                type="button"
                variant="outline"
                size="sm"
                className="min-h-11"
                disabled={isBusy}
                onClick={() => setDeactivatingCenter(row)}
              >
                {t("deactivate")}
              </Button>
            ) : (
              <Button
                type="button"
                variant="outline"
                size="sm"
                className="min-h-11"
                disabled={isBusy}
                onClick={() => void handleActivate(row)}
              >
                {t("activate")}
              </Button>
            )}
          </div>
        ) : null,
    },
  ];

  const capacityRows = capacityQuery.data?.workCenters ?? [];

  const capacityColumns: Column<WorkCenterCapacityRowResponse>[] = [
    {
      id: "workCenter",
      header: t("workCenter"),
      className: "min-w-44 font-medium",
      cell: (_v, row) => (
        <div>
          <span className="font-mono">{row.code ?? "-"}</span> · {row.name ?? "-"}
        </div>
      ),
    },
    {
      id: "dailyCapacityHours",
      header: t("dailyCapacityHours"),
      className: "text-right font-mono min-w-28",
      cell: (_v, row) => `${formatNumber(row.dailyCapacityHours)} ${t("hoursUnit")}`,
    },
    {
      id: "totalCapacityHours",
      header: t("totalCapacityHours"),
      className: "text-right font-mono min-w-28",
      cell: (_v, row) => `${formatNumber(row.totalCapacityHours)} ${t("hoursUnit")}`,
    },
    {
      id: "totalPlannedHours",
      header: t("totalPlannedHours"),
      className: "text-right font-mono min-w-28 font-semibold",
      cell: (_v, row) => `${formatNumber(row.totalPlannedHours)} ${t("hoursUnit")}`,
    },
    {
      id: "utilizationPercent",
      header: t("utilizationPercent"),
      className: "text-right font-mono min-w-28 font-bold",
      cell: (_v, row) => `${formatNumber(row.utilizationPercent)}%`,
    },
    {
      id: "isOverloaded",
      header: t("capacityStatus"),
      className: "min-w-28",
      cell: (_v, row) => (
        <StatusBadge
          label={row.isOverloaded ? t("overloaded") : t("normalCapacity")}
          variant={row.isOverloaded ? "danger" : "success"}
        />
      ),
    },
    {
      id: "details",
      header: t("details"),
      isAction: true,
      cell: (_v, row) => (
        <Button
          type="button"
          variant="outline"
          size="sm"
          className="min-h-11"
          onClick={() =>
            setExpandedCenterId(expandedCenterId === row.workCenterId ? null : (row.workCenterId ?? null))
          }
        >
          {expandedCenterId === row.workCenterId ? t("hideDays") : t("showDays")}
        </Button>
      ),
    },
  ];

  return (
    <Drawer
      isOpen={isOpen}
      onClose={() => {
        if (!isBusy) onClose();
      }}
      title={t("title")}
      description={t("subtitle")}
      size="xl"
      closeDisabled={isBusy}
    >
      <div className="space-y-5 py-2">
        <div className="flex border-b border-erp-border gap-2">
          <button
            type="button"
            onClick={() => setActiveTab("workCenters")}
            className={`px-4 py-2 text-sm font-medium border-b-2 -mb-px transition-colors ${
              activeTab === "workCenters"
                ? "border-erp-navy text-erp-navy font-semibold"
                : "border-transparent text-erp-text-muted hover:text-erp-text-main"
            }`}
          >
            {t("tabs.workCenters")}
          </button>
          <button
            type="button"
            onClick={() => setActiveTab("capacity")}
            className={`px-4 py-2 text-sm font-medium border-b-2 -mb-px transition-colors ${
              activeTab === "capacity"
                ? "border-erp-navy text-erp-navy font-semibold"
                : "border-transparent text-erp-text-muted hover:text-erp-text-main"
            }`}
          >
            {t("tabs.capacity")}
          </button>
        </div>

        {activeTab === "workCenters" && (
          <div className="space-y-4">
            <div className="flex justify-between items-center">
              <h4 className="text-sm font-semibold text-erp-navy">{t("workCentersListTitle")}</h4>
              {canManage && editingCenter === null && (
                <Button
                  type="button"
                  variant="primary"
                  size="md"
                  icon={<IconPlus size={16} />}
                  onClick={openCreate}
                >
                  {t("createButton")}
                </Button>
              )}
            </div>

            {editingCenter !== null && (
              <form
                onSubmit={(e) => void handleSave(e)}
                className="border border-erp-border bg-erp-surface-subtle p-4 space-y-4"
              >
                <h5 className="text-sm font-semibold text-erp-navy">
                  {editingCenter === "new" ? t("createTitle") : t("editTitle")}
                </h5>
                {formError && <Alert variant="danger">{formError}</Alert>}

                <div className="grid grid-cols-1 sm:grid-cols-2 gap-3 items-end">
                  <Input
                    label={t("code")}
                    placeholder={t("codePlaceholder")}
                    value={code}
                    disabled={editingCenter !== "new" || isBusy}
                    onChange={(e) => setCode(e.target.value.toUpperCase())}
                    maxLength={50}
                    wrapperClassName="!mb-0"
                  />
                  <Input
                    label={t("name")}
                    placeholder={t("namePlaceholder")}
                    value={name}
                    disabled={isBusy}
                    required
                    onChange={(e) => setName(e.target.value)}
                    maxLength={200}
                    wrapperClassName="!mb-0"
                  />
                  <Input
                    type="number"
                    min={0.25}
                    max={24}
                    step="0.25"
                    label={t("dailyCapacityHours")}
                    placeholder={t("dailyCapacityPlaceholder")}
                    value={dailyCapacityHours}
                    disabled={isBusy}
                    required
                    onChange={(e) => setDailyCapacityHours(e.target.value)}
                    wrapperClassName="!mb-0"
                  />
                  <Input
                    type="number"
                    min={0}
                    step="0.01"
                    label={t("hourlyRate")}
                    placeholder={t("hourlyRatePlaceholder")}
                    value={hourlyRate}
                    disabled={isBusy}
                    onChange={(e) => setHourlyRate(e.target.value)}
                    wrapperClassName="!mb-0"
                  />
                </div>

                <div className="flex justify-end gap-2 pt-2">
                  <Button
                    type="button"
                    variant="outline"
                    className="min-h-11"
                    disabled={isBusy}
                    onClick={() => setEditingCenter(null)}
                  >
                    {tCommon("actions.cancel")}
                  </Button>
                  <Button
                    type="submit"
                    variant="primary"
                    className="min-h-11"
                    isLoading={isBusy}
                    disabled={isBusy}
                  >
                    {tCommon("actions.save")}
                  </Button>
                </div>
              </form>
            )}

            {workCenters.length === 0 && !workCentersQuery.isLoading ? (
              <EmptyState
                icon="empty"
                title={t("emptyTitle")}
                description={t("emptyDescription")}
                actionLabel={canManage ? t("createButton") : undefined}
                onAction={canManage ? openCreate : undefined}
              />
            ) : (
              <DataTable<WorkCenterResponse>
                columns={workCenterColumns}
                data={workCenters}
                isLoading={workCentersQuery.isLoading}
                isError={workCentersQuery.isError}
                error={workCentersQuery.error}
                onRetry={() => {
                  void workCentersQuery.refetch();
                }}
                emptyTitle={t("emptyTitle")}
                emptyDescription={t("emptyDescription")}
              />
            )}
          </div>
        )}

        {activeTab === "capacity" && (
          <div className="space-y-4">
            <div className="border border-erp-border bg-erp-surface-subtle p-3 flex flex-wrap items-end gap-3">
              <Input
                type="date"
                label={t("fromDate")}
                placeholder="YYYY-MM-DD"
                value={fromDate}
                onChange={(e) => setFromDate(e.target.value)}
                wrapperClassName="!mb-0"
              />
              <Input
                type="date"
                label={t("toDate")}
                placeholder="YYYY-MM-DD"
                value={toDate}
                onChange={(e) => setToDate(e.target.value)}
                wrapperClassName="!mb-0"
              />
              <Button
                type="button"
                variant="outline"
                className="min-h-11"
                disabled={capacityQuery.isLoading}
                onClick={() => {
                  void capacityQuery.refetch();
                }}
              >
                {tCommon("actions.search")}
              </Button>
            </div>

            {capacityRows.length === 0 && !capacityQuery.isLoading ? (
              <EmptyState
                icon="empty"
                title={t("emptyCapacityTitle")}
                description={t("emptyCapacityDescription")}
              />
            ) : (
              <div className="space-y-4">
                <DataTable<WorkCenterCapacityRowResponse>
                  columns={capacityColumns}
                  data={capacityRows}
                  isLoading={capacityQuery.isLoading}
                  isError={capacityQuery.isError}
                  error={capacityQuery.error}
                  onRetry={() => {
                    void capacityQuery.refetch();
                  }}
                  emptyTitle={t("emptyCapacityTitle")}
                  emptyDescription={t("emptyCapacityDescription")}
                />

                {expandedCenterId && (
                  <div className="border border-erp-border p-4 bg-erp-surface space-y-3">
                    <h5 className="text-sm font-semibold text-erp-navy">
                      {t("dailyPointsTitle")} (
                      {capacityRows.find((r) => r.workCenterId === expandedCenterId)?.name ?? ""})
                    </h5>
                    <div className="grid grid-cols-2 sm:grid-cols-4 lg:grid-cols-7 gap-2">
                      {capacityRows
                        .find((r) => r.workCenterId === expandedCenterId)
                        ?.days?.map((day) => (
                          <div
                            key={day.date}
                            className={`p-2 border text-xs space-y-1 ${
                              day.isOverloaded
                                ? "border-erp-danger bg-erp-danger/10 text-erp-danger font-medium"
                                : "border-erp-border bg-erp-surface-subtle text-erp-text-main"
                            }`}
                          >
                            <div className="font-mono font-semibold">{day.date}</div>
                            <div>
                              {t("capacityShort")}: {formatNumber(day.capacityHours)}h
                            </div>
                            <div>
                              {t("plannedShort")}: {formatNumber(day.plannedHours)}h
                            </div>
                            <div>
                              {t("utilShort")}: {formatNumber(day.utilizationPercent)}%
                            </div>
                            {day.isOverloaded && (
                              <div className="font-bold text-erp-danger">{t("overloadedShort")}</div>
                            )}
                          </div>
                        ))}
                    </div>
                  </div>
                )}
              </div>
            )}
          </div>
        )}
      </div>

      <ConfirmationModal
        isOpen={deactivatingCenter !== null}
        onClose={() => setDeactivatingCenter(null)}
        onConfirm={() => void handleDeactivateConfirm()}
        title={t("deactivateTitle")}
        message={t("deactivateConfirmMessage", {
          name: deactivatingCenter?.name ?? deactivatingCenter?.code ?? "",
        })}
        confirmText={t("deactivate")}
        variant="danger"
        isLoading={mutations.setActive.isPending}
      />
    </Drawer>
  );
}
