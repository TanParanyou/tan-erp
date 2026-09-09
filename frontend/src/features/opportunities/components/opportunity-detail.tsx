"use client";

import React from "react";
import Link from "next/link";
import { useTranslations, useLocale } from "next-intl";
import { useOpportunityDetail } from "../api/opportunity-queries";
import { useCustomerDetail } from "@/features/customers/api/customer-queries";
import { useCustomerSiteList } from "@/features/sites/api/site-queries";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { Button } from "@/components/ui/Button";
import { IconChevronLeft, IconAlertCircle, IconBriefcase } from "@/components/common/Icons";
import { EntityDetailHeader } from "@/components/ui/EntityDetailHeader";
import { Badge } from "@/components/ui/Badge";
import { Avatar } from "@/components/ui/Avatar";
import { getOpportunityStageLabelKey, getWorkTypeLabelKey } from "../opportunity-labels";

interface OpportunityDetailProps {
  opportunityId: string;
}

export function OpportunityDetail({ opportunityId }: OpportunityDetailProps) {
  const t = useTranslations("opportunities");
  const tCommon = useTranslations("common");
  const locale = useLocale();

  const {
    data: opportunity,
    isLoading,
    isError,
    error,
    refetch,
  } = useOpportunityDetail(opportunityId);

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
      case "estimation":
        return t("stageEstimation");
      case "proposal":
        return t("stageProposal");
      case "won":
        return t("stageWon");
      case "lost":
        return t("stageLost");
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
      case "estimation":
      case "proposal":
        return "warning";
      case "won":
        return "success";
      case "lost":
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
        <Button type="button" variant="outline" onClick={() => refetch()} style={{ minHeight: "44px" }}>
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
      />

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
