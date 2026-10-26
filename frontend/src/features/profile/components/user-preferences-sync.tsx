"use client";

import { useEffect } from "react";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { useTheme } from "@/providers/theme-provider";
import { readServerPreferences } from "../preferences";

/** Applies the signed-in user's saved preferences from /me to <html>; renders nothing. */
export function UserPreferencesSync() {
  const { currentUser } = useSelectedMembership();
  const { applyPreferences } = useTheme();
  const preferences = readServerPreferences(currentUser.user?.preferences);
  const themeMode = preferences?.themeMode;
  const density = preferences?.density;
  const textScale = preferences?.textScale;

  useEffect(() => {
    if (themeMode === undefined || density === undefined || textScale === undefined) return;
    applyPreferences({ themeMode, density, textScale });
  }, [themeMode, density, textScale, applyPreferences]);

  return null;
}
