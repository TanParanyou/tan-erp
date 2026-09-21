"use client";

import React, { useState, useRef } from "react";
import { useTranslations, useLocale } from "next-intl";
import {
  useOpportunityDetail,
  useQualifyOpportunity,
  useOpportunityWorkImages,
} from "../api/opportunity-queries";
import { CustomerQuickViewDrawer } from "@/features/customers/components/customer-quick-view-drawer";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can, PERMISSIONS } from "@/lib/permissions/can";
import { ApiError } from "@/lib/api/api-error";
import { useToast } from "@/hooks/useToast";
import { useDisclosure } from "@/hooks/useDisclosure";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { Button } from "@/components/ui/Button";
import { ConfirmationModal } from "@/components/ui/ConfirmationModal";
import { DropdownMenu } from "@/components/ui/DropdownMenu";
import {
  IconAlertCircle,
  IconBriefcase,
  IconEdit,
  IconUser,
  IconClose,
  IconCamera,
  IconClock,
  IconCheckCircle,
} from "@/components/common/Icons";
import { EntityDetailHeader } from "@/components/ui/EntityDetailHeader";
import { Badge } from "@/components/ui/Badge";
import { Avatar } from "@/components/ui/Avatar";
import { formatDate, formatDateTime } from "@/lib/formatters/formatters";
import { getOpportunityStageLabelKey, resolveWorkTypeLabel } from "../opportunity-labels";
import { OpportunityStageStepper } from "./opportunity-stage-stepper";
import { OpportunityQGateEditor } from "./opportunity-q-gate-editor";
import { OpportunityOwnerReassignModal } from "./opportunity-owner-reassign-modal";
import { OpportunityOpenEditorDrawer } from "./opportunity-open-editor-drawer";
import { OpportunityCloseModal } from "./opportunity-close-modal";
import { OpportunityReopenModal } from "./opportunity-reopen-modal";
import { OpportunityStageTimeline } from "./opportunity-stage-timeline";
import { OpportunityWorkImagesSection } from "./opportunity-work-images-section";
import { useOpportunitySurvey } from "@/features/surveys/api/survey-queries";
import { SurveyAppointmentModal } from "@/features/surveys/components/survey-appointment-modal";
import { SurveyCard } from "@/features/surveys/components/survey-card";
import { useOpportunityEstimate, useCreateEstimate, useAcceptQuotation } from "@/features/estimates/api/estimate-queries";
import { EstimateCard } from "@/features/estimates/components/estimate-card";
import { cn } from "@/lib/utils/cn";

interface QualificationIntent {
  idempotencyKey: string;
  opportunityId: string;
  expectedVersion: string;
}

interface OpportunityDetailProps {
  opportunityId: string;
}

type MobileTab = "overview" | "workImages" | "timeline";

export function OpportunityDetail({ opportunityId }: OpportunityDetailProps) {
  const t = useTranslations("opportunities");
  const tCommon = useTranslations("common");
  const tEstimates = useTranslations("estimates");
  const locale = useLocale();
  const { toast } = useToast();
  const { selectedMembership } = useSelectedMembership();

  // Mobile segmented workspace active tab
  const [activeTab, setActiveTab] = useState<MobileTab>("overview");

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
  const acceptQuotationModal = useDisclosure();
  const [qualifyModalError, setQualifyModalError] = useState<string | null>(null);
  const customerDrawer = useDisclosure();

  const qualificationIntentRef = useRef<QualificationIntent | null>(null);
  const acceptIntentKeyRef = useRef<{ versionKey: string; idempotencyKey: string } | null>(null);

  const {
    data: opportunity,
    isLoading,
    isError,
    error,
    refetch,
  } = useOpportunityDetail(opportunityId);

  const qualifyMutation = useQualifyOpportunity();

  // Work images query for badge count
  const { data: workImagesData } = useOpportunityWorkImages(opportunityId);
  const workImagesCount = workImagesData?.items?.length ?? 0;

  // Load scoped Survey info if exists
  const { data: survey } = useOpportunitySurvey(opportunityId);

  // Load scoped Estimate info
  const { data: estimate } = useOpportunityEstimate(opportunityId);
  const createEstimateMutation = useCreateEstimate(opportunityId);
  const acceptQuotationMutation = useAcceptQuotation(opportunityId, estimate?.id ?? "");

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

  // Q-gate verification checklist
  const hasScopeSummary = Boolean(opportunity?.scopeSummary && opportunity.scopeSummary.trim().length > 0);
  const hasWorkTypes = Boolean(opportunity?.workTypes && opportunity.workTypes.length > 0);
  const hasNextActionPair = Boolean(
    opportunity?.nextActionAtUtc &&
    opportunity?.nextActionNote &&
    opportunity.nextActionNote.trim().length > 0
  );
  const isQGateEligible = hasScopeSummary && hasWorkTypes && hasNextActionPair;

  const isDraft = opportunity?.stage === "draft";
  const isQualified = opportunity?.stage === "qualified";
  const isProposed = opportunity?.stage === "proposed";
  const isWon = opportunity?.stage === "won";
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
  const canAccept = can(selectedMembership, PERMISSIONS.QUOTATIONS_ACCEPT);
  const canQualify = isDraft && canTransition && isQGateEligible;
  const canScheduleSurvey = isQualified && canCreateSurvey;
  const canAcceptQuotation = isProposed && Boolean(estimate) && canAccept;
  const canReassign = isOpen && canUpdate;
  const canClose = isOpen && canTransition;
  const canReopen = isClosed && canTransition;

  const handleCancelAcceptQuotation = () => {
    acceptIntentKeyRef.current = null;
    acceptQuotationModal.close();
  };

  const handleConfirmAcceptQuotation = async () => {
    if (!opportunity || !opportunity.id || !opportunity.rowVersion || !estimate || !estimate.id) return;

    const currentVersionKey = `${opportunity.rowVersion}`;
    let idempKey: string;
    if (acceptIntentKeyRef.current && acceptIntentKeyRef.current.versionKey === currentVersionKey) {
      idempKey = acceptIntentKeyRef.current.idempotencyKey;
    } else {
      idempKey = crypto.randomUUID();
      acceptIntentKeyRef.current = { versionKey: currentVersionKey, idempotencyKey: idempKey };
    }

    try {
      await acceptQuotationMutation.mutateAsync({
        payload: {
          expectedOpportunityVersion: opportunity.rowVersion,
        },
        idempotencyKey: idempKey,
      });
      acceptIntentKeyRef.current = null;
      acceptQuotationModal.close();
      toast.success(t("quotationAcceptedSuccess"));
    } catch (err: unknown) {
      const message = err instanceof Error ? err.message : t("acceptQuotationFailed");
      toast.error(message);
    }
  };

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
        className="w-full flex justify-center items-center py-16"
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
        className="erp-card max-w-lg mx-auto my-8 p-8 text-center border-erp-danger-border bg-erp-danger-bg flex flex-col items-center gap-4 shadow-xs"
      >
        <IconAlertCircle size={32} className="text-erp-danger" />
        <h2 className="text-lg font-bold text-erp-danger m-0">
          {tCommon("feedback.operationFailed")}
        </h2>
        <p className="text-sm text-erp-text-muted m-0">
          {error?.message || t("errors.loadDetail")}
        </p>
        <Button type="button" variant="outline" size="md" onClick={() => refetch()}>
          {tCommon("actions.refresh")}
        </Button>
      </div>
    );
  }

  return (
    <div className="w-full flex flex-col gap-5">
      {/* 1. Architectural Header with Operational Financial HUD */}
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
            {canAcceptQuotation && (
              <Button
                variant="primary"
                size="md"
                onClick={() => acceptQuotationModal.open()}
                disabled={acceptQuotationMutation.isPending}
                isLoading={acceptQuotationMutation.isPending}
                className="font-semibold"
              >
                {t("acceptQuotationAction")}
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

      {/* 2. Architectural Stage Stepper */}
      <OpportunityStageStepper currentStage={opportunity.stage} />

      {/* 3. Segmented Mobile Workspace Tab Switcher (Visible on Mobile only, hidden on md+) */}
      <div
        role="tablist"
        aria-label={t("mobileWorkspaceTabs")}
        className="flex md:hidden w-full border border-erp-border bg-erp-surface p-1 gap-1"
      >
        <button
          type="button"
          role="tab"
          aria-selected={activeTab === "overview"}
          onClick={() => setActiveTab("overview")}
          className={cn(
            "flex-1 py-2 px-2 text-xs font-bold transition-colors text-center border",
            activeTab === "overview"
              ? "bg-erp-navy text-white border-erp-navy"
              : "bg-erp-surface text-erp-text-main border-transparent hover:bg-erp-surface-subtle"
          )}
        >
          {t("tabOverview")}
        </button>
        <button
          type="button"
          role="tab"
          aria-selected={activeTab === "workImages"}
          onClick={() => setActiveTab("workImages")}
          className={cn(
            "flex-1 py-2 px-2 text-xs font-bold transition-colors text-center border flex items-center justify-center gap-1.5",
            activeTab === "workImages"
              ? "bg-erp-navy text-white border-erp-navy"
              : "bg-erp-surface text-erp-text-main border-transparent hover:bg-erp-surface-subtle"
          )}
        >
          <span>{t("tabWorkImages")}</span>
          <span
            className={cn(
              "px-1.5 py-0.2 font-mono text-[10px]",
              activeTab === "workImages"
                ? "bg-white text-erp-navy"
                : "bg-erp-surface-subtle text-erp-navy border border-erp-border"
            )}
          >
            {workImagesCount}
          </span>
        </button>
        <button
          type="button"
          role="tab"
          aria-selected={activeTab === "timeline"}
          onClick={() => setActiveTab("timeline")}
          className={cn(
            "flex-1 py-2 px-2 text-xs font-bold transition-colors text-center border",
            activeTab === "timeline"
              ? "bg-erp-navy text-white border-erp-navy"
              : "bg-erp-surface text-erp-text-main border-transparent hover:bg-erp-surface-subtle"
          )}
        >
          {t("tabTimeline")}
        </button>
      </div>

      {/* 4. Split Master-Detail Layout (Desktop 8:4 / Tablet 7:5 / Mobile Single Tab) */}
      <div className="w-full md:grid md:grid-cols-12 md:gap-6 items-start">
        {/* Left Column: Operational Workspace (8 cols on lg, 7 cols on md) */}
        <div className="md:col-span-7 lg:col-span-8 flex flex-col gap-6">
          {/* Stage Command Hub (Q-Gate in Draft / Survey in Surveying / Estimate in Estimating) */}
          <div className={cn(activeTab !== "overview" && "hidden md:block")}>
            {/* Q-Gate Guidance for Draft Opportunities */}
            {isDraft && !isQGateEligible && (
              <div
                role="region"
                aria-label={t("qGateChecklistTitle")}
                className="erp-card p-4 border-erp-warning-border bg-erp-warning-bg flex flex-col gap-2 mb-6"
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
              <div className="mb-6">
                <OpportunityQGateEditor opportunity={opportunity} />
              </div>
            )}

            {/* Survey Information Card */}
            {survey && (
              <div className="mb-6">
                <SurveyCard
                  survey={survey}
                  siteLabel={primarySite?.label || primarySite?.addressLine1 || undefined}
                  opportunityId={opportunity.id}
                  opportunityRowVersion={opportunity.rowVersion}
                  canEdit={canUpdate}
                />
              </div>
            )}

            {/* Official Estimate Card */}
            {(opportunity.stage === "estimating" || estimate) && (
              <div className="mb-6">
                <EstimateCard
                  estimate={estimate ?? null}
                  opportunityId={opportunity.id}
                  opportunityRowVersion={opportunity.rowVersion}
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
                        opportunityId: opportunity.id,
                        siteSurveyRevisionId: survey?.currentRevision?.id,
                        currency: "THB",
                      });
                    } catch (err: unknown) {
                      if (err instanceof ApiError && err.code === "ESTIMATE_VERSION_CONFLICT") {
                        toast.error(tEstimates("versionConflict"));
                      } else {
                        toast.error(tEstimates("createFailed"));
                      }
                    }
                  }}
                />
              </div>
            )}

            {/* Scope & Specifications Summary Card */}
            <div className="erp-card p-5 md:p-6 flex flex-col gap-4">
              <div className="flex items-center justify-between border-b border-erp-border-subtle pb-3">
                <h2 className="text-base font-bold text-erp-navy m-0">
                  {t("sectionProjectAndScope")}
                </h2>
                {canUpdate && (
                  <Button
                    variant="ghost"
                    size="sm"
                    onClick={() => editDrawer.open()}
                    className="text-xs text-erp-navy hover:bg-erp-surface-subtle"
                  >
                    <IconEdit size={14} className="mr-1" />
                    {t("editOpportunityAction")}
                  </Button>
                )}
              </div>

              <div className="flex flex-col gap-3">
                <div>
                  <span className="text-xs font-semibold text-erp-text-muted block mb-1">
                    {t("scopeSummary")}:
                  </span>
                  <p className="text-sm text-erp-text-main whitespace-pre-wrap bg-erp-surface-subtle p-3 border border-erp-border/60 min-h-[50px] m-0">
                    {opportunity.scopeSummary || "-"}
                  </p>
                </div>

                <div>
                  <span className="text-xs font-semibold text-erp-text-muted block mb-1.5">
                    {t("workTypes")}:
                  </span>
                  <div className="flex flex-wrap gap-1.5">
                    {opportunity.workTypes && opportunity.workTypes.length > 0 ? (
                      opportunity.workTypes.map((wt) => (
                        <span
                          key={wt}
                          className="inline-block px-2.5 py-1 text-xs font-semibold border border-erp-border bg-erp-surface text-erp-navy"
                        >
                          {resolveWorkTypeLabel(wt, t)}
                        </span>
                      ))
                    ) : (
                      <span className="text-xs text-erp-text-muted">-</span>
                    )}
                  </div>
                </div>
              </div>
            </div>
          </div>

          {/* Work Images Section (Always available in dedicated tab or in left column) */}
          <div className={cn(activeTab !== "workImages" && "hidden md:block")}>
            {opportunity?.id && opportunity.stage && opportunity.rowVersion && (
              <OpportunityWorkImagesSection
                opportunityId={opportunity.id}
                currentStage={opportunity.stage}
                rowVersion={opportunity.rowVersion}
                canManage={canUpdate}
              />
            )}
          </div>
        </div>

        {/* Right Column: Project Dossier & Governance Sidebar (4 cols on lg, 5 cols on md) */}
        <div className="md:col-span-5 lg:col-span-4 flex flex-col gap-5 mt-6 md:mt-0">
          {/* 1. Customer & Primary Site Dossier Card */}
          <div className={cn(activeTab !== "overview" && "hidden md:block")}>
            <div className="erp-card p-5 flex flex-col gap-3.5 shadow-xs">
              <div className="flex items-center justify-between border-b border-erp-border-subtle pb-2.5">
                <h3 className="text-sm font-bold text-erp-navy m-0">
                  {t("customerAndSiteTitle")}
                </h3>
                {customer && (
                  <button
                    type="button"
                    onClick={() => customerDrawer.open()}
                    className="text-xs font-semibold text-erp-navy hover:underline cursor-pointer bg-transparent border-0 p-0"
                  >
                    {t("viewCustomerProfile")}
                  </button>
                )}
              </div>

              <dl className="erp-dl text-xs">
                <dt>{t("customer")}:</dt>
                <dd>
                  {customer ? (
                    <button
                      type="button"
                      onClick={() => customerDrawer.open()}
                      className="font-semibold text-erp-navy hover:underline text-left cursor-pointer inline-flex items-center gap-1 focus-visible:outline-2 focus-visible:outline-erp-navy bg-transparent border-0 p-0 text-xs"
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
                    <div className="flex flex-col">
                      <span className="font-semibold text-erp-text-main">
                        {primarySite.label || primarySite.addressLine1}
                      </span>
                      {primarySite.addressLine1 && primarySite.label && (
                        <span className="text-[11px] text-erp-text-muted mt-0.5">
                          {primarySite.addressLine1}
                        </span>
                      )}
                    </div>
                  ) : (
                    <span className="text-erp-text-muted">-</span>
                  )}
                </dd>
              </dl>
            </div>
          </div>

          {/* 2. Next Action Follow-up Card */}
          <div className={cn(activeTab !== "timeline" && "hidden md:block")}>
            <div className="erp-card p-5 flex flex-col gap-3 shadow-xs">
              <div className="flex items-center justify-between border-b border-erp-border-subtle pb-2.5">
                <h3 className="text-sm font-bold text-erp-navy m-0 flex items-center gap-1.5">
                  <IconClock size={16} className="text-erp-navy" />
                  <span>{t("nextActionCardTitle")}</span>
                </h3>
                {canUpdate && (
                  <Button
                    variant="ghost"
                    size="sm"
                    onClick={() => editDrawer.open()}
                    className="text-xs text-erp-navy hover:bg-erp-surface-subtle p-1 h-auto"
                    title={t("editOpportunityAction")}
                  >
                    <IconEdit size={13} />
                  </Button>
                )}
              </div>

              {opportunity.nextActionAtUtc ? (
                <div className="flex flex-col gap-2">
                  <div className="flex items-center justify-between gap-2">
                    <time className="font-mono text-xs font-bold text-erp-text-main">
                      {formatDateTime(opportunity.nextActionAtUtc, locale)}
                    </time>
                    {new Date(opportunity.nextActionAtUtc).getTime() < Date.now() ? (
                      <Badge variant="danger" size="sm">
                        {t("nextActionOverdue")}
                      </Badge>
                    ) : (
                      <Badge variant="neutral" size="sm">
                        {t("nextActionUpcoming")}
                      </Badge>
                    )}
                  </div>
                  {opportunity.nextActionNote && (
                    <p className="text-xs text-erp-text-muted bg-erp-surface-subtle p-2.5 border border-erp-border/60 m-0">
                      {opportunity.nextActionNote}
                    </p>
                  )}
                </div>
              ) : (
                <p className="text-xs text-erp-text-muted m-0 italic">
                  {t("nextActionEmpty")}
                </p>
              )}
            </div>
          </div>

          {/* 3. Commercial & Governance Metadata Card */}
          <div className={cn(activeTab !== "timeline" && "hidden md:block")}>
            <div className="erp-card p-5 flex flex-col gap-3 shadow-xs">
              <div className="border-b border-erp-border-subtle pb-2.5">
                <h3 className="text-sm font-bold text-erp-navy m-0">
                  {t("commercialAndOwnerTitle")}
                </h3>
              </div>

              <dl className="erp-dl text-xs">
                <dt>{t("expectedBudget")}:</dt>
                <dd className="font-semibold text-erp-navy">
                  {opportunity.expectedBudget !== null && opportunity.expectedBudget !== undefined
                    ? `${opportunity.expectedBudget.toLocaleString()} ${opportunity.currencyCode ?? "THB"}`
                    : "-"}
                </dd>

                <dt>{t("ownerLabel")}:</dt>
                <dd className="font-medium text-erp-text-main">
                  {opportunity.owner
                    ? `${opportunity.owner.displayName}${opportunity.owner.email ? ` (${opportunity.owner.email})` : ""}`
                    : "-"}
                </dd>

                <dt>{t("branchLabel")}:</dt>
                <dd className="font-medium text-erp-text-main">
                  {opportunity.branch?.name || "-"}
                </dd>

                <dt>{t("sourceCode")}:</dt>
                <dd>{opportunity.sourceCode || "-"}</dd>

                <dt>{t("targetDecisionDate")}:</dt>
                <dd className="font-mono">{formatDate(opportunity.targetDecisionDate, locale)}</dd>

                <dt>{t("createdAt")}:</dt>
                <dd className="font-mono">{formatDateTime(opportunity.createdAtUtc, locale)}</dd>
              </dl>
            </div>
          </div>

          {/* 4. Stage History Timeline Card */}
          <div className={cn(activeTab !== "timeline" && "hidden md:block")}>
            {opportunity?.id && <OpportunityStageTimeline opportunityId={opportunity.id} />}
          </div>
        </div>
      </div>

      {/* Modals and Drawers */}
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
            ? `${t("qualifyModalDesc")}\n\n[${t("qualifyErrorPrefix")}: ${qualifyModalError}]`
            : t("qualifyModalDesc")
        }
        confirmText={tCommon("actions.confirm")}
        cancelText={tCommon("actions.cancel")}
        variant="info"
        isLoading={qualifyMutation.isPending}
      />

      <ConfirmationModal
        isOpen={acceptQuotationModal.isOpen}
        onClose={handleCancelAcceptQuotation}
        onConfirm={handleConfirmAcceptQuotation}
        title={t("acceptQuotationModalTitle")}
        message={t("acceptQuotationModalDesc")}
        confirmText={t("acceptQuotationAction")}
        cancelText={tCommon("actions.cancel")}
        variant="info"
        isLoading={acceptQuotationMutation.isPending}
      />

      <CustomerQuickViewDrawer
        customerId={opportunity.customer?.id ?? null}
        isOpen={customerDrawer.isOpen}
        onClose={customerDrawer.close}
      />
    </div>
  );
}
