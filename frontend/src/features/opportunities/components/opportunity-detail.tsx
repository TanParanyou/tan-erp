"use client";

import React, { useState, useRef } from "react";
import Link from "next/link";
import { useTranslations, useLocale } from "next-intl";
import { useOpportunityDetail, useQualifyOpportunity } from "../api/opportunity-queries";
import { useCustomerDetail } from "@/features/customers/api/customer-queries";
import { useCustomerSiteList } from "@/features/sites/api/site-queries";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can, PERMISSIONS } from "@/lib/permissions/can";
import { ApiError } from "@/lib/api/api-error";
import { useToast } from "@/hooks/useToast";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { Button } from "@/components/ui/Button";
import { ConfirmationModal } from "@/components/ui/ConfirmationModal";
import { IconChevronLeft, IconAlertCircle, IconBriefcase, IconCheckCircle } from "@/components/common/Icons";
import { EntityDetailHeader } from "@/components/ui/EntityDetailHeader";
import { Badge } from "@/components/ui/Badge";
import { Avatar } from "@/components/ui/Avatar";
import { getOpportunityStageLabelKey, getWorkTypeLabelKey } from "../opportunity-labels";

interface QualificationIntent {
  idempotencyKey: string;
  opportunityId: string;
  expectedVersion: string;
}

interface OpportunityDetailProps {
  opportunityId: string;
}

export function OpportunityDetail({ opportunityId }: OpportunityDetailProps) {
  const t = useTranslations("opportunities");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const { toast } = useToast();
  const { selectedMembership } = useSelectedMembership();

  const [showQualifyModal, setShowQualifyModal] = useState(false);
  const [qualifyModalError, setQualifyModalError] = useState<string | null>(null);

  const qualificationIntentRef = useRef<QualificationIntent | null>(null);

  const {
    data: opportunity,
    isLoading,
    isError,
    error,
    refetch,
  } = useOpportunityDetail(opportunityId);

  const qualifyMutation = useQualifyOpportunity();

  // Load scoped Customer info
  const customerId = opportunity?.customerId;
  const { data: customer } = useCustomerDetail(customerId);

  // Load scoped Customer Sites
  const { data: siteData } = useCustomerSiteList(customerId);
  const primarySite = siteData?.items?.find((s) => s.id === opportunity?.primarySiteId);

  const resolveStageLabel = (stage: string | null | undefined): string => {
    const key = getOpportunityStageLabelKey(stage);
    if (!key) return t("unknownStage");
    switch (key) {
      case "draft":
        return t("stageDraft");
      case "qualified":
        return t("stageQualified");
      case "surveying":
        return t("stageSurveying");
      case "estimating":
        return t("stageEstimating");
      case "proposed":
        return t("stageProposed");
      case "won":
        return t("stageWon");
      case "lost":
        return t("stageLost");
      case "cancelled":
        return t("stageCancelled");
      default:
        return t("unknownStage");
    }
  };

  const resolveStageVariant = (
    stage: string | null | undefined
  ): "neutral" | "primary" | "success" | "warning" | "danger" | "info" => {
    const key = getOpportunityStageLabelKey(stage);
    switch (key) {
      case "draft":
        return "neutral";
      case "qualified":
        return "info";
      case "surveying":
      case "estimating":
      case "proposed":
        return "warning";
      case "won":
        return "success";
      case "lost":
      case "cancelled":
        return "danger";
      default:
        return "neutral";
    }
  };

  const resolveWorkTypeLabel = (wt: string): string => {
    const key = getWorkTypeLabelKey(wt);
    if (!key) return wt;
    switch (key) {
      case "built-in":
        return t("workTypeBuiltIn");
      case "interior":
        return t("workTypeInterior");
      case "curtain":
        return t("workTypeCurtain");
      case "wallpaper":
        return t("workTypeWallpaper");
      case "exterior":
        return t("workTypeExterior");
      case "other":
        return t("workTypeOther");
      default:
        return wt;
    }
  };

  const isDraft = opportunity?.stage === "draft";
  const canTransition = can(selectedMembership, PERMISSIONS.OPPORTUNITIES_TRANSITION);
  const canQualify = isDraft && canTransition;

  // Q-gate verification checklist
  const hasScopeSummary = Boolean(opportunity?.scopeSummary && opportunity.scopeSummary.trim().length > 0);
  const hasWorkTypes = Boolean(opportunity?.workTypes && opportunity.workTypes.length > 0);
  const hasNextActionPair = Boolean(
    opportunity?.nextActionAtUtc &&
    opportunity?.nextActionNote &&
    opportunity.nextActionNote.trim().length > 0
  );
  const isQGateEligible = hasScopeSummary && hasWorkTypes && hasNextActionPair;

  const handleOpenQualifyModal = () => {
    setQualifyModalError(null);
    setShowQualifyModal(true);
  };

  const handleCloseQualifyModal = () => {
    if (qualifyMutation.isPending) return;
    setShowQualifyModal(false);
    setQualifyModalError(null);
    qualificationIntentRef.current = null;
  };

  const handleConfirmQualify = async () => {
    if (!opportunity || !opportunity.id || !opportunity.rowVersion) return;
    setQualifyModalError(null);

    const oppId = opportunity.id;
    const version = opportunity.rowVersion;

    if (
      !qualificationIntentRef.current ||
      qualificationIntentRef.current.opportunityId !== oppId ||
      qualificationIntentRef.current.expectedVersion !== version
    ) {
      qualificationIntentRef.current = {
        opportunityId: oppId,
        expectedVersion: version,
        idempotencyKey: crypto.randomUUID(),
      };
    }

    const intent = qualificationIntentRef.current;
    if (!intent) return;

    try {
      await qualifyMutation.mutateAsync({
        opportunityId: intent.opportunityId,
        expectedVersion: intent.expectedVersion,
        idempotencyKey: intent.idempotencyKey,
      });

      qualificationIntentRef.current = null;
      setShowQualifyModal(false);
      toast.success(t("qualifySuccess"));
    } catch (err: unknown) {
      if (err instanceof ApiError) {
        if (err.status === 409 && err.code === "OPPORTUNITY_VERSION_CONFLICT") {
          qualificationIntentRef.current = null;
          setQualifyModalError(t("errors.qualifyConflict"));
          toast.error(t("errors.qualifyConflict"));
          return;
        }
        if (err.status === 409 && err.code === "OPPORTUNITY_INVALID_TRANSITION") {
          setQualifyModalError(t("errors.qualifyInvalidTransition"));
          toast.error(t("errors.qualifyInvalidTransition"));
          return;
        }
      }
      const message = err instanceof Error ? err.message : t("errors.saveUnexpected");
      setQualifyModalError(message);
      toast.error(message);
    }
  };

  if (isLoading) {
    return (
      <div
        role="status"
        aria-live="polite"
        style={{
          display: "flex",
          justifyContent: "center",
          alignItems: "center",
          padding: "4rem 0",
        }}
      >
        <MonoSpinner size="lg" />
      </div>
    );
  }

  if (isError || !opportunity) {
    return (
      <div
        role="alert"
        aria-live="polite"
        className="erp-card"
        style={{
          padding: "2rem",
          maxWidth: "480px",
          margin: "2rem auto",
          textAlign: "center",
          borderColor: "var(--erp-border-danger)",
          backgroundColor: "var(--erp-bg-danger-light)",
        }}
      >
        <IconAlertCircle size={32} />
        <h2 style={{ fontSize: "1.125rem", fontWeight: 700, color: "var(--erp-danger)", margin: "0.5rem 0" }}>
          {tCommon("feedback.operationFailed")}
        </h2>
        <p style={{ color: "var(--erp-text-muted)", margin: "0 0 1rem 0", fontSize: "0.875rem" }}>
          {error?.message || t("errors.loadDetail")}
        </p>
        <Button type="button" variant="outline" size="md" onClick={() => refetch()}>
          {tCommon("actions.refresh")}
        </Button>
      </div>
    );
  }

  return (
    <div className="w-full flex flex-col gap-6">
      {/* Entity Detail Hero Header */}
      <EntityDetailHeader
        backLabel={t("backToList")}
        backHref={`/${locale}/opportunities`}
        code={opportunity.code ?? opportunity.id}
        copyCodeLabel={tCommon("actions.copyCode")}
        copiedLabel={tCommon("actions.copied")}
        title={opportunity.title || "-"}
        subtitle={opportunity.scopeSummary}
        avatar={
          <Avatar
            icon={<IconBriefcase size={20} />}
            variant="navy"
            size="lg"
            title={opportunity.title || undefined}
          />
        }
        statusBadge={
          <Badge
            variant={resolveStageVariant(opportunity.stage)}
            size="md"
          >
            {resolveStageLabel(opportunity.stage)}
          </Badge>
        }
        badges={
          opportunity.sourceCode
            ? [
                <Badge key="source" variant="outline" size="sm">
                  {opportunity.sourceCode}
                </Badge>,
              ]
            : undefined
        }
        metrics={[
          {
            label: t("expectedBudget"),
            value:
              opportunity.expectedBudget !== null &&
              opportunity.expectedBudget !== undefined
                ? `${opportunity.expectedBudget.toLocaleString()} ${opportunity.currencyCode ?? "THB"}`
                : "-",
            isFinancial: true,
          },
          {
            label: t("customer"),
            value: customer
              ? customer.displayNameTh || customer.displayNameEn || customer.code || "-"
              : opportunity.customerId || "-",
          },
          {
            label: t("targetDecisionDate"),
            value: opportunity.targetDecisionDate || "-",
            isMono: true,
          },
          {
            label: t("nextActionAt"),
            value: opportunity.nextActionAtUtc
              ? new Date(opportunity.nextActionAtUtc).toLocaleDateString(
                  locale === "th" ? "th-TH" : "en-US"
                )
              : "-",
          },
        ]}
        actions={
          canQualify ? (
            <Button
              variant="primary"
              size="md"
              onClick={handleOpenQualifyModal}
              className="font-semibold"
            >
              {t("qualifyAction")}
            </Button>
          ) : undefined
        }
      />

      <ConfirmationModal
        isOpen={showQualifyModal}
        onClose={handleCloseQualifyModal}
        onConfirm={handleConfirmQualify}
        title={t("qualifyModalTitle")}
        message={
          qualifyModalError
            ? `${t("qualifyModalDesc")}\n\n[ข้อผิดพลาด: ${qualifyModalError}]`
            : t("qualifyModalDesc")
        }
        confirmText={tCommon("actions.confirm")}
        cancelText={tCommon("actions.cancel")}
        variant="info"
        isLoading={qualifyMutation.isPending}
      />

      {/* Q-Gate Guidance for Draft Opportunities */}
      {isDraft && !isQGateEligible && (
        <div
          role="region"
          aria-label={t("qGateChecklistTitle")}
          className="erp-card p-4 border-erp-warning-border bg-erp-warning-bg flex flex-col gap-2"
        >
          <div className="flex items-center gap-2 font-semibold text-erp-warning">
            <IconAlertCircle size={18} />
            <span>{t("qGateChecklistTitle")}</span>
          </div>
          <ul className="list-disc list-inside text-xs text-erp-text-muted flex flex-col gap-1">
            <li className={hasScopeSummary ? "line-through text-erp-text-muted" : "text-erp-danger font-medium"}>
              {t("qGateScopeRequired")}
            </li>
            <li className={hasWorkTypes ? "line-through text-erp-text-muted" : "text-erp-danger font-medium"}>
              {t("qGateWorkTypesRequired")}
            </li>
            <li className={hasNextActionPair ? "line-through text-erp-text-muted" : "text-erp-danger font-medium"}>
              {t("qGateNextActionRequired")}
            </li>
          </ul>
          <p className="text-[11px] text-erp-text-muted italic mt-1">
            {t("qGateNotice")}
          </p>
        </div>
      )}

      {/* Scope and Customer Information */}
      <div className="erp-card" style={{ padding: "1.5rem", display: "flex", flexDirection: "column", gap: "1.25rem" }}>
        <h2 style={{ fontSize: "1.125rem", fontWeight: 700, color: "var(--erp-navy)", margin: 0, borderBottom: "1px solid var(--erp-border-subtle)", paddingBottom: "0.75rem" }}>
          {t("opportunityDetail")}
        </h2>

        <dl className="erp-dl">
          <dt>{t("customer")}:</dt>
          <dd>
            {customer ? (
              <Link
                href={`/${locale}/customers/${customer.id}`}
                style={{ fontWeight: 600, color: "var(--erp-navy)", textDecoration: "none" }}
              >
                {customer.code ? `[${customer.code}] ` : ""}
                {customer.displayNameTh || customer.displayNameEn || "-"}
              </Link>
            ) : (
              <span style={{ fontFamily: "monospace" }}>{opportunity.customerId}</span>
            )}
          </dd>

          <dt>{t("primarySite")}:</dt>
          <dd>
            {opportunity.primarySiteId ? (
              primarySite ? (
                <span>
                  {primarySite.label || primarySite.addressLine1}
                </span>
              ) : (
                <span style={{ fontFamily: "monospace" }}>{opportunity.primarySiteId}</span>
              )
            ) : (
              <span style={{ color: "var(--erp-text-muted)" }}>-</span>
            )}
          </dd>

          <dt>{t("workTypes")}:</dt>
          <dd>
            <div style={{ display: "flex", flexWrap: "wrap", gap: "0.5rem" }}>
              {opportunity.workTypes && opportunity.workTypes.length > 0 ? (
                opportunity.workTypes.map((wt) => (
                  <span
                    key={wt}
                    style={{
                      display: "inline-block",
                      padding: "0.25rem 0.5rem",
                      fontSize: "0.75rem",
                      fontWeight: 600,
                      border: "1px solid var(--erp-border)",
                      backgroundColor: "var(--erp-surface)",
                    }}
                  >
                    {resolveWorkTypeLabel(wt)}
                  </span>
                ))
              ) : (
                "-"
              )}
            </div>
          </dd>

          <dt>{t("scopeSummary")}:</dt>
          <dd style={{ whiteSpace: "pre-wrap" }}>{opportunity.scopeSummary || "-"}</dd>

          <dt>{t("expectedBudget")}:</dt>
          <dd>
            {opportunity.expectedBudget !== null && opportunity.expectedBudget !== undefined ? (
              <span style={{ fontWeight: 600 }}>
                {opportunity.expectedBudget.toLocaleString()} {opportunity.currencyCode ?? "THB"}
              </span>
            ) : (
              "-"
            )}
          </dd>

          <dt>{t("sourceCode")}:</dt>
          <dd>{opportunity.sourceCode || "-"}</dd>

          <dt>{t("targetDecisionDate")}:</dt>
          <dd>{opportunity.targetDecisionDate || "-"}</dd>

          <dt>{t("nextActionAt")}:</dt>
          <dd>
            {opportunity.nextActionAtUtc ? (
              <div>
                <div>{new Date(opportunity.nextActionAtUtc).toLocaleString(locale === "th" ? "th-TH" : "en-US")}</div>
                {opportunity.nextActionNote && (
                  <div style={{ color: "var(--erp-text-muted)", fontSize: "0.8125rem", marginTop: "0.25rem" }}>
                    {opportunity.nextActionNote}
                  </div>
                )}
              </div>
            ) : (
              "-"
            )}
          </dd>

          <dt>{t("createdAt")}:</dt>
          <dd>
            {opportunity.createdAtUtc
              ? new Date(opportunity.createdAtUtc).toLocaleString(locale === "th" ? "th-TH" : "en-US")
              : "-"}
          </dd>
        </dl>
      </div>
    </div>
  );
}
