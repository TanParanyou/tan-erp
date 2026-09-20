"use client";

import React from "react";
import Link from "next/link";
import { IconChevronRight, IconChevronLeft } from "@/components/common/Icons";
import { useTranslations } from "next-intl";

export interface Breadcrumb {
  label: string;
  href?: string;
}

export interface PageHeaderProps {
  title: string;
  subtitle?: string;
  breadcrumbs?: Breadcrumb[];
  backHref?: string;
  backLabel?: string;
  onBack?: () => void;
  actions?: React.ReactNode;
  className?: string;
}

export function PageHeader({
  title,
  subtitle,
  breadcrumbs,
  backHref,
  backLabel,
  onBack,
  actions,
  className = "",
}: PageHeaderProps) {
  const t = useTranslations("shell");

  return (
    <div className={`${className}`}>
      {/* 2-Tier Architecture: Breadcrumbs or Back Link (Tier 1) */}
      {breadcrumbs && breadcrumbs.length > 0 ? (
        <nav
          className="flex items-center gap-1 text-xs text-erp-text-muted mb-2 overflow-x-auto"
          aria-label="Breadcrumb navigation"
        >
          <Link
            href="/"
            className="hover:text-erp-navy text-erp-text-muted transition-colors focus-visible:outline-2 focus-visible:outline-erp-navy"
          >
            {t("home")}
          </Link>
          {breadcrumbs.map((crumb, idx) => (
            <React.Fragment key={idx}>
              <IconChevronRight size={12} className="text-erp-text-muted shrink-0" />
              {crumb.href ? (
                <Link
                  href={crumb.href}
                  className="hover:text-erp-navy text-erp-text-muted transition-colors focus-visible:outline-2 focus-visible:outline-erp-navy"
                >
                  {crumb.label}
                </Link>
              ) : (
                <span className="text-erp-text-main font-medium">{crumb.label}</span>
              )}
            </React.Fragment>
          ))}
        </nav>
      ) : backHref || onBack ? (
        <div className="mb-2">
          {onBack ? (
            <button
              type="button"
              onClick={onBack}
              className="inline-flex items-center gap-1 text-xs font-medium text-erp-text-muted hover:text-erp-navy transition-colors focus-visible:outline-2 focus-visible:outline-erp-navy cursor-pointer"
            >
              <IconChevronLeft size={14} className="shrink-0" />
              <span>{backLabel || t("home")}</span>
            </button>
          ) : (
            <Link
              href={backHref!}
              className="inline-flex items-center gap-1 text-xs font-medium text-erp-text-muted hover:text-erp-navy transition-colors focus-visible:outline-2 focus-visible:outline-erp-navy"
            >
              <IconChevronLeft size={14} className="shrink-0" />
              <span>{backLabel || t("home")}</span>
            </Link>
          )}
        </div>
      ) : null}

      {/* Title & Actions (Tier 2) */}
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-3 sm:gap-4">
        <div className="flex items-center gap-3">
          {(backHref || onBack) && breadcrumbs && breadcrumbs.length > 0 && (
            onBack ? (
              <button
                type="button"
                onClick={onBack}
                aria-label={backLabel || title}
                className="inline-flex items-center justify-center w-8 h-8 border border-erp-border bg-erp-surface text-erp-text-muted hover:text-erp-navy hover:border-erp-navy transition-colors shrink-0 focus-visible:outline-2 focus-visible:outline-erp-navy cursor-pointer"
              >
                <IconChevronLeft size={16} />
              </button>
            ) : (
              <Link
                href={backHref!}
                aria-label={backLabel || title}
                className="inline-flex items-center justify-center w-8 h-8 border border-erp-border bg-erp-surface text-erp-text-muted hover:text-erp-navy hover:border-erp-navy transition-colors shrink-0 focus-visible:outline-2 focus-visible:outline-erp-navy"
              >
                <IconChevronLeft size={16} />
              </Link>
            )
          )}
          <div>
            <h1 className="text-xl sm:text-2xl font-bold text-erp-text-main tracking-tight">
              {title}
            </h1>
            {subtitle && (
              <p className="mt-1 text-xs sm:text-sm text-erp-text-muted leading-relaxed">
                {subtitle}
              </p>
            )}
          </div>
        </div>
        {actions && (
          <div className="flex flex-wrap items-center gap-2 sm:shrink-0">{actions}</div>
        )}
      </div>
    </div>
  );
}

export default PageHeader;
