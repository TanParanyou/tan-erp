"use client";

import React from "react";
import { useTheme } from "@/providers/theme-provider";
import { useTranslations } from "next-intl";
import { IconSun, IconMoon } from "@/components/common/Icons";

interface ThemeToggleProps {
  className?: string;
  style?: React.CSSProperties;
}

export function ThemeToggle({ className, style }: ThemeToggleProps) {
  const [mounted, setMounted] = React.useState(false);
  const { resolvedTheme, toggleTheme } = useTheme();
  const t = useTranslations("shell");

  React.useEffect(() => {
    setMounted(true);
  }, []);

  const isDark = mounted ? resolvedTheme === "dark" : false;
  const label = isDark ? t("lightMode") : t("darkMode");

  return (
    <button
      type="button"
      onClick={toggleTheme}
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
        border: "1px solid rgba(255, 255, 255, 0.3)",
        borderRadius: 0,
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
