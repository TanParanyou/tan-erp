import { describe, expect, it } from "vitest";
import type { EstimateRevisionResponse } from "@/lib/api/api-client";
import { toWorkspaceForm, toDraftRequest } from "./estimate-workspace-mapper";

const revision: EstimateRevisionResponse = {
  discountType: "percent", discountValue: 0.1, discountReasonCode: "AGREED",
  sections: [{ id: "section", code: "SEC", nameTh: "งาน", sortOrder: 1, workItems: [
    { id: "work", code: "W", quantity: 1, unitCode: "lot", sellingRuleType: "markup", sellingRuleValue: 0.3, sortOrder: 1,
      costComponents: [{ id: "cost", type: "material", description: "ไม้", quantity: 0, unitCost: 0, unitCode: "sheet", currency: "THB", sortOrder: 1, itemId: "item", costRecordId: "record", costRecordVersion: 7, isProvisional: false }] },
  ] }],
};

describe("workspace DTO mapping", () => {
  it("keeps percent values in UI units and preserves zero quantities and catalog references", () => {
    const form = toWorkspaceForm(revision);
    expect(form.discountValue).toBe(10);
    expect(form.sections[0].workItems[0].sellingRuleValue).toBe(30);
    const request = toDraftRequest(form, "version");
    expect(request.sections?.[0].workItems?.[0].sellingRuleValue).toBe(0.3);
    expect(request.sections?.[0].workItems?.[0].costComponents?.[0]).toMatchObject({ quantity: 0, unitCost: 0, itemId: "item", costRecordId: "record", costRecordVersion: 7 });
    expect(JSON.stringify(request)).not.toContain("uiKey");
  });
  it("preserves client selection identities when a save assigns server IDs", () => {
    const form = toWorkspaceForm(revision);
    form.sections[0].uiKey = "client-section";
    form.sections[0].workItems[0].uiKey = "client-work";
    expect(toWorkspaceForm(revision, form).sections[0]).toMatchObject({ uiKey: "client-section", workItems: [{ uiKey: "client-work" }] });
  });
  it("does not fabricate codes, units or valid pricing rules for absent API fields", () => {
    const form = toWorkspaceForm({ sections: [{ workItems: [{}] }] });
    expect(form.sections[0].code).toBe("");
    expect(form.sections[0].workItems[0]).toMatchObject({ code: "", unitCode: "", sellingRuleType: "", quantity: 0 });
  });
});
