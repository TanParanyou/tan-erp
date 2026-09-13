"use client";

import React, { useState, useRef } from "react";
import { useTranslations } from "next-intl";
import { useCreateSiteSurvey } from "../api/survey-queries";
import { useCustomerSiteList } from "@/features/sites/api/site-queries";
import type { OpportunityResponse, SiteResponse } from "@/lib/api/api-client";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import { Button } from "@/components/ui/Button";
import { Select } from "@/components/ui/Select";
import { Input } from "@/components/ui/Input";
import { UserAutocomplete } from "@/components/forms/UserAutocomplete";
import { Alert } from "@/components/ui/Alert";
import { IconAlertCircle } from "@/components/common/Icons";
import { Modal } from "@/components/ui/Modal";

interface SurveyAppointmentModalProps {
  isOpen: boolean;
  onClose: () => void;
  opportunity: OpportunityResponse;
  onSuccess?: () => void;
}

export function SurveyAppointmentModal({
  isOpen,
  onClose,
  opportunity,
  onSuccess,
}: SurveyAppointmentModalProps) {
  const t = useTranslations("surveys");
  const tCommon = useTranslations("common");
  const { toast } = useToast();
  const createMutation = useCreateSiteSurvey(opportunity.id || "");

  const [siteId, setSiteId] = useState<string>(opportunity.primarySite?.id || "");
  const [assignedSurveyorId, setAssignedSurveyorId] = useState<string>("");
  const [scheduledStart, setScheduledStart] = useState<string>("");
  const [scheduledEnd, setScheduledEnd] = useState<string>("");
  const [errorMsg, setErrorMsg] = useState<string | null>(null);

  const idempotencyKeyRef = useRef<string>(crypto.randomUUID());

  // Load Customer Sites
  const { data: siteData } = useCustomerSiteList(opportunity.customer?.id || undefined);
  const siteList = siteData?.items ?? [];

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!siteId.trim()) {
      setErrorMsg(t("siteRequired"));
      return;
    }

    if (!assignedSurveyorId.trim()) {
      setErrorMsg(t("surveyorRequired"));
      return;
    }

    // Schedule validation if dates provided
    let startUtc: string | null = null;
    let endUtc: string | null = null;

    if (scheduledStart.trim()) {
      startUtc = new Date(scheduledStart).toISOString();
    }
    if (scheduledEnd.trim()) {
      endUtc = new Date(scheduledEnd).toISOString();
    }

    if (startUtc && endUtc && new Date(endUtc) <= new Date(startUtc)) {
      setErrorMsg(t("endBeforeStartError"));
      return;
    }

    if (!opportunity.id || !opportunity.rowVersion) {
      return;
    }

    setErrorMsg(null);
    try {
      await createMutation.mutateAsync({
        payload: {
          siteId: siteId.trim(),
          assignedSurveyorId: assignedSurveyorId.trim(),
          scheduledStartUtc: startUtc,
          scheduledEndUtc: endUtc,
          expectedOpportunityVersion: opportunity.rowVersion,
        },
        idempotencyKey: idempotencyKeyRef.current,
      });

      toast.success(t("scheduleSuccess"));
      idempotencyKeyRef.current = crypto.randomUUID();
      onSuccess?.();
      onClose();
    } catch (err: unknown) {
      if (err instanceof ApiError) {
        if (err.status === 409 && err.code === "OPPORTUNITY_VERSION_CONFLICT") {
          setErrorMsg(t("versionConflict"));
          toast.error(t("versionConflict"));
          return;
        }
        if (err.code === "SURVEY_SCHEDULE_INVALID") {
          setErrorMsg(t("endBeforeStartError"));
          toast.error(t("endBeforeStartError"));
          return;
        }
      }
      const message = err instanceof Error ? err.message : tCommon("errors.unexpected");
      setErrorMsg(message);
      toast.error(message);
    }
  };

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title={t("scheduleModalTitle")}
      description={t("scheduleModalDesc")}
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
            label={t("siteLabel")}
            value={siteId}
            onChange={(e) => {
              setSiteId(e.target.value);
              setErrorMsg(null);
            }}
            options={[
              { value: "", label: t("siteSelectPlaceholder") },
              ...siteList.map((s: SiteResponse) => ({
                value: s.id ?? "",
                label: s.label || s.addressLine1 || "-",
              })),
            ]}
            required
            disabled={createMutation.isPending}
          />

          <UserAutocomplete
            value={assignedSurveyorId}
            onChange={(userId) => {
              setAssignedSurveyorId(userId);
              setErrorMsg(null);
            }}
            branchId={opportunity.branch?.id || undefined}
            required
            disabled={createMutation.isPending}
            label={t("surveyorLabel")}
            placeholder={t("surveyorPlaceholder")}
          />

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
            <Input
              type="datetime-local"
              label={t("scheduledStartLabel")}
              value={scheduledStart}
              onChange={(e) => {
                setScheduledStart(e.target.value);
                setErrorMsg(null);
              }}
              disabled={createMutation.isPending}
            />
            <Input
              type="datetime-local"
              label={t("scheduledEndLabel")}
              value={scheduledEnd}
              onChange={(e) => {
                setScheduledEnd(e.target.value);
                setErrorMsg(null);
              }}
              disabled={createMutation.isPending}
            />
          </div>

          <div className="flex justify-end gap-3 pt-2 border-t border-erp-border">
            <Button
              type="button"
              variant="outline"
              size="md"
              onClick={onClose}
              disabled={createMutation.isPending}
            >
              {tCommon("actions.cancel")}
            </Button>
            <Button
              type="submit"
              variant="primary"
              size="md"
              isLoading={createMutation.isPending}
              disabled={createMutation.isPending || !siteId.trim() || !assignedSurveyorId.trim()}
            >
              {t("confirmScheduleAction")}
            </Button>
          </div>
        </form>
      </div>
    </Modal>
  );
}
