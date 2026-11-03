export const MARGIN_BENCHMARK_PERCENT = 30;

export type MarginTone = "success" | "warning" | "danger";

/** Green meets the benchmark, amber is below it and needs attention, red means the line loses money. */
export function marginTone(marginRate: number, grossProfit: number, benchmark: number = MARGIN_BENCHMARK_PERCENT): MarginTone {
  if (grossProfit < 0) return "danger";
  return marginRate >= benchmark ? "success" : "warning";
}

export const MARGIN_TEXT_CLASS: Record<MarginTone, string> = {
  success: "text-erp-success",
  warning: "text-erp-warning-text",
  danger: "text-erp-danger",
};

export const MARGIN_BADGE_CLASS: Record<MarginTone, string> = {
  success: "bg-erp-success-bg text-erp-success border-erp-success-border",
  warning: "bg-erp-warning-bg text-erp-warning-text border-erp-warning-border",
  danger: "bg-erp-danger-bg text-erp-danger border-erp-danger-border",
};
