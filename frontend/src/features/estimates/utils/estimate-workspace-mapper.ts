import type { EstimateRevisionResponse, UpdateEstimateDraftRequest } from "@/lib/api/api-client";
import type { EstimateDiscountType, EstimateWorkspaceFormData } from "../schemas/estimate-workspace-schema";
import { getLocalizedText, type CatalogItemModel } from "../api/estimate-catalog-client";

function discountType(value: string | null | undefined): EstimateDiscountType {
  return value === "none" || value === "percent" || value === "fixed-amount" ? value : "";
}

/** Preserve client identities across our own save response, whose sort orders mirror the request. */
export function toWorkspaceForm(revision?: EstimateRevisionResponse | null, previous?: EstimateWorkspaceFormData): EstimateWorkspaceFormData {
  return {
    discountType: revision ? discountType(revision.discountType) : "none",
    discountValue: (revision?.discountValue ?? 0) * (revision?.discountType === "percent" ? 100 : 1),
    discountReasonCode: revision?.discountReasonCode ?? "",
    sections: [...(revision?.sections ?? [])].sort((a, b) => (a.sortOrder ?? 0) - (b.sortOrder ?? 0)).map((section, s) => ({
      id: section.id,
      uiKey: previous?.sections[s]?.uiKey ?? section.id ?? crypto.randomUUID(),
      code: section.code ?? "",
      nameTh: section.nameTh ?? "",
      nameEn: section.nameEn ?? "",
      sortOrder: section.sortOrder ?? s + 1,
      workItems: [...(section.workItems ?? [])].sort((a, b) => (a.sortOrder ?? 0) - (b.sortOrder ?? 0)).map((work, w) => ({
        id: work.id,
        uiKey: previous?.sections[s]?.workItems[w]?.uiKey ?? work.id ?? crypto.randomUUID(),
        code: work.code ?? "",
        descriptionTh: work.descriptionTh ?? "",
        descriptionEn: work.descriptionEn ?? "",
        quantity: work.quantity ?? 0,
        unitCode: work.unitCode ?? "",
        sellingRuleType: work.sellingRuleType ?? "",
        sellingRuleValue: (work.sellingRuleValue ?? 0) * (work.sellingRuleType === "margin" || work.sellingRuleType === "markup" ? 100 : 1),
        sellingRuleReasonCode: work.sellingRuleReasonCode ?? null,
        itemId: work.item?.id ?? null,
        item: work.item ?? null,
        overrideReasonCode: work.overrideReasonCode ?? null,
        overrideReason: work.overrideReason ?? null,
        sortOrder: work.sortOrder ?? w + 1,
        costComponents: [...(work.costComponents ?? [])].sort((a, b) => (a.sortOrder ?? 0) - (b.sortOrder ?? 0)).map((cost, c) => ({
          id: cost.id,
          type: cost.type ?? "",
          description: cost.description ?? "",
          quantity: cost.quantity ?? 0,
          unitCode: cost.unitCode ?? "",
          unitCost: cost.unitCost ?? 0,
          currency: cost.currency ?? "",
          sortOrder: cost.sortOrder ?? c + 1,
          itemId: cost.itemId ?? null,
          costRecordId: cost.costRecordId ?? null,
          costRecordVersion: cost.costRecordVersion ?? null,
          provisionalReasonCode: cost.provisionalReasonCode ?? null,
          provisionalNote: cost.provisionalNote ?? null,
          isProvisional: cost.isProvisional,
        })),
      })),
    })),
  };
}

export function toDraftRequest(data: EstimateWorkspaceFormData, version: string): UpdateEstimateDraftRequest {
  return {
    expectedRevisionVersion: version,
    sections: data.sections.map((section, s) => ({
      id: section.id ?? null, code: section.code, nameTh: section.nameTh, nameEn: section.nameEn, sortOrder: s + 1,
      workItems: section.workItems.map((work, w) => ({
        id: work.id ?? null, code: work.code, descriptionTh: work.descriptionTh, descriptionEn: work.descriptionEn,
        quantity: work.quantity, unitCode: work.unitCode, sellingRuleType: work.sellingRuleType,
        sellingRuleValue: work.sellingRuleValue / (work.sellingRuleType === "margin" || work.sellingRuleType === "markup" ? 100 : 1),
        sellingRuleReasonCode: work.sellingRuleReasonCode?.trim() || null,
        itemId: work.itemId?.trim() || null,
        overrideReasonCode: work.overrideReasonCode?.trim() || null,
        overrideReason: work.overrideReason?.trim() || null,
        sortOrder: w + 1,
        costComponents: work.costComponents.map((cost, c) => ({
          id: cost.id ?? null, type: cost.type, description: cost.description, quantity: cost.quantity,
          unitCode: cost.unitCode, unitCost: cost.unitCost, currency: cost.currency, sortOrder: c + 1,
          itemId: cost.itemId ?? null, costRecordId: cost.costRecordId ?? null, costRecordVersion: cost.costRecordVersion ?? null,
          provisionalReasonCode: cost.provisionalReasonCode ?? null, provisionalNote: cost.provisionalNote ?? null,
        })),
      })),
    })),
  };
}

export function catalogCostComponents(items: CatalogItemModel[], locale: string, start: number) {
  return items.flatMap((item, index) => {
    const cost = item.resolvedCost;
    if (!cost) return [];
    return [{
      type: item.costComponentType, description: getLocalizedText(item.name, locale), quantity: 1,
      unitCode: cost.unitCode, unitCost: cost.amount, currency: cost.currency, sortOrder: start + index + 1,
      itemId: item.id, costRecordId: cost.costRecordId, costRecordVersion: cost.version,
      isProvisional: !cost.costSourceId || (!cost.sourceReference && !cost.evidenceFileId),
    }];
  });
}
