"use client";

import React, { createContext, useCallback, useContext, useEffect, useState, useMemo } from "react";

export type ThemeMode = "light" | "dark" | "system";
export type ResolvedTheme = "light" | "dark";
export type Density = "comfortable" | "compact";

export const MIN_TEXT_SCALE = 90;
export const MAX_TEXT_SCALE = 130;
export const TEXT_SCALE_STEPS: readonly number[] = [90, 100, 110, 120, 130];

export interface ThemePreferences {
  themeMode: ThemeMode;
  density: Density;
  textScale: number;
}

export function isThemeMode(value: string): value is ThemeMode {
  return value === "light" || value === "dark" || value === "system";
}

export function isDensity(value: string): value is Density {
  return value === "comfortable" || value === "compact";
}

export function isTextScale(value: number): boolean {
  return Number.isInteger(value) && value >= MIN_TEXT_SCALE && value <= MAX_TEXT_SCALE;
}

interface ThemeContextValue {
  theme: ThemeMode;
  resolvedTheme: ResolvedTheme;
  density: Density;
  textScale: number;
  setTheme: (theme: ThemeMode) => void;
  toggleTheme: () => void;
  /** Applies preferences locally (html attributes + localStorage); it never calls the API. */
  applyPreferences: (preferences: ThemePreferences) => void;
}

const ThemeContext = createContext<ThemeContextValue | null>(null);

const STORAGE_KEY = "tan_erp_theme";
const DENSITY_STORAGE_KEY = "tan_erp_density";
const TEXT_SCALE_STORAGE_KEY = "tan_erp_text_scale";

function writeStorage(key: string, value: string): void {
  try {
    localStorage.setItem(key, value);
  } catch {
    // Ignore localStorage write failures in private/restricted browsing
  }
}

function getSystemTheme(): ResolvedTheme {
  if (typeof window === "undefined" || typeof window.matchMedia !== "function") return "light";
  return window.matchMedia("(prefers-color-scheme: dark)").matches ? "dark" : "light";
}

interface ThemeProviderProps {
  children: React.ReactNode;
  defaultTheme?: ThemeMode;
}

export function ThemeProvider({
  children,
  defaultTheme = "light",
}: ThemeProviderProps) {
  const [theme, setThemeState] = useState<ThemeMode>(() => {
    if (typeof window !== "undefined") {
      const stored = localStorage.getItem(STORAGE_KEY);
      if (stored === "light" || stored === "dark" || stored === "system") {
        return stored;
      }
    }
    return defaultTheme;
  });

  // localStorage is only the pre-hydration default; the server value from /me replaces it once loaded.
  const [density, setDensity] = useState<Density>(() => {
    if (typeof window === "undefined") return "comfortable";
    const stored = localStorage.getItem(DENSITY_STORAGE_KEY);
    return stored !== null && isDensity(stored) ? stored : "comfortable";
  });

  const [textScale, setTextScale] = useState<number>(() => {
    if (typeof window === "undefined") return 100;
    const stored = Number(localStorage.getItem(TEXT_SCALE_STORAGE_KEY));
    return isTextScale(stored) ? stored : 100;
  });

  const [systemTheme, setSystemTheme] = useState<ResolvedTheme>(() => getSystemTheme());

  useEffect(() => {
    if (typeof window === "undefined" || typeof window.matchMedia !== "function") return;
    const mediaQuery = window.matchMedia("(prefers-color-scheme: dark)");
    const handler = (e: MediaQueryListEvent) => {
      setSystemTheme(e.matches ? "dark" : "light");
    };

    mediaQuery.addEventListener("change", handler);
    return () => mediaQuery.removeEventListener("change", handler);
  }, []);

  const resolvedTheme: ResolvedTheme = useMemo(() => {
    if (theme === "system") return systemTheme;
    return theme;
  }, [theme, systemTheme]);

  useEffect(() => {
    const root = document.documentElement;
    root.setAttribute("data-theme", resolvedTheme);
    if (resolvedTheme === "dark") {
      root.classList.add("dark");
    } else {
      root.classList.remove("dark");
    }
  }, [resolvedTheme]);

  useEffect(() => {
    document.documentElement.setAttribute("data-density", density);
  }, [density]);

  useEffect(() => {
    // Root font-size percentage scales every rem-based size.
    document.documentElement.style.fontSize = textScale === 100 ? "" : `${textScale}%`;
  }, [textScale]);

  const setTheme = (newTheme: ThemeMode) => {
    setThemeState(newTheme);
    writeStorage(STORAGE_KEY, newTheme);
  };

  const applyPreferences = useCallback((preferences: ThemePreferences) => {
    setThemeState(preferences.themeMode);
    writeStorage(STORAGE_KEY, preferences.themeMode);
    setDensity(preferences.density);
    writeStorage(DENSITY_STORAGE_KEY, preferences.density);
    setTextScale(preferences.textScale);
    writeStorage(TEXT_SCALE_STORAGE_KEY, String(preferences.textScale));
  }, []);

  const toggleTheme = () => {
    const nextTheme: ThemeMode = resolvedTheme === "dark" ? "light" : "dark";
    setTheme(nextTheme);
  };

  const contextValue = useMemo(
    () => ({
      theme,
      resolvedTheme,
      density,
      textScale,
      setTheme,
      toggleTheme,
      applyPreferences,
    }),
    [theme, resolvedTheme, density, textScale, applyPreferences]
  );

  return (
    <ThemeContext.Provider value={contextValue}>
      {children}
    </ThemeContext.Provider>
  );
}

const defaultThemeContext: ThemeContextValue = {
  theme: "light",
  resolvedTheme: "light",
  density: "comfortable",
  textScale: 100,
  setTheme: () => {},
  toggleTheme: () => {},
  applyPreferences: () => {},
};

export function useTheme(): ThemeContextValue {
  const context = useContext(ThemeContext);
  return context || defaultThemeContext;
}

