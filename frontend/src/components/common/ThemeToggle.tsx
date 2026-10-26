"use client";

import React from "react";
import { useTheme, type ThemeMode } from "@/providers/theme-provider";
import { useTranslations } from "next-intl";
import { IconSun, IconMoon } from "@/components/common/Icons";

interface ThemeToggleProps {
  className?: string;
  style?: React.CSSProperties;
  /** Called with the new mode after a toggle, so a signed-in caller can persist it. */
  onThemeChange?: (mode: ThemeMode) => void;
  disabled?: boolean;
}

export function ThemeToggle({ className, style, onThemeChange, disabled = false }: ThemeToggleProps) {
  const [mounted, setMounted] = React.useState(false);
  const { resolvedTheme, setTheme } = useTheme();
  const t = useTranslations("shell");

  React.useEffect(() => {
    setMounted(true);
  }, []);

  const isDark = mounted ? resolvedTheme === "dark" : false;
  const label = isDark ? t("lightMode") : t("darkMode");

  return (
    <button
      type="button"
      disabled={disabled}
      onClick={() => {
        const next: ThemeMode = isDark ? "light" : "dark";
        setTheme(next);
        onThemeChange?.(next);
      }}
      className={className}
      aria-label={`${t("toggleTheme")}: ${label}`}
      title={`${t("toggleTheme")}: ${label}`}
      style={{
        display: "inline-flex",
        alignItems: "center",
        justifyContent: "center",
        minHeight: "44px",
        minWidth: "44px",
        padding: "0.5rem",
        backgroundColor: "transparent",
        color: "inherit",
        border: "1px solid color-mix(in srgb, var(--erp-text-on-navy) 30%, transparent)",
        borderRadius: "var(--erp-radius)",
        cursor: "pointer",
        ...style,
      }}
    >
      {isDark ? (
        <IconSun size={20} strokeWidth={2} />
      ) : (
        <IconMoon size={20} strokeWidth={2} />
      )}
    </button>
  );
}
