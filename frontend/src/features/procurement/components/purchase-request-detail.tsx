"use client";

import { useState } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { useLocale, useTranslations } from "next-intl";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Modal } from "@/components/ui/Modal";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { Select } from "@/components/ui/Select";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { PageHeader } from "@/components/layout/PageHeader";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import { formatCurrency, formatDate, formatNumber } from "@/lib/formatters/formatters";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import type { PurchaseRequestAction, PurchaseRequestResponse } from "@/lib/api/api-client";
import {
  usePurchaseRequest,
  usePurchaseRequestMutations,
  useSupplierList,
} from "../api/procurement-queries";
import {
  isPurchaseRequestStatus,
  procurementErrorCode,
  purchaseRequestStatusVariant,
} from "../procurement-status";

interface PurchaseRequestDetailProps {
  purchaseRequestId: string;
}

type NoteAction = Extract<PurchaseRequestAction, "reject" | "cancel">;

export function PurchaseRequestDetail({ purchaseRequestId }: PurchaseRequestDetailProps) {
  const query = usePurchaseRequest(purchaseRequestId);
  const t = useTranslations("procurement.requests");

  if (query.isLoading) {
    return <div className="py-16 flex justify-center" role="status"><MonoSpinner size="md" /></div>;
  }
  if (query.isError || !query.data) {
    return <Alert variant="danger">{t("loadError")}</Alert>;
  }

  return <DetailView key={query.data.rowVersion} request={query.data} />;
}

function DetailView({ request }: { request: PurchaseRequestResponse }) {
  const t = useTranslations("procurement.requests");
  const tErrors = useTranslations("procurement.errors");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const router = useRouter();
  const { toast } = useToast();
  const { selectedMembership } = useSelectedMembership();
  const canCreate = can(selectedMembership, PERMISSIONS.PURCHASE_ORDERS_CREATE);
  const canApprove = can(selectedMembership, PERMISSIONS.PURCHASE_ORDERS_APPROVE);
  const mutations = usePurchaseRequestMutations();
  const suppliers = useSupplierList({ status: "active", page: 1, pageSize: 100 }, canCreate);

  const status = request.status ?? "";
  const lines = request.lines ?? [];
  const [noteAction, setNoteAction] = useState<NoteAction | null>(null);
  const [note, setNote] = useState("");
  const [noteError, setNoteError] = useState(false);
  const [message, setMessage] = useState<string | null>(null);

  // Convert Modal state
  const [isConvertOpen, setIsConvertOpen] = useState(false);
  const [supplierId, setSupplierId] = useState("");
  const [expectedDeliveryDate, setExpectedDeliveryDate] = useState("");
  const [convertNote, setConvertNote] = useState("");
  const [convertError, setConvertError] = useState<string | null>(null);

  const busy = mutations.act.isPending || mutations.convert.isPending;

  function describe(err: unknown): string {
    const code = err instanceof ApiError ? procurementErrorCode(err.code) : null;
    return code ? tErrors(code) : tErrors("failed");
  }

  async function run(action: PurchaseRequestAction, actionNote: string | null): Promise<boolean> {
    setMessage(null);
    try {
      await mutations.act.mutateAsync({
        id: request.id ?? "",
        rowVersion: request.rowVersion ?? "",
        action,
        note: actionNote,
      });
      toast.success(t(`actionDone.${action}`));
      return true;
    } catch (err: unknown) {
      setMessage(describe(err));
      return false;
    }
  }

  async function confirmNoteAction(): Promise<void> {
    if (!noteAction) return;
    if (!note.trim()) {
      setNoteError(true);
      return;
    }

    if (await run(noteAction, note.trim())) setNoteAction(null);
  }

  async function handleConvert(): Promise<void> {
    setConvertError(null);
    if (!supplierId) {
      setConvertError(t("supplierRequired"));
      return;
    }

    try {
      const order = await mutations.convert.mutateAsync({
        payload: {
          purchaseRequestIds: [request.id ?? ""],
          supplierId,
          expectedDeliveryDate: expectedDeliveryDate || null,
          note: convertNote.trim() || null,
        },
      });
      toast.success(t("convertSuccess"));
      setIsConvertOpen(false);
      router.push(`/${locale}/procurement/purchase-orders/${order.id}`);
    } catch (err: unknown) {
      setConvertError(describe(err));
    }
  }

  return (
    <section className="space-y-5">
      <PageHeader
        title={`${request.number ?? "-"}`}
        subtitle={t("detailSubtitle")}
        breadcrumbs={[
          { label: t("title"), href: `/${locale}/procurement/purchase-requests` },
          { label: request.number ?? "-" },
        ]}
        actions={
          <StatusBadge
            label={isPurchaseRequestStatus(status) ? t(`statuses.${status}`) : "-"}
            variant={purchaseRequestStatusVariant(status)}
          />
        }
      />
      {message && <Alert variant="danger" onClose={() => setMessage(null)}>{message}</Alert>}

      <div className="erp-card p-5 space-y-4">
        <dl className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3 text-sm">
          <div>
            <dt className="text-xs text-erp-text-muted">{t("project")}</dt>
            <dd className="font-medium">
              {request.project?.id ? (
                <Link href={`/${locale}/projects/${request.project.id}`} className="font-mono text-erp-navy underline">
                  {request.project.code}
                </Link>
              ) : (
                "-"
              )}
            </dd>
          </div>
          <div>
            <dt className="text-xs text-erp-text-muted">{t("requiredByDate")}</dt>
            <dd className="font-medium">
              {request.requiredByDate ? formatDate(request.requiredByDate, locale) : "-"}
            </dd>
          </div>
          <div>
            <dt className="text-xs text-erp-text-muted">{t("totalEstimatedAmount")}</dt>
            <dd className="font-mono font-semibold">
              {formatCurrency(request.totalEstimatedAmount, "THB", locale)}
            </dd>
          </div>
          <div>
            <dt className="text-xs text-erp-text-muted">{t("createdBy")}</dt>
            <dd className="font-medium">{request.createdBy?.displayName ?? "-"}</dd>
          </div>
          <div>
            <dt className="text-xs text-erp-text-muted">{t("decidedBy")}</dt>
            <dd className="font-medium">{request.decidedBy?.displayName ?? "-"}</dd>
          </div>
          <div>
            <dt className="text-xs text-erp-text-muted">{t("convertedToPo", { number: "" }).replace(":", "").trim()}</dt>
            <dd className="font-medium">
              {request.convertedPurchaseOrder?.id ? (
                <Link
                  href={`/${locale}/procurement/purchase-orders/${request.convertedPurchaseOrder.id}`}
                  className="font-mono text-erp-navy underline"
                >
                  {request.convertedPurchaseOrder.number ?? "-"}
                </Link>
              ) : (
                "-"
              )}
            </dd>
          </div>
        </dl>
        {request.note && <p className="text-xs text-erp-text-body">{t("noteLine", { note: request.note })}</p>}
        {request.decisionNote && <p className="text-xs text-erp-text-muted">{t("decisionNote", { note: request.decisionNote })}</p>}
        {request.cancelReason && <p className="text-xs text-erp-text-muted">{t("cancelReason", { reason: request.cancelReason })}</p>}

        <div className="flex flex-wrap gap-2">
          {status === "draft" && canCreate && (
            <>
              <Button href={`/${locale}/procurement/purchase-requests/${request.id}/edit`} variant="outline" className="min-h-11">
                {t("editDraft")}
              </Button>
              <Button type="button" variant="primary" className="min-h-11" disabled={busy} onClick={() => void run("submit", null)}>
                {t("submit")}
              </Button>
            </>
          )}
          {status === "submitted" && canApprove && (
            <>
              <Button type="button" variant="primary" className="min-h-11" disabled={busy} onClick={() => void run("approve", null)}>
                {t("approve")}
              </Button>
              <Button
                type="button"
                variant="danger"
                className="min-h-11"
                disabled={busy}
                onClick={() => {
                  setNote("");
                  setNoteError(false);
                  setNoteAction("reject");
                }}
              >
                {t("reject")}
              </Button>
            </>
          )}
          {status === "approved" && canCreate && !request.convertedPurchaseOrder && (
            <Button
              type="button"
              variant="primary"
              className="min-h-11"
              disabled={busy}
              onClick={() => {
                setConvertError(null);
                setIsConvertOpen(true);
              }}
            >
              {t("convertSingle")}
            </Button>
          )}
          {["draft", "submitted", "approved"].includes(status) && canCreate && !request.convertedPurchaseOrder && (
            <Button
              type="button"
              variant="outline"
              className="min-h-11"
              disabled={busy}
              onClick={() => {
                setNote("");
                setNoteError(false);
                setNoteAction("cancel");
              }}
            >
              {t("cancel")}
            </Button>
          )}
        </div>
      </div>

      <div className="erp-card p-5 space-y-3" role="region" aria-label={t("linesTitle")}>
        <h3 className="text-sm font-bold text-erp-navy">{t("linesTitle")}</h3>
        <div className="overflow-x-auto">
          <table className="w-full text-left text-xs">
            <thead>
              <tr className="border-b border-erp-border">
                <th className="py-2 pr-2">{t("lineNo")}</th>
                <th className="py-2 pr-2">{t("item")}</th>
                <th className="py-2 pr-2 text-right">{t("quantity")}</th>
                <th className="py-2 pr-2 text-right">{t("estimatedUnitPrice")}</th>
                <th className="py-2 pr-2 text-right">{t("estimatedLineTotal")}</th>
                <th className="py-2">{t("note")}</th>
              </tr>
            </thead>
            <tbody>
              {lines.map((line) => (
                <tr key={line.id} className="border-b border-erp-border/60 align-middle">
                  <td className="py-2 pr-2 font-mono">{line.lineNo}</td>
                  <td className="py-2 pr-2">
                    {line.itemCode} · {line.itemNameTh} ({line.unitCode})
                  </td>
                  <td className="py-2 pr-2 text-right font-mono">{formatNumber(line.quantity)}</td>
                  <td className="py-2 pr-2 text-right font-mono">
                    {formatCurrency(line.estimatedUnitPrice, "THB", locale)}
                  </td>
                  <td className="py-2 pr-2 text-right font-mono">
                    {formatCurrency(line.estimatedLineTotal, "THB", locale)}
                  </td>
                  <td className="py-2 text-erp-text-muted">{line.note || "-"}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      <Modal
        isOpen={noteAction !== null}
        onClose={() => { if (!busy) setNoteAction(null); }}
        title={noteAction ? t(noteAction === "reject" ? "reject" : "cancel") : undefined}
        description={noteAction ? t(noteAction === "reject" ? "rejectDescription" : "cancelDescription") : undefined}
        size="sm"
        closeDisabled={busy}
        closeOnOverlayClick={!busy}
        closeOnEscape={!busy}
        footer={
          <div className="flex justify-end gap-3">
            <Button type="button" variant="outline" className="min-h-11" disabled={busy} onClick={() => setNoteAction(null)}>
              {tCommon("actions.cancel")}
            </Button>
            <Button
              type="button"
              variant="danger"
              className="min-h-11"
              isLoading={mutations.act.isPending}
              disabled={busy}
              onClick={() => void confirmNoteAction()}
            >
              {t("confirm")}
            </Button>
          </div>
        }
      >
        <Input
          label={t("reason")}
          required
          value={note}
          maxLength={500}
          disabled={busy}
          placeholder={t("reasonPlaceholder")}
          error={noteError ? t("reasonRequired") : undefined}
          onChange={(event) => {
            setNote(event.target.value);
            if (event.target.value.trim()) setNoteError(false);
          }}
        />
      </Modal>

      <Modal
        isOpen={isConvertOpen}
        onClose={() => { if (!mutations.convert.isPending) setIsConvertOpen(false); }}
        title={t("convertTitle")}
        description={t("convertDescription")}
        size="md"
        closeDisabled={mutations.convert.isPending}
        closeOnOverlayClick={!mutations.convert.isPending}
        closeOnEscape={!mutations.convert.isPending}
        footer={
          <div className="flex justify-end gap-3">
            <Button
              type="button"
              variant="outline"
              className="min-h-11"
              disabled={mutations.convert.isPending}
              onClick={() => setIsConvertOpen(false)}
            >
              {tCommon("actions.cancel")}
            </Button>
            <Button
              type="button"
              variant="primary"
              className="min-h-11"
              isLoading={mutations.convert.isPending}
              disabled={mutations.convert.isPending || !supplierId}
              onClick={() => void handleConvert()}
            >
              {t("convertSubmit")}
            </Button>
          </div>
        }
      >
        <div className="space-y-4">
          {convertError && <Alert variant="danger">{convertError}</Alert>}
          <Select
            label={t("supplier")}
            required
            value={supplierId}
            placeholder={t("selectSupplier")}
            disabled={mutations.convert.isPending}
            options={(suppliers.data?.items ?? []).map((s) => ({
              value: s.id ?? "",
              label: `${s.code ?? "-"} · ${s.nameTh ?? "-"}`,
            }))}
            onChange={(event) => setSupplierId(event.target.value)}
          />
          <Input
            type="date"
            label={t("expectedDelivery")}
            placeholder={t("expectedDeliveryPlaceholder")}
            value={expectedDeliveryDate}
            disabled={mutations.convert.isPending}
            onChange={(event) => setExpectedDeliveryDate(event.target.value)}
          />
          <Input
            label={t("convertNote")}
            placeholder={t("convertNotePlaceholder")}
            value={convertNote}
            maxLength={500}
            disabled={mutations.convert.isPending}
            onChange={(event) => setConvertNote(event.target.value)}
          />
        </div>
      </Modal>
    </section>
  );
}
