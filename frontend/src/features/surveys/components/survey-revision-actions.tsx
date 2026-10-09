"use client";

import { useState } from "react";
import { useTranslations } from "next-intl";
import type { SiteSurveyResponse } from "@/lib/api/api-client";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Modal } from "@/components/ui/Modal";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import { useCloneSurveyRevision, useVoidSurveyRevision } from "../api/survey-queries";

type RevisionAction = "clone" | "void";

const MAX_REASON_LENGTH = 500;

interface SurveyRevisionActionsProps {
  survey: SiteSurveyResponse;
  opportunityId: string;
  canClone: boolean;
  canVoid: boolean;
}

export function SurveyRevisionActions({ survey, opportunityId, canClone, canVoid }: SurveyRevisionActionsProps) {
  const t = useTranslations("surveys");
  const tCommon = useTranslations("common");
  const { toast } = useToast();
  const [activeAction, setActiveAction] = useState<RevisionAction | null>(null);
  const [reason, setReason] = useState("");
  const [reasonError, setReasonError] = useState(false);

  const surveyId = survey.id ?? "";
  const currentRevision = survey.currentRevision;
  const currentStatus = currentRevision?.status;
  // Clone source: latest ready revision; a voided current revision is the fallback so a survey is never a dead end.
  const cloneSourceId = survey.latestReadyRevision?.id ?? (currentStatus === "void" ? currentRevision?.id : undefined);

  const cloneMutation = useCloneSurveyRevision(opportunityId, surveyId);
  const voidMutation = useVoidSurveyRevision(opportunityId, surveyId, currentRevision?.id ?? "");
  const isPending = cloneMutation.isPending || voidMutation.isPending;

  const showClone = canClone && currentStatus !== undefined && currentStatus !== "draft" && Boolean(cloneSourceId);
  const showVoid = canVoid && currentRevision !== null && currentRevision !== undefined && currentStatus !== "void";

  function openAction(action: RevisionAction): void {
    setReason("");
    setReasonError(false);
    setActiveAction(action);
  }

  function closeAction(): void {
    if (isPending) return;
    setActiveAction(null);
    setReason("");
    setReasonError(false);
  }

  async function handleConfirm(): Promise<void> {
    if (!activeAction || !currentRevision) return;
    const trimmed = reason.trim();
    if (!trimmed) {
      setReasonError(true);
      return;
    }

    try {
      if (activeAction === "clone") {
        if (!cloneSourceId) return;
        await cloneMutation.mutateAsync({
          payload: { sourceRevisionId: cloneSourceId, reason: trimmed },
          idempotencyKey: `clone-revision-${cloneSourceId}-${Date.now()}`,
        });
        toast.success(t("cloneSuccess"));
      } else {
        await voidMutation.mutateAsync({
          payload: { expectedRevisionVersion: currentRevision.rowVersion, reason: trimmed },
          idempotencyKey: `void-revision-${currentRevision.id}-${Date.now()}`,
        });
        toast.success(t("voidSuccess"));
      }
      setActiveAction(null);
      setReason("");
      setReasonError(false);
    } catch (error: unknown) {
      if (error instanceof ApiError && error.code === "SURVEY_DRAFT_EXISTS") {
        toast.error(t("revisionDraftExists"));
      } else if (error instanceof ApiError && error.code === "SURVEY_VERSION_CONFLICT") {
        toast.error(t("versionConflict"));
      } else {
        toast.error(t("revisionActionFailed"));
      }
    }
  }

  if (!showClone && !showVoid) return null;

  const isVoid = activeAction === "void";

  return (
    <div className="flex flex-wrap justify-end gap-2" role="group" aria-label={t("revisionActionsTitle")}>
      {showClone && (
        <Button type="button" variant="outline" size="sm" className="min-h-11" onClick={() => openAction("clone")}>
          {t("cloneRevisionAction")}
        </Button>
      )}
      {showVoid && (
        <Button type="button" variant="danger" size="sm" className="min-h-11" onClick={() => openAction("void")}>
          {t("voidRevisionAction")}
        </Button>
      )}

      <Modal
        isOpen={activeAction !== null}
        onClose={closeAction}
        title={isVoid ? t("voidRevisionTitle") : t("cloneRevisionTitle")}
        description={isVoid ? t("voidRevisionDesc") : t("cloneRevisionDesc")}
        size="sm"
        closeDisabled={isPending}
        closeOnOverlayClick={!isPending}
        closeOnEscape={!isPending}
        footer={(
          <div className="flex justify-end gap-3">
            <Button type="button" variant="outline" onClick={closeAction} disabled={isPending} className="min-h-11">
              {tCommon("actions.cancel")}
            </Button>
            <Button
              type="button"
              variant={isVoid ? "danger" : "primary"}
              onClick={() => void handleConfirm()}
              isLoading={isPending}
              disabled={isPending}
              className="min-h-11"
            >
              {isVoid ? t("voidRevisionAction") : t("cloneRevisionAction")}
            </Button>
          </div>
        )}
      >
        <Input
          label={isVoid ? t("voidReasonLabel") : t("cloneReasonLabel")}
          placeholder={isVoid ? t("voidReasonPlaceholder") : t("cloneReasonPlaceholder")}
          required
          value={reason}
          onChange={(event) => {
            setReason(event.currentTarget.value);
            if (event.currentTarget.value.trim()) setReasonError(false);
          }}
          error={reasonError ? t("revisionReasonRequired") : undefined}
          disabled={isPending}
          maxLength={MAX_REASON_LENGTH}
        />
      </Modal>
    </div>
  );
}
