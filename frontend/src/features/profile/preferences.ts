import type { UserPreferencesDto } from "@/lib/api/api-client";
import { isDensity, isTextScale, isThemeMode, type ThemePreferences } from "@/providers/theme-provider";

/** Narrows the structured /me preferences to UI types; returns null when absent or not a supported value. */
export function readServerPreferences(dto: UserPreferencesDto | null | undefined): ThemePreferences | null {
  if (!dto) return null;
  const { themeMode, density, textScale } = dto;
  if (typeof themeMode !== "string" || !isThemeMode(themeMode)) return null;
  if (typeof density !== "string" || !isDensity(density)) return null;
  if (typeof textScale !== "number" || !isTextScale(textScale)) return null;
  return { themeMode, density, textScale };
}
