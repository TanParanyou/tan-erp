"use client";

import React, { useState, useRef } from "react";
import Link from "next/link";
import { useTranslations, useLocale } from "next-intl";
import type { EstimateDetailResponse } from "@/lib/api/api-client";
import { Button } from "@/components/ui/Button";
import { Badge } from "@/components/ui/Badge";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { IconEdit } from "@/components/common/Icons";
import { ConfirmationModal } from "@/components/ui/ConfirmationModal";
import { useDisclosure } from "@/hooks/useDisclosure";
import { useToast } from "@/hooks/useToast";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can, PERMISSIONS } from "@/lib/permissions/can";
import { ApiError } from "@/lib/api/api-error";
import { useIssueQuotation } from "../api/estimate-queries";
import { EstimateWorkspaceDrawer } from "./estimate-workspace-drawer";
import { EstimateLifecycleActions } from "./estimate-lifecycle-actions";
import {
  formatFinancialNumber,
  formatPercentRate,
} from "../utils/estimate-formatters";

interface EstimateCardProps {
  estimate: EstimateDetailResponse | null;
  opportunityId?: string;
  opportunityRowVersion?: string;
  customerId?: string;
  branchId?: string;
  siteSurveyRevisionId?: string | null;
  siteSurveySnapshotHash?: string | null;
  canEdit?: boolean;
  onCreateEstimate?: () => void;
  isCreating?: boolean;
}

export function EstimateCard({
  estimate,
  opportunityId,
  opportunityRowVersion,
  canEdit = true,
  onCreateEstimate,
  isCreating = false,
}: EstimateCardProps) {
  const t = useTranslations("estimates");
  const tQuotationDocument = useTranslations("quotationDocument");
  const uiLocale = useLocale();
  const tCommon = useTranslations("common");
  const { toast } = useToast();
  const { selectedMembership } = useSelectedMembership();
  const [isWorkspaceOpen, setIsWorkspaceOpen] = useState(false);
  const [readinessFocusTargetId, setReadinessFocusTargetId] = useState<string | undefined>();
  const issueQuotationModal = useDisclosure();
  const intentKeyRef = useRef<{ versionKey: string; idempotencyKey: string } | null>(null);

  const issueQuotationMutation = useIssueQuotation(
    opportunityId ?? "",
    estimate?.id ?? ""
  );

  const canIssue = can(selectedMembership, PERMISSIONS.QUOTATIONS_ISSUE);
  const isApproved = estimate?.currentRevision?.status === "approved";
  const hasCalculationSnapshot = Boolean(estimate?.currentRevision?.calculationSnapshotJson);
  const grandTotal = estimate?.currentRevision?.grandTotal ?? 0;
  const canIssueQuotation = Boolean(
    opportunityId &&
    opportunityRowVersion &&
    estimate &&
    isApproved &&
    estimate.currentRevision?.calculationOutdated === false &&
    hasCalculationSnapshot &&
    canIssue &&
    grandTotal > 0
  );

  const handleCancelIssueQuotation = () => {
    intentKeyRef.current = null;
    issueQuotationModal.close();
  };

  const handleConfirmIssueQuotation = async () => {
    if (!estimate || !opportunityId || !opportunityRowVersion) return;

    const currentVersionKey = `${estimate.rowVersion}|${opportunityRowVersion}`;
    let idempKey: string;
    if (intentKeyRef.current && intentKeyRef.current.versionKey === currentVersionKey) {
      idempKey = intentKeyRef.current.idempotencyKey;
    } else {
      idempKey = crypto.randomUUID();
      intentKeyRef.current = { versionKey: currentVersionKey, idempotencyKey: idempKey };
    }

    try {
      await issueQuotationMutation.mutateAsync({
        payload: {
          expectedEstimateVersion: estimate.rowVersion,
          expectedOpportunityVersion: opportunityRowVersion,
        },
        idempotencyKey: idempKey,
      });
      intentKeyRef.current = null;
      issueQuotationModal.close();
      toast.success(t("quotationIssuedSuccess"));
    } catch (err: unknown) {
      if (err instanceof ApiError && err.code === "ESTIMATE_VERSION_CONFLICT") {
        toast.error(t("versionConflict"));
      } else {
        const message = err instanceof Error ? err.message : t("quotationIssuedFailed");
        toast.error(message);
      }
    }
  };

  if (!estimate) {
    return (
      <div className="bg-erp-surface border border-erp-border p-6 shadow-sm">
        <div className="flex flex-wrap items-center justify-between gap-4">
          <div>
            <h3 className="font-bold text-erp-text-main text-base">{t("cardTitle")}</h3>
            <p className="text-sm text-erp-text-muted mt-1">
              {t("noEstimateYet")}
            </p>
          </div>
          {canEdit && onCreateEstimate && (
            <Button
              type="button"
              variant="primary"
              onClick={onCreateEstimate}
              disabled={isCreating}
              isLoading={isCreating}
              className="!rounded-none bg-erp-navy text-white hover:bg-erp-navy-hover"
            >
              {isCreating ? t("creating") : t("createEstimate")}
            </Button>
          )}
        </div>
      </div>
    );
  }

  const currentRevision = estimate.currentRevision;
  const currency = currentRevision?.currency ?? "THB";
  const revisionStatus = currentRevision?.status ?? "draft";
  const readinessStatus = currentRevision?.readiness;
  const readinessStatusLabel = readinessStatus === "blocked"
    ? t("readiness.status.blocked")
    : readinessStatus === "requiresAttention"
      ? t("readiness.status.requiresAttention")
      : readinessStatus === "ready"
        ? t("readiness.status.ready")
        : undefined;
  const readinessReasonLabel = (code: string): string => {
    switch (code) {
      case "ESTIMATE_FIELD_REQUIRED": return t("readiness.reasons.fieldRequired");
      case "ESTIMATE_COST_INCOMPLETE": return t("readiness.reasons.costIncomplete");
      case "ESTIMATE_CALCULATION_OUTDATED": return t("readiness.reasons.calculationOutdated");
      case "ESTIMATE_ZERO_DENOMINATOR": return t("readiness.reasons.zeroDenominator");
      case "ESTIMATE_PROVISIONAL_COST_REASON_REQUIRED": return t("readiness.reasons.provisionalReasonRequired");
      case "ESTIMATE_PROVISIONAL_COST": return t("readiness.reasons.provisionalCost");
      case "ESTIMATE_FIXED_PRICE_REASON_REQUIRED": return t("readiness.reasons.fixedPriceReasonRequired");
      case "ESTIMATE_FIXED_PRICE_OVERRIDE": return t("readiness.reasons.fixedPriceOverride");
      default: return t("readiness.reasons.unknown");
    }
  };
  const readinessTargetTypeLabel = (targetType: string): string => {
    switch (targetType) {
      case "revision": return t("readiness.targets.revision");
      case "section": return t("readiness.targets.section");
      case "workItem": return t("readiness.targets.workItem");
      case "costComponent": return t("readiness.targets.costComponent");
      default: return t("readiness.targets.unknown");
    }
  };
  const readinessTargetFieldLabel = (targetField: string): string => {
    switch (targetField) {
      case "sections": return t("readiness.targets.sections");
      case "workItems": return t("readiness.targets.workItems");
      case "costComponents": return t("readiness.targets.costComponents");
      case "calculation": return t("readiness.targets.calculation");
      case "marginRate": return t("readiness.targets.marginRate");
      case "provisionalReasonCode": return t("readiness.targets.provisionalReasonCode");
      default: return t("readiness.targets.unknown");
    }
  };
  const statusLabel =
    revisionStatus === "quoted"
      ? t("statuses.quoted")
      : revisionStatus === "draft"
        ? t("statuses.draft")
        : revisionStatus === "approved"
          ? t("statuses.approved")
        : revisionStatus;

  return (
    <>
      <div className="bg-erp-surface border border-erp-border p-6 shadow-sm">
        <div className="flex flex-wrap items-start justify-between gap-4 border-b border-erp-border/60 pb-4">
          <div>
            <div className="flex items-center gap-3">
              <h3 className="font-bold text-erp-navy text-lg">{estimate.number}</h3>
              <Badge variant="neutral" className="!rounded-none font-mono text-xs">
                {t("revision", { number: currentRevision?.revisionNo ?? 1 })}
              </Badge>
              <StatusBadge label={statusLabel} variant={revisionStatus === "quoted" ? "info" : undefined} />
            </div>
            <p className="text-xs text-erp-text-muted mt-1 font-mono">
              ID: {estimate.id}
            </p>
          </div>

          <div className="flex items-center gap-2">
            {estimate.currentRevision?.status === "quoted" && canIssue && (
              <Link
                href={`/${uiLocale}/estimates/${estimate.id}/quotation`}
                className="inline-flex items-center border border-erp-navy px-3 py-1.5 text-sm font-medium text-erp-navy hover:bg-erp-surface-subtle"
              >
                {tQuotationDocument("viewDocument")}
              </Link>
            )}

            {canIssueQuotation && (
              <Button
                type="button"
                variant="primary"
                size="sm"
                onClick={() => issueQuotationModal.open()}
                disabled={issueQuotationMutation.isPending}
                isLoading={issueQuotationMutation.isPending}
                className="!rounded-none bg-erp-navy text-white hover:bg-erp-navy-hover"
              >
                {t("issueQuotation")}
              </Button>
            )}

            {canEdit && (revisionStatus === "draft" || revisionStatus === "returned") && (
              <Button
                type="button"
                variant="outline"
                size="sm"
                onClick={() => setIsWorkspaceOpen(true)}
                className="!rounded-none border-erp-navy text-erp-navy hover:bg-erp-surface-subtle"
              >
                <IconEdit className="w-4 h-4 mr-1.5" />
                {t("openWorkspace")}
              </Button>
            )}
          </div>
        </div>

        {opportunityId && (
          <div className="mt-4">
            <EstimateLifecycleActions
              estimate={estimate}
              opportunityId={opportunityId}
              canEdit={canEdit}
              onOpenWorkspace={() => {
                setReadinessFocusTargetId(undefined);
                setIsWorkspaceOpen(true);
              }}
            />
          </div>
        )}

        {/* Financial Summary Grid */}
        <div className="grid grid-cols-2 md:grid-cols-4 gap-4 mt-4 bg-erp-surface-subtle p-4 border border-erp-border text-sm">
          <div>
            <span className="text-xs text-erp-text-muted block">{t("totalBeforeDiscount")}</span>
            <span className="font-mono font-bold text-erp-text-main">
              {formatFinancialNumber(currentRevision?.sellingBeforeDiscount)} {currency}
            </span>
          </div>
          <div>
            <span className="text-xs text-erp-text-muted block">{t("discount")}</span>
            <span className="font-mono font-bold text-erp-text-main">
              {formatFinancialNumber(currentRevision?.discountAmount)} {currency}
            </span>
          </div>
          <div>
            <span className="text-xs text-erp-text-muted block">{t("grandTotal")}</span>
            <span className="font-mono font-bold text-erp-navy text-base">
              {formatFinancialNumber(currentRevision?.grandTotal)} {currency}
            </span>
          </div>
          <div>
            <span className="text-xs text-erp-text-muted block">{t("marginRate")}</span>
            <span className="font-mono font-bold text-emerald-600">
              {formatPercentRate(currentRevision?.marginRate)}
            </span>
          </div>
        </div>
        {readinessStatusLabel && currentRevision && (
          <section className="mt-4 border border-erp-border p-4" aria-label={t("readiness.title")}>
            <div className="flex flex-wrap items-center justify-between gap-3">
              <h4 className="text-sm font-semibold text-erp-text-main">{t("readiness.title")}</h4>
              <Badge variant={readinessStatus === "ready" ? "success" : readinessStatus === "blocked" ? "danger" : "warning"}>
                {readinessStatusLabel}
              </Badge>
            </div>
            {(currentRevision.readinessReasons?.length ?? 0) > 0 && (
              <ul className="mt-3 space-y-2" aria-label={t("readiness.reasonsTitle")}>
                {currentRevision.readinessReasons?.map((reason, index) => (
                  <li key={`${reason.code}-${reason.targetId ?? "revision"}-${index}`} className="text-sm text-erp-text-body">
                    {canEdit && reason.targetId && (reason.targetType === "section" || reason.targetType === "workItem") ? (
                      <button
                        type="button"
                        className="min-h-11 text-left underline decoration-dotted underline-offset-2 hover:text-erp-navy focus-visible:outline-2 focus-visible:outline-erp-navy"
                        onClick={() => {
                          setReadinessFocusTargetId(reason.targetId ?? undefined);
                          setIsWorkspaceOpen(true);
                        }}
                      >
                        <span>{readinessReasonLabel(reason.code ?? "")}</span>
                        <span className="ml-2 text-xs text-erp-text-muted">
                          {readinessTargetTypeLabel(reason.targetType ?? "")} · {readinessTargetFieldLabel(reason.targetField ?? "")}; {t("readiness.openTarget")}
                        </span>
                      </button>
                    ) : (
                      <>
                        <span>{readinessReasonLabel(reason.code ?? "")}</span>
                        <span className="ml-2 text-xs text-erp-text-muted">
                          {readinessTargetTypeLabel(reason.targetType ?? "")} · {readinessTargetFieldLabel(reason.targetField ?? "")}
                        </span>
                      </>
                    )}
                  </li>
                ))}
              </ul>
            )}
          </section>
        )}
      </div>

      <EstimateWorkspaceDrawer
        isOpen={isWorkspaceOpen}
        onClose={() => setIsWorkspaceOpen(false)}
        opportunityId={opportunityId}
        estimate={estimate}
        focusTargetId={readinessFocusTargetId}
      />

      <ConfirmationModal
        isOpen={issueQuotationModal.isOpen}
        onClose={handleCancelIssueQuotation}
        onConfirm={handleConfirmIssueQuotation}
        title={t("issueQuotationModalTitle")}
        message={t("issueQuotationModalDesc")}
        confirmText={t("issueQuotation")}
        cancelText={tCommon("actions.cancel")}
        variant="info"
        isLoading={issueQuotationMutation.isPending}
      />
    </>
  );
}
