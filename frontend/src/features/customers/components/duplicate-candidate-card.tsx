"use client";

import React from "react";
import Link from "next/link";
import { useTranslations } from "next-intl";
import type { CustomerResponse } from "@/lib/api/api-client";
import { IconAlertTriangle, IconEye } from "@/components/common/Icons";

type DuplicateCandidate = NonNullable<CustomerResponse["duplicateCandidates"]>[number];

export interface DuplicateCandidateCardProps {
  candidates: DuplicateCandidate[];
  createdCustomerHref?: string;
  onViewCandidate?: (candidateId: string) => void;
  onSelectExisting?: (candidateId: string) => void;
  isLiveAlert?: boolean;
}

export function DuplicateCandidateCard({
  candidates,
  createdCustomerHref,
  onViewCandidate,
  onSelectExisting,
  isLiveAlert = false,
}: DuplicateCandidateCardProps): React.JSX.Element | null {
  const t = useTranslations("customers");

  if (candidates.length === 0) {
    return null;
  }

  return (
    <div
      role="region"
      aria-label={t("duplicateCandidates")}
      className="erp-card p-4 md:p-5 border-erp-warning-border bg-erp-warning-bg border-l-4 border-l-erp-warning"
    >
      <div className="flex items-center gap-2 mb-2">
        <IconAlertTriangle size={20} className="text-erp-warning shrink-0" />
        <h3 className="text-base font-bold text-erp-warning">
          {isLiveAlert
            ? t("duplicateDetectedLive", { count: candidates.length })
            : t("duplicateCandidates")}
        </h3>
      </div>
      <p className="text-sm text-erp-warning-text mb-3">
        {t("duplicateNotice")}
      </p>

      <ul className="list-none p-0 m-0 flex flex-col gap-2">
        {candidates.map((candidate) => (
          <li
            key={candidate.id}
            className="p-3 bg-erp-surface border border-erp-border flex flex-col sm:flex-row sm:items-center justify-between gap-2 text-sm"
          >
            <div className="flex flex-col">
              <span className="font-semibold text-erp-navy">
                {candidate.code} — {candidate.displayNameTh}
              </span>
              <span className="text-xs text-erp-text-muted mt-0.5">
                {[candidate.maskedPhone, candidate.maskedEmail].filter(Boolean).join(" • ") || "-"}
              </span>
            </div>

            <div className="flex items-center gap-2 shrink-0">
              {onViewCandidate && candidate.id && (
                <button
                  type="button"
                  onClick={() => onViewCandidate(candidate.id!)}
                  className="inline-flex items-center gap-1.5 px-2.5 py-1 text-xs font-medium text-erp-navy bg-erp-surface border border-erp-border hover:bg-erp-surface-subtle transition-colors"
                >
                  <IconEye size={14} className="text-erp-navy" />
                  {t("viewInDrawer")}
                </button>
              )}
              {onSelectExisting && candidate.id && (
                <button
                  type="button"
                  onClick={() => onSelectExisting(candidate.id!)}
                  className="inline-flex items-center px-2.5 py-1 text-xs font-semibold text-white bg-erp-navy hover:bg-erp-navy-hover transition-colors"
                >
                  {t("useExistingCustomer")}
                </button>
              )}
            </div>
          </li>
        ))}
      </ul>

      {createdCustomerHref ? (
        <Link
          href={createdCustomerHref}
          className="inline-block mt-3 text-erp-navy text-sm font-semibold hover:underline"
        >
          {t("viewCreatedCustomer")}
        </Link>
      ) : null}
    </div>
  );
}
