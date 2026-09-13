"use client";

import React, { useState, useRef } from "react";
import { useTranslations, useLocale } from "next-intl";
import { useOpportunityDetail, useQualifyOpportunity } from "../api/opportunity-queries";
import { CustomerQuickViewDrawer } from "@/features/customers/components/customer-quick-view-drawer";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can, PERMISSIONS } from "@/lib/permissions/can";
import { ApiError } from "@/lib/api/api-error";
import { useToast } from "@/hooks/useToast";
import { useDisclosure } from "@/hooks/useDisclosure";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { Button } from "@/components/ui/Button";
import { ConfirmationModal } from "@/components/ui/ConfirmationModal";
import { DropdownMenu, type DropdownMenuItem } from "@/components/ui/DropdownMenu";
import { IconChevronLeft, IconAlertCircle, IconBriefcase, IconCheckCircle, IconEdit, IconUser, IconClose } from "@/components/common/Icons";
import { EntityDetailHeader } from "@/components/ui/EntityDetailHeader";
import { Badge } from "@/components/ui/Badge";
import { Avatar } from "@/components/ui/Avatar";
import { formatDate, formatDateTime } from "@/lib/formatters/formatters";
import { getOpportunityStageLabelKey, resolveWorkTypeLabel } from "../opportunity-labels";
import { OpportunityQGateEditor } from "./opportunity-q-gate-editor";
import { OpportunityOwnerReassignModal } from "./opportunity-owner-reassign-modal";
import { OpportunityOpenEditorDrawer } from "./opportunity-open-editor-drawer";
import { OpportunityCloseModal } from "./opportunity-close-modal";
import { OpportunityReopenModal } from "./opportunity-reopen-modal";
import { OpportunityStageTimeline } from "./opportunity-stage-timeline";
import { useOpportunitySurvey } from "@/features/surveys/api/survey-queries";
import { SurveyAppointmentModal } from "@/features/surveys/components/survey-appointment-modal";
import { SurveyCard } from "@/features/surveys/components/survey-card";
import { useOpportunityEstimate, useCreateEstimate } from "@/features/estimates/api/estimate-queries";
import { EstimateCard } from "@/features/estimates/components/estimate-card";

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

  const qualifyModal = useDisclosure({
    onClose: () => {
      setQualifyModalError(null);
      qualificationIntentRef.current = null;
    },
  });
  const reassignModal = useDisclosure();
  const editDrawer = useDisclosure();
  const closeOutcomeModal = useDisclosure();
  const reopenModal = useDisclosure();
  const surveyModal = useDisclosure();
  const [qualifyModalError, setQualifyModalError] = useState<string | null>(null);
  const customerDrawer = useDisclosure();

  const qualificationIntentRef = useRef<QualificationIntent | null>(null);

  const {
    data: opportunity,
    isLoading,
    isError,
    error,
    refetch,
  } = useOpportunityDetail(opportunityId);

  const qualifyMutation = useQualifyOpportunity();

  // Load scoped Survey info if exists
  const { data: survey } = useOpportunitySurvey(opportunityId);

  // Load scoped Estimate info
  const { data: estimate } = useOpportunityEstimate(opportunityId);
  const createEstimateMutation = useCreateEstimate(opportunityId);

  // Structured Customer & Primary Site from backend projection
  const customer = opportunity?.customer;
  const primarySite = opportunity?.primarySite;

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

  const isDraft = opportunity?.stage === "draft";
  const isQualified = opportunity?.stage === "qualified";
  const isOpen =
    opportunity?.stage === "draft" ||
    opportunity?.stage === "qualified" ||
    opportunity?.stage === "surveying" ||
    opportunity?.stage === "estimating" ||
    opportunity?.stage === "proposed";
  const isClosed = opportunity?.stage === "lost" || opportunity?.stage === "cancelled";
  const canTransition = can(selectedMembership, PERMISSIONS.OPPORTUNITIES_TRANSITION);
  const canUpdate = can(selectedMembership, PERMISSIONS.OPPORTUNITIES_UPDATE);
  const canCreateSurvey = can(selectedMembership, PERMISSIONS.SURVEYS_CREATE);
  const canQualify = isDraft && canTransition;
  const canScheduleSurvey = isQualified && canCreateSurvey;
  const canReassign = isOpen && canUpdate;
  const canClose = isOpen && canTransition;
  const canReopen = isClosed && canTransition;

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
    qualifyModal.open();
  };

  const handleCloseQualifyModal = () => {
    if (qualifyMutation.isPending) return;
    qualifyModal.close();
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
      qualifyModal.close();
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
            value: customer ? (
              <button
                type="button"
                onClick={() => customerDrawer.open()}
                className="font-semibold text-erp-navy hover:underline text-left cursor-pointer truncate max-w-full inline-block focus-visible:outline-2 focus-visible:outline-erp-navy bg-transparent border-0 p-0 text-sm"
                title={customer.displayNameTh || customer.displayNameEn || customer.code || ""}
              >
                {customer.displayNameTh || customer.displayNameEn || customer.code || "-"}
              </button>
            ) : (
              "-"
            ),
          },
          {
            label: t("targetDecisionDate"),
            value: formatDate(opportunity.targetDecisionDate, locale),
            isMono: true,
          },
          {
            label: t("nextActionAt"),
            value: formatDateTime(opportunity.nextActionAtUtc, locale),
            isMono: true,
          },
        ]}
        actions={
          <div className="flex items-center gap-2">
            {/* Primary Action Button based on Opportunity Stage */}
            {canQualify && (
              <Button
                variant="primary"
                size="md"
                onClick={handleOpenQualifyModal}
                className="font-semibold"
              >
                {t("qualifyAction")}
              </Button>
            )}
            {canScheduleSurvey && (
              <Button
                variant="primary"
                size="md"
                onClick={() => surveyModal.open()}
                className="font-semibold"
              >
                {t("scheduleSurveyAction")}
              </Button>
            )}
            {canReopen && (
              <Button
                variant="primary"
                size="md"
                onClick={() => reopenModal.open()}
                className="font-semibold"
              >
                {t("reopenAction")}
              </Button>
            )}

            {/* Management Actions Dropdown Menu */}
            {(canReassign || canClose) && (
              <DropdownMenu
                triggerLabel={tCommon("actions.manage")}
                items={[
                  ...(canReassign
                    ? [
                        {
                          key: "edit-opportunity",
                          label: t("editOpportunityAction"),
                          icon: <IconEdit size={16} />,
                          onClick: () => editDrawer.open(),
                        },
                        {
                          key: "reassign-owner",
                          label: t("reassignOwnerAction"),
                          icon: <IconUser size={16} />,
                          onClick: () => reassignModal.open(),
                        },
                      ]
                    : []),
                  ...(canClose
                    ? [
                        {
                          key: "close-opportunity",
                          label: t("closeAction"),
                          icon: <IconClose size={16} />,
                          onClick: () => closeOutcomeModal.open(),
                          variant: "danger" as const,
                          dividerAbove: canReassign,
                        },
                      ]
                    : []),
                ]}
              />
            )}
          </div>
        }
      />

      {canScheduleSurvey && (
        <SurveyAppointmentModal
          isOpen={surveyModal.isOpen}
          onClose={surveyModal.close}
          opportunity={opportunity}
        />
      )}

      {canReassign && (
        <OpportunityOpenEditorDrawer
          isOpen={editDrawer.isOpen}
          onClose={editDrawer.close}
          opportunity={opportunity}
        />
      )}

      {canReassign && (
        <OpportunityOwnerReassignModal
          isOpen={reassignModal.isOpen}
          onClose={reassignModal.close}
          opportunity={opportunity}
        />
      )}

      {canClose && (
        <OpportunityCloseModal
          isOpen={closeOutcomeModal.isOpen}
          onClose={closeOutcomeModal.close}
          opportunity={opportunity}
        />
      )}

      {canReopen && (
        <OpportunityReopenModal
          isOpen={reopenModal.isOpen}
          onClose={reopenModal.close}
          opportunity={opportunity}
        />
      )}

      <ConfirmationModal
        isOpen={qualifyModal.isOpen}
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

      {/* Inline Q-Gate Editor for Draft Opportunities */}
      {isDraft && canUpdate && (
        <OpportunityQGateEditor
          opportunity={opportunity}
        />
      )}

      {/* Survey Information Card */}
      {survey && (
        <SurveyCard
          survey={survey}
          siteLabel={primarySite?.label || primarySite?.addressLine1 || undefined}
          opportunityId={opportunity.id}
          opportunityRowVersion={opportunity.rowVersion}
          canEdit={canUpdate}
        />
      )}

      {/* Official Estimate Card */}
      {(opportunity.stage === "estimating" || estimate) && (
        <EstimateCard
          estimate={estimate ?? null}
          opportunityId={opportunity.id}
          customerId={opportunity.customer?.id}
          branchId={opportunity.branch?.id}
          siteSurveyRevisionId={survey?.currentRevision?.id}
          siteSurveySnapshotHash={survey?.currentRevision?.snapshotHash}
          canEdit={canUpdate}
          isCreating={createEstimateMutation.isPending}
          onCreateEstimate={async () => {
            if (!opportunity.branch?.id || !opportunity.customer?.id) return;
            try {
              await createEstimateMutation.mutateAsync({
                customerId: opportunity.customer.id,
                opportunityId: opportunity.id,
                branchId: opportunity.branch.id,
                siteSurveyRevisionId: survey?.currentRevision?.id,
                siteSurveySnapshotHash: survey?.currentRevision?.snapshotHash,
                currency: "THB",
              });
              toast.success("สร้างใบประเมินราคาเรียบร้อยแล้ว");
            } catch {
              toast.error(tCommon("error"));
            }
          }}
        />
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
              <button
                type="button"
                onClick={() => customerDrawer.open()}
                className="font-semibold text-erp-navy hover:underline text-left cursor-pointer inline-flex items-center gap-1 focus-visible:outline-2 focus-visible:outline-erp-navy bg-transparent border-0 p-0 text-sm"
              >
                {customer.code ? `[${customer.code}] ` : ""}
                {customer.displayNameTh || customer.displayNameEn || "-"}
              </button>
            ) : (
              <span className="text-erp-text-muted">-</span>
            )}
          </dd>

          <dt>{t("primarySite")}:</dt>
          <dd>
            {primarySite ? (
              <span>
                {primarySite.label || primarySite.addressLine1}
              </span>
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
                    {resolveWorkTypeLabel(wt, t)}
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
          <dd className="font-mono">{formatDate(opportunity.targetDecisionDate, locale)}</dd>

          <dt>{t("nextActionAt")}:</dt>
          <dd>
            {opportunity.nextActionAtUtc ? (
              <div>
                <div className="font-mono">{formatDateTime(opportunity.nextActionAtUtc, locale)}</div>
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

          <dt>{t("ownerLabel")}:</dt>
          <dd>
            {opportunity.owner ? (
              <span className="font-medium text-erp-text-main">
                {opportunity.owner.displayName} {opportunity.owner.email ? `(${opportunity.owner.email})` : ""}
              </span>
            ) : (
              "-"
            )}
          </dd>

          <dt>{t("branchLabel")}:</dt>
          <dd className="font-medium text-erp-text-main">
            {opportunity.branch?.name || "-"}
          </dd>

          <dt>{t("createdAt")}:</dt>
          <dd className="font-mono">
            {formatDateTime(opportunity.createdAtUtc, locale)}
          </dd>
        </dl>
      </div>

      {opportunity?.id && <OpportunityStageTimeline opportunityId={opportunity.id} />}

      <CustomerQuickViewDrawer
        customerId={opportunity.customer?.id ?? null}
        isOpen={customerDrawer.isOpen}
        onClose={customerDrawer.close}
      />
    </div>
  );
}
