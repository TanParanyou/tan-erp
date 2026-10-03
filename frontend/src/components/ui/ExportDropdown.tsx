"use client";

import React, { useState, useRef, useEffect, useCallback } from "react";
import { useTranslations } from "next-intl";
import { cn } from "@/lib/utils/cn";
import { IconDownload, IconChevronDown, IconFileText, IconTable } from "@/components/common/Icons";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import type { ExportFormat } from "@/lib/export/export-types";

export interface ExportDropdownProps {
  onExport: (format: ExportFormat) => void | Promise<void>;
  isLoading?: boolean;
  disabled?: boolean;
  label?: string;
  variant?: "outline" | "default" | "bulk";
  size?: "sm" | "md";
  align?: "left" | "right";
  direction?: "down" | "up";
  className?: string;
}

export function ExportDropdown({
  onExport,
  isLoading = false,
  disabled = false,
  label,
  variant = "outline",
  size = "sm",
  align = "right",
  direction = "down",
  className,
}: ExportDropdownProps) {
  const t = useTranslations("common.actions");
  const [isOpen, setIsOpen] = useState(false);
  const containerRef = useRef<HTMLDivElement>(null);

  const buttonLabel = label ?? t("export");

  const toggle = useCallback(() => {
    if (disabled || isLoading) return;
    setIsOpen((prev) => !prev);
  }, [disabled, isLoading]);

  const close = useCallback(() => {
    setIsOpen(false);
  }, []);

  const handleSelect = useCallback(
    (format: ExportFormat) => {
      close();
      void onExport(format);
    },
    [close, onExport]
  );

  // Click outside to dismiss
  useEffect(() => {
    if (!isOpen) return;

    const handleClickOutside = (e: MouseEvent) => {
      if (containerRef.current && !containerRef.current.contains(e.target as Node)) {
        close();
      }
    };

    const handleKeyDown = (e: KeyboardEvent) => {
      if (e.key === "Escape") {
        close();
      }
    };

    document.addEventListener("mousedown", handleClickOutside);
    document.addEventListener("keydown", handleKeyDown);

    return () => {
      document.removeEventListener("mousedown", handleClickOutside);
      document.removeEventListener("keydown", handleKeyDown);
    };
  }, [isOpen, close]);

  const getVariantClasses = () => {
    switch (variant) {
      case "bulk":
        return "bg-erp-surface hover:bg-erp-surface-muted text-erp-text-main border border-erp-border min-h-[38px] px-3";
      case "default":
        return "erp-btn erp-btn-primary";
      case "outline":
      default:
        return "erp-btn erp-btn-outline";
    }
  };

  const getSizeClasses = () => {
    if (variant === "bulk") return "text-xs font-medium";
    return size === "sm" ? "erp-btn-sm h-10 min-h-[40px] px-3" : "erp-btn-md h-11 px-4";
  };

  const buttonClasses = cn(
    "inline-flex items-center gap-1.5 font-semibold text-xs tracking-wide transition-colors rounded-none shrink-0",
    getVariantClasses(),
    getSizeClasses(),
    (disabled || isLoading) && "opacity-50 cursor-not-allowed pointer-events-none"
  );

  return (
    <div ref={containerRef} className={cn("relative inline-block text-left", className)}>
      <button
        type="button"
        onClick={toggle}
        disabled={disabled || isLoading}
        aria-expanded={isOpen}
        aria-haspopup="menu"
        aria-label={buttonLabel}
        className={buttonClasses}
        style={{ borderRadius: "0px" }}
      >
        {isLoading ? <MonoSpinner size="sm" /> : <IconDownload size={15} />}
        <span>{buttonLabel}</span>
        <IconChevronDown
          size={13}
          className={cn(
            "transition-transform duration-200",
            direction === "up" ? (isOpen ? "rotate-0" : "rotate-180") : isOpen && "rotate-180"
          )}
        />
      </button>

      {isOpen && (
        <div
          role="menu"
          aria-orientation="vertical"
          className={cn(
            "absolute z-50 min-w-[200px] border border-erp-border bg-erp-surface py-1 shadow-lg focus:outline-none",
            direction === "up" ? "bottom-full mb-1" : "mt-1",
            align === "right" ? "right-0" : "left-0"
          )}
          style={{ borderRadius: "0px" }}
        >
          <button
            type="button"
            role="menuitem"
            onClick={() => handleSelect("csv")}
            className="flex w-full items-center gap-2.5 px-3 py-2 text-left text-xs font-medium text-erp-text-main hover:bg-erp-surface-muted transition-colors focus:bg-erp-surface-muted focus:outline-none"
          >
            <IconFileText size={15} className="text-erp-text-muted shrink-0" />
            <span>{t("exportCsv")}</span>
          </button>
          <button
            type="button"
            role="menuitem"
            onClick={() => handleSelect("xlsx")}
            className="flex w-full items-center gap-2.5 px-3 py-2 text-left text-xs font-medium text-erp-text-main hover:bg-erp-surface-muted transition-colors focus:bg-erp-surface-muted focus:outline-none border-t border-erp-border/50"
          >
            <IconTable size={15} className="text-erp-navy shrink-0" />
            <span>{t("exportExcel")}</span>
          </button>
        </div>
      )}
    </div>
  );
}
