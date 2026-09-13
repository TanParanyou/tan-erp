import { formatNumber, formatCurrency } from "@/lib/formatters/formatters";

/**
 * Formats a financial amount with standard 2 decimal places and comma separators.
 * e.g. 433150 -> "433,150.00"
 */
export function formatFinancialNumber(
  val: number | string | null | undefined,
  minimumFractionDigits: number = 2,
  maximumFractionDigits: number = 2
): string {
  if (val === null || val === undefined) return "-";
  const num = Number(val);
  if (isNaN(num)) return "-";

  return num.toLocaleString("th-TH", {
    minimumFractionDigits,
    maximumFractionDigits,
  });
}

/**
 * Formats an amount into full localized currency.
 * e.g. 601500, "THB", "th" -> "฿601,500.00"
 */
export function formatCurrencyAmount(
  val: number | string | null | undefined,
  currency: string = "THB",
  locale: string = "th"
): string {
  return formatCurrency(val, currency, locale);
}

/**
 * Formats percentage rate with optional decimals.
 * e.g. 38.8712 -> "38.87%"
 */
export function formatPercentRate(
  val: number | string | null | undefined,
  decimals: number = 2
): string {
  if (val === null || val === undefined) return "0.00%";
  const num = Number(val);
  if (isNaN(num)) return "0.00%";
  return `${num.toFixed(decimals)}%`;
}

/**
 * Formats financial amount with an explicit plus or minus sign.
 * e.g. 168350 -> "+168,350.00", -5000 -> "-5,000.00"
 */
export function formatSignedFinancialAmount(
  val: number | string | null | undefined,
  currencySuffix?: string
): string {
  if (val === null || val === undefined) return "-";
  const num = Number(val);
  if (isNaN(num)) return "-";

  const formatted = Math.abs(num).toLocaleString("th-TH", {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  });

  const sign = num > 0 ? "+" : num < 0 ? "-" : "";
  const suffix = currencySuffix ? ` ${currencySuffix}` : "";
  return `${sign}${formatted}${suffix}`;
}

/**
 * Formats integer count with comma separators.
 * e.g. 12 -> "12"
 */
export function formatIntegerCount(val: number | null | undefined): string {
  return formatNumber(val);
}
