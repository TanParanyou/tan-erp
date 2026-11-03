"use client";

import React, { useState } from "react";
import { useLocale, useTranslations } from "next-intl";
import { Modal } from "@/components/ui/Modal";
import { Alert } from "@/components/ui/Alert";
import { Badge } from "@/components/ui/Badge";
import { Select } from "@/components/ui/Select";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/Table";
import { useEstimateHistory, useEstimateRevisionLines } from "../api/estimate-queries";
import { diffRevisionLines, type RevisionDiffKind } from "../utils/estimate-revision-diff";
import { formatFinancialNumber, formatPercentRate } from "../utils/estimate-formatters";

const KNOWN_ACTIONS = ["created", "draft_updated", "calculated", "submitted", "approval_step_approved", "approved", "returned", "revision_created", "cancelled"] as const;
type KnownAction = (typeof KNOWN_ACTIONS)[number];
const KNOWN_STATUSES = ["draft", "submitted", "returned", "quoted", "approved", "cancelled", "superseded", "rejected"];
const knownAction = (action: string): KnownAction | null => KNOWN_ACTIONS.find((candidate) => `estimates.${candidate}` === action) ?? null;

export interface EstimateHistoryModalProps {
  isOpen: boolean;
  onClose: () => void;
  estimateId: string;
}

/** Revision-over-revision totals, a line-level comparison of two revisions, and the estimate's event trail. */
export function EstimateHistoryModal({ isOpen, onClose, estimateId }: EstimateHistoryModalProps) {
  const t = useTranslations("estimates");
  const locale = useLocale();
  const { data, isLoading, isError } = useEstimateHistory(estimateId, isOpen);
  const [fromId, setFromId] = useState<string | null>(null);
  const [toId, setToId] = useState<string | null>(null);
  const revisions = data?.revisions ?? [];
  const effectiveToId = toId ?? revisions[revisions.length - 1]?.id ?? null;
  const effectiveFromId = fromId ?? revisions[revisions.length - 2]?.id ?? null;
  const canCompare = Boolean(effectiveFromId && effectiveToId && effectiveFromId !== effectiveToId);
  const fromLines = useEstimateRevisionLines(estimateId, effectiveFromId, isOpen && canCompare);
  const toLines = useEstimateRevisionLines(estimateId, effectiveToId, isOpen && canCompare);
  const diffRows = fromLines.data && toLines.data ? diffRevisionLines(fromLines.data.lines, toLines.data.lines) : [];
  const revisionOptions = revisions.map((revision) => ({ value: revision.id, label: t("revision", { number: revision.revisionNo }) }));
  const kindVariant: Record<RevisionDiffKind, "success" | "danger" | "warning"> = { added: "success", removed: "danger", changed: "warning" };
  const change = (from: number | undefined, to: number | undefined) =>
    `${from === undefined ? "-" : formatFinancialNumber(from)} → ${to === undefined ? "-" : formatFinancialNumber(to)}`;
  const dateTime = new Intl.DateTimeFormat(locale, { dateStyle: "medium", timeStyle: "short" });
  const delta = (current: number, previous: number | undefined) =>
    previous === undefined ? "-" : `${current - previous >= 0 ? "+" : ""}${formatFinancialNumber(current - previous)}`;

  return (
    <Modal isOpen={isOpen} onClose={onClose} title={t("history.title")} description={t("history.description")} size="xl">
      <div className="space-y-5">
        {isLoading && <p className="text-sm text-erp-text-muted">{t("history.loading")}</p>}
        {isError && <Alert variant="danger">{t("history.loadFailed")}</Alert>}
        {data && (
          <>
            <section aria-label={t("history.revisions")}>
              <h3 className="mb-2 text-sm font-bold text-erp-navy">{t("history.revisions")}</h3>
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>{t("history.revision")}</TableHead>
                    <TableHead className="text-right">{t("netBeforeTax")}</TableHead>
                    <TableHead className="text-right">{t("history.changeFromPrevious")}</TableHead>
                    <TableHead className="text-right">{t("marginRate")}</TableHead>
                    <TableHead className="text-right">{t("grandTotal")}</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {data.revisions.map((revision, index) => {
                    const previous = index > 0 ? data.revisions[index - 1] : undefined;
                    return (
                      <TableRow key={revision.id}>
                        <TableCell>
                          <span className="font-mono font-semibold">{t("revision", { number: revision.revisionNo })}</span>
                          <Badge variant="neutral" className="ml-2">{KNOWN_STATUSES.includes(revision.status) ? t(`statuses.${revision.status}`) : "-"}</Badge>
                        </TableCell>
                        <TableCell className="text-right font-mono">{formatFinancialNumber(revision.netBeforeTax)}</TableCell>
                        <TableCell className="text-right font-mono">{delta(revision.netBeforeTax, previous?.netBeforeTax)}</TableCell>
                        <TableCell className="text-right font-mono">{formatPercentRate(revision.marginRate * 100)}</TableCell>
                        <TableCell className="text-right font-mono">{formatFinancialNumber(revision.grandTotal)} {revision.currency}</TableCell>
                      </TableRow>
                    );
                  })}
                </TableBody>
              </Table>
            </section>
            <section aria-label={t("history.lineDiff")}>
              <h3 className="mb-2 text-sm font-bold text-erp-navy">{t("history.lineDiff")}</h3>
              {revisions.length < 2 ? <p className="text-sm text-erp-text-muted">{t("history.needTwoRevisions")}</p> : (
                <div className="space-y-3">
                  <div className="grid gap-3 sm:grid-cols-2">
                    <Select id="history-compare-from" label={t("history.compareFrom")} value={effectiveFromId ?? ""} options={revisionOptions} onChange={(event) => setFromId(event.target.value)} wrapperClassName="mb-0" />
                    <Select id="history-compare-to" label={t("history.compareTo")} value={effectiveToId ?? ""} options={revisionOptions} onChange={(event) => setToId(event.target.value)} wrapperClassName="mb-0" />
                  </div>
                  {!canCompare && <p className="text-sm text-erp-text-muted">{t("history.pickDifferent")}</p>}
                  {canCompare && (fromLines.isLoading || toLines.isLoading) && <p className="text-sm text-erp-text-muted">{t("history.loading")}</p>}
                  {canCompare && (fromLines.isError || toLines.isError) && <Alert variant="danger">{t("history.loadFailed")}</Alert>}
                  {canCompare && fromLines.data && toLines.data && (diffRows.length === 0 ? <p className="text-sm text-erp-text-muted">{t("history.noLineChanges")}</p> : (
                    <Table wrapperClassName="min-w-[640px]" className="text-xs">
                      <TableHeader>
                        <TableRow>
                          <TableHead>{t("history.lineItem")}</TableHead>
                          <TableHead>{t("history.lineChange")}</TableHead>
                          <TableHead className="text-right">{t("history.lineQuantity")}</TableHead>
                          <TableHead className="text-right">{t("totalCost")}</TableHead>
                          <TableHead className="text-right">{t("totalSellingPrice")}</TableHead>
                        </TableRow>
                      </TableHeader>
                      <TableBody>
                        {diffRows.map((row) => (
                          <TableRow key={row.key}>
                            <TableCell><span className="font-mono">{row.sectionCode} · {row.workItemCode}</span><span className="block text-erp-text-muted">{row.descriptionTh}</span></TableCell>
                            <TableCell><Badge variant={kindVariant[row.kind]}>{t(`history.kinds.${row.kind}`)}</Badge></TableCell>
                            <TableCell className="text-right font-mono">{change(row.from?.quantity, row.to?.quantity)} {row.unitCode}</TableCell>
                            <TableCell className="text-right font-mono">{change(row.from?.totalCost, row.to?.totalCost)}</TableCell>
                            <TableCell className="text-right font-mono">{change(row.from?.totalSellingPrice, row.to?.totalSellingPrice)}</TableCell>
                          </TableRow>
                        ))}
                      </TableBody>
                    </Table>
                  ))}
                </div>
              )}
            </section>
            <section aria-label={t("history.events")}>
              <h3 className="mb-2 text-sm font-bold text-erp-navy">{t("history.events")}</h3>
              {data.events.length === 0 ? <p className="text-sm text-erp-text-muted">{t("history.noEvents")}</p> : (
                <ul className="divide-y divide-erp-border border border-erp-border">
                  {data.events.map((event, index) => {
                    const action = knownAction(event.action);
                    return (
                      <li key={`${event.occurredAtUtc}-${index}`} className="flex flex-wrap items-baseline justify-between gap-2 px-3 py-2 text-sm">
                        <span className="font-medium text-erp-text-main">{action ? t(`history.actions.${action}`) : event.action}</span>
                        <span className="text-xs text-erp-text-muted">{event.actor.displayName} · {dateTime.format(new Date(event.occurredAtUtc))}</span>
                      </li>
                    );
                  })}
                </ul>
              )}
            </section>
          </>
        )}
      </div>
    </Modal>
  );
}
