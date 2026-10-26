"use client";

import React, { useState } from "react";
import { useTranslations } from "next-intl";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { Select } from "@/components/ui/Select";
import { SegmentedControl } from "@/components/ui/SegmentedControl";
import { FormSection } from "@/components/forms/FormSection";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { useToast } from "@/hooks/useToast";
import {
  TEXT_SCALE_STEPS,
  isDensity,
  isThemeMode,
  useTheme,
  type Density,
  type ThemeMode,
} from "@/providers/theme-provider";
import { useUpdateMyPreferences } from "../api/profile-queries";
import { readServerPreferences } from "../preferences";
import { profileErrorKey } from "../profile-errors";

/** Draft starts from the saved server values; dirty means different from what is applied now. */
export function AppearancePreferencesForm() {
  const t = useTranslations("myProfile");
  const { toast } = useToast();
  const { theme, density, textScale, applyPreferences } = useTheme();
  const { currentUser } = useSelectedMembership();
  const updateMutation = useUpdateMyPreferences();
  const saved = readServerPreferences(currentUser.user?.preferences);

  const [draftMode, setDraftMode] = useState<ThemeMode>(saved?.themeMode ?? theme);
  const [draftDensity, setDraftDensity] = useState<Density>(saved?.density ?? density);
  const [draftScale, setDraftScale] = useState<number>(saved?.textScale ?? textScale);
  const [submitError, setSubmitError] = useState<string | null>(null);

  const isDirty = draftMode !== theme || draftDensity !== density || draftScale !== textScale;
  const isBusy = updateMutation.isPending;

  const onSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    if (isBusy) return;
    setSubmitError(null);
    const next = { themeMode: draftMode, density: draftDensity, textScale: draftScale };
    try {
      await updateMutation.mutateAsync(next);
      applyPreferences(next);
      toast.success(t("appearance.saveSuccess"));
    } catch (error: unknown) {
      setSubmitError(t(`errors.${profileErrorKey(error)}`));
    }
  };

  return (
    <form onSubmit={onSubmit} noValidate className="flex flex-col gap-6">
      {submitError && <Alert variant="danger">{submitError}</Alert>}

      <FormSection title={t("tabs.appearance")} description={t("appearance.description")}>
        <div className="grid grid-cols-1 gap-6 md:grid-cols-2">
          <div className="flex flex-col gap-1.5">
            <span className="text-sm font-medium text-erp-text-main">{t("appearance.themeMode")}</span>
            <SegmentedControl<ThemeMode>
              size="md"
              aria-label={t("appearance.themeMode")}
              value={draftMode}
              onChange={(value) => isThemeMode(value) && setDraftMode(value)}
              disabled={isBusy}
              options={[
                { value: "light", label: t("appearance.themeLight") },
                { value: "dark", label: t("appearance.themeDark") },
                { value: "system", label: t("appearance.themeSystem") },
              ]}
            />
          </div>

          <div className="flex flex-col gap-1.5">
            <span className="text-sm font-medium text-erp-text-main">{t("appearance.density")}</span>
            <SegmentedControl<Density>
              size="md"
              aria-label={t("appearance.density")}
              value={draftDensity}
              onChange={(value) => isDensity(value) && setDraftDensity(value)}
              disabled={isBusy}
              options={[
                { value: "comfortable", label: t("appearance.densityComfortable") },
                { value: "compact", label: t("appearance.densityCompact") },
              ]}
            />
          </div>

          <Select
            label={t("appearance.textScale")}
            helperText={t("appearance.textScaleHint")}
            disabled={isBusy}
            value={String(draftScale)}
            onChange={(event) => setDraftScale(Number(event.target.value))}
            options={TEXT_SCALE_STEPS.map((step) => ({ value: String(step), label: `${step}%` }))}
          />
        </div>
      </FormSection>

      <div className="flex justify-end">
        <Button type="submit" isLoading={isBusy} disabled={isBusy || !isDirty}>
          {t("appearance.save")}
        </Button>
      </div>
    </form>
  );
}
