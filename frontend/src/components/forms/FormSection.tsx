"use client";

import React from "react";
import { cn } from "@/lib/utils/cn";

export interface FormSectionProps
  extends Omit<React.HTMLAttributes<HTMLDivElement>, "title"> {
  /** Section heading / title */
  title?: React.ReactNode;
  /** Subtitle or contextual hint below the title */
  description?: React.ReactNode;
  /** Action element aligned on the right of the header (e.g. action buttons, badges) */
  headerAction?: React.ReactNode;
  /** Additional classes for the header container */
  headerClassName?: string;
  /** Additional classes for the content container */
  contentClassName?: string;
  /** Children elements */
  children: React.ReactNode;
}

/**
 * Atelier Architectural Navy Sharp - Unified Form Section Card
 * Renders a structured, accessible ERP card section with solid navy title,
 * optional subtle description, right-aligned header action, and consistent borders.
 */
export function FormSection({
  title,
  description,
  headerAction,
  className,
  headerClassName,
  contentClassName,
  children,
  ...props
}: FormSectionProps) {
  const hasHeader = Boolean(title || description || headerAction);

  return (
    <div
      className={cn("erp-card p-4 sm:p-6 flex flex-col gap-4 sm:gap-5", className)}
      {...props}
    >
      {hasHeader && (
        <div
          className={cn(
            "flex flex-col sm:flex-row sm:items-center justify-between border-b border-erp-border-subtle pb-3 gap-2.5 sm:gap-3",
            headerClassName
          )}
        >
          <div className="flex flex-col gap-1 min-w-0">
            {title && (
              <h2 className="text-sm sm:text-base font-bold text-erp-navy m-0 tracking-wide uppercase truncate">
                {title}
              </h2>
            )}
            {description && (
              <p className="text-xs text-erp-text-subtle m-0 leading-normal">
                {description}
              </p>
            )}
          </div>

          {headerAction && (
            <div className="shrink-0 flex items-center gap-2 self-start sm:self-auto">
              {headerAction}
            </div>
          )}
        </div>
      )}

      <div className={cn("flex flex-col gap-4 sm:gap-5", contentClassName)}>
        {children}
      </div>
    </div>
  );
}

export default FormSection;
