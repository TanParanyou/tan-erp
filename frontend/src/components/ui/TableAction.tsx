"use client";

import React from "react";
import Link from "next/link";
import { cn } from "@/lib/utils/cn";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { Tooltip } from "@/components/ui/Tooltip";

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
    "relative inline-flex h-8 w-8 items-center justify-center rounded-none border transition-colors",
    "focus-visible:outline-2 focus-visible:outline-erp-navy",
    "disabled:pointer-events-none disabled:opacity-40 disabled:hover:bg-transparent",
    variantStyles[variant],
    className
  );

  const iconContent = isLoading ? <MonoSpinner size="sm" /> : icon;

  const actionElement =
    href && !disabled && !isLoading ? (
      <Link href={href} className={baseClasses} aria-label={label}>
        {iconContent}
      </Link>
    ) : (
      <button
        type="button"
        onClick={onClick}
        disabled={disabled || isLoading}
        className={baseClasses}
        aria-label={label}
      >
        {iconContent}
      </button>
    );

  return (
    <Tooltip content={label} position="top" disabled={disabled || isLoading}>
      {actionElement}
    </Tooltip>
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
