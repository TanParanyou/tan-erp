import type { FieldSurveyItem } from "./field-survey-schema";

/** scope_summary is capped at 2000 chars in the backend and also carries the style header and user notes. */
export const FIELD_SURVEY_SUMMARY_MAX_CHARS = 1200;
const MAX_TITLE_CHARS = 80;

export interface FieldSurveySummaryLabels {
  header: (count: number) => string;
  itemLine: (index: number, title: string, details: string) => string;
  more: (count: number) => string;
}

/**
 * Rule-based room summary: header plus one line per item, stopping before the length budget
 * and reporting how many items were left out. No external service is involved.
 */
export function buildFieldSurveySummary(
  items: readonly FieldSurveyItem[],
  describeItem: (item: FieldSurveyItem) => string,
  labels: FieldSurveySummaryLabels
): string {
  if (items.length === 0) return "";

  const lines = [labels.header(items.length)];
  let length = lines[0].length;

  for (const [index, item] of items.entries()) {
    const title = item.title.trim().slice(0, MAX_TITLE_CHARS);
    const line = labels.itemLine(index + 1, title, describeItem(item));
    const remaining = items.length - index;
    const moreLine = labels.more(remaining);

    if (length + line.length + 1 + moreLine.length + 1 > FIELD_SURVEY_SUMMARY_MAX_CHARS) {
      lines.push(moreLine);
      break;
    }
    lines.push(line);
    length += line.length + 1;
  }

  return lines.join("\n");
}
