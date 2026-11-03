import type { EstimateRevisionLinesResponse } from "@/lib/api/api-client";

type RevisionLine = EstimateRevisionLinesResponse["lines"][number];

export type RevisionDiffKind = "added" | "removed" | "changed";

export interface RevisionDiffRow {
  key: string;
  kind: RevisionDiffKind;
  sectionCode: string;
  workItemCode: string;
  descriptionTh: string;
  unitCode: string;
  from?: RevisionLine;
  to?: RevisionLine;
}

// Revisions copy their lines under new ids, so a line is matched by section code + work item code (+ order for repeated codes).
function indexLines(lines: readonly RevisionLine[]): Map<string, RevisionLine> {
  const seen = new Map<string, number>();
  const indexed = new Map<string, RevisionLine>();
  for (const line of lines) {
    const base = `${line.sectionCode}|${line.workItemCode}`;
    const count = seen.get(base) ?? 0;
    seen.set(base, count + 1);
    indexed.set(`${base}|${count}`, line);
  }
  return indexed;
}

const differs = (a: RevisionLine, b: RevisionLine) =>
  a.quantity !== b.quantity || a.unitCode !== b.unitCode || a.totalCost !== b.totalCost || a.totalSellingPrice !== b.totalSellingPrice;

export function diffRevisionLines(from: readonly RevisionLine[], to: readonly RevisionLine[]): RevisionDiffRow[] {
  const before = indexLines(from);
  const after = indexLines(to);
  const rows: RevisionDiffRow[] = [];
  for (const [key, line] of after) {
    const previous = before.get(key);
    if (!previous) rows.push({ key, kind: "added", sectionCode: line.sectionCode, workItemCode: line.workItemCode, descriptionTh: line.descriptionTh, unitCode: line.unitCode, to: line });
    else if (differs(previous, line)) rows.push({ key, kind: "changed", sectionCode: line.sectionCode, workItemCode: line.workItemCode, descriptionTh: line.descriptionTh, unitCode: line.unitCode, from: previous, to: line });
  }
  for (const [key, line] of before) {
    if (!after.has(key)) rows.push({ key, kind: "removed", sectionCode: line.sectionCode, workItemCode: line.workItemCode, descriptionTh: line.descriptionTh, unitCode: line.unitCode, from: line });
  }
  return rows;
}
