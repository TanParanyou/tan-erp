import { describe, expect, it } from "vitest";
import { validateSurveyReadiness, type SurveyWorkspaceFormData } from "./survey-workspace-schema";

const REQUIRED = ["site_access_confirmed", "utilities_checked"] as const;

function buildForm(overrides: Partial<SurveyWorkspaceFormData> = {}): SurveyWorkspaceFormData {
  return {
    visitedAt: "2026-10-04T09:00",
    scopeSummary: "ขอบเขต",
    assumptions: [],
    constraints: [],
    missingDetails: [],
    areas: [
      {
        code: "AREA-01",
        name: "ห้องนอน",
        sortOrder: 1,
        measurements: [{ measurementType: "width", value: 3.2, unitCode: "m", captureMethod: "measured", sortOrder: 1 }],
      },
    ],
    checklist: [
      { itemCode: "site_access_confirmed", result: "pass" },
      { itemCode: "utilities_checked", result: "not_applicable", note: "ไม่มีงานระบบ" },
    ],
    evidence: [{ fileId: "file-1", kind: "site_photo", sortOrder: 1 }],
    ...overrides,
  };
}

describe("validateSurveyReadiness with template requirements", () => {
  const template = { requiredChecklistItems: REQUIRED, minimumEvidenceCount: 1 };

  it("passes when the checklist is complete and evidence meets the minimum", () => {
    expect(validateSurveyReadiness(buildForm(), template).isValid).toBe(true);
  });

  it("ignores checklist and evidence when no template is known", () => {
    expect(validateSurveyReadiness(buildForm({ checklist: [], evidence: [] }), null).isValid).toBe(true);
  });

  it("reports the first unanswered checklist item", () => {
    const result = validateSurveyReadiness(
      buildForm({ checklist: [{ itemCode: "site_access_confirmed", result: "pass" }] }),
      template
    );
    expect(result).toEqual({
      isValid: false,
      errorKey: "readinessErrors.missingChecklistItem",
      errorParams: { item: "utilities_checked" },
    });
  });

  it("requires a note for non-pass answers", () => {
    const result = validateSurveyReadiness(
      buildForm({
        checklist: [
          { itemCode: "site_access_confirmed", result: "pass" },
          { itemCode: "utilities_checked", result: "fail", note: "  " },
        ],
      }),
      template
    );
    expect(result.errorKey).toBe("readinessErrors.checklistNoteRequired");
  });

  it("requires the minimum amount of evidence", () => {
    const result = validateSurveyReadiness(buildForm({ evidence: [] }), template);
    expect(result).toEqual({ isValid: false, errorKey: "readinessErrors.missingEvidence", errorParams: { count: 1 } });
  });
});
