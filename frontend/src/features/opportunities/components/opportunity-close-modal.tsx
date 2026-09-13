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

  if (!isOpen) return null;

  const isLost = selectedStage === "lost";
  const title = t("closeOutcomeModalTitle");
  const desc = t("closeOutcomeModalDesc");

  const lostOptions = [
    { value: "lost_price_too_high", label: t("reasons.lost_price_too_high") },
    { value: "lost_competitor_selected", label: t("reasons.lost_competitor_selected") },
    { value: "lost_scope_mismatch", label: t("reasons.lost_scope_mismatch") },
    { value: "lost_timeline_unfeasible", label: t("reasons.lost_timeline_unfeasible") },
    { value: "lost_no_response", label: t("reasons.lost_no_response") },
    { value: "lost_other", label: t("reasons.lost_other") },
  ];

  const cancelledOptions = [
    { value: "cancelled_customer_abandoned", label: t("reasons.cancelled_customer_abandoned") },
    { value: "cancelled_duplicate", label: t("reasons.cancelled_duplicate") },
    { value: "cancelled_invalid_lead", label: t("reasons.cancelled_invalid_lead") },
    { value: "cancelled_other", label: t("reasons.cancelled_other") },
  ];

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
    <div
      role="dialog"
      aria-modal="true"
      aria-labelledby="close-modal-title"
      className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/50 backdrop-blur-xs"
    >
      <div
        className="w-full max-w-lg bg-erp-surface border border-erp-border p-6 flex flex-col gap-4 shadow-xl"
        style={{ borderRadius: "0px" }}
      >
        <div className="flex flex-col gap-1">
          <h2 id="close-modal-title" className="text-lg font-bold text-erp-danger">
            {title}
          </h2>
          <p className="text-sm text-erp-text-muted">
            {desc}
          </p>
        </div>

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
    </div>
  );
}
