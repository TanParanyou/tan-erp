"use client";

import { useState } from "react";
import Link from "next/link";
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
import { formatDate, formatDateTime, formatNumber } from "@/lib/formatters/formatters";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import type { MrpRecommendationDecision, MrpRecommendationResponse, MrpRunResponse } from "@/lib/api/api-client";
import { useSupplierList } from "@/features/procurement/api/procurement-queries";
import { useWarehouseList } from "@/features/inventory/api/inventory-queries";
import { useMrpMutations, useMrpRun } from "../api/mrp-queries";
import { isMrpAction, isMrpReasonSource, isMrpRecommendationStatus, mrpErrorCode, mrpStatusVariant } from "../mrp-status";

interface MrpRunDetailProps {
  runId: string;
}

const PAGE_LIMIT = 100;

export function MrpRunDetail({ runId }: MrpRunDetailProps) {
  const query = useMrpRun(runId);
  const t = useTranslations("mrp.runs");

  if (query.isLoading) return <div className="py-16 flex justify-center" role="status"><MonoSpinner size="md" /></div>;
  if (query.isError || !query.data) return <Alert variant="danger">{t("loadError")}</Alert>;
  return <DetailView run={query.data} />;
}

function DetailView({ run }: { run: MrpRunResponse }) {
  const t = useTranslations("mrp.runs");
  const tErrors = useTranslations("mrp.errors");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const { toast } = useToast();
  const { selectedMembership } = useSelectedMembership();
  const canApprove = can(selectedMembership, PERMISSIONS.MRP_APPROVE);
  const canConvert = can(selectedMembership, PERMISSIONS.MRP_CONVERT);
  const mutations = useMrpMutations();
  const [message, setMessage] = useState<string | null>(null);
  const [target, setTarget] = useState<MrpRecommendationResponse | null>(null);
  const [supplierId, setSupplierId] = useState("");
  const [unitPrice, setUnitPrice] = useState("");
  const [warehouseId, setWarehouseId] = useState("");
  const [convertError, setConvertError] = useState<string | null>(null);

  const suppliers = useSupplierList({ status: "active", page: 1, pageSize: PAGE_LIMIT }, target?.action === "buy");
  const warehouses = useWarehouseList({ status: "active", page: 1, pageSize: PAGE_LIMIT }, target?.action === "make");
  const busy = mutations.decide.isPending || mutations.convert.isPending;

  function describe(err: unknown): string {
    const code = err instanceof ApiError ? mrpErrorCode(err.code) : null;
    return code ? tErrors(code) : tErrors("failed");
  }

  async function decide(rec: MrpRecommendationResponse, decision: MrpRecommendationDecision): Promise<void> {
    setMessage(null);
    try {
      await mutations.decide.mutateAsync({ runId: run.id ?? "", recommendationId: rec.id ?? "", rowVersion: rec.rowVersion ?? "", decision });
      toast.success(t(decision === "approve" ? "approved" : "rejected"));
    } catch (err: unknown) {
      setMessage(describe(err));
    }
  }

  function openConvert(rec: MrpRecommendationResponse): void {
    setTarget(rec);
    setSupplierId("");
    setUnitPrice("");
    setWarehouseId("");
    setConvertError(null);
  }

  async function confirmConvert(): Promise<void> {
    if (!target) return;
    const price = Number(unitPrice);
    if (target.action === "buy" && (!supplierId || unitPrice === "" || !(price >= 0))) {
      setConvertError(t("convertBuyRequired"));
      return;
    }

    if (target.action === "make" && !warehouseId) {
      setConvertError(t("convertMakeRequired"));
      return;
    }

    try {
      await mutations.convert.mutateAsync({
        runId: run.id ?? "",
        recommendationId: target.id ?? "",
        rowVersion: target.rowVersion ?? "",
        payload: target.action === "buy" ? { supplierId, unitPrice: price, warehouseId: null } : { supplierId: null, unitPrice: null, warehouseId },
      });
      toast.success(t("converted"));
      setTarget(null);
    } catch (err: unknown) {
      setConvertError(describe(err));
    }
  }

  const recommendations = run.recommendations ?? [];

  return (
    <section className="space-y-5">
      <PageHeader
        title={run.number ?? "-"}
        subtitle={t("detailSubtitle")}
        breadcrumbs={[{ label: t("title"), href: `/${locale}/production/mrp` }, { label: run.number ?? "-" }]}
      />
      {message && <Alert variant="danger" onClose={() => setMessage(null)}>{message}</Alert>}

      <div className="erp-card p-5 space-y-3">
        <dl className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4 text-sm">
          <div><dt className="text-xs text-erp-text-muted">{t("asOfDate")}</dt><dd className="font-medium">{run.asOfDate ? formatDate(run.asOfDate, locale) : "-"}</dd></div>
          <div><dt className="text-xs text-erp-text-muted">{t("purchaseLeadTime")}</dt><dd className="font-mono font-medium">{run.purchaseLeadTimeDays}</dd></div>
          <div><dt className="text-xs text-erp-text-muted">{t("productionLeadTime")}</dt><dd className="font-mono font-medium">{run.productionLeadTimeDays}</dd></div>
          <div><dt className="text-xs text-erp-text-muted">{t("createdBy")}</dt><dd className="font-medium">{run.createdBy?.displayName ?? "-"} · {run.createdAtUtc ? formatDateTime(run.createdAtUtc, locale) : "-"}</dd></div>
          <div className="sm:col-span-2 lg:col-span-4">
            <dt className="text-xs text-erp-text-muted">{t("snapshot")}</dt>
            <dd className="text-xs text-erp-text-body">
              {t("snapshotSummary", { demands: run.snapshot?.demandCount ?? 0, supplies: run.snapshot?.supplyCount ?? 0, boms: run.snapshot?.bomCount ?? 0, stock: run.snapshot?.stockItemCount ?? 0 })}
              <span className="ml-2 font-mono text-erp-text-muted" title={t("inputHash")}>{(run.inputHash ?? "").slice(0, 12)}</span>
            </dd>
          </div>
        </dl>
        <p className="text-xs text-erp-text-muted">{t("frozenNote")}</p>
      </div>

      <div className="erp-card p-5 space-y-3" role="region" aria-label={t("recommendationsTitle")}>
        <h3 className="text-sm font-bold text-erp-navy">{t("recommendationsTitle")}</h3>
        {recommendations.length === 0 ? (
          <p className="text-xs text-erp-text-muted">{t("recommendationsEmpty")}</p>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-left text-xs">
              <thead>
                <tr className="border-b border-erp-border">
                  <th className="py-2 pr-2">{t("action")}</th>
                  <th className="py-2 pr-2">{t("item")}</th>
                  <th className="py-2 pr-2 text-right">{t("quantity")}</th>
                  <th className="py-2 pr-2">{t("needBy")}</th>
                  <th className="py-2 pr-2">{t("orderBy")}</th>
                  <th className="py-2 pr-2">{t("why")}</th>
                  <th className="py-2 pr-2">{t("status")}</th>
                  <th className="py-2 text-right">{t("actions")}</th>
                </tr>
              </thead>
              <tbody>
                {recommendations.map((rec) => (
                  <tr key={rec.id} className="border-b border-erp-border/60 align-top">
                    <td className="py-2 pr-2 font-semibold">{isMrpAction(rec.action) ? t(`actionLabels.${rec.action}`) : "-"}</td>
                    <td className="py-2 pr-2">{rec.item?.code ?? "-"} · {rec.item?.nameTh ?? "-"} ({rec.item?.unitCode ?? "-"})</td>
                    <td className="py-2 pr-2 text-right font-mono">{formatNumber(rec.quantity)}</td>
                    <td className="py-2 pr-2">{rec.needBy ? formatDate(rec.needBy, locale) : "-"}</td>
                    <td className="py-2 pr-2">{rec.orderBy ? formatDate(rec.orderBy, locale) : "-"}</td>
                    <td className="py-2 pr-2">
                      <div className="text-erp-text-muted">{t("netting", { gross: formatNumber(rec.grossRequirement), stock: formatNumber(rec.stockUsed), receipts: formatNumber(rec.scheduledReceiptsUsed) })}</div>
                      <ul className="mt-1 space-y-0.5">
                        {(rec.reasons ?? []).map((reason, index) => (
                          <li key={`${reason.sourceRef}-${index}`}>
                            {isMrpReasonSource(reason.sourceType) ? t(`sources.${reason.sourceType}`) : "-"} <span className="font-mono">{reason.sourceRef}</span> · {formatNumber(reason.quantity)}
                          </li>
                        ))}
                      </ul>
                    </td>
                    <td className="py-2 pr-2">
                      <StatusBadge label={isMrpRecommendationStatus(rec.status) ? t(`statuses.${rec.status}`) : "-"} variant={mrpStatusVariant(rec.status)} />
                      {rec.converted?.number && (
                        <div className="mt-1 font-mono">
                          <Link href={rec.converted.type === "work_order" ? `/${locale}/production/work-orders/${rec.converted.id}` : `/${locale}/procurement/purchase-orders/${rec.converted.id}`} className="text-erp-navy underline">{rec.converted.number}</Link>
                        </div>
                      )}
                    </td>
                    <td className="py-2 text-right">
                      <div className="flex flex-wrap justify-end gap-2">
                        {rec.status === "proposed" && canApprove && rec.action !== "shortage" && (
                          <Button type="button" size="sm" variant="primary" className="min-h-11" disabled={busy} onClick={() => void decide(rec, "approve")}>{t("approve")}</Button>
                        )}
                        {rec.status === "proposed" && canApprove && (
                          <Button type="button" size="sm" variant="outline" className="min-h-11" disabled={busy} onClick={() => void decide(rec, "reject")}>{t("reject")}</Button>
                        )}
                        {rec.status === "approved" && canConvert && (
                          <Button type="button" size="sm" variant="primary" className="min-h-11" disabled={busy} onClick={() => openConvert(rec)}>{t("convert")}</Button>
                        )}
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      <Modal
        isOpen={target !== null}
        onClose={() => { if (!busy) setTarget(null); }}
        title={target ? t(target.action === "buy" ? "convertBuyTitle" : "convertMakeTitle") : undefined}
        description={target ? t(target.action === "buy" ? "convertBuyDescription" : "convertMakeDescription") : undefined}
        size="sm"
        closeDisabled={busy}
        closeOnOverlayClick={!busy}
        closeOnEscape={!busy}
        footer={(
          <div className="flex justify-end gap-3">
            <Button type="button" variant="outline" className="min-h-11" disabled={busy} onClick={() => setTarget(null)}>{tCommon("actions.cancel")}</Button>
            <Button type="button" variant="primary" className="min-h-11" isLoading={mutations.convert.isPending} disabled={busy} onClick={() => void confirmConvert()}>{t("confirmConvert")}</Button>
          </div>
        )}
      >
        <div className="space-y-4">
          {convertError && <Alert variant="danger" onClose={() => setConvertError(null)}>{convertError}</Alert>}
          {target?.action === "buy" ? (
            <>
              <Select
                label={t("supplier")}
                required
                value={supplierId}
                placeholder={t("selectSupplier")}
                disabled={busy}
                options={(suppliers.data?.items ?? []).map((s) => ({ value: s.id ?? "", label: `${s.code ?? "-"} · ${s.nameTh ?? "-"}` }))}
                onChange={(event) => setSupplierId(event.target.value)}
              />
              <Input type="number" min={0} step="0.01" label={t("unitPrice")} required value={unitPrice} disabled={busy} onChange={(event) => setUnitPrice(event.target.value)} />
            </>
          ) : (
            <Select
              label={t("warehouse")}
              required
              value={warehouseId}
              placeholder={t("selectWarehouse")}
              disabled={busy}
              options={(warehouses.data?.items ?? []).map((w) => ({ value: w.id ?? "", label: `${w.code ?? "-"} · ${w.name ?? "-"}` }))}
              onChange={(event) => setWarehouseId(event.target.value)}
            />
          )}
        </div>
      </Modal>
    </section>
  );
}
