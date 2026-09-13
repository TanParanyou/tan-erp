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

interface OpportunityReopenModalProps {
  isOpen: boolean;
  onClose: () => void;
  opportunity: OpportunityResponse;
  onSuccess?: () => void;
}

export function OpportunityReopenModal({
  isOpen,
  onClose,
  opportunity,
  onSuccess,
}: OpportunityReopenModalProps) {
  const t = useTranslations("opportunities");
  const tCommon = useTranslations("common");
  const { toast } = useToast();
  const transitionMutation = useTransitionOpportunityStage();

  const [reasonCode, setReasonCode] = useState("");
  const [note, setNote] = useState("");
  const [errorMsg, setErrorMsg] = useState<string | null>(null);

  const idempotencyKeyRef = useRef<string>(crypto.randomUUID());

  if (!isOpen) return null;

  const reopenOptions = [
    { value: "reopen_customer_reengaged", label: t("reasons.reopen_customer_reengaged") },
    { value: "reopen_budget_adjusted", label: t("reasons.reopen_budget_adjusted") },
    { value: "reopen_scope_redefined", label: t("reasons.reopen_scope_redefined") },
    { value: "reopen_erroneous_closure", label: t("reasons.reopen_erroneous_closure") },
    { value: "reopen_other", label: t("reasons.reopen_other") },
  ];

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
      // Reopen to qualified if scope and work types are complete, else draft
      const targetStage = opportunity.scopeSummary && opportunity.workTypes?.length ? "qualified" : "draft";

      await transitionMutation.mutateAsync({
        opportunityId: opportunity.id,
        targetStage,
        expectedVersion: opportunity.rowVersion,
        reasonCode,
        note: note.trim() || undefined,
        idempotencyKey: idempotencyKeyRef.current,
      });

      toast.success(t("reopenSuccess"));
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
      aria-labelledby="reopen-modal-title"
      className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/50 backdrop-blur-xs"
    >
      <div
        className="w-full max-w-lg bg-erp-surface border border-erp-border p-6 flex flex-col gap-4 shadow-xl"
        style={{ borderRadius: "0px" }}
      >
        <div className="flex flex-col gap-1">
          <h2 id="reopen-modal-title" className="text-lg font-bold text-erp-navy">
            {t("reopenModalTitle")}
          </h2>
          <p className="text-sm text-erp-text-muted">
            {t("reopenModalDesc")}
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
            label={t("reasonCodeLabel")}
            value={reasonCode}
            onChange={(e) => {
              setReasonCode(e.target.value);
              setErrorMsg(null);
            }}
            options={[
              { value: "", label: t("reasonCodePlaceholder") },
              ...reopenOptions,
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
              variant="primary"
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
