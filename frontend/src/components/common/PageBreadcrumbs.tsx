"use client";

import React from "react";
import Link from "next/link";
import { IconChevronRight, IconHome } from "@/components/common/Icons";
import { useTranslations } from "next-intl";

export interface BreadcrumbItem {
  label: string;
  href?: string;
  active?: boolean;
}

export interface PageBreadcrumbsProps {
  items: BreadcrumbItem[];
  className?: string;
}

export function PageBreadcrumbs({ items, className = "" }: PageBreadcrumbsProps) {
  const t = useTranslations("shell");

  return (
    <nav className={`overflow-x-auto text-xs sm:text-sm ${className}`} aria-label="Breadcrumbs">
      <ol className="inline-flex min-w-max items-center gap-1">
        <li className="inline-flex items-center">
          <Link
            href="/"
            className="inline-flex items-center text-erp-text-muted hover:text-erp-navy transition-colors focus-visible:outline-2 focus-visible:outline-erp-navy"
          >
            <IconHome size={14} className="mr-1" />
            <span>{t("home")}</span>
          </Link>
        </li>
        {items.map((item, index) => (
          <li key={`${item.label}-${index}`} className="inline-flex items-center">
            <IconChevronRight size={14} className="mx-1 text-erp-text-muted" />
            {item.href && !item.active ? (
              <Link
                href={item.href}
                className="text-erp-text-muted hover:text-erp-navy transition-colors focus-visible:outline-2 focus-visible:outline-erp-navy"
              >
                {item.label}
              </Link>
            ) : (
              <span
                aria-current={item.active ? "page" : undefined}
                className="font-semibold text-erp-text-main"
              >
                {item.label}
              </span>
            )}
          </li>
        ))}
      </ol>
    </nav>
  );
}

export default PageBreadcrumbs;
