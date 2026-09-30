"use client";

import React from "react";
import { useTranslations } from "next-intl";
import { IconEye } from "@/components/common/Icons";

export interface DuplicateCandidateRowProps {
  id: string | null | undefined;
  code: string | null | undefined;
  displayName: string | null | undefined;
  maskedPhone: string | null | undefined;
  maskedEmail: string | null | undefined;
  onView?: (candidateId: string) => void;
  onSelect?: (candidateId: string) => void;
  disabled?: boolean;
}

export function DuplicateCandidateRow({
  id,
  code,
  displayName,
  maskedPhone,
  maskedEmail,
  onView,
  onSelect,
  disabled = false,
}: DuplicateCandidateRowProps): React.JSX.Element {
  const t = useTranslations("customers");
  const contactSummary = [maskedPhone, maskedEmail].filter(Boolean).join(" • ") || "-";

  return (
    <li className="p-3 bg-erp-surface border border-erp-border flex flex-col sm:flex-row sm:items-center justify-between gap-2 text-sm">
      <div className="flex flex-col">
        <span className="font-semibold text-erp-navy">{code} — {displayName}</span>
        <span className="text-xs text-erp-text-muted mt-0.5">{contactSummary}</span>
      </div>
      <div className="flex items-center gap-2 shrink-0">
        {id && onView && (
          <button type="button" onClick={() => onView(id)} disabled={disabled} className="inline-flex items-center gap-1.5 px-2.5 py-1 text-xs font-medium text-erp-navy bg-erp-surface border border-erp-border hover:bg-erp-surface-subtle disabled:cursor-not-allowed disabled:opacity-50 transition-colors">
            <IconEye size={14} className="text-erp-navy" />{t("viewInDrawer")}
          </button>
        )}
        {id && onSelect && (
          <button type="button" onClick={() => onSelect(id)} disabled={disabled} className="inline-flex items-center px-2.5 py-1 text-xs font-semibold text-white bg-erp-navy hover:bg-erp-navy-hover disabled:cursor-not-allowed disabled:opacity-50 transition-colors">
            {t("useExistingCustomer")}
          </button>
        )}
      </div>
    </li>
  );
}
