"use client";

import React from "react";
import Link from "next/link";
import { cn } from "@/lib/utils/cn";

export interface TableEntityCellProps {
  title: string;
  subtitle?: string | null;
  href?: string;
  meta?: React.ReactNode;
  className?: string;
  code?: string | null;
}

export function TableEntityCell({
  title,
  subtitle,
  href,
  meta,
  className = "",
  code,
}: TableEntityCellProps) {
  const titleElement = href ? (
    <Link
      href={href}
      className="text-sm font-bold text-erp-navy no-underline hover:underline leading-snug"
    >
      {title}
    </Link>
  ) : (
    <span className="text-sm font-bold text-erp-text-main leading-snug">
      {title}
    </span>
  );

  return (
    <div className={cn("flex flex-col gap-0.5", className)}>
      {titleElement}
      {(code || subtitle) && (
        <div className="flex items-center gap-1.5 text-xs text-erp-text-muted">
          {code && (
            <span className="font-mono font-semibold text-erp-text-main">
              {code}
            </span>
          )}
          {code && subtitle && <span>•</span>}
          {subtitle && (
            <span className="truncate max-w-[200px] text-erp-text-muted">
              {subtitle}
            </span>
          )}
        </div>
      )}
      {meta && <div className="mt-0.5">{meta}</div>}
    </div>
  );
}

export default TableEntityCell;
