"use client";

import React from "react";
import Link from "next/link";
import { IconArrowLeft } from "@/components/common/Icons";
import { PageBreadcrumbs, type BreadcrumbItem } from "./PageBreadcrumbs";

export interface DetailNavigationProps {
  breadcrumbs: BreadcrumbItem[];
  backHref: string;
  backLabel: string;
  actions?: React.ReactNode;
  className?: string;
}

export function DetailNavigation({
  breadcrumbs,
  backHref,
  backLabel,
  actions,
  className = "",
}: DetailNavigationProps) {
  return (
    <div
      className={`mb-6 flex flex-col gap-3 border-b border-erp-border pb-4 md:flex-row md:items-end md:justify-between ${className}`}
    >
      <div className="flex min-w-0 flex-col gap-2">
        <PageBreadcrumbs items={breadcrumbs} />
        <Link
          href={backHref}
          className="inline-flex min-h-10 w-fit items-center gap-2 text-sm font-semibold text-erp-text-main hover:text-erp-navy transition-colors focus-visible:outline-2 focus-visible:outline-erp-navy"
        >
          <IconArrowLeft size={16} aria-hidden="true" />
          <span>{backLabel}</span>
        </Link>
      </div>
      {actions && <div className="flex items-center gap-2">{actions}</div>}
    </div>
  );
}

export default DetailNavigation;
