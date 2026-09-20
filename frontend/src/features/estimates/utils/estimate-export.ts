import { generateCsvContent, type CsvColumn, exportToCsv } from "@/lib/export/export-csv";

export interface EstimateCalculationSnapshot {
  readonly calculationVersion: number;
  readonly calculatedAtUtc: string;
  readonly currency: string;
  readonly netCost: number;
  readonly sellingBeforeDiscount: number;
  readonly discountAmount: number;
  readonly netBeforeTax: number;
  readonly taxAmount: number;
  readonly grandTotal: number;
  readonly marginAmount: number;
  readonly marginRate: number;
  readonly markupRate: number;
  readonly sectionsCount: number;
  readonly workItemsCount: number;
  readonly costComponentsCount: number;
}

export function isEstimateCalculationSnapshot(data: unknown): data is EstimateCalculationSnapshot {
  if (typeof data !== "object" || data === null) {
    return false;
  }
  const record = data as Record<string, unknown>;
  return (
    typeof record.calculationVersion === "number" &&
    typeof record.calculatedAtUtc === "string" &&
    typeof record.currency === "string" &&
    typeof record.netCost === "number" &&
    typeof record.sellingBeforeDiscount === "number" &&
    typeof record.discountAmount === "number" &&
    typeof record.netBeforeTax === "number" &&
    typeof record.taxAmount === "number" &&
    typeof record.grandTotal === "number" &&
    typeof record.marginAmount === "number" &&
    typeof record.marginRate === "number" &&
    typeof record.markupRate === "number" &&
    typeof record.sectionsCount === "number" &&
    typeof record.workItemsCount === "number" &&
    typeof record.costComponentsCount === "number"
  );
}

const snapshotCsvColumns: readonly CsvColumn<EstimateCalculationSnapshot>[] = [
  { header: "Calculation Version", accessor: (s) => s.calculationVersion },
  { header: "Calculated At (UTC)", accessor: (s) => s.calculatedAtUtc },
  { header: "Currency", accessor: (s) => s.currency },
  { header: "Net Cost", accessor: (s) => s.netCost },
  { header: "Selling Before Discount", accessor: (s) => s.sellingBeforeDiscount },
  { header: "Discount Amount", accessor: (s) => s.discountAmount },
  { header: "Net Before Tax", accessor: (s) => s.netBeforeTax },
  { header: "Tax Amount", accessor: (s) => s.taxAmount },
  { header: "Grand Total", accessor: (s) => s.grandTotal },
  { header: "Margin Amount", accessor: (s) => s.marginAmount },
  { header: "Margin Rate", accessor: (s) => s.marginRate },
  { header: "Markup Rate", accessor: (s) => s.markupRate },
  { header: "Sections Count", accessor: (s) => s.sectionsCount },
  { header: "Work Items Count", accessor: (s) => s.workItemsCount },
  { header: "Cost Components Count", accessor: (s) => s.costComponentsCount },
];

export function buildEstimateCsv(snapshot: EstimateCalculationSnapshot): string {
  return generateCsvContent(snapshotCsvColumns, [snapshot]);
}

export function exportEstimateSnapshotCsv(filename: string, snapshot: EstimateCalculationSnapshot): void {
  exportToCsv({
    filename,
    columns: snapshotCsvColumns,
    data: [snapshot],
  });
}
