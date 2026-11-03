"use client";

import React from "react";
import { useLocale, useTranslations } from "next-intl";
import { Modal } from "@/components/ui/Modal";
import { Alert } from "@/components/ui/Alert";
import { Badge } from "@/components/ui/Badge";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/Table";
import { useEstimateHistory } from "../api/estimate-queries";
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

/** Revision-over-revision totals and the estimate's event trail; the line-level diff is not available. */
export function EstimateHistoryModal({ isOpen, onClose, estimateId }: EstimateHistoryModalProps) {
  const t = useTranslations("estimates");
  const locale = useLocale();
  const { data, isLoading, isError } = useEstimateHistory(estimateId, isOpen);
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
