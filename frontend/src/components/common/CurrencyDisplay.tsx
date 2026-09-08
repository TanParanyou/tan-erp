"use client";

import React from "react";
import { useLocale } from "next-intl";
import { formatCurrency } from "@/lib/formatters/formatters";

export interface CurrencyDisplayProps {
  amount: number | string | null | undefined;
  currency?: string;
  locale?: string;
  className?: string;
}

export function CurrencyDisplay({
  amount,
  currency = "THB",
  locale: propLocale,
  className = "",
}: CurrencyDisplayProps) {
  const currentLocale = useLocale();
  const effectiveLocale = propLocale || currentLocale || "th";
  const formatted = formatCurrency(amount, currency, effectiveLocale);

  return <span className={`font-mono ${className}`}>{formatted}</span>;
}

export default CurrencyDisplay;
