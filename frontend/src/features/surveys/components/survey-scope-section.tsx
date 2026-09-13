"use client";

import React from "react";
import { useFormContext, Controller } from "react-hook-form";
import { useTranslations } from "next-intl";
import { Input } from "@/components/ui/Input";
import { Textarea } from "@/components/ui/Textarea";
import type { SurveyWorkspaceFormData } from "../schemas/survey-workspace-schema";

interface SurveyScopeSectionProps {
  isReady: boolean;
}

export function SurveyScopeSection({ isReady }: SurveyScopeSectionProps) {
  const t = useTranslations("surveys");
  const { control } = useFormContext<SurveyWorkspaceFormData>();

  return (
    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
      <div>
        <label
          htmlFor="visited-at-input"
          className="block text-sm font-semibold text-erp-text-main mb-1"
        >
          {t("visitedAtLabel")}
        </label>
        <Controller
          control={control}
          name="visitedAt"
          render={({ field }) => (
            <Input
              id="visited-at-input"
              type="datetime-local"
              value={field.value}
              onChange={field.onChange}
              disabled={isReady}
            />
          )}
        />
      </div>

      <div className="md:col-span-2">
        <label
          htmlFor="scope-summary-input"
          className="block text-sm font-semibold text-erp-text-main mb-1"
        >
          {t("scopeSummaryLabel")} <span className="text-erp-danger">*</span>
        </label>
        <Controller
          control={control}
          name="scopeSummary"
          render={({ field }) => (
            <Textarea
              id="scope-summary-input"
              rows={3}
              placeholder={t("scopeSummaryPlaceholder")}
              value={field.value}
              onChange={field.onChange}
              disabled={isReady}
            />
          )}
        />
      </div>
    </div>
  );
}
