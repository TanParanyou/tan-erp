"use client";

import React from "react";
import { useFormContext, useWatch } from "react-hook-form";
import { useTranslations } from "next-intl";
import { Select } from "@/components/ui/Select";
import { Input } from "@/components/ui/Input";
import {
  SURVEY_CHECKLIST_RESULTS,
  type SurveyChecklistItemFormData,
  type SurveyChecklistResultValue,
  type SurveyWorkspaceFormData,
} from "../schemas/survey-workspace-schema";

interface SurveyChecklistSectionProps {
  requiredItems: readonly string[];
  isReady: boolean;
}

const MAX_NOTE_LENGTH = 500;

/**
 * Site checklist required by the revision's survey template. Items come from the template; labels are
 * resolved by item code, so a new template version only needs new translations.
 */
export function SurveyChecklistSection({ requiredItems, isReady }: SurveyChecklistSectionProps) {
  const t = useTranslations("surveys");
  const { control, setValue } = useFormContext<SurveyWorkspaceFormData>();
  const checklist = useWatch({ control, name: "checklist" }) ?? [];

  const resultOptions = SURVEY_CHECKLIST_RESULTS.map((value) => ({
    value,
    label: t(`checklistResults.${value}`),
  }));

  const update = (itemCode: string, patch: Partial<SurveyChecklistItemFormData>) => {
    const existing = checklist.find((c) => c.itemCode === itemCode);
    const next: SurveyChecklistItemFormData = { itemCode, result: "pass", note: null, ...existing, ...patch };
    const others = checklist.filter((c) => c.itemCode !== itemCode);
    setValue("checklist", [...others, next], { shouldDirty: true });
  };

  return (
    <section className="space-y-3" aria-label={t("checklistTitle")}>
      <div>
        <h4 className="text-sm font-bold text-erp-navy">{t("checklistTitle")}</h4>
        <p className="text-xs text-erp-text-muted mt-0.5">{t("checklistDesc")}</p>
      </div>

      <div className="space-y-2">
        {requiredItems.map((itemCode) => {
          const entry = checklist.find((c) => c.itemCode === itemCode);
          const needsNote = entry !== undefined && entry.result !== "pass";
          return (
            <div
              key={itemCode}
              className="grid grid-cols-1 md:grid-cols-[1fr_200px] gap-3 p-3 border border-erp-border bg-erp-surface-subtle"
            >
              <div className="space-y-2">
                <div className="text-sm font-medium text-erp-text-main">{t(`checklistItems.${itemCode}`)}</div>
                {needsNote && (
                  <Input
                    label={t("checklistNoteLabel")}
                    required
                    value={entry?.note ?? ""}
                    maxLength={MAX_NOTE_LENGTH}
                    disabled={isReady}
                    onChange={(e) => update(itemCode, { note: e.target.value })}
                  />
                )}
              </div>
              <Select
                aria-label={t("checklistResultLabel")}
                value={entry?.result ?? ""}
                placeholder={t("checklistUnanswered")}
                disabled={isReady}
                options={resultOptions}
                onChange={(e) => {
                  const value = e.target.value;
                  if (value === "") {
                    setValue(
                      "checklist",
                      checklist.filter((c) => c.itemCode !== itemCode),
                      { shouldDirty: true }
                    );
                    return;
                  }
                  update(itemCode, { result: value as SurveyChecklistResultValue });
                }}
              />
            </div>
          );
        })}
      </div>
    </section>
  );
}
