import type { SectionFormData, WorkItemFormData } from "../schemas/estimate-workspace-schema";
import { calculateWorkItemSummary } from "./estimate-calculations";
import { MARGIN_BENCHMARK_PERCENT } from "./estimate-margin-tone";

export function workspaceWorkMatches(section: SectionFormData, work: WorkItemFormData, search: string, filter: "all" | "low_margin") {
  if (filter === "low_margin" && calculateWorkItemSummary(work).marginRate >= MARGIN_BENCHMARK_PERCENT) return false;
  const query = search.trim().toLowerCase();
  return !query || [section.code, section.nameTh, section.nameEn, work.code, work.descriptionTh, work.descriptionEn,
    ...work.costComponents.map((cost) => cost.description)].some((value) => value.toLowerCase().includes(query));
}
