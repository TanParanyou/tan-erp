"use client";

import React, { useState } from "react";
import Link from "next/link";
import { useTranslations, useLocale } from "next-intl";
import { useOpportunityList } from "../api/opportunity-queries";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Select } from "@/components/ui/Select";
import { IconSearch, IconPlus, IconAlertCircle, IconBriefcase } from "@/components/common/Icons";
import { can } from "@/lib/permissions/can";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { getOpportunityStageLabelKey, CANONICAL_OPPORTUNITY_STAGES } from "../opportunity-labels";

export function OpportunityList() {
  const t = useTranslations("opportunities");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const { selectedMembership } = useSelectedMembership();

  const [searchInput, setSearchInput] = useState("");
  const [activeSearch, setActiveSearch] = useState("");
  const [stageFilter, setStageFilter] = useState<string>("");

  const canCreate = can(selectedMembership, "opportunities.create");

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

  const resolveListErrorMessage = (error: Error | null): string => {
    if (error?.message === "No authentication token available") {
      return tCommon("feedback.operationFailed");
    }
    if (error?.message === "No active membership selected") {
      return t("errors.branchRequiredDetail");
    }
    return t("errors.loadList");
  };

  const {
    data,
    isLoading,
    isError,
    error,
    refetch,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
  } = useOpportunityList({
    search: activeSearch || undefined,
    stage: stageFilter || undefined,
  });

  const handleSearchSubmit = (e: React.FormEvent): void => {
    e.preventDefault();
    setActiveSearch(searchInput.trim());
  };

  const handleClearSearch = (): void => {
    setSearchInput("");
    setActiveSearch("");
  };

  const allItems = data?.pages.flatMap((page) => page.items ?? []) ?? [];

  return (
    <div style={{ display: "flex", flexDirection: "column", gap: "1.5rem" }}>
      {/* Header bar */}
      <div
        style={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "flex-start",
          flexWrap: "wrap",
          gap: "1rem",
        }}
      >
        <div>
          <h1
            style={{
              fontSize: "1.5rem",
              fontWeight: 700,
              color: "var(--erp-navy)",
              margin: "0 0 0.25rem 0",
              letterSpacing: "-0.01em",
            }}
          >
            {t("title")}
          </h1>
          <p style={{ color: "var(--erp-text-muted)", margin: 0, fontSize: "0.875rem" }}>
            {t("subtitle")}
          </p>
        </div>

        {canCreate && (
          <Link
            href={`/${locale}/opportunities/create`}
            style={{
              display: "inline-flex",
              alignItems: "center",
              gap: "0.5rem",
              backgroundColor: "var(--erp-navy)",
              color: "#FFFFFF",
              padding: "0.5rem 1rem",
              fontSize: "0.875rem",
              fontWeight: 600,
              textDecoration: "none",
              border: "1px solid var(--erp-navy)",
              minHeight: "44px",
            }}
          >
            <IconPlus size={18} />
            <span>{t("createOpportunity")}</span>
          </Link>
        )}
      </div>

      {/* Filter and search bar */}
      <div
        className="erp-card"
        style={{
          padding: "1rem",
          display: "flex",
          gap: "1rem",
          flexWrap: "wrap",
          alignItems: "flex-end",
        }}
      >
        <form
          onSubmit={handleSearchSubmit}
          style={{ display: "flex", gap: "0.5rem", flex: "1 1 300px", alignItems: "flex-end" }}
        >
          <div style={{ flex: 1 }}>
            <Input
              name="search"
              label={tCommon("actions.search")}
              value={searchInput}
              onChange={(e) => setSearchInput(e.target.value)}
              placeholder={t("searchPlaceholder")}
            />
          </div>
          <Button
            type="submit"
            variant="primary"
            style={{ minHeight: "44px", padding: "0 1rem" }}
            aria-label={tCommon("actions.search")}
          >
            <IconSearch size={18} />
          </Button>
          {activeSearch && (
            <Button
              type="button"
              variant="outline"
              onClick={handleClearSearch}
              style={{ minHeight: "44px" }}
            >
              {tCommon("actions.clear")}
            </Button>
          )}
        </form>

        <div style={{ flex: "0 1 200px" }}>
          <Select
            name="stageFilter"
            label={t("stage")}
            value={stageFilter}
            onChange={(e) => setStageFilter(e.target.value)}
            options={[
              { value: "", label: t("stageAll") },
              ...CANONICAL_OPPORTUNITY_STAGES.map((s) => ({
                value: s,
                label: resolveStageLabel(s),
              })),
            ]}
          />
        </div>
      </div>

      {/* Loading state: Minimal mono spinner */}
      {isLoading && (
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
      )}

      {/* Error state */}
      {isError && !isLoading && (
        <div
          role="alert"
          aria-live="polite"
          className="erp-card"
          style={{
            padding: "1.5rem",
            display: "flex",
            flexDirection: "column",
            alignItems: "center",
            gap: "0.75rem",
            borderColor: "var(--erp-border-danger)",
            backgroundColor: "var(--erp-bg-danger-light)",
          }}
        >
          <IconAlertCircle size={32} />
          <p style={{ color: "var(--erp-danger)", fontWeight: 600, margin: 0, textAlign: "center" }}>
            {resolveListErrorMessage(error)}
          </p>
          <Button type="button" variant="outline" onClick={() => refetch()} style={{ minHeight: "44px" }}>
            {tCommon("actions.refresh")}
          </Button>
        </div>
      )}

      {/* Empty state */}
      {!isLoading && !isError && allItems.length === 0 && (
        <div
          className="erp-card"
          style={{
            padding: "3rem 1.5rem",
            display: "flex",
            flexDirection: "column",
            alignItems: "center",
            gap: "0.75rem",
            textAlign: "center",
          }}
        >
          <IconBriefcase size={40} />
          <h2 style={{ fontSize: "1.125rem", fontWeight: 700, margin: 0, color: "var(--erp-navy)" }}>
            {t("emptyTitle")}
          </h2>
          <p style={{ color: "var(--erp-text-muted)", margin: 0, fontSize: "0.875rem", maxWidth: "360px" }}>
            {t("emptyDetail")}
          </p>
          {canCreate && (
            <Link
              href={`/${locale}/opportunities/create`}
              style={{
                display: "inline-flex",
                alignItems: "center",
                gap: "0.5rem",
                backgroundColor: "var(--erp-navy)",
                color: "#FFFFFF",
                padding: "0.5rem 1rem",
                fontSize: "0.875rem",
                fontWeight: 600,
                textDecoration: "none",
                marginTop: "0.5rem",
                minHeight: "44px",
              }}
            >
              <IconPlus size={18} />
              <span>{t("createFirstCta")}</span>
            </Link>
          )}
        </div>
      )}

      {/* Opportunity Data Table / List */}
      {!isLoading && !isError && allItems.length > 0 && (
        <div className="erp-card" style={{ padding: 0, overflow: "hidden" }}>
          <div style={{ overflowX: "auto" }}>
            <table
              className="erp-table"
              style={{ width: "100%", borderCollapse: "collapse", fontSize: "0.875rem" }}
            >
              <thead>
                <tr style={{ backgroundColor: "var(--erp-table-header-bg)", borderBottom: "1px solid var(--erp-border)" }}>
                  <th style={{ padding: "0.75rem 1rem", textAlign: "left", fontWeight: 700, color: "var(--erp-navy)" }}>
                    {t("code")}
                  </th>
                  <th style={{ padding: "0.75rem 1rem", textAlign: "left", fontWeight: 700, color: "var(--erp-navy)" }}>
                    {t("titleField")}
                  </th>
                  <th style={{ padding: "0.75rem 1rem", textAlign: "left", fontWeight: 700, color: "var(--erp-navy)" }}>
                    {t("stage")}
                  </th>
                  <th style={{ padding: "0.75rem 1rem", textAlign: "right", fontWeight: 700, color: "var(--erp-navy)" }}>
                    {t("expectedBudget")}
                  </th>
                  <th style={{ padding: "0.75rem 1rem", textAlign: "left", fontWeight: 700, color: "var(--erp-navy)" }}>
                    {t("nextActionAt")}
                  </th>
                </tr>
              </thead>
              <tbody>
                {allItems.map((opp) => (
                  <tr
                    key={opp.id}
                    style={{ borderBottom: "1px solid var(--erp-border-subtle)" }}
                  >
                    <td style={{ padding: "0.75rem 1rem", whiteSpace: "nowrap" }}>
                      <Link
                        href={`/${locale}/opportunities/${opp.id}`}
                        style={{
                          fontWeight: 600,
                          color: "var(--erp-navy)",
                          textDecoration: "none",
                          display: "inline-block",
                          minHeight: "44px",
                          lineHeight: "44px",
                        }}
                      >
                        {opp.code ?? opp.id}
                      </Link>
                    </td>
                    <td style={{ padding: "0.75rem 1rem" }}>
                      <Link
                        href={`/${locale}/opportunities/${opp.id}`}
                        style={{
                          fontWeight: 600,
                          color: "var(--erp-navy)",
                          textDecoration: "none",
                        }}
                      >
                        {opp.title}
                      </Link>
                      {opp.scopeSummary && (
                        <div style={{ color: "var(--erp-text-muted)", fontSize: "0.75rem", marginTop: "0.125rem" }}>
                          {opp.scopeSummary}
                        </div>
                      )}
                    </td>
                    <td style={{ padding: "0.75rem 1rem", whiteSpace: "nowrap" }}>
                      <span
                        style={{
                          display: "inline-block",
                          padding: "0.25rem 0.5rem",
                          fontSize: "0.75rem",
                          fontWeight: 700,
                          border: "1px solid var(--erp-border)",
                          backgroundColor: "var(--erp-surface)",
                          color: "var(--erp-navy)",
                        }}
                      >
                        {resolveStageLabel(opp.stage)}
                      </span>
                    </td>
                    <td style={{ padding: "0.75rem 1rem", textAlign: "right", whiteSpace: "nowrap" }}>
                      {opp.expectedBudget !== null && opp.expectedBudget !== undefined ? (
                        <span>
                          {opp.expectedBudget.toLocaleString()} {opp.currencyCode ?? "THB"}
                        </span>
                      ) : (
                        <span style={{ color: "var(--erp-text-muted)" }}>-</span>
                      )}
                    </td>
                    <td style={{ padding: "0.75rem 1rem", whiteSpace: "nowrap" }}>
                      {opp.nextActionAtUtc ? (
                        <div>
                          <div>{new Date(opp.nextActionAtUtc).toLocaleDateString(locale === "th" ? "th-TH" : "en-US")}</div>
                          {opp.nextActionNote && (
                            <div style={{ color: "var(--erp-text-muted)", fontSize: "0.75rem" }}>
                              {opp.nextActionNote}
                            </div>
                          )}
                        </div>
                      ) : (
                        <span style={{ color: "var(--erp-text-muted)" }}>-</span>
                      )}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          {/* Load more keyset pagination button */}
          {hasNextPage && (
            <div
              style={{
                padding: "1rem",
                display: "flex",
                justifyContent: "center",
                borderTop: "1px solid var(--erp-border)",
              }}
            >
              <Button
                type="button"
                variant="outline"
                onClick={() => fetchNextPage()}
                disabled={isFetchingNextPage}
                style={{ minHeight: "44px", minWidth: "160px" }}
              >
                {isFetchingNextPage ? <MonoSpinner size="sm" /> : t("loadMore")}
              </Button>
            </div>
          )}
        </div>
      )}
    </div>
  );
}
