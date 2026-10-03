"use client";

import React from "react";
import { useTranslations } from "next-intl";
import { Modal } from "@/components/ui/Modal";
import { Button } from "@/components/ui/Button";
import { IconAlertTriangle } from "@/components/common/Icons";
import type { DuplicateCustomerResponse } from "@/lib/api/api-client";
import { DuplicateCandidateRow } from "./duplicate-candidate-row";

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
      closeDisabled={isLoading}
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
            disabled={isLoading}
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
            {candidates.map((candidate) => <DuplicateCandidateRow
              key={candidate.id}
              id={candidate.id}
              code={candidate.code}
              displayName={candidate.displayNameTh}
              maskedPhone={candidate.maskedPhone}
              maskedEmail={candidate.maskedEmail}
              onView={onViewCandidate}
              onSelect={onSelectExisting}
              disabled={isLoading}
            />)}
          </ul>
        </div>
      </div>
    </Modal>
  );
}
