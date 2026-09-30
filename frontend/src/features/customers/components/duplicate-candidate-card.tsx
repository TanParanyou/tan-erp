"use client";

import React from "react";
import Link from "next/link";
import { useTranslations } from "next-intl";
import type { CustomerResponse } from "@/lib/api/api-client";
import { IconAlertTriangle } from "@/components/common/Icons";
import { DuplicateCandidateRow } from "./duplicate-candidate-row";

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
        {candidates.map((candidate) => <DuplicateCandidateRow
          key={candidate.id}
          id={candidate.id}
          code={candidate.code}
          displayName={candidate.displayNameTh}
          maskedPhone={candidate.maskedPhone}
          maskedEmail={candidate.maskedEmail}
          onView={onViewCandidate}
          onSelect={onSelectExisting}
        />)}
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
