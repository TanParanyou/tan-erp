"use client";

import React, { useState, useRef } from "react";
import { useTranslations } from "next-intl";
import { useTransitionOpportunityStage } from "../api/opportunity-queries";
import type { OpportunityResponse } from "@/lib/api/api-client";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import { Button } from "@/components/ui/Button";
import { Select } from "@/components/ui/Select";
import { Textarea } from "@/components/ui/Textarea";
import { Alert } from "@/components/ui/Alert";
import { IconAlertCircle } from "@/components/common/Icons";
import { Modal } from "@/components/ui/Modal";
import { getLostReasonOptions, getCancelledReasonOptions } from "../opportunity-labels";

interface OpportunityCloseModalProps {
  isOpen: boolean;
  onClose: () => void;
  opportunity: OpportunityResponse;
  targetStage?: "lost" | "cancelled";
  onSuccess?: () => void;
}

export function OpportunityCloseModal({
  isOpen,
  onClose,
  opportunity,
  targetStage: initialTargetStage = "lost",
  onSuccess,
}: OpportunityCloseModalProps) {
  const t = useTranslations("opportunities");
  const tCommon = useTranslations("common");
  const { toast } = useToast();
  const transitionMutation = useTransitionOpportunityStage();

  const [selectedStage, setSelectedStage] = useState<"lost" | "cancelled">(initialTargetStage);
  const [reasonCode, setReasonCode] = useState("");
  const [note, setNote] = useState("");
  const [errorMsg, setErrorMsg] = useState<string | null>(null);

  const idempotencyKeyRef = useRef<string>(crypto.randomUUID());

  const isLost = selectedStage === "lost";
  const title = t("closeOutcomeModalTitle");
  const desc = t("closeOutcomeModalDesc");

  const lostOptions = getLostReasonOptions(t);
  const cancelledOptions = getCancelledReasonOptions(t);
  const reasonOptions = isLost ? lostOptions : cancelledOptions;

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!reasonCode) {
      setErrorMsg(t("reasonRequired"));
      return;
    }

    if (!opportunity.id || !opportunity.rowVersion) {
      return;
    }

    setErrorMsg(null);
    try {
      await transitionMutation.mutateAsync({
        opportunityId: opportunity.id,
        targetStage: selectedStage,
        expectedVersion: opportunity.rowVersion,
        reasonCode,
        note: note.trim() || undefined,
        idempotencyKey: idempotencyKeyRef.current,
      });

      toast.success(isLost ? t("closeLostSuccess") : t("closeCancelledSuccess"));
      idempotencyKeyRef.current = crypto.randomUUID();
      onSuccess?.();
      onClose();
    } catch (err: unknown) {
      if (err instanceof ApiError) {
        if (err.status === 409 && err.code === "OPPORTUNITY_VERSION_CONFLICT") {
          setErrorMsg(t("errors.qualifyConflict"));
          toast.error(t("errors.qualifyConflict"));
          return;
        }
      }
      const message = err instanceof Error ? err.message : t("errors.saveUnexpected");
      setErrorMsg(message);
      toast.error(message);
    }
  };

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title={title}
      description={desc}
      size="lg"
    >
      <div className="flex flex-col gap-4">
        {errorMsg && (
          <Alert variant="danger" title={tCommon("feedback.operationFailed")}>
            <div className="flex items-center gap-2">
              <IconAlertCircle size={16} />
              <span>{errorMsg}</span>
            </div>
          </Alert>
        )}

        <form onSubmit={handleSubmit} className="flex flex-col gap-4">
          <Select
            label={t("closeOutcomeTypeLabel")}
            value={selectedStage}
            onChange={(e) => {
              setSelectedStage(e.target.value as "lost" | "cancelled");
              setReasonCode("");
              setErrorMsg(null);
            }}
            options={[
              { value: "lost", label: t("closeOutcomeLostOption") },
              { value: "cancelled", label: t("closeOutcomeCancelledOption") },
            ]}
            required
            disabled={transitionMutation.isPending}
          />

          <Select
            label={t("reasonCodeLabel")}
            value={reasonCode}
            onChange={(e) => {
              setReasonCode(e.target.value);
              setErrorMsg(null);
            }}
            options={[
              { value: "", label: t("reasonCodePlaceholder") },
              ...reasonOptions,
            ]}
            required
            disabled={transitionMutation.isPending}
          />

          <Textarea
            label={t("noteLabel")}
            placeholder={t("notePlaceholder")}
            value={note}
            onChange={(e) => setNote(e.target.value)}
            disabled={transitionMutation.isPending}
            rows={3}
          />

          <div className="flex justify-end gap-3 pt-2 border-t border-erp-border">
            <Button
              type="button"
              variant="outline"
              size="md"
              onClick={onClose}
              disabled={transitionMutation.isPending}
            >
              {tCommon("actions.cancel")}
            </Button>
            <Button
              type="submit"
              variant="danger"
              size="md"
              isLoading={transitionMutation.isPending}
              disabled={transitionMutation.isPending || !reasonCode}
            >
              {tCommon("actions.confirm")}
            </Button>
          </div>
        </form>
      </div>
    </Modal>
  );
}
