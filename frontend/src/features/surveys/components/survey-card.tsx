"use client";

import React, { useState } from "react";
import { useTranslations } from "next-intl";
import { useSafeLocale } from "@/lib/i18n/i18n-context";
import { formatDateTime } from "@/lib/formatters/formatters";
import type { SiteSurveyResponse } from "@/lib/api/api-client";
import { Badge } from "@/components/ui/Badge";
import { Button } from "@/components/ui/Button";
import { IconCheckCircle, IconClock, IconUser, IconMapPin, IconEdit, IconEye } from "@/components/common/Icons";
import { SurveyWorkspaceDrawer } from "./survey-workspace-drawer";

interface SurveyCardProps {
  survey: SiteSurveyResponse;
  siteLabel?: string;
  surveyorName?: string;
  opportunityId?: string;
  opportunityRowVersion?: string;
  canEdit?: boolean;
}

export function SurveyCard({
  survey,
  siteLabel,
  surveyorName,
  opportunityId,
  opportunityRowVersion,
  canEdit = true,
}: SurveyCardProps) {
  const [isWorkspaceOpen, setIsWorkspaceOpen] = useState(false);
  const t = useTranslations("surveys");
  const locale = useSafeLocale();

  const resolveSurveyStatusVariant = (status: string | null | undefined): "neutral" | "info" | "success" | "danger" => {
    switch (status) {
      case "scheduled":
        return "info";
      case "in_progress":
        return "neutral";
      case "completed":
        return "success";
      case "cancelled":
        return "danger";
      default:
        return "neutral";
    }
  };

  const resolveRevisionStatusVariant = (status: string | null | undefined): "neutral" | "info" | "success" | "danger" => {
    switch (status) {
      case "draft":
        return "neutral";
      case "ready":
        return "success";
      case "superseded":
        return "info";
      case "void":
        return "danger";
      default:
        return "neutral";
    }
  };

  const statusKey = survey.status ?? "scheduled";
  const revStatusKey = survey.currentRevision?.status ?? "draft";
  const revNum = survey.currentRevision?.revisionNumber ?? 1;

  // Single Source of Truth: Structured contract projections from Backend
  const siteDisplay = survey.site?.label ?? siteLabel ?? "-";
  const surveyorDisplay = survey.assignedSurveyor?.displayName ?? surveyorName ?? "-";

  return (
    <div
      role="region"
      aria-label={t("surveyCardTitle")}
      className="erp-card p-4 border-erp-border bg-erp-surface flex flex-col gap-3 shadow-xs"
      style={{ borderRadius: "0px" }}
    >
      <div className="flex items-center justify-between border-b border-erp-border-subtle pb-2">
        <div className="flex items-center gap-2">
          <IconCheckCircle size={20} className="text-erp-navy" />
          <h3 className="font-bold text-erp-navy text-sm">
            {t("surveyNumberLabel")}: <span className="font-mono text-erp-text-main">{survey.surveyNumber || "-"}</span>
          </h3>
        </div>
        <div className="flex items-center gap-2">
          <Badge variant={resolveSurveyStatusVariant(survey.status)} size="sm">
            {t(`statuses.${statusKey}`)}
          </Badge>
          {survey.currentRevision && (
            <Badge variant={resolveRevisionStatusVariant(survey.currentRevision.status)} size="sm">
              {t("revisionBadge", { number: revNum })} ({t(`revisionStatuses.${revStatusKey}`)})
            </Badge>
          )}
        </div>
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-3 text-xs">
        <div className="flex items-start gap-2">
          <IconMapPin size={16} className="text-erp-text-muted mt-0.5" />
          <div className="flex flex-col">
            <span className="text-erp-text-muted">{t("siteLabel")}:</span>
            <span className="font-medium text-erp-text-main">{siteDisplay}</span>
          </div>
        </div>

        <div className="flex items-start gap-2">
          <IconUser size={16} className="text-erp-text-muted mt-0.5" />
          <div className="flex flex-col">
            <span className="text-erp-text-muted">{t("surveyorLabel")}:</span>
            <span className="font-medium text-erp-text-main">{surveyorDisplay}</span>
          </div>
        </div>

        <div className="flex items-start gap-2">
          <IconClock size={16} className="text-erp-text-muted mt-0.5" />
          <div className="flex flex-col">
            <span className="text-erp-text-muted">{t("scheduledTimeLabel")}:</span>
            <span className="font-medium font-mono text-erp-text-main">
              {survey.scheduledStartUtc ? formatDateTime(survey.scheduledStartUtc, locale) : "-"}
            </span>
          </div>
        </div>
      </div>

      {/* Revision summary & Workspace actions */}
      <div className="flex flex-wrap items-center justify-between gap-2 pt-2 border-t border-erp-border-subtle text-xs">
        <div className="flex items-center gap-3">
          {survey.currentRevision?.status === "ready" ? (
            <div className="flex items-center gap-1.5 text-emerald-700 font-medium">
              <IconCheckCircle size={15} />
              <span>{t("readyBadge")}</span>
              {survey.currentRevision.snapshotHash && (
                <span className="font-mono text-[11px] text-erp-text-muted ml-1 bg-erp-border-subtle px-1.5 py-0.5">
                  #{survey.currentRevision.snapshotHash.slice(0, 8)}
                </span>
              )}
            </div>
          ) : (
            <span className="text-erp-text-muted">
              {t("revisionStatuses.draft")} • {survey.currentRevision?.areas?.length ?? 0} {t("areasTitle")}
            </span>
          )}
        </div>

        {opportunityId && opportunityRowVersion && (
          <Button
            variant={survey.currentRevision?.status === "ready" ? "secondary" : "primary"}
            size="sm"
            onClick={() => setIsWorkspaceOpen(true)}
            className="flex items-center gap-1.5 font-semibold"
          >
            {survey.currentRevision?.status === "ready" || !canEdit ? (
              <>
                <IconEye size={15} />
                <span>{t("openWorkspace")}</span>
              </>
            ) : (
              <>
                <IconEdit size={15} />
                <span>{t("openWorkspace")}</span>
              </>
            )}
          </Button>
        )}
      </div>

      {opportunityId && opportunityRowVersion && (
        <SurveyWorkspaceDrawer
          isOpen={isWorkspaceOpen}
          onClose={() => setIsWorkspaceOpen(false)}
          opportunityId={opportunityId}
          survey={survey}
          currentOpportunityVersion={opportunityRowVersion}
        />
      )}
    </div>
  );
}
