"use client";

import React from "react";
import { useTranslations } from "next-intl";
import { Modal } from "@/components/ui/Modal";
import { Button } from "@/components/ui/Button";
import { IconAlertTriangle, IconEye } from "@/components/common/Icons";
import type { DuplicateCustomerResponse } from "@/lib/api/api-client";

export interface DuplicateConfirmationModalProps {
  isOpen: boolean;
  onClose: () => void;
  onConfirm: () => void;
  onViewCandidate: (candidateId: string) => void;
  onSelectExisting: (candidateId: string) => void;
  candidates: DuplicateCustomerResponse[];
  isLoading?: boolean;
}

export function DuplicateConfirmationModal({
  isOpen,
  onClose,
  onConfirm,
  onViewCandidate,
  onSelectExisting,
  candidates,
  isLoading = false,
}: DuplicateConfirmationModalProps): React.JSX.Element {
  const t = useTranslations("customers");

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title={t("duplicateConfirmTitle")}
      size="lg"
      footer={
        <div className="flex flex-col-reverse sm:flex-row items-center justify-end gap-3 w-full">
          <Button
            type="button"
            variant="secondary"
            onClick={onClose}
            disabled={isLoading}
            className="w-full sm:w-auto"
          >
            {t("cancelSave")}
          </Button>
          <Button
            type="button"
            variant="primary"
            onClick={onConfirm}
            isLoading={isLoading}
            className="w-full sm:w-auto font-semibold"
          >
            {t("confirmCreateNewCustomer")}
          </Button>
        </div>
      }
    >
      <div className="space-y-4">
        <div className="flex items-start gap-3 p-3.5 bg-erp-warning-bg border border-erp-warning-border">
          <IconAlertTriangle size={20} className="text-erp-warning shrink-0 mt-0.5" />
          <p className="text-sm text-erp-warning-text leading-relaxed">
            {t("duplicateConfirmDesc")}
          </p>
        </div>

        <div className="space-y-2">
          <h4 className="text-xs font-bold uppercase tracking-wider text-erp-text-muted">
            {t("duplicateCandidates")} ({candidates.length})
          </h4>

          <ul className="list-none p-0 m-0 space-y-2 max-h-60 overflow-y-auto">
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
                  {candidate.id && (
                    <button
                      type="button"
                      onClick={() => onViewCandidate(candidate.id!)}
                      className="inline-flex items-center gap-1.5 px-2.5 py-1 text-xs font-medium text-erp-navy bg-erp-surface border border-erp-border hover:bg-erp-surface-subtle transition-colors"
                    >
                      <IconEye size={14} className="text-erp-navy" />
                      {t("viewInDrawer")}
                    </button>
                  )}

                  {candidate.id && (
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
        </div>
      </div>
    </Modal>
  );
}
