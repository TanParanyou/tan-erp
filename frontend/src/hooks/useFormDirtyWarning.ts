"use client";

import { useEffect } from "react";
import { useTranslations } from "next-intl";

/**
 * Hook to warn users before leaving the page when there are unsaved form changes.
 */
export function useFormDirtyWarning(isDirty: boolean, customMessage?: string) {
  const t = useTranslations("common.feedback");
  const message = customMessage || t("unsavedChangesWarning");
  useEffect(() => {
    if (!isDirty) return;

    const handleBeforeUnload = (e: BeforeUnloadEvent) => {
      e.preventDefault();
      e.returnValue = message;
      return message;
    };

    window.addEventListener("beforeunload", handleBeforeUnload);

    return () => {
      window.removeEventListener("beforeunload", handleBeforeUnload);
    };
  }, [isDirty, message]);
}
