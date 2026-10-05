"use client";

import React, { useMemo } from "react";
import { useLocale, useTranslations } from "next-intl";
import { CopyButton } from "@/components/common/CopyButton";
import {
  formatAttributesToText,
  getAttributeKeyLabel,
  recordToAttributePairs,
} from "@/lib/utils/item-attributes";
import { cn } from "@/lib/utils/cn";
import {
  type ItemAttributePair,
  type ItemAttributesRecord,
} from "@/types/item-attributes";

export interface ItemAttributesDisplayProps {
  attributes?: ItemAttributesRecord | ItemAttributePair[] | null;
  variant?: "table" | "grid" | "badge" | "compact";
  copyable?: boolean;
  className?: string;
  emptyFallback?: React.ReactNode;
}

export function ItemAttributesDisplay({
  attributes,
  variant = "table",
  copyable = true,
  className,
  emptyFallback,
}: ItemAttributesDisplayProps) {
  const currentLocale = useLocale() as "th" | "en";
  const t = useTranslations("itemMaster");

  const pairs = useMemo<ItemAttributePair[]>(() => {
    if (!attributes) return [];
    if (Array.isArray(attributes)) {
      return attributes.filter((p) => p.key?.trim() && p.value?.trim());
    }
    return recordToAttributePairs(attributes).filter(
      (p) => p.key?.trim() && p.value?.trim()
    );
  }, [attributes]);

  const allSpecsText = useMemo(() => {
    return formatAttributesToText(pairs, currentLocale);
  }, [pairs, currentLocale]);

  if (pairs.length === 0) {
    if (emptyFallback) return <>{emptyFallback}</>;
    return (
      <span className="text-erp-text-muted text-xs italic">
        {t("noAttributes")}
      </span>
    );
  }

  // 1. Variant: Badge Chips
  if (variant === "badge") {
    return (
      <div className={cn("flex flex-wrap items-center gap-1.5", className)}>
        {pairs.map((pair) => (
          <span
            key={pair.key}
            className="inline-flex items-center gap-1 px-2 py-0.5 bg-erp-surface-subtle border border-erp-border text-[11px]"
          >
            <span className="text-erp-text-muted">
              {getAttributeKeyLabel(pair.key, currentLocale)}:
            </span>
            <span className="font-mono font-medium text-erp-navy">
              {pair.value}
            </span>
          </span>
        ))}
      </div>
    );
  }

  // 2. Variant: Compact Key-Value inline
  if (variant === "compact") {
    return (
      <div className={cn("space-y-1 text-xs", className)}>
        {pairs.map((pair) => (
          <div key={pair.key} className="flex items-center justify-between gap-2">
            <span className="text-erp-text-muted">
              {getAttributeKeyLabel(pair.key, currentLocale)}
            </span>
            <span className="font-mono font-semibold text-erp-text-main">
              {pair.value}
            </span>
          </div>
        ))}
      </div>
    );
  }

  // 3. Variant: 2-Column Grid
  if (variant === "grid") {
    return (
      <div
        className={cn(
          "grid grid-cols-1 sm:grid-cols-2 gap-2 bg-erp-surface border border-erp-border p-2.5 text-xs",
          className
        )}
      >
        {pairs.map((pair) => (
          <div key={pair.key} className="p-1">
            <span className="text-erp-text-muted block text-[11px]">
              {getAttributeKeyLabel(pair.key, currentLocale)}
            </span>
            <span className="font-mono font-semibold text-erp-text-main">
              {pair.value}
            </span>
          </div>
        ))}
      </div>
    );
  }

  // 4. Variant: Table (Default)
  return (
    <div className={cn("space-y-1.5", className)}>
      {copyable && allSpecsText && (
        <div className="flex justify-end">
          <CopyButton
            text={allSpecsText}
            variant="inline"
            label={t("copyAllAttributes")}
            copiedLabel={t("copiedAttributes")}
            className="text-[11px] py-0.5 px-2 bg-transparent border-0 text-erp-navy hover:underline"
          />
        </div>
      )}
      <div className="border border-erp-border divide-y divide-erp-border/60 bg-erp-surface text-xs">
        {pairs.map((pair) => (
          <div
            key={pair.key}
            className="flex justify-between items-center px-3 py-2 group hover:bg-erp-surface-subtle transition-colors"
          >
            <div className="flex flex-col">
              <span className="font-medium text-erp-text-main">
                {getAttributeKeyLabel(pair.key, currentLocale)}
              </span>
              <span className="font-mono text-[10px] text-erp-text-muted">
                {pair.key}
              </span>
            </div>
            <div className="flex items-center gap-1.5">
              <span className="font-mono font-bold text-erp-navy">
                {pair.value}
              </span>
              {copyable && (
                <CopyButton
                  text={pair.value}
                  variant="icon"
                  size="sm"
                  className="opacity-0 group-hover:opacity-100 transition-opacity h-5 w-5 border-0 bg-transparent p-0 text-erp-text-muted hover:text-erp-navy"
                  label={t("copyAttributeValue")}
                  copiedLabel={t("copiedAttributes")}
                />
              )}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
