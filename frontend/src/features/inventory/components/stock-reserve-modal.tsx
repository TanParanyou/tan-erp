"use client";

import { useRef, useState } from "react";
import { useTranslations } from "next-intl";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Modal } from "@/components/ui/Modal";
import { Select } from "@/components/ui/Select";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import { useProjectList } from "@/features/projects/api/project-queries";
import type { StockBalanceResponse } from "@/lib/api/api-client";
import { useStockMutations } from "../api/inventory-queries";
import { inventoryErrorCode } from "../inventory-status";

interface StockReserveModalProps {
  balance: StockBalanceResponse | null;
  onClose: () => void;
}

const PAGE_LIMIT = 100;

export function StockReserveModal({ balance, onClose }: StockReserveModalProps) {
  const t = useTranslations("inventory.reserve");
  const tErrors = useTranslations("inventory.errors");
  const tCommon = useTranslations("common");
  const { toast } = useToast();
  const mutations = useStockMutations();
  const projects = useProjectList({ page: 1, pageSize: PAGE_LIMIT }, balance !== null);
  const [projectId, setProjectId] = useState("");
  const [quantity, setQuantity] = useState("1");
  const [note, setNote] = useState("");
  const [error, setError] = useState<string | null>(null);
  const keyRef = useRef(crypto.randomUUID());
  const busy = mutations.reserve.isPending;

  async function submit(): Promise<void> {
    setError(null);
    const parsed = Number(quantity);
    if (!balance || !projectId || !(parsed > 0) || !Number.isFinite(parsed)) {
      setError(t("invalid"));
      return;
    }

    try {
      await mutations.reserve.mutateAsync({
        payload: { warehouseId: balance.warehouse?.id ?? "", itemId: balance.item?.id ?? "", projectId, quantity: parsed, note: note.trim() || null },
        idempotencyKey: keyRef.current,
      });
      toast.success(t("success"));
      keyRef.current = crypto.randomUUID();
      setQuantity("1");
      setNote("");
      onClose();
    } catch (err: unknown) {
      const code = err instanceof ApiError ? inventoryErrorCode(err.code) : null;
      setError(code ? tErrors(code) : tErrors("failed"));
    }
  }

  return (
    <Modal
      isOpen={balance !== null}
      onClose={() => { if (!busy) onClose(); }}
      title={t("title")}
      description={balance ? t("description", { item: balance.item?.code ?? "-", available: balance.available ?? 0 }) : undefined}
      size="sm"
      closeDisabled={busy}
      closeOnOverlayClick={!busy}
      closeOnEscape={!busy}
      footer={(
        <div className="flex justify-end gap-3">
          <Button type="button" variant="outline" className="min-h-11" disabled={busy} onClick={onClose}>{tCommon("actions.cancel")}</Button>
          <Button type="button" variant="primary" className="min-h-11" isLoading={busy} disabled={busy} onClick={() => void submit()}>{t("confirm")}</Button>
        </div>
      )}
    >
      <div className="space-y-3">
        {error && <Alert variant="danger" onClose={() => setError(null)}>{error}</Alert>}
        <Select
          label={t("project")}
          required
          value={projectId}
          placeholder={t("selectProject")}
          disabled={busy}
          options={(projects.data?.items ?? []).map((p) => ({ value: p.id ?? "", label: `${p.code ?? "-"} · ${p.name ?? "-"}` }))}
          onChange={(event) => setProjectId(event.target.value)}
        />
        <Input type="number" min={0} step="0.0001" label={t("quantity")} required value={quantity} disabled={busy} onChange={(event) => setQuantity(event.target.value)} />
        <Input label={t("note")} maxLength={500} value={note} disabled={busy} onChange={(event) => setNote(event.target.value)} />
      </div>
    </Modal>
  );
}
