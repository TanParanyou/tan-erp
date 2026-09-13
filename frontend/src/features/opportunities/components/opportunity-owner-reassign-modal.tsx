"use client";

import React, { useState, useRef } from "react";
import { useTranslations } from "next-intl";
import { useReassignOpportunityOwner } from "../api/opportunity-queries";
import type { OpportunityResponse } from "@/lib/api/api-client";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import { Button } from "@/components/ui/Button";
import { UserAutocomplete } from "@/components/forms/UserAutocomplete";
import { Alert } from "@/components/ui/Alert";
import { IconAlertCircle } from "@/components/common/Icons";

import { Modal } from "@/components/ui/Modal";

interface OpportunityOwnerReassignModalProps {
  isOpen: boolean;
  onClose: () => void;
  opportunity: OpportunityResponse;
  onSuccess?: () => void;
}

export function OpportunityOwnerReassignModal({
  isOpen,
  onClose,
  opportunity,
  onSuccess,
}: OpportunityOwnerReassignModalProps) {
  const t = useTranslations("opportunities");
  const tCommon = useTranslations("common");
  const { toast } = useToast();
  const reassignMutation = useReassignOpportunityOwner();

  const [targetOwnerUserId, setTargetOwnerUserId] = useState("");
  const [errorMsg, setErrorMsg] = useState<string | null>(null);

  const idempotencyKeyRef = useRef<string>(crypto.randomUUID());

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!targetOwnerUserId.trim()) {
      setErrorMsg(t("userSelectionRequired"));
      return;
    }

    if (!opportunity.id || !opportunity.rowVersion) {
      return;
    }

    setErrorMsg(null);
    try {
      await reassignMutation.mutateAsync({
        opportunityId: opportunity.id,
        expectedVersion: opportunity.rowVersion,
        targetOwnerUserId: targetOwnerUserId.trim(),
        idempotencyKey: idempotencyKeyRef.current,
      });

      toast.success(t("reassignSuccess"));
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
      title={t("reassignOwnerModalTitle")}
      description={t("reassignOwnerModalDesc")}
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
          <UserAutocomplete
            value={targetOwnerUserId}
            onChange={(userId) => {
              setTargetOwnerUserId(userId);
              setErrorMsg(null);
            }}
            branchId={opportunity.branchId || undefined}
            required
            disabled={reassignMutation.isPending}
            label={t("newOwnerLabel")}
            placeholder={t("newOwnerPlaceholder")}
          />

          <div className="flex justify-end gap-3 pt-2 border-t border-erp-border">
            <Button
              type="button"
              variant="outline"
              size="md"
              onClick={onClose}
              disabled={reassignMutation.isPending}
            >
              {tCommon("actions.cancel")}
            </Button>
            <Button
              type="submit"
              variant="primary"
              size="md"
              isLoading={reassignMutation.isPending}
              disabled={reassignMutation.isPending || !targetOwnerUserId.trim()}
            >
              {tCommon("actions.confirm")}
            </Button>
          </div>
        </form>
      </div>
    </Modal>
  );
}
