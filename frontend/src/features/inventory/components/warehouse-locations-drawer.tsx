"use client";

import { useRef, useState } from "react";
import { useTranslations } from "next-intl";
import { Drawer } from "@/components/ui/Drawer";
import { DataTable, type Column } from "@/components/ui/DataTable";
import { Input } from "@/components/ui/Input";
import { Button } from "@/components/ui/Button";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { Alert } from "@/components/ui/Alert";
import { EmptyState } from "@/components/ui/EmptyState";
import { IconPlus } from "@/components/common/Icons";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import type { WarehouseLocationResponse, WarehouseResponse } from "@/lib/api/api-client";
import { useWarehouseLocations, useWarehouseLocationMutations } from "../api/inventory-queries";
import { inventoryErrorCode } from "../inventory-status";

interface WarehouseLocationsDrawerProps {
  warehouse: WarehouseResponse | null;
  isOpen: boolean;
  onClose: () => void;
  canManage: boolean;
}

export function WarehouseLocationsDrawer({
  warehouse,
  isOpen,
  onClose,
  canManage,
}: WarehouseLocationsDrawerProps) {
  const t = useTranslations("inventory.warehouses.locations");
  const tErrors = useTranslations("inventory.errors");
  const tCommon = useTranslations("common");
  const { toast } = useToast();

  const [code, setCode] = useState("");
  const [name, setName] = useState("");
  const [error, setError] = useState<string | null>(null);
  const createKeyRef = useRef(crypto.randomUUID());

  const result = useWarehouseLocations(warehouse?.id, isOpen);
  const { create, update } = useWarehouseLocationMutations(warehouse?.id);

  const locations = result.data?.items ?? [];
  const isBusy = create.isPending || update.isPending;

  function describe(err: unknown): string {
    const errorCode = err instanceof ApiError ? inventoryErrorCode(err.code) : null;
    return errorCode ? tErrors(errorCode) : tErrors("failed");
  }

  async function handleCreate(event: React.FormEvent): Promise<void> {
    event.preventDefault();
    if (!code.trim() || !name.trim()) {
      setError(t("validationRequired"));
      return;
    }
    setError(null);
    try {
      await create.mutateAsync({
        payload: {
          code: code.trim().toUpperCase(),
          name: name.trim(),
        },
        idempotencyKey: createKeyRef.current,
      });
      createKeyRef.current = crypto.randomUUID();
      setCode("");
      setName("");
      toast.success(t("created"));
    } catch (err: unknown) {
      setError(describe(err));
    }
  }

  async function toggleStatus(location: WarehouseLocationResponse): Promise<void> {
    if (location.isDefault) return;
    setError(null);
    const newActive = location.status !== "active";
    try {
      await update.mutateAsync({
        location,
        payload: {
          name: location.name ?? "",
          active: newActive,
        },
      });
      toast.success(newActive ? t("activated") : t("deactivated"));
    } catch (err: unknown) {
      setError(describe(err));
    }
  }

  const columns: Column<WarehouseLocationResponse>[] = [
    {
      id: "code",
      header: t("code"),
      accessorKey: "code",
      className: "min-w-28 font-mono font-medium",
      cell: (_v, row) => (
        <div className="flex items-center gap-2">
          <span>{row.code ?? "-"}</span>
          {row.isDefault && (
            <span className="inline-flex items-center px-1.5 py-0.5 text-xs font-semibold bg-erp-navy/10 text-erp-navy border border-erp-navy/30">
              {t("defaultBadge")}
            </span>
          )}
        </div>
      ),
    },
    {
      id: "name",
      header: t("name"),
      accessorKey: "name",
      className: "min-w-40",
      cell: (_v, row) => row.name ?? "-",
    },
    {
      id: "status",
      header: t("status"),
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
          <Button
            type="button"
            variant="outline"
            size="sm"
            className="min-h-11"
            disabled={row.isDefault || isBusy}
            onClick={() => void toggleStatus(row)}
          >
            {row.status === "active" ? t("deactivate") : t("activate")}
          </Button>
        ) : null,
    },
  ];

  return (
    <Drawer
      isOpen={isOpen}
      onClose={() => {
        if (!isBusy) onClose();
      }}
      title={warehouse ? `${t("title")} - ${warehouse.name}` : t("title")}
      description={t("subtitle")}
      size="lg"
      closeDisabled={isBusy}
    >
      <div className="space-y-5 py-2">
        {error && <Alert variant="danger">{error}</Alert>}

        {canManage && (
          <form
            onSubmit={(e) => void handleCreate(e)}
            className="border border-erp-border bg-erp-surface-subtle p-4 space-y-3"
          >
            <h4 className="text-sm font-semibold text-erp-navy">{t("createTitle")}</h4>
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-3 items-end">
              <Input
                label={t("code")}
                placeholder={t("codePlaceholder")}
                value={code}
                onChange={(e) => setCode(e.target.value.toUpperCase())}
                disabled={isBusy}
                maxLength={50}
                required
                wrapperClassName="!mb-0"
              />
              <Input
                label={t("name")}
                placeholder={t("namePlaceholder")}
                value={name}
                onChange={(e) => setName(e.target.value)}
                disabled={isBusy}
                maxLength={200}
                required
                wrapperClassName="!mb-0"
              />
            </div>
            <div className="flex justify-end pt-2">
              <Button
                type="submit"
                variant="primary"
                size="md"
                isLoading={create.isPending}
                disabled={isBusy}
                icon={<IconPlus size={16} />}
              >
                {t("createButton")}
              </Button>
            </div>
          </form>
        )}

        {locations.length === 0 && !result.isLoading ? (
          <EmptyState
            icon="empty"
            title={t("emptyTitle")}
            description={t("emptyDescription")}
          />
        ) : (
          <DataTable<WarehouseLocationResponse>
            columns={columns}
            data={locations}
            isLoading={result.isLoading}
            isError={result.isError}
            error={result.error}
            onRetry={() => {
              void result.refetch();
            }}
            emptyTitle={t("emptyTitle")}
            emptyDescription={t("emptyDescription")}
          />
        )}
      </div>
    </Drawer>
  );
}
