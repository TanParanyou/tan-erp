"use client";

import React, { useState } from "react";
import type { LocalizedRichText, RichTextDocument } from "@/lib/rich-text/document";
import { RichTextEditor } from "./RichTextEditor";
import { cn } from "@/lib/utils/cn";

export interface MultiLangRichTextProps {
  value?: LocalizedRichText | null;
  onChange: (val: LocalizedRichText) => void;
  label?: string;
  disabled?: boolean;
  minHeight?: string;
  className?: string;
}

export function MultiLangRichText({
  value = {},
  onChange,
  label,
  disabled,
  minHeight = "200px",
  className,
}: MultiLangRichTextProps) {
  const [activeTab, setActiveTab] = useState<"th" | "en">("th");

  const handleLocaleChange = (locale: "th" | "en", doc: RichTextDocument) => {
    onChange({
      ...(value || {}),
      [locale]: doc,
    });
  };

  const currentDoc = value?.[activeTab] || null;

  return (
    <div className={cn("flex flex-col gap-2 text-left", className)}>
      <div className="flex items-center justify-between">
        {label && (
          <span className="text-xs font-semibold uppercase tracking-wider text-erp-text-main">
            {label}
          </span>
        )}
        <div className="flex items-center border border-erp-border bg-erp-surface-subtle p-0.5 rounded-none">
          <button
            type="button"
            onClick={() => setActiveTab("th")}
            className={cn(
              "px-2.5 py-0.5 text-xs font-medium transition-colors rounded-none",
              activeTab === "th"
                ? "bg-erp-navy text-white shadow-xs font-bold"
                : "text-erp-text-muted hover:text-erp-text-main"
            )}
          >
            ภาษาไทย (TH)
          </button>
          <button
            type="button"
            onClick={() => setActiveTab("en")}
            className={cn(
              "px-2.5 py-0.5 text-xs font-medium transition-colors rounded-none",
              activeTab === "en"
                ? "bg-erp-navy text-white shadow-xs font-bold"
                : "text-erp-text-muted hover:text-erp-text-main"
            )}
          >
            English (EN)
          </button>
        </div>
      </div>

      <RichTextEditor
        key={activeTab}
        value={currentDoc}
        onChange={(doc) => handleLocaleChange(activeTab, doc)}
        disabled={disabled}
        minHeight={minHeight}
      />
    </div>
  );
}

MultiLangRichText.displayName = "MultiLangRichText";
