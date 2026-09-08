"use client";

import React from "react";
import { IconClose } from "@/components/common/Icons";
import { useTranslations } from "next-intl";

export interface BulkActionButtonProps {
  icon: React.ReactNode;
  label: string;
  onClick: () => void;
  variant?: "default" | "success" | "danger";
  disabled?: boolean;
  className?: string;
  showLabel?: boolean;
}

export function BulkActionButton({
  icon,
  label,
  onClick,
  variant = "default",
  disabled,
  className = "",
  showLabel = false,
}: BulkActionButtonProps) {
  const variantStyles = {
    default: "bg-erp-surface hover:bg-erp-surface-muted text-erp-text-main border-erp-border",
    success: "bg-erp-surface hover:bg-erp-surface-muted text-erp-success border-erp-border",
    danger: "bg-erp-danger hover:brightness-95 text-white border-transparent",
  };

  return (
    <div className="relative group/btn flex items-center justify-center">
      <button
        type="button"
        onClick={onClick}
        disabled={disabled}
        aria-label={label}
        className={`flex items-center justify-center min-w-[38px] min-h-[38px] p-2 rounded-none border transition-colors shrink-0 disabled:opacity-50 disabled:cursor-not-allowed focus-visible:outline-2 focus-visible:outline-white ${variantStyles[variant]} ${
          showLabel ? "w-auto px-3 gap-1.5" : ""
        } ${className}`}
      >
        {icon}
        {showLabel && (
          <span className="text-xs font-medium whitespace-nowrap">{label}</span>
        )}
      </button>

      {!showLabel && (
        <div
          role="tooltip"
          className="pointer-events-none absolute bottom-full left-1/2 -translate-x-1/2 mb-2 hidden group-hover/btn:flex flex-col items-center z-[60]"
        >
          <div className="whitespace-nowrap rounded-none bg-erp-text-main text-white px-2 py-1 text-xs font-medium shadow-md">
            {label}
          </div>
        </div>
      )}
    </div>
  );
}

export interface BulkActionToolbarProps {
  selectedCount: number;
  onClear: () => void;
  children: React.ReactNode;
}

export function BulkActionToolbar({
  selectedCount,
  onClear,
  children,
}: BulkActionToolbarProps) {
  const t = useTranslations("common.actions");
  const tTable = useTranslations("common.table");

  if (selectedCount === 0) return null;

  return (
    <div
      role="region"
      aria-label="Bulk actions toolbar"
      className="fixed bottom-4 left-1/2 -translate-x-1/2 z-50 bg-erp-navy text-white rounded-none border border-erp-border shadow-2xl px-4 py-2.5 flex items-center justify-center gap-4 max-w-[calc(100vw-24px)]"
    >
      {/* Selection Info */}
      <div className="flex items-center gap-2 shrink-0">
        <span className="flex items-center justify-center min-w-6 h-6 px-1.5 rounded-none bg-white/20 text-white text-xs font-mono font-bold">
          {selectedCount}
        </span>
        <span className="hidden sm:inline text-xs sm:text-sm font-medium whitespace-nowrap">
          {tTable("items")}
        </span>
      </div>

      <div className="h-5 w-px bg-white/20 shrink-0" />

      {/* Action Buttons */}
      <div className="flex items-center gap-2 shrink-0">{children}</div>

      <div className="h-5 w-px bg-white/20 shrink-0" />

      {/* Clear Button */}
      <button
        type="button"
        onClick={onClear}
        className="flex items-center justify-center w-8 h-8 rounded-none hover:bg-white/10 text-white/80 hover:text-white transition-colors focus-visible:outline-2 focus-visible:outline-white shrink-0"
        aria-label={t("clearSelection")}
      >
        <IconClose size={18} />
      </button>
    </div>
  );
}

export default BulkActionToolbar;
