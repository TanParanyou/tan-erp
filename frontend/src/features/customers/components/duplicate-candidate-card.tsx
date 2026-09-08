"use client";

import React from "react";
import Link from "next/link";
import { useTranslations } from "next-intl";
import type { CustomerResponse } from "@/lib/api/api-client";

type DuplicateCandidate = NonNullable<CustomerResponse["duplicateCandidates"]>[number];

interface DuplicateCandidateCardProps {
  candidates: DuplicateCandidate[];
  createdCustomerHref?: string;
}

export function DuplicateCandidateCard({
  candidates,
  createdCustomerHref,
}: DuplicateCandidateCardProps): React.JSX.Element | null {
  const t = useTranslations("customers");

  if (candidates.length === 0) {
    return null;
  }

  return (
    <div
      role="region"
      aria-label={t("duplicateCandidates")}
      className="erp-card p-5 border-erp-warning-border bg-erp-warning-bg"
    >
      <h3 className="text-base font-bold text-erp-warning mb-2">
        {t("duplicateCandidates")}
      </h3>
      <p className="text-sm text-erp-warning-text mb-3">
        {t("duplicateNotice")}
      </p>
      <ul className="list-none p-0 m-0 flex flex-col gap-2">
        {candidates.map((candidate) => (
          <li
            key={candidate.id}
            className="px-3 py-2 bg-erp-surface border border-erp-border text-[0.8125rem] flex justify-between"
          >
            <strong>
              {candidate.code} - {candidate.displayNameTh}
            </strong>
            <span>{candidate.maskedPhone ?? candidate.maskedEmail}</span>
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
