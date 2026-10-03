"use client";

import { useRef, useState } from "react";
import { useTranslations } from "next-intl";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { Modal } from "@/components/ui/Modal";
import { Select } from "@/components/ui/Select";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import { useStockMutations, useWarehouseList } from "../api/inventory-queries";
import { inventoryErrorCode } from "../inventory-status";

interface PutIntoStockButtonProps {
  goodsReceiptId: string;
}

const PAGE_LIMIT = 100;

/** Puts one goods receipt into a warehouse; the backend refuses a second posting of the same receipt. */
export function PutIntoStockButton({ goodsReceiptId }: PutIntoStockButtonProps) {
  const t = useTranslations("inventory.receive");
  const tErrors = useTranslations("inventory.errors");
  const tCommon = useTranslations("common");
  const { toast } = useToast();
  const mutations = useStockMutations();
  const [isOpen, setIsOpen] = useState(false);
  const warehouses = useWarehouseList({ status: "active", page: 1, pageSize: PAGE_LIMIT }, isOpen);
  const [warehouseId, setWarehouseId] = useState("");
  const [error, setError] = useState<string | null>(null);
  const keyRef = useRef(crypto.randomUUID());
  const busy = mutations.receive.isPending;

  async function submit(): Promise<void> {
    setError(null);
    if (!warehouseId) {
      setError(t("warehouseRequired"));
      return;
    }

    try {
      const document = await mutations.receive.mutateAsync({ goodsReceiptId, warehouseId, idempotencyKey: keyRef.current });
      toast.success(t("success", { number: document.number ?? "-" }));
      setIsOpen(false);
    } catch (err: unknown) {
      const code = err instanceof ApiError ? inventoryErrorCode(err.code) : null;
      setError(code ? tErrors(code) : tErrors("failed"));
    }
  }

  return (
    <>
      <Button type="button" variant="outline" size="sm" className="min-h-11" onClick={() => { setError(null); setIsOpen(true); }}>{t("action")}</Button>
      <Modal
        isOpen={isOpen}
        onClose={() => { if (!busy) setIsOpen(false); }}
        title={t("title")}
        description={t("description")}
        size="sm"
        closeDisabled={busy}
        closeOnOverlayClick={!busy}
        closeOnEscape={!busy}
        footer={(
          <div className="flex justify-end gap-3">
            <Button type="button" variant="outline" className="min-h-11" disabled={busy} onClick={() => setIsOpen(false)}>{tCommon("actions.cancel")}</Button>
            <Button type="button" variant="primary" className="min-h-11" isLoading={busy} disabled={busy} onClick={() => void submit()}>{t("confirm")}</Button>
          </div>
        )}
      >
        <div className="space-y-3">
          {error && <Alert variant="danger" onClose={() => setError(null)}>{error}</Alert>}
          <Select
            label={t("warehouse")}
            required
            value={warehouseId}
            placeholder={t("selectWarehouse")}
            disabled={busy}
            options={(warehouses.data?.items ?? []).map((w) => ({ value: w.id ?? "", label: `${w.code ?? "-"} · ${w.name ?? "-"}` }))}
            onChange={(event) => setWarehouseId(event.target.value)}
          />
        </div>
      </Modal>
    </>
  );
}
