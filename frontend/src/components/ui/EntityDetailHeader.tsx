"use client";

import React from "react";
import Link from "next/link";
import { IconChevronLeft } from "@/components/common/Icons";
import { CopyButton } from "@/components/common/CopyButton";
import { cn } from "@/lib/utils/cn";
import { Badge } from "./Badge";

export interface EntityDetailMetric {
  label: string;
  value: React.ReactNode;
  isFinancial?: boolean;
  isMono?: boolean;
}

export interface EntityDetailHeaderProps {
  /** Text link back to list, e.g. "Back to list" */
  backLabel?: string;
  /** Link href to list, e.g. "/th/customers" */
  backHref?: string;
  /** Primary identifier code, rendered in prominent mono font, e.g. "CUS-2026-0001" */
  code?: string | null;
  /** Whether to show a copy button next to the code badge. Defaults to true if code is provided. */
  enableCopyCode?: boolean;
  /** Accessible label / tooltip for copy action. Defaults to "Copy" */
  copyCodeLabel?: string;
  /** Feedback label when copied. Defaults to "Copied" */
  copiedLabel?: string;
  /** Primary entity title */
  title?: string | null;
  /** Secondary subtitle or English name */
  subtitle?: string | null;
  /** Avatar or entity icon */
  avatar?: React.ReactNode;
  /** Main status badge */
  statusBadge?: React.ReactNode;
  /** Additional metadata badges (e.g. type, category) */
  badges?: React.ReactNode[];
  /** Key metrics or financial strips displayed in the hero area */
  metrics?: EntityDetailMetric[];
  /** Action buttons (e.g. Activate, Edit) */
  actions?: React.ReactNode;
  className?: string;
}

export function EntityDetailHeader({
  backLabel,
  backHref,
  code,
  enableCopyCode = true,
  copyCodeLabel = "Copy",
  copiedLabel = "Copied",
  title,
  subtitle,
  avatar,
  statusBadge,
  badges,
  metrics,
  actions,
  className,
}: EntityDetailHeaderProps) {
  return (
    <div
      className={cn(
        "w-full bg-erp-surface border border-erp-border p-5 md:p-6 flex flex-col gap-5 shadow-xs",
        className
      )}
    >
      {/* Top navigation row if back link exists */}
      {backHref && backLabel && (
        <div className="flex items-center justify-between border-b border-erp-border-subtle pb-3">
          <Link
            href={backHref}
            className="inline-flex items-center gap-1.5 text-xs font-semibold text-erp-navy hover:underline focus-visible:outline-2 focus-visible:outline-erp-navy"
          >
            <IconChevronLeft size={14} strokeWidth={2.5} />
            <span>{backLabel}</span>
          </Link>
        </div>
      )}

      {/* Main Identity & Actions Row */}
      <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
        {/* Entity Identity */}
        <div className="flex items-start gap-4 min-w-0">
          {avatar && <div className="shrink-0 mt-0.5">{avatar}</div>}

          <div className="flex flex-col gap-1 min-w-0">
            {/* Code and Badges Line */}
            <div className="flex items-center flex-wrap gap-2">
              {code && (
                <div className="inline-flex items-stretch border border-erp-border bg-erp-surface-subtle">
                  <Badge
                    variant="neutral"
                    size="md"
                    className="font-mono font-bold tracking-wider select-all border-0 bg-transparent text-erp-navy"
                  >
                    {code}
                  </Badge>
                  {enableCopyCode && (
                    <CopyButton
                      text={code}
                      label={copyCodeLabel}
                      copiedLabel={copiedLabel}
                      variant="icon"
                      size="sm"
                      className="border-0 border-l border-erp-border bg-transparent hover:bg-erp-surface-muted h-auto px-2 py-1"
                    />
                  )}
                </div>
              )}
              {statusBadge}
              {badges?.map((badge, idx) => (
                <React.Fragment key={idx}>{badge}</React.Fragment>
              ))}
            </div>

            {/* Title */}
            <h1 className="text-xl md:text-2xl font-bold text-erp-navy m-0 truncate tracking-tight">
              {title}
            </h1>

            {/* Subtitle */}
            {subtitle && (
              <p className="text-xs md:text-sm text-erp-text-muted m-0 truncate">
                {subtitle}
              </p>
            )}
          </div>
        </div>

        {/* Action Buttons */}
        {actions && (
          <div className="flex items-center gap-2.5 shrink-0 self-start md:self-center">
            {actions}
          </div>
        )}
      </div>

      {/* Key Metrics / Financial Summary Strip */}
      {metrics && metrics.length > 0 && (
        <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 gap-3 pt-4 border-t border-erp-border-subtle bg-erp-surface-subtle/50 -mx-5 -mb-5 md:-mx-6 md:-mb-6 p-4 md:px-6">
          {metrics.map((metric, idx) => (
            <div key={idx} className="flex flex-col gap-0.5">
              <span className="text-[11px] font-medium text-erp-text-muted uppercase tracking-wider">
                {metric.label}
              </span>
              <span
                className={cn(
                  "text-sm font-semibold text-erp-text-main truncate",
                  metric.isFinancial &&
                    "text-xl md:text-2xl font-extrabold text-erp-navy tracking-tight",
                  metric.isMono && "font-mono text-xs"
                )}
              >
                {metric.value}
              </span>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
