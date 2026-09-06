import { useSafeLocale } from "@/lib/i18n/i18n-context";

/**
 * Centralized formatting functions for ERP calculations, financial displays, and datetime values.
 */

/**
 * Resolve supported BCP-47 locale tag for Intl formatting.
 */
export function resolveIntlLocale(locale: string = "th"): string {
  return locale === "en" ? "en-US" : "th-TH";
}

/**
 * Format a number as currency (defaults to Thai Baht "THB").
 */
export function formatCurrency(
  num: number | string | null | undefined,
  currency: string = "THB",
  locale: string = "th",
  decimals: number = 2
): string {
  if (num === null || num === undefined || num === "") return "-";
  const parsed = typeof num === "number" ? num : Number(num);
  if (isNaN(parsed)) return "-";

  const intlLocale = resolveIntlLocale(locale);

  try {
    return new Intl.NumberFormat(intlLocale, {
      style: "currency",
      currency,
      minimumFractionDigits: decimals,
      maximumFractionDigits: decimals,
    }).format(parsed);
  } catch {
    return `${currency} ${parsed.toFixed(decimals)}`;
  }
}

/**
 * Format a number with thousands separators and customizable decimal precision.
 * Useful for stock quantities, unit measures, and dimensions.
 */
export function formatNumber(
  num: number | string | null | undefined,
  minDecimals: number = 0,
  maxDecimals: number = 4,
  locale: string = "th"
): string {
  if (num === null || num === undefined || num === "") return "-";
  const parsed = typeof num === "number" ? num : Number(num);
  if (isNaN(parsed)) return "-";

  const intlLocale = resolveIntlLocale(locale);

  return new Intl.NumberFormat(intlLocale, {
    minimumFractionDigits: minDecimals,
    maximumFractionDigits: maxDecimals,
  }).format(parsed);
}

/**
 * Format a date string or Date object into a readable date (e.g. 15 ม.ค. 2567 or Jan 15, 2024).
 */
export function formatDate(
  dateStr: string | Date | null | undefined,
  locale: string = "th"
): string {
  if (!dateStr) return "-";
  const date = typeof dateStr === "string" ? new Date(dateStr) : dateStr;
  if (isNaN(date.getTime())) return "-";

  const intlLocale = resolveIntlLocale(locale);

  return date.toLocaleDateString(intlLocale, {
    year: "numeric",
    month: "short",
    day: "numeric",
  });
}

/**
 * Format a date string or Date object with both date and time (HH:mm).
 */
export function formatDateTime(
  dateStr: string | Date | null | undefined,
  locale: string = "th"
): string {
  if (!dateStr) return "-";
  const date = typeof dateStr === "string" ? new Date(dateStr) : dateStr;
  if (isNaN(date.getTime())) return "-";

  const intlLocale = resolveIntlLocale(locale);

  return date.toLocaleString(intlLocale, {
    year: "numeric",
    month: "short",
    day: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
}

/**
 * Format raw byte size into human-readable unit (e.g. 2.4 MB).
 */
export function formatFileSize(bytes: number | null | undefined): string {
  if (bytes === null || bytes === undefined || isNaN(bytes) || bytes < 0) return "-";
  if (bytes === 0) return "0 B";

  const units = ["B", "KB", "MB", "GB", "TB"];
  const i = Math.floor(Math.log(bytes) / Math.log(1024));
  const size = bytes / Math.pow(1024, i);

  const formattedSize = size % 1 === 0 ? size.toFixed(0) : size.toFixed(1);
  return `${formattedSize} ${units[i]}`;
}

/**
 * Hook providing locale-aware formatting functions that automatically respect the active route locale.
 */
export function useFormatters() {
  const locale = useSafeLocale();

  return {
    locale,
    formatCurrency: (num: number | string | null | undefined, currency: string = "THB", decimals: number = 2) =>
      formatCurrency(num, currency, locale, decimals),
    formatNumber: (num: number | string | null | undefined, minDecimals: number = 0, maxDecimals: number = 4) =>
      formatNumber(num, minDecimals, maxDecimals, locale),
    formatDate: (dateStr: string | Date | null | undefined) =>
      formatDate(dateStr, locale),
    formatDateTime: (dateStr: string | Date | null | undefined) =>
      formatDateTime(dateStr, locale),
    formatFileSize,
  };
}
