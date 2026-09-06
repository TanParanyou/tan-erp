"use client";

import { useLocale as useNextIntlLocale } from "next-intl";
import { defaultLocale, type SupportedLocale } from "./locales";

export { useTranslations, useLocale } from "next-intl";

/**
 * Hook to safely retrieve active locale from next-intl, falling back to defaultLocale if outside provider.
 * Used by current-user-query which may be called outside the NextIntlClientProvider tree.
 */
export function useSafeLocale(): SupportedLocale {
  try {
    const rawLocale = useNextIntlLocale();
    if (rawLocale === "th" || rawLocale === "en") {
      return rawLocale;
    }
  } catch {
    // Fallback if rendered outside NextIntlClientProvider
  }
  return defaultLocale;
}
