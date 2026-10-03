"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { useLocale, useTranslations } from "next-intl";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { PageHeader } from "@/components/layout/PageHeader";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import { formatDateTime, formatNumber } from "@/lib/formatters/formatters";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import type { BomRevisionAction, BomResponse, BomRevisionResponse } from "@/lib/api/api-client";
import { useBom, useBomMutations } from "../api/production-queries";
import { bomRevisionStatusVariant, isBomRevisionStatus, productionErrorCode } from "../production-status";

interface BomDetailProps {
  bomId: string;
}

export function BomDetail({ bomId }: BomDetailProps) {
  const query = useBom(bomId);
  const t = useTranslations("production.boms");

  if (query.isLoading) return <div className="py-16 flex justify-center" role="status"><MonoSpinner size="md" /></div>;
  if (query.isError || !query.data) return <Alert variant="danger">{t("loadError")}</Alert>;
  return <DetailView bom={query.data} />;
}

function DetailView({ bom }: { bom: BomResponse }) {
  const t = useTranslations("production.boms");
  const tErrors = useTranslations("production.errors");
  const locale = useLocale();
  const router = useRouter();
  const { toast } = useToast();
  const { selectedMembership } = useSelectedMembership();
  const canManage = can(selectedMembership, PERMISSIONS.BOMS_MANAGE);
  const canApprove = can(selectedMembership, PERMISSIONS.BOMS_APPROVE);
  const mutations = useBomMutations();
  const [message, setMessage] = useState<string | null>(null);

  const revisions = bom.revisions ?? [];
  const hasDraft = revisions.some((r) => r.status === "draft");
  const latest = revisions[0];
  const busy = mutations.act.isPending || mutations.createRevision.isPending;

  function describe(err: unknown): string {
    const code = err instanceof ApiError ? productionErrorCode(err.code) : null;
    return code ? tErrors(code) : tErrors("failed");
  }

  async function run(revision: BomRevisionResponse, action: BomRevisionAction): Promise<void> {
    setMessage(null);
    try {
      await mutations.act.mutateAsync({ id: bom.id ?? "", revisionId: revision.id ?? "", rowVersion: revision.rowVersion ?? "", action });
      toast.success(t(`actionDone.${action}`));
    } catch (err: unknown) {
      setMessage(describe(err));
    }
  }

  // A new draft starts as a copy of the latest revision so only the changes need to be entered.
  async function startRevision(): Promise<void> {
    setMessage(null);
    if (!latest) return;
    try {
      const saved = await mutations.createRevision.mutateAsync({
        id: bom.id ?? "",
        payload: {
          outputQuantity: latest.outputQuantity ?? 1,
          note: latest.note ?? null,
          lines: (latest.lines ?? []).map((line) => ({ componentItemId: line.component?.id ?? "", quantity: line.quantity ?? 1, scrapPercent: line.scrapPercent ?? 0 })),
        },
      });
      const draft = (saved.revisions ?? []).find((r) => r.status === "draft");
      if (draft) router.push(`/${locale}/production/boms/${bom.id}/revisions/${draft.id}`);
    } catch (err: unknown) {
      setMessage(describe(err));
    }
  }

  return (
    <section className="space-y-5">
      <PageHeader
        title={bom.code ?? "-"}
        subtitle={`${bom.item?.code ?? "-"} · ${bom.item?.nameTh ?? "-"}`}
        breadcrumbs={[{ label: t("title"), href: `/${locale}/production/boms` }, { label: bom.code ?? "-" }]}
        actions={canManage && !hasDraft ? <Button type="button" variant="outline" className="min-h-11" isLoading={mutations.createRevision.isPending} disabled={busy} onClick={() => void startRevision()}>{t("newRevision")}</Button> : undefined}
      />
      {message && <Alert variant="danger" onClose={() => setMessage(null)}>{message}</Alert>}

      {revisions.map((revision) => (
        <div key={revision.id} className="erp-card p-5 space-y-3" role="region" aria-label={t("revisionTitle", { revision: revision.revisionNo ?? 0 })}>
          <div className="flex flex-wrap items-center justify-between gap-3">
            <h3 className="text-sm font-bold text-erp-navy">
              {t("revisionTitle", { revision: revision.revisionNo ?? 0 })}{" "}
              <StatusBadge label={isBomRevisionStatus(revision.status) ? t(`statuses.${revision.status}`) : "-"} variant={bomRevisionStatusVariant(revision.status)} />
            </h3>
            <div className="flex flex-wrap gap-2">
              {revision.status === "draft" && canManage && (
                <Button href={`/${locale}/production/boms/${bom.id}/revisions/${revision.id}`} variant="outline" className="min-h-11">{t("editDraft")}</Button>
              )}
              {revision.status === "draft" && canApprove && (
                <Button type="button" variant="primary" className="min-h-11" disabled={busy} onClick={() => void run(revision, "approve")}>{t("approve")}</Button>
              )}
              {revision.status === "approved" && canManage && (
                <Button type="button" variant="outline" className="min-h-11" disabled={busy} onClick={() => void run(revision, "obsolete")}>{t("obsolete")}</Button>
              )}
            </div>
          </div>
          <dl className="grid grid-cols-1 gap-3 text-sm sm:grid-cols-2 lg:grid-cols-4">
            <div><dt className="text-xs text-erp-text-muted">{t("outputQuantity")}</dt><dd className="font-mono font-medium">{formatNumber(revision.outputQuantity)}</dd></div>
            <div><dt className="text-xs text-erp-text-muted">{t("createdBy")}</dt><dd className="font-medium">{revision.createdBy?.displayName ?? "-"}</dd></div>
            <div><dt className="text-xs text-erp-text-muted">{t("approvedBy")}</dt><dd className="font-medium">{revision.approvedBy?.displayName ?? "-"}{revision.approvedAtUtc ? ` · ${formatDateTime(revision.approvedAtUtc, locale)}` : ""}</dd></div>
            <div><dt className="text-xs text-erp-text-muted">{t("note")}</dt><dd className="font-medium">{revision.note ?? "-"}</dd></div>
          </dl>
          <div className="overflow-x-auto">
            <table className="w-full text-left text-xs">
              <thead>
                <tr className="border-b border-erp-border">
                  <th className="py-2 pr-2">{t("component")}</th>
                  <th className="py-2 pr-2 text-right">{t("quantity")}</th>
                  <th className="py-2 pr-2 text-right">{t("scrapPercent")}</th>
                  <th className="py-2 text-right">{t("grossQuantity")}</th>
                </tr>
              </thead>
              <tbody>
                {(revision.lines ?? []).map((line) => (
                  <tr key={line.id} className="border-b border-erp-border/60">
                    <td className="py-2 pr-2">{line.component?.code ?? "-"} · {line.component?.nameTh ?? "-"} ({line.component?.unitCode ?? "-"})</td>
                    <td className="py-2 pr-2 text-right font-mono">{formatNumber(line.quantity)}</td>
                    <td className="py-2 pr-2 text-right font-mono">{formatNumber(line.scrapPercent)}</td>
                    <td className="py-2 text-right font-mono">{formatNumber(line.grossQuantity)}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      ))}
    </section>
  );
}
