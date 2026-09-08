"use client";

import React from "react";
import Link from "next/link";
import { IconChevronRight } from "@/components/common/Icons";
import { useTranslations } from "next-intl";

export interface Breadcrumb {
  label: string;
  href?: string;
}

export interface PageHeaderProps {
  title: string;
  subtitle?: string;
  breadcrumbs?: Breadcrumb[];
  actions?: React.ReactNode;
  className?: string;
}

export function PageHeader({
  title,
  subtitle,
  breadcrumbs,
  actions,
  className = "",
}: PageHeaderProps) {
  const t = useTranslations("shell");

  return (
    <div className={`mb-6 ${className}`}>
      {/* 2-Tier Architecture: Breadcrumbs (Tier 1) */}
      {breadcrumbs && breadcrumbs.length > 0 && (
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
      )}

      {/* Title & Actions (Tier 2) */}
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-3 sm:gap-4">
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
        {actions && (
          <div className="flex flex-wrap items-center gap-2 sm:shrink-0">{actions}</div>
        )}
      </div>
    </div>
  );
}

export default PageHeader;
