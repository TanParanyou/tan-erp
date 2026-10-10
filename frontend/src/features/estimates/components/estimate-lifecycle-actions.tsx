"use client";

import { useState } from "react";
import { useTranslations } from "next-intl";
import type { EstimateDetailResponse } from "@/lib/api/api-client";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Modal } from "@/components/ui/Modal";
import { Textarea } from "@/components/ui/Textarea";
import { useToast } from "@/hooks/useToast";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import { ApiError } from "@/lib/api/api-error";
import {
  useCancelEstimate,
  useCreateEstimateRevision,
  useReviewEstimate,
  useSubmitEstimate,
} from "../api/estimate-queries";

type LifecycleAction = "submit" | "approve" | "return" | "revision" | "cancel";

interface EstimateLifecycleActionsProps {
  estimate: EstimateDetailResponse;
  opportunityId?: string;
  canEdit?: boolean;
  onOpenWorkspace?: () => void;
}

export function EstimateLifecycleActions({ estimate, opportunityId, canEdit = false, onOpenWorkspace }: EstimateLifecycleActionsProps) {
  const t = useTranslations("estimates.lifecycle");
  const tEstimate = useTranslations("estimates");
  const tCommon = useTranslations("common");
  const { toast } = useToast();
  const { selectedMembership } = useSelectedMembership();
  const [activeAction, setActiveAction] = useState<LifecycleAction | null>(null);
  const [reason, setReason] = useState("");
  const [reasonCode, setReasonCode] = useState("");
  const [reasonError, setReasonError] = useState(false);

  const revision = estimate.currentRevision;
  const revisionStatus = revision?.status;
  const estimateId = estimate.id ?? "";
  const contextOpportunityId = opportunityId ?? "";
  const submitMutation = useSubmitEstimate(contextOpportunityId, estimateId);
  const reviewMutation = useReviewEstimate(contextOpportunityId, estimateId);
  const revisionMutation = useCreateEstimateRevision(contextOpportunityId, estimateId);
  const cancelMutation = useCancelEstimate(contextOpportunityId, estimateId);
  const isPending = submitMutation.isPending || reviewMutation.isPending || revisionMutation.isPending || cancelMutation.isPending;

  const canSubmit = can(selectedMembership, "estimates.submit");
  const canReview = can(selectedMembership, "estimates.approve");
  const canRevise = can(selectedMembership, "estimates.revise");
  const canCancel = can(selectedMembership, "estimates.cancel");
  const hasCurrentCalculation = Boolean(
    revision && typeof revision.calculationVersion === "number" && revision.calculationVersion > 0 &&
    revision.calculationSnapshotJson && revision.calculationOutdated === false
  );
  const isSubmitReadinessAllowed = revision?.readiness === "ready" || revision?.readiness === "requiresAttention";
  const showSubmit = Boolean(canSubmit && hasCurrentCalculation && isSubmitReadinessAllowed && (revisionStatus === "draft" || revisionStatus === "returned"));
  const showApprove = Boolean(canReview && revisionStatus === "submitted");
  const showRevision = Boolean(canRevise && (revisionStatus === "approved" || revisionStatus === "quoted"));
  const showCancel = Boolean(canCancel && (revisionStatus === "draft" || revisionStatus === "returned" || revisionStatus === "submitted"));
  const showNextStep = revisionStatus === "draft" || revisionStatus === "returned";
  const nextStepMessage = !canSubmit
    ? t("submitPermissionRequired")
    : !hasCurrentCalculation
      ? t("submitCalculationRequired")
      : revision?.readiness === "blocked"
        ? t("submitReadinessBlocked")
        : t("submitUnavailable");

  function openAction(action: LifecycleAction): void {
    setReason("");
    setReasonCode("");
    setReasonError(false);
    setActiveAction(action);
  }

  function closeAction(): void {
    if (isPending) return;
    setActiveAction(null);
    setReason("");
    setReasonCode("");
    setReasonError(false);
  }

  async function handleConfirm(): Promise<void> {
    if (!revision || !estimate.rowVersion || !activeAction) return;
    if ((activeAction === "revision" || activeAction === "cancel") && !reason.trim()) {
      setReasonError(true);
      return;
    }
    if (activeAction === "return" && (!reasonCode.trim() || !reason.trim())) {
      setReasonError(true);
      return;
    }

    try {
      if (activeAction === "submit") {
        await submitMutation.mutateAsync({
          payload: { revisionNo: revision.revisionNo, calculationVersion: revision.calculationVersion },
          ifMatch: estimate.rowVersion,
        });
        toast.success(t("submitSuccess"));
      } else if (activeAction === "approve") {
        await reviewMutation.mutateAsync({
          payload: { revisionNo: revision.revisionNo, decision: "approved" },
          ifMatch: estimate.rowVersion,
        });
        toast.success(t("approveSuccess"));
      } else if (activeAction === "return") {
        await reviewMutation.mutateAsync({
          payload: { revisionNo: revision.revisionNo, decision: "returned", reasonCode: reasonCode.trim(), note: reason.trim() },
          ifMatch: estimate.rowVersion,
        });
        toast.success(t("returnSuccess"));
      } else if (activeAction === "revision") {
        await revisionMutation.mutateAsync({
          payload: { reason: reason.trim() },
          ifMatch: estimate.rowVersion,
        });
        toast.success(t("revisionSuccess"));
      } else {
        await cancelMutation.mutateAsync({
          payload: { reason: reason.trim() },
          ifMatch: estimate.rowVersion,
        });
        toast.success(t("cancelSuccess"));
      }
      setActiveAction(null);
      setReason("");
      setReasonCode("");
      setReasonError(false);
    } catch (error: unknown) {
      if (error instanceof ApiError && error.code === "ESTIMATE_VERSION_CONFLICT") {
        toast.error(tEstimate("versionConflict"));
      } else if (error instanceof ApiError && (error.code === "ESTIMATE_CANCEL_AUTHORITY_REQUIRED" || ((activeAction === "approve" || activeAction === "return") && error.code === "PERMISSION_DENIED"))) {
        toast.error(t("reviewUnavailable"));
      } else {
        toast.error(t("actionFailed"));
      }
    }
  }

  const modalTitle = activeAction === "submit"
    ? t("submitTitle")
      : activeAction === "approve"
        ? t("approveTitle")
        : activeAction === "return"
          ? t("returnTitle")
        : activeAction === "revision"
        ? t("revisionTitle")
        : activeAction === "cancel"
          ? t("cancelTitle")
          : undefined;
  const modalDescription = activeAction === "submit"
    ? t("submitDescription")
      : activeAction === "approve"
        ? t("approveDescription")
        : activeAction === "return"
          ? t("returnDescription")
        : activeAction === "revision"
        ? t("revisionDescription")
        : activeAction === "cancel"
          ? t("cancelDescription")
          : undefined;
  const confirmLabel = activeAction === "submit"
    ? t("submit")
      : activeAction === "approve"
        ? t("approve")
        : activeAction === "return"
          ? t("return")
        : activeAction === "revision"
        ? t("createRevision")
        : t("cancel");

  if (!showSubmit && !showApprove && !showRevision && !showCancel && !showNextStep) return null;

  return (
    <div className="flex flex-col gap-2">
      {showNextStep && (
        <section className="flex flex-wrap items-center justify-between gap-3 border-l-4 border-erp-navy bg-erp-surface-subtle px-4 py-3" aria-label={t("nextStepTitle")}>
          <div className="min-w-0">
            <p className="text-xs font-semibold uppercase tracking-wide text-erp-navy">{t("nextStepTitle")}</p>
            <p className="mt-1 text-sm font-semibold text-erp-text-main">{t("submit")}</p>
            {!showSubmit && <p className="mt-1 text-sm text-erp-text-muted">{nextStepMessage}</p>}
          </div>
          {showSubmit ? (
            <Button type="button" variant="primary" className="min-h-11" onClick={() => openAction("submit")}>
              {t("submit")}
            </Button>
          ) : canSubmit && canEdit && onOpenWorkspace ? (
            <Button type="button" variant="outline" className="min-h-11" onClick={onOpenWorkspace}>
              {t("openEstimateToContinue")}
            </Button>
          ) : null}
        </section>
      )}
      {(showApprove || showRevision || showCancel) && (
        <div className="flex flex-wrap justify-end gap-2">
        {showApprove && (
          <>
          <Button type="button" variant="primary" size="sm" className="min-h-11" onClick={() => openAction("approve")}>
            {t("approve")}
          </Button>
          <Button type="button" variant="outline" size="sm" className="min-h-11" onClick={() => openAction("return")}>
            {t("return")}
          </Button>
          </>
        )}
        {showRevision && (
          <Button type="button" variant="outline" size="sm" className="min-h-11" onClick={() => openAction("revision")}>
            {t("createRevision")}
          </Button>
        )}
        {showCancel && (
          <Button type="button" variant="danger" size="sm" className="min-h-11" onClick={() => openAction("cancel")}>
            {t("cancel")}
          </Button>
        )}
        </div>
      )}

      <Modal
        isOpen={activeAction !== null}
        onClose={closeAction}
        title={modalTitle}
        description={modalDescription}
        size="sm"
        closeDisabled={isPending}
        closeOnOverlayClick={!isPending}
        closeOnEscape={!isPending}
        footer={(
          <div className="flex justify-end gap-3">
            <Button type="button" variant="outline" onClick={closeAction} disabled={isPending} className="min-h-11">
              {tCommon("actions.cancel")}
            </Button>
            <Button type="button" variant={activeAction === "cancel" ? "danger" : "primary"} onClick={() => void handleConfirm()} isLoading={isPending} disabled={isPending} className="min-h-11">
              {confirmLabel}
            </Button>
          </div>
        )}
      >
        {(activeAction === "revision" || activeAction === "cancel") && (
          <Input
            label={activeAction === "revision" ? t("revisionReason") : t("cancelReason")}
            placeholder={activeAction === "revision" ? t("revisionReasonPlaceholder") : t("cancelReasonPlaceholder")}
            required
            value={reason}
            onChange={(event) => {
              setReason(event.currentTarget.value);
              if (event.currentTarget.value.trim()) setReasonError(false);
            }}
            error={reasonError ? t("reasonRequired") : undefined}
            disabled={isPending}
            maxLength={2000}
          />
        )}
        {activeAction === "return" && (
          <div className="space-y-4">
            <Input
              label={t("returnReasonCode")}
              placeholder={t("returnReasonCodePlaceholder")}
              required
              value={reasonCode}
              onChange={(event) => {
                setReasonCode(event.currentTarget.value);
                if (event.currentTarget.value.trim() && reason.trim()) setReasonError(false);
              }}
              error={reasonError && !reasonCode.trim() ? t("returnReasonRequired") : undefined}
              disabled={isPending}
              maxLength={64}
            />
            <Textarea
              label={t("returnNote")}
              placeholder={t("returnNotePlaceholder")}
              required
              value={reason}
              onChange={(event) => {
                setReason(event.currentTarget.value);
                if (event.currentTarget.value.trim() && reasonCode.trim()) setReasonError(false);
              }}
              error={reasonError && !reason.trim() ? t("returnNoteRequired") : undefined}
              disabled={isPending}
              maxLength={2000}
              rows={4}
            />
          </div>
        )}
      </Modal>
    </div>
  );
}
