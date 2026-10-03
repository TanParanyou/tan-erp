import React from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { FormProvider, useForm } from "react-hook-form";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import enMessages from "@/messages/en.json";
import { SurveyChecklistSection } from "./survey-checklist-section";
import { SurveyEvidenceSection } from "./survey-evidence-section";
import * as opportunityQueries from "@/features/opportunities/api/opportunity-queries";
import type { SurveyWorkspaceFormData } from "../schemas/survey-workspace-schema";

vi.mock("@/components/common/AuthenticatedFileImage", () => ({
  AuthenticatedFileImage: ({ fileId, alt }: { fileId: string; alt?: string }) => <div data-testid={`img-${fileId}`}>{alt}</div>,
}));

let latestValues: SurveyWorkspaceFormData | null = null;

function Harness({ children, defaults }: { children: React.ReactNode; defaults?: Partial<SurveyWorkspaceFormData> }) {
  const methods = useForm<SurveyWorkspaceFormData>({
    defaultValues: {
      visitedAt: "",
      scopeSummary: "",
      assumptions: [],
      constraints: [],
      missingDetails: [],
      areas: [],
      checklist: [],
      evidence: [],
      ...defaults,
    },
  });
  latestValues = methods.watch();
  return (
    <NextIntlClientProvider locale="th" messages={thMessages}>
      <FormProvider {...methods}>{children}</FormProvider>
    </NextIntlClientProvider>
  );
}

describe("SurveyChecklistSection", () => {
  it("shows a translated row per required item and asks for a note on non-pass answers", () => {
    render(
      <Harness>
        <SurveyChecklistSection requiredItems={["site_access_confirmed", "utilities_checked"]} isReady={false} />
      </Harness>
    );

    expect(screen.getByText(thMessages.surveys.checklistItems.site_access_confirmed)).toBeDefined();
    const selects = screen.getAllByLabelText(thMessages.surveys.checklistResultLabel);
    expect(selects).toHaveLength(2);
    expect(screen.queryByLabelText(new RegExp(thMessages.surveys.checklistNoteLabel))).toBeNull();

    fireEvent.change(selects[1], { target: { value: "fail" } });

    expect(screen.getByLabelText(new RegExp(thMessages.surveys.checklistNoteLabel))).toBeDefined();
    expect(latestValues?.checklist).toEqual([{ itemCode: "utilities_checked", result: "fail", note: null }]);
  });

  it("locks the answers once the revision is ready", () => {
    render(
      <Harness defaults={{ checklist: [{ itemCode: "site_access_confirmed", result: "pass" }] }}>
        <SurveyChecklistSection requiredItems={["site_access_confirmed"]} isReady />
      </Harness>
    );

    expect((screen.getByLabelText(thMessages.surveys.checklistResultLabel) as HTMLSelectElement).disabled).toBe(true);
  });
});

describe("SurveyEvidenceSection", () => {
  beforeEach(() => {
    vi.restoreAllMocks();
    vi.spyOn(opportunityQueries, "useInfiniteOpportunityWorkImages").mockReturnValue({
      data: { pages: [{ items: [{ id: "w1", fileId: "file-1", caption: "มุมห้อง" }, { id: "w2", fileId: "file-2", caption: null }] }] },
      isLoading: false,
      isError: false,
      hasNextPage: false,
      fetchNextPage: vi.fn(),
      isFetchingNextPage: false,
    } as unknown as ReturnType<typeof opportunityQueries.useInfiniteOpportunityWorkImages>);
  });

  it("selects work images as evidence with a default kind", () => {
    render(
      <Harness>
        <SurveyEvidenceSection opportunityId="opp-1" minimumEvidenceCount={1} isReady={false} />
      </Harness>
    );

    const toggles = screen.getAllByLabelText(thMessages.surveys.evidenceUseAction);
    expect(toggles).toHaveLength(2);

    fireEvent.click(toggles[0]);

    expect(latestValues?.evidence).toEqual([{ fileId: "file-1", kind: "site_photo", caption: null, sortOrder: 1 }]);
    expect(screen.getAllByLabelText(thMessages.surveys.evidenceKindLabel)).toHaveLength(1);

    fireEvent.click(toggles[0]);
    expect(latestValues?.evidence).toEqual([]);
  });

  it("keeps saved evidence visible when its photo is no longer in the gallery", () => {
    render(
      <Harness defaults={{ evidence: [{ fileId: "file-gone", kind: "other", caption: "เดิม", sortOrder: 1 }] }}>
        <SurveyEvidenceSection opportunityId="opp-1" minimumEvidenceCount={1} isReady={false} />
      </Harness>
    );

    expect(screen.getByTestId("img-file-gone")).toBeDefined();
    expect(screen.getAllByLabelText(thMessages.surveys.evidenceUseAction)).toHaveLength(3);
  });

  it("shows an empty hint when there are no photos", () => {
    vi.spyOn(opportunityQueries, "useInfiniteOpportunityWorkImages").mockReturnValue({
      data: { pages: [{ items: [] }] },
      isLoading: false,
      isError: false,
      hasNextPage: false,
      fetchNextPage: vi.fn(),
      isFetchingNextPage: false,
    } as unknown as ReturnType<typeof opportunityQueries.useInfiniteOpportunityWorkImages>);

    render(
      <Harness>
        <SurveyEvidenceSection opportunityId="opp-1" minimumEvidenceCount={1} isReady={false} />
      </Harness>
    );

    expect(screen.getByText(thMessages.surveys.evidenceEmptyHint)).toBeDefined();
  });
});

describe("survey checklist/evidence translations", () => {
  it("exist in both languages for every template item and enum value", () => {
    for (const messages of [thMessages, enMessages]) {
      for (const code of ["site_access_confirmed", "utilities_checked", "existing_conditions_inspected", "customer_requirements_confirmed"]) {
        expect(messages.surveys.checklistItems[code as keyof typeof messages.surveys.checklistItems]).toBeTruthy();
      }
      for (const key of ["pass", "fail", "not_applicable"]) {
        expect(messages.surveys.checklistResults[key as keyof typeof messages.surveys.checklistResults]).toBeTruthy();
      }
      for (const key of ["site_photo", "measurement_sketch", "other"]) {
        expect(messages.surveys.evidenceKinds[key as keyof typeof messages.surveys.evidenceKinds]).toBeTruthy();
      }
    }
  });
});
