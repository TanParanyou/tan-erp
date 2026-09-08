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
      className="erp-card"
      style={{
        padding: "1.25rem",
        borderColor: "var(--erp-warning-border)",
        backgroundColor: "var(--erp-warning-bg)",
      }}
    >
      <h3
        style={{ fontSize: "1rem", fontWeight: 700, color: "var(--erp-warning)", margin: "0 0 0.5rem 0" }}
      >
        {t("duplicateCandidates")}
      </h3>
      <p style={{ fontSize: "0.875rem", color: "#78350F", margin: "0 0 0.75rem 0" }}>
        {t("duplicateNotice")}
      </p>
      <ul
        style={{
          listStyle: "none",
          padding: 0,
          margin: 0,
          display: "flex",
          flexDirection: "column",
          gap: "0.5rem",
        }}
      >
        {candidates.map((candidate) => (
          <li
            key={candidate.id}
            style={{
              padding: "0.5rem 0.75rem",
              backgroundColor: "var(--erp-surface)",
              border: "1px solid var(--erp-border)",
              fontSize: "0.8125rem",
              display: "flex",
              justifyContent: "space-between",
            }}
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
          style={{
            display: "inline-block",
            marginTop: "0.75rem",
            color: "var(--erp-navy)",
            fontSize: "0.875rem",
            fontWeight: 600,
          }}
        >
          {t("viewCreatedCustomer")}
        </Link>
      ) : null}
    </div>
  );
}
