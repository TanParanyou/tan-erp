"use client";

import { useQuery } from "@tanstack/react-query";
import { useTranslations } from "next-intl";

export interface EstimateOptionItem<T extends string = string> {
  value: T;
  labelKey: string;
  defaultLabelTh: string;
  defaultLabelEn: string;
}

export interface EstimateOptionsData {
  sellingRuleTypes: EstimateOptionItem[];
  costTypes: EstimateOptionItem[];
  commonUnits: EstimateOptionItem[];
  currencies: EstimateOptionItem[];
}

export const DEFAULT_SELLING_RULE_OPTIONS: EstimateOptionItem[] = [
  {
    value: "margin",
    labelKey: "sellingRuleTypes.margin",
    defaultLabelTh: "Margin % (คิดจากราคาขาย)",
    defaultLabelEn: "Margin % (On Selling Price)",
  },
  {
    value: "markup",
    labelKey: "sellingRuleTypes.markup",
    defaultLabelTh: "Markup % (คิดจากต้นทุน)",
    defaultLabelEn: "Markup % (On Cost)",
  },
  {
    value: "fixed",
    labelKey: "sellingRuleTypes.fixed",
    defaultLabelTh: "ราคาคงที่ (Fixed Amount)",
    defaultLabelEn: "Fixed Amount",
  },
];

export const DEFAULT_COST_TYPE_OPTIONS: EstimateOptionItem[] = [
  {
    value: "material",
    labelKey: "costTypes.material",
    defaultLabelTh: "ค่าวัสดุ (Material)",
    defaultLabelEn: "Material",
  },
  {
    value: "labor",
    labelKey: "costTypes.labor",
    defaultLabelTh: "ค่าแรง (Labor)",
    defaultLabelEn: "Labor",
  },
  {
    value: "subcontract",
    labelKey: "costTypes.subcontract",
    defaultLabelTh: "ค่าจ้างเหมา (Subcontract)",
    defaultLabelEn: "Subcontract",
  },
  {
    value: "equipment",
    labelKey: "costTypes.equipment",
    defaultLabelTh: "ค่าเครื่องจักร/อุปกรณ์ (Equipment)",
    defaultLabelEn: "Equipment",
  },
  {
    value: "overhead",
    labelKey: "costTypes.overhead",
    defaultLabelTh: "โสหุ้ย (Overhead)",
    defaultLabelEn: "Overhead",
  },
  {
    value: "other",
    labelKey: "costTypes.other",
    defaultLabelTh: "อื่นๆ (Other)",
    defaultLabelEn: "Other",
  },
];

export const DEFAULT_UNIT_OPTIONS: EstimateOptionItem[] = [
  {
    value: "lot",
    labelKey: "units.lot",
    defaultLabelTh: "ชุด/เหมา (Lot)",
    defaultLabelEn: "Lot",
  },
  {
    value: "set",
    labelKey: "units.set",
    defaultLabelTh: "ชุด (Set)",
    defaultLabelEn: "Set",
  },
  {
    value: "sqm",
    labelKey: "units.sqm",
    defaultLabelTh: "ตร.ม. (Sq.m.)",
    defaultLabelEn: "Sq.m.",
  },
  {
    value: "m",
    labelKey: "units.m",
    defaultLabelTh: "เมตร (m)",
    defaultLabelEn: "Meter",
  },
  {
    value: "pcs",
    labelKey: "units.pcs",
    defaultLabelTh: "ชิ้น (Pcs)",
    defaultLabelEn: "Pieces",
  },
  {
    value: "job",
    labelKey: "units.job",
    defaultLabelTh: "งาน (Job)",
    defaultLabelEn: "Job",
  },
];

export const DEFAULT_CURRENCIES: EstimateOptionItem[] = [
  {
    value: "THB",
    labelKey: "currency",
    defaultLabelTh: "บาท (THB)",
    defaultLabelEn: "Thai Baht (THB)",
  },
];

/**
 * Async fetcher for estimate options.
 * Ready to be connected to a future master data endpoint (e.g. apiClient.getEstimateOptions).
 */
export async function fetchEstimateOptions(): Promise<EstimateOptionsData> {
  return Promise.resolve({
    sellingRuleTypes: DEFAULT_SELLING_RULE_OPTIONS,
    costTypes: DEFAULT_COST_TYPE_OPTIONS,
    commonUnits: DEFAULT_UNIT_OPTIONS,
    currencies: DEFAULT_CURRENCIES,
  });
}

/**
 * useEstimateOptions hook with TanStack Query.
 * Formats options into UI-ready items with translated labels.
 */
export function useEstimateOptions() {
  const t = useTranslations("estimates");

  const query = useQuery({
    queryKey: ["estimates", "options"] as const,
    queryFn: fetchEstimateOptions,
    staleTime: 1000 * 60 * 60, // 1 hour
    initialData: {
      sellingRuleTypes: DEFAULT_SELLING_RULE_OPTIONS,
      costTypes: DEFAULT_COST_TYPE_OPTIONS,
      commonUnits: DEFAULT_UNIT_OPTIONS,
      currencies: DEFAULT_CURRENCIES,
    },
  });

  const data = query.data;

  const sellingRuleOptions = data.sellingRuleTypes.map((item) => ({
    value: item.value,
    label: t(item.labelKey as Parameters<typeof t>[0]),
  }));

  const costTypeOptions = data.costTypes.map((item) => ({
    value: item.value,
    label: t(item.labelKey as Parameters<typeof t>[0]),
  }));

  const unitOptions = data.commonUnits.map((item) => ({
    value: item.value,
    label: t(item.labelKey as Parameters<typeof t>[0]),
  }));

  const currencyOptions = data.currencies.map((item) => ({
    value: item.value,
    label: item.value,
  }));

  return {
    ...query,
    options: {
      sellingRuleTypes: sellingRuleOptions,
      costTypes: costTypeOptions,
      units: unitOptions,
      currencies: currencyOptions,
    },
    raw: data,
  };
}
