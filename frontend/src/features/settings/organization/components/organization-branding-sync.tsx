"use client";

import { useEffect } from "react";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { useTheme } from "@/providers/theme-provider";

/** Applies the selected organization's brand colour from /me to <html>; renders nothing. */
export function OrganizationBrandingSync() {
  const { selectedMembership } = useSelectedMembership();
  const { applyBrandColor } = useTheme();
  const primaryColor = selectedMembership?.organization?.branding?.primaryColor ?? null;

  useEffect(() => {
    applyBrandColor(primaryColor);
  }, [primaryColor, applyBrandColor]);

  return null;
}
