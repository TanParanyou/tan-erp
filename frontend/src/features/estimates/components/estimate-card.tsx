"use client";

import React, { useState } from "react";
import { useTranslations } from "next-intl";
import type { EstimateDetailResponse } from "@/lib/api/api-client";
import { Button } from "@/components/ui/Button";
import { Badge } from "@/components/ui/Badge";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { IconEdit } from "@/components/common/Icons";
import { EstimateWorkspaceDrawer } from "./estimate-workspace-drawer";
import {
  formatFinancialNumber,
  formatPercentRate,
} from "../utils/estimate-formatters";

interface EstimateCardProps {
  estimate: EstimateDetailResponse | null;
  opportunityId?: string;
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
  canEdit = true,
  onCreateEstimate,
  isCreating = false,
}: EstimateCardProps) {
  const t = useTranslations("estimates");
  const [isWorkspaceOpen, setIsWorkspaceOpen] = useState(false);

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
              <StatusBadge label={currentRevision?.status ?? "draft"} />
            </div>
            <p className="text-xs text-erp-text-muted mt-1 font-mono">
              ID: {estimate.id}
            </p>
          </div>

          {canEdit && (
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
      </div>

      <EstimateWorkspaceDrawer
        isOpen={isWorkspaceOpen}
        onClose={() => setIsWorkspaceOpen(false)}
        opportunityId={opportunityId}
        estimate={estimate}
      />
    </>
  );
}
