"use client";

import { useMemo, useState } from "react";
import { useLocale, useTranslations } from "next-intl";
import { Modal } from "@/components/ui/Modal";
import { DataTable, type Column } from "@/components/ui/DataTable";
import { Input } from "@/components/ui/Input";
import { Select } from "@/components/ui/Select";
import { Button } from "@/components/ui/Button";
import { Alert } from "@/components/ui/Alert";
import { EmptyState } from "@/components/ui/EmptyState";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import { formatCurrency, formatDateTime, formatNumber } from "@/lib/formatters/formatters";
import type { InventoryPeriodCloseResponse } from "@/lib/api/api-client";
import { useCloseInventoryPeriod, useInventoryPeriodCloses } from "../api/inventory-queries";
import { inventoryErrorCode } from "../inventory-status";

interface InventoryPeriodCloseModalProps {
  isOpen: boolean;
  onClose: () => void;
}

export function InventoryPeriodCloseModal({ isOpen, onClose }: InventoryPeriodCloseModalProps) {
  const t = useTranslations("inventory.periodClose");
  const tErrors = useTranslations("inventory.errors");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const { toast } = useToast();

  const now = new Date();
  const [periodYear, setPeriodYear] = useState<number>(now.getFullYear());
  const [periodMonth, setPeriodMonth] = useState<number>(now.getMonth() + 1);
  const [note, setNote] = useState("");
  const [error, setError] = useState<string | null>(null);

  const closesQuery = useInventoryPeriodCloses(isOpen);
  const closeMutation = useCloseInventoryPeriod();

  const isBusy = closeMutation.isPending;
  const history = closesQuery.data?.items ?? [];

  const monthOptions = useMemo(
    () =>
      Array.from({ length: 12 }, (_, i) => {
        const m = i + 1;
        return {
          value: String(m),
          label: `${String(m).padStart(2, "0")} - ${t(`months.${m}`)}`,
        };
      }),
    [t]
  );

  function describe(err: unknown): string {
    const code = err instanceof ApiError ? inventoryErrorCode(err.code) : null;
    return code ? tErrors(code) : tErrors("failed");
  }

  async function handleSubmit(event: React.FormEvent): Promise<void> {
    event.preventDefault();
    setError(null);
    if (!periodYear || !periodMonth) {
      setError(t("validationRequired"));
      return;
    }

    try {
      await closeMutation.mutateAsync({
        payload: {
          periodYear: Number(periodYear),
          periodMonth: Number(periodMonth),
          note: note.trim() || null,
        },
      });
      toast.success(t("closeSuccess"));
      setNote("");
    } catch (err: unknown) {
      setError(describe(err));
    }
  }

  const columns: Column<InventoryPeriodCloseResponse>[] = [
    {
      id: "period",
      header: t("period"),
      className: "font-mono font-semibold min-w-28",
      cell: (_v, row) =>
        `${row.periodYear}-${String(row.periodMonth ?? 0).padStart(2, "0")}`,
    },
    {
      id: "periodEndDate",
      header: t("periodEndDate"),
      className: "min-w-28 font-mono",
      cell: (_v, row) => row.periodEndDate ?? "-",
    },
    {
      id: "lineCount",
      header: t("lineCount"),
      className: "text-right font-mono min-w-24",
      cell: (_v, row) => formatNumber(row.lineCount),
    },
    {
      id: "totalOnHandQuantity",
      header: t("totalOnHandQuantity"),
      className: "text-right font-mono min-w-28",
      cell: (_v, row) => formatNumber(row.totalOnHandQuantity),
    },
    {
      id: "totalValuationAmount",
      header: t("totalValuationAmount"),
      className: "text-right font-mono min-w-32",
      cell: (_v, row) => formatCurrency(row.totalValuationAmount, "THB", locale),
    },
    {
      id: "closedBy",
      header: t("closedBy"),
      className: "min-w-36 text-xs",
      cell: (_v, row) => row.closedBy?.displayName || row.closedBy?.email || "-",
    },
    {
      id: "closedAtUtc",
      header: t("closedAt"),
      className: "min-w-36 text-xs",
      cell: (_v, row) => (row.closedAtUtc ? formatDateTime(row.closedAtUtc, locale) : "-"),
    },
  ];

  return (
    <Modal
      isOpen={isOpen}
      onClose={() => {
        if (!isBusy) onClose();
      }}
      title={t("title")}
      description={t("subtitle")}
      size="xl"
      closeDisabled={isBusy}
      footer={(
        <div className="flex justify-end">
          <Button
            type="button"
            variant="outline"
            className="min-h-11"
            disabled={isBusy}
            onClick={onClose}
          >
            {tCommon("actions.close")}
          </Button>
        </div>
      )}
    >
      <div className="space-y-6">
        {error && (
          <Alert variant="danger" onClose={() => setError(null)}>
            {error}
          </Alert>
        )}

        <form
          onSubmit={(e) => void handleSubmit(e)}
          className="border border-erp-border bg-erp-surface-subtle p-4 space-y-4"
        >
          <h4 className="text-sm font-semibold text-erp-navy">{t("formTitle")}</h4>
          <p className="text-xs text-erp-text-muted">{t("formDescription")}</p>

          <div className="grid grid-cols-1 sm:grid-cols-3 gap-3">
            <Input
              type="number"
              min={2000}
              max={2100}
              label={t("periodYear")}
              placeholder={t("periodYearPlaceholder")}
              value={periodYear}
              disabled={isBusy}
              required
              onChange={(e) => setPeriodYear(Number(e.target.value))}
            />
            <Select
              label={t("periodMonth")}
              value={String(periodMonth)}
              placeholder={t("selectMonth")}
              disabled={isBusy}
              options={monthOptions}
              onChange={(e) => setPeriodMonth(Number(e.target.value))}
            />
            <Input
              label={t("note")}
              placeholder={t("notePlaceholder")}
              value={note}
              disabled={isBusy}
              maxLength={500}
              onChange={(e) => setNote(e.target.value)}
            />
          </div>

          <div className="flex justify-end">
            <Button
              type="submit"
              variant="primary"
              className="min-h-11"
              isLoading={isBusy}
              disabled={isBusy}
            >
              {t("closePeriodButton")}
            </Button>
          </div>
        </form>

        <div className="space-y-2">
          <h4 className="text-sm font-semibold text-erp-navy">{t("historyTitle")}</h4>
          {history.length === 0 && !closesQuery.isLoading ? (
            <EmptyState
              icon="empty"
              title={t("emptyHistoryTitle")}
              description={t("emptyHistoryDescription")}
            />
          ) : (
            <DataTable<InventoryPeriodCloseResponse>
              columns={columns}
              data={history}
              isLoading={closesQuery.isLoading}
              isError={closesQuery.isError}
              error={closesQuery.error}
              onRetry={() => {
                void closesQuery.refetch();
              }}
              emptyTitle={t("emptyHistoryTitle")}
              emptyDescription={t("emptyHistoryDescription")}
            />
          )}
        </div>
      </div>
    </Modal>
  );
}
