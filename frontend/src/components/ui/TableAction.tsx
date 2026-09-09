"use client";

import React from "react";
import Link from "next/link";
import { cn } from "@/lib/utils/cn";
import { MonoSpinner } from "@/components/ui/MonoSpinner";

export type TableActionVariant = "default" | "danger" | "primary" | "success";

export interface TableActionProps {
  icon: React.ReactNode;
  label: string;
  onClick?: (e: React.MouseEvent<HTMLButtonElement>) => void;
  href?: string;
  variant?: TableActionVariant;
  disabled?: boolean;
  isLoading?: boolean;
  className?: string;
}

const variantStyles: Record<TableActionVariant, string> = {
  default:
    "text-erp-text-muted hover:bg-erp-surface-muted hover:text-erp-navy border-erp-border",
  danger:
    "text-erp-text-muted hover:bg-erp-danger-bg hover:text-erp-danger hover:border-erp-danger-border",
  primary:
    "text-erp-navy hover:bg-erp-surface-muted hover:text-erp-navy border-erp-border",
  success:
    "text-erp-success hover:bg-erp-surface-muted hover:text-erp-success border-erp-border",
};

export function TableAction({
  icon,
  label,
  onClick,
  href,
  variant = "default",
  disabled = false,
  isLoading = false,
  className,
}: TableActionProps) {
  const baseClasses = cn(
    "relative group/action inline-flex h-8 w-8 items-center justify-center rounded-none border transition-colors",
    "focus-visible:outline-2 focus-visible:outline-erp-navy",
    "disabled:pointer-events-none disabled:opacity-40 disabled:hover:bg-transparent",
    variantStyles[variant],
    className
  );

  const content = (
    <>
      {isLoading ? <MonoSpinner size="sm" /> : icon}
      <span
        role="tooltip"
        className="pointer-events-none absolute bottom-full left-1/2 -translate-x-1/2 mb-1.5 hidden group-hover/action:flex flex-col items-center z-[60]"
      >
        <span className="whitespace-nowrap rounded-none bg-erp-navy text-white px-2 py-0.5 text-[11px] font-medium shadow-md">
          {label}
        </span>
      </span>
    </>
  );

  if (href && !disabled && !isLoading) {
    return (
      <Link href={href} className={baseClasses} aria-label={label} title={label}>
        {content}
      </Link>
    );
  }

  return (
    <button
      type="button"
      onClick={onClick}
      disabled={disabled || isLoading}
      className={baseClasses}
      aria-label={label}
      title={label}
    >
      {content}
    </button>
  );
}

export function TableActionGroup({
  children,
  className,
}: {
  children: React.ReactNode;
  className?: string;
}) {
  return (
    <div className={cn("flex items-center justify-end gap-1.5", className)}>
      {children}
    </div>
  );
}

export default TableAction;
