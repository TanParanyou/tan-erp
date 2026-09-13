import type {
  CostComponentFormData,
  WorkItemFormData,
  SectionFormData,
} from "../schemas/estimate-workspace-schema";

export interface WorkItemFinancialSummary {
  unitCostSum: number;
  totalCost: number;
  unitSellingPrice: number;
  totalSellingPrice: number;
  grossProfit: number;
  marginRate: number;
  markupRate: number;
}

export interface WorkspaceFinancialSummary {
  totalCost: number;
  sellingBeforeDiscount: number;
  discountAmount: number;
  netSelling: number;
  grossProfit: number;
  marginRate: number;
  markupRate: number;
  isGpHealthy: boolean;
  totalSections: number;
  totalWorkItems: number;
}

/**
 * Calculates subtotal for a single cost component.
 */
export function calculateCostComponentSubtotal(
  quantity: number | undefined | null,
  unitCost: number | undefined | null
): number {
  const q = Number(quantity) || 0;
  const c = Number(unitCost) || 0;
  return Math.max(0, q * c);
}

/**
 * Calculates unit cost sum across all cost components in a work item.
 */
export function calculateWorkItemUnitCost(
  costComponents: CostComponentFormData[] | undefined | null
): number {
  if (!costComponents || costComponents.length === 0) return 0;
  return costComponents.reduce(
    (sum, comp) => sum + calculateCostComponentSubtotal(comp.quantity, comp.unitCost),
    0
  );
}

/**
 * Calculates unit selling price from unit cost and pricing rule.
 * - margin: selling = cost / (1 - marginDecimal)
 * - markup: selling = cost * (1 + markupDecimal)
 * - fixed: selling = ruleValue
 */
export function calculateUnitSellingPrice(
  unitCost: number,
  sellingRuleType: string | undefined | null,
  sellingRuleValue: number | undefined | null
): number {
  const ruleVal = Number(sellingRuleValue) || 0;
  const type = sellingRuleType || "margin";

  if (type === "margin") {
    const marginDecimal = ruleVal / 100;
    if (marginDecimal >= 1) return unitCost; // prevent division by zero or negative
    if (marginDecimal <= 0) return unitCost;
    return unitCost / (1 - marginDecimal);
  }

  if (type === "markup") {
    const markupDecimal = ruleVal / 100;
    return unitCost * (1 + Math.max(0, markupDecimal));
  }

  // fixed amount
  return Math.max(0, ruleVal);
}

/**
 * Calculates full financial summary for a work item.
 */
export function calculateWorkItemSummary(item: WorkItemFormData): WorkItemFinancialSummary {
  const quantity = Math.max(0, Number(item.quantity) || 0);
  const unitCostSum = calculateWorkItemUnitCost(item.costComponents);
  const totalCost = unitCostSum * quantity;
  const unitSellingPrice = calculateUnitSellingPrice(
    unitCostSum,
    item.sellingRuleType,
    item.sellingRuleValue
  );
  const totalSellingPrice = unitSellingPrice * quantity;
  const grossProfit = totalSellingPrice - totalCost;

  const marginRate = totalSellingPrice > 0 ? (grossProfit / totalSellingPrice) * 100 : 0;
  const markupRate = totalCost > 0 ? (grossProfit / totalCost) * 100 : 0;

  return {
    unitCostSum,
    totalCost,
    unitSellingPrice,
    totalSellingPrice,
    grossProfit,
    marginRate,
    markupRate,
  };
}

/**
 * Checks whether Gross Profit margin meets or exceeds organizational benchmark (default 30%).
 */
export function isGrossProfitHealthy(marginRate: number, benchmark: number = 30): boolean {
  return marginRate >= benchmark;
}

/**
 * Calculates live financial summary for the entire workspace (HUD).
 */
export function calculateLiveWorkspaceHud(
  sections: SectionFormData[] | undefined | null,
  discountAmount: number | undefined | null,
  benchmark: number = 30
): WorkspaceFinancialSummary {
  let totalCost = 0;
  let sellingBeforeDiscount = 0;
  let totalWorkItems = 0;

  if (sections && sections.length > 0) {
    for (const sec of sections) {
      if (sec.workItems && sec.workItems.length > 0) {
        totalWorkItems += sec.workItems.length;
        for (const item of sec.workItems) {
          const itemSummary = calculateWorkItemSummary(item);
          totalCost += itemSummary.totalCost;
          sellingBeforeDiscount += itemSummary.totalSellingPrice;
        }
      }
    }
  }

  const discount = Math.max(0, Number(discountAmount) || 0);
  const netSelling = Math.max(0, sellingBeforeDiscount - discount);
  const grossProfit = netSelling - totalCost;
  const marginRate = netSelling > 0 ? (grossProfit / netSelling) * 100 : 0;
  const markupRate = totalCost > 0 ? (grossProfit / totalCost) * 100 : 0;
  const isGpHealthy = isGrossProfitHealthy(marginRate, benchmark);

  return {
    totalCost,
    sellingBeforeDiscount,
    discountAmount: discount,
    netSelling,
    grossProfit,
    marginRate,
    markupRate,
    isGpHealthy,
    totalSections: sections ? sections.length : 0,
    totalWorkItems,
  };
}

export interface SectionFinancialSummary {
  totalCost: number;
  totalSellingPrice: number;
  grossProfit: number;
  marginRate: number;
  markupRate: number;
  totalItems: number;
  isGpHealthy: boolean;
}

/**
 * Calculates live financial summary for a specific section.
 */
export function calculateSectionSummary(
  section: SectionFormData | undefined | null,
  benchmark: number = 30
): SectionFinancialSummary {
  let totalCost = 0;
  let totalSellingPrice = 0;
  const items = section?.workItems || [];

  for (const item of items) {
    const itemSummary = calculateWorkItemSummary(item);
    totalCost += itemSummary.totalCost;
    totalSellingPrice += itemSummary.totalSellingPrice;
  }

  const grossProfit = totalSellingPrice - totalCost;
  const marginRate = totalSellingPrice > 0 ? (grossProfit / totalSellingPrice) * 100 : 0;
  const markupRate = totalCost > 0 ? (grossProfit / totalCost) * 100 : 0;
  const isGpHealthy = isGrossProfitHealthy(marginRate, benchmark);

  return {
    totalCost,
    totalSellingPrice,
    grossProfit,
    marginRate,
    markupRate,
    totalItems: items.length,
    isGpHealthy,
  };
}

