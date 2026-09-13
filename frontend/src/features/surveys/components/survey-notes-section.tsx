"use client";

import React, { useState } from "react";
import { useFormContext, useWatch } from "react-hook-form";
import { useTranslations } from "next-intl";
import { Input } from "@/components/ui/Input";
import { IconClose } from "@/components/common/Icons";
import type { SurveyWorkspaceFormData } from "../schemas/survey-workspace-schema";

interface SurveyNotesSectionProps {
  isReady: boolean;
}

interface TagListProps {
  title: string;
  name: "assumptions" | "constraints" | "missingDetails";
  isReady: boolean;
}

function TagList({ title, name, isReady }: TagListProps) {
  const t = useTranslations("surveys");
  const { control, setValue } = useFormContext<SurveyWorkspaceFormData>();
  const items = useWatch({ control, name }) || [];
  const [inputValue, setInputValue] = useState("");

  const handleAddItem = () => {
    const trimmed = inputValue.trim();
    if (!trimmed) return;
    setValue(name, [...items, trimmed], { shouldDirty: true });
    setInputValue("");
  };

  const handleRemoveItem = (index: number) => {
    setValue(
      name,
      items.filter((_, i) => i !== index),
      { shouldDirty: true }
    );
  };

  return (
    <div className="space-y-2">
      <label className="block text-sm font-semibold text-erp-text-main">
        {title}
      </label>
      <div className="space-y-1.5">
        {items.map((item, idx) => (
          <div
            key={idx}
            className="flex items-center justify-between text-xs p-2 bg-erp-surface-subtle border border-erp-border text-erp-text-body"
          >
            <span className="break-all">{item}</span>
            {!isReady && (
              <button
                type="button"
                onClick={() => handleRemoveItem(idx)}
                className="text-erp-text-muted hover:text-erp-danger p-0.5 ml-2 transition-colors"
                aria-label={`Remove ${item}`}
              >
                <IconClose size={14} strokeWidth={2} />
              </button>
            )}
          </div>
        ))}

        {!isReady && (
          <div className="pt-1">
            <Input
              placeholder={t("addNotePlaceholder")}
              value={inputValue}
              onChange={(e) => setInputValue(e.target.value)}
              onKeyDown={(e) => {
                if (e.key === "Enter") {
                  e.preventDefault();
                  handleAddItem();
                }
              }}
            />
          </div>
        )}
      </div>
    </div>
  );
}

export function SurveyNotesSection({ isReady }: SurveyNotesSectionProps) {
  const t = useTranslations("surveys");

  return (
    <div className="grid grid-cols-1 md:grid-cols-3 gap-4 border-t border-erp-border pt-6">
      <TagList
        title={t("assumptionsTitle")}
        name="assumptions"
        isReady={isReady}
      />
      <TagList
        title={t("constraintsTitle")}
        name="constraints"
        isReady={isReady}
      />
      <TagList
        title={t("missingDetailsTitle")}
        name="missingDetails"
        isReady={isReady}
      />
    </div>
  );
}
