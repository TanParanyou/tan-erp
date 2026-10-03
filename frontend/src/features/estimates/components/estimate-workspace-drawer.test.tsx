import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { act, fireEvent, render, screen, waitFor } from "@testing-library/react";
import { vi, describe, beforeEach, it, expect } from "vitest";
import { ApiError } from "@/lib/api/api-error";
import type { EstimateDetailResponse } from "@/lib/api/api-client";
import type { EstimateWorkspaceFormData } from "../schemas/estimate-workspace-schema";
import { EstimateWorkspaceDrawer } from "./estimate-workspace-drawer";

const mocks = vi.hoisted(() => ({
  update: vi.fn(),
  calculate: vi.fn(),
  toast: { success: vi.fn(), error: vi.fn() },
}));

vi.mock("next-intl", () => ({
  useTranslations: () => (key: string) => key,
  useLocale: () => "th",
}));

vi.mock("@/features/estimates/api/estimate-queries", async () => {
  const { useState } = await import("react");
  return {
    useUpdateEstimateDraft: () => {
      const [isPending, setIsPending] = useState(false);
      return {
        isPending,
        mutateAsync: async (input: unknown) => {
          setIsPending(true);
          try {
            return await mocks.update(input);
          } finally {
            setIsPending(false);
          }
        },
      };
    },
    useCalculateEstimate: () => ({ isPending: false, mutateAsync: mocks.calculate }),
  };
});

vi.mock("@/hooks/useToast", () => ({
  useToast: () => ({ toast: mocks.toast }),
}));

vi.mock("@/hooks/useConfirm", () => ({
  useConfirm: () => ({ confirm: vi.fn(), ConfirmDialog: () => null }),
}));

vi.mock("@/hooks/useKeyboardShortcut", () => ({
  useKeyboardShortcut: () => undefined,
}));

vi.mock("./estimate-item-catalog-modal", () => ({ EstimateItemCatalogModal: () => null }));

vi.mock("./estimate-workspace-hud", () => ({
  EstimateWorkspaceHud: () => null,
}));

vi.mock("./estimate-section-card", async () => {
  const { useFormContext } = await import("react-hook-form");
  return {
    EstimateSectionCard: ({ sectionIndex }: { sectionIndex: number }) => {
      const { register, watch } = useFormContext<EstimateWorkspaceFormData>();
      const namePath: `sections.${number}.nameTh` = `sections.${sectionIndex}.nameTh`;
      const workItemsPath: `sections.${number}.workItems` = `sections.${sectionIndex}.workItems`;
      const workItems = watch(workItemsPath);
      return (
        <>
          <input aria-label="section name" {...register(namePath)} />
          {workItems?.map((_, itemIndex) => {
            const valuePath: `sections.${number}.workItems.${number}.sellingRuleValue` =
              `sections.${sectionIndex}.workItems.${itemIndex}.sellingRuleValue`;
            return (
              <input
                key={valuePath}
                aria-label={`selling rule value ${itemIndex + 1}`}
                type="number"
                {...register(valuePath, { valueAsNumber: true })}
              />
            );
          })}
        </>
      );
    },
  };
});

const estimate = {
  id: "estimate-1",
  branchId: "branch-1",
  number: "EST-1",
  rowVersion: "estimate-version-1",
  currentRevision: {
    id: "revision-1",
    estimateId: "estimate-1",
    revisionNo: 1,
    status: "draft",
    currency: "THB",
    rowVersion: "revision-version-1",
    discountType: "none",
    discountValue: 0,
    calculationVersion: 0,
    calculationOutdated: true,
    sections: [{
      id: "section-1",
      code: "SEC-1",
      nameTh: "เดิม",
      nameEn: "Original",
      sortOrder: 1,
      workItems: [],
    }],
  },
} satisfies EstimateDetailResponse;

function renderDrawer(estimateOverride: EstimateDetailResponse = estimate) {
  const queryClient = new QueryClient({
    defaultOptions: { queries: { retry: false }, mutations: { retry: false } },
  });
  return render(
    <QueryClientProvider client={queryClient}>
      <EstimateWorkspaceDrawer
        isOpen
        onClose={vi.fn()}
        opportunityId="opportunity-1"
        estimate={estimateOverride}
      />
    </QueryClientProvider>,
  );
}

describe("EstimateWorkspaceDrawer save state", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("locks duplicate saves and retains the unsaved input after a version conflict", async () => {
    let rejectUpdate: ((reason: unknown) => void) | undefined;
    mocks.update.mockImplementation(() => new Promise<never>((_, reject) => {
      rejectUpdate = reject;
    }));
    renderDrawer();

    const sectionName = await screen.findByRole("textbox", { name: "section name" });
    fireEvent.change(sectionName, { target: { value: "แก้ไขที่ยังไม่บันทึก" } });
    const saveButton = screen.getByRole("button", { name: /saveDraftAction/ });
    fireEvent.click(saveButton);

    await waitFor(() => expect(saveButton).toBeDisabled());
    fireEvent.click(saveButton);
    expect(mocks.update).toHaveBeenCalledTimes(1);

    await act(async () => {
      rejectUpdate?.(new ApiError({
        status: 409,
        code: "ESTIMATE_VERSION_CONFLICT",
        message: "stale revision",
      }));
    });

    expect(mocks.toast.error).toHaveBeenCalledWith("versionConflict");
    expect(sectionName).toHaveValue("แก้ไขที่ยังไม่บันทึก");
    expect(saveButton).toBeEnabled();
  });

  it("round trips margin and markup percentages while preserving fixed prices", async () => {
    const estimateWithSellingRules: EstimateDetailResponse = {
      ...estimate,
      currentRevision: {
        ...estimate.currentRevision,
        sections: [{
          ...estimate.currentRevision.sections[0],
          workItems: [
            {
              id: "work-item-margin",
              estimateSectionId: "section-1",
              code: "ITM-MARGIN",
              descriptionTh: "งาน Margin",
              descriptionEn: null,
              quantity: 1,
              unitCode: "lot",
              sellingRuleType: "margin",
              sellingRuleValue: 0.25,
              sellingRuleReasonCode: null,
              overrideReasonCode: "test-custom-work-item",
              overrideReason: "Test fixture custom item rationale",
              unitCost: 100,
              totalCost: 100,
              unitSellingPrice: 133.33,
              totalSellingPrice: 133.33,
              sortOrder: 1,
              costComponents: [],
            },
            {
              id: "work-item-markup",
              estimateSectionId: "section-1",
              code: "ITM-MARKUP",
              descriptionTh: "งาน Markup",
              descriptionEn: null,
              quantity: 1,
              unitCode: "lot",
              sellingRuleType: "markup",
              sellingRuleValue: 0.3,
              sellingRuleReasonCode: null,
              overrideReasonCode: "test-custom-work-item",
              overrideReason: "Test fixture custom item rationale",
              unitCost: 100,
              totalCost: 100,
              unitSellingPrice: 130,
              totalSellingPrice: 130,
              sortOrder: 2,
              costComponents: [],
            },
            {
              id: "work-item-fixed",
              estimateSectionId: "section-1",
              code: "ITM-FIXED",
              descriptionTh: "งานราคาคงที่",
              descriptionEn: null,
              quantity: 1,
              unitCode: "lot",
              sellingRuleType: "fixed_price",
              sellingRuleValue: 1250,
              sellingRuleReasonCode: "approved-override",
              overrideReasonCode: "test-custom-work-item",
              overrideReason: "Test fixture custom item rationale",
              unitCost: 100,
              totalCost: 100,
              unitSellingPrice: 1250,
              totalSellingPrice: 1250,
              sortOrder: 3,
              costComponents: [],
            },
          ],
        }],
      },
    };
    const estimateRevision = estimateWithSellingRules.currentRevision;
    if (!estimateRevision) throw new Error("Estimate fixture requires a current revision");
    mocks.update.mockResolvedValue({
      rowVersion: "revision-version-2",
      sections: estimateRevision.sections,
    });

    renderDrawer(estimateWithSellingRules);

    expect(await screen.findByRole("spinbutton", { name: "selling rule value 1" })).toHaveValue(25);
    expect(screen.getByRole("spinbutton", { name: "selling rule value 2" })).toHaveValue(30);
    expect(screen.getByRole("spinbutton", { name: "selling rule value 3" })).toHaveValue(1250);

    fireEvent.click(screen.getByRole("button", { name: /saveDraftAction/ }));

    await waitFor(() => expect(mocks.update).toHaveBeenCalledWith(expect.objectContaining({
      payload: expect.objectContaining({
        sections: [expect.objectContaining({
          workItems: [
            expect.objectContaining({ sellingRuleValue: 0.25 }),
            expect.objectContaining({ sellingRuleValue: 0.3 }),
            expect.objectContaining({ sellingRuleValue: 1250 }),
          ],
        })],
      }),
    })));
    await waitFor(() => expect(mocks.toast.success).toHaveBeenCalledWith("saveDraftSuccess"));
    expect(screen.getByRole("spinbutton", { name: "selling rule value 1" })).toHaveValue(25);
    expect(screen.getByRole("spinbutton", { name: "selling rule value 2" })).toHaveValue(30);
    expect(screen.getByRole("spinbutton", { name: "selling rule value 3" })).toHaveValue(1250);
  });

  it.each([
    ["ESTIMATE_UNIT_INVALID", "unitInvalid"],
    ["ITEM_COST_STALE", "costStale"],
    ["ITEM_COST_AMBIGUOUS", "costAmbiguous"],
    ["ITEM_COST_NOT_FOUND", "costNotFound"],
  ])("shows the localized %s cost resolution message", async (code, messageKey) => {
    mocks.update.mockRejectedValueOnce(new ApiError({ status: 409, code, message: code }));
    renderDrawer();

    const sectionName = await screen.findByRole("textbox", { name: "section name" });
    fireEvent.change(sectionName, { target: { value: "เก็บค่าที่แก้ไว้" } });
    fireEvent.click(screen.getByRole("button", { name: /saveDraftAction/ }));

    await waitFor(() => expect(mocks.toast.error).toHaveBeenCalledWith(messageKey));
    expect(sectionName).toHaveValue("เก็บค่าที่แก้ไว้");
  });
  it("saves changed BOQ before calculating with the returned revision version", async () => {
    mocks.update.mockResolvedValue({ ...estimate.currentRevision, rowVersion: "version-saved", sections: [{ ...estimate.currentRevision.sections[0], nameTh: "ใหม่" }] });
    mocks.calculate.mockResolvedValue({ ...estimate.currentRevision, rowVersion: "version-calculated", discountType: "percent", discountValue: 0.1, discountReasonCode: "AGREED" });
    renderDrawer();
    fireEvent.change(await screen.findByRole("textbox", { name: "section name" }), { target: { value: "ใหม่" } });
    fireEvent.change(screen.getByLabelText("discountType"), { target: { value: "percent" } });
    fireEvent.change(screen.getByLabelText("discountValue"), { target: { value: "10" } });
    fireEvent.change(screen.getByLabelText(/discountReasonCode/), { target: { value: "AGREED" } });
    fireEvent.click(screen.getByRole("button", { name: "workspace.saveAndCalculate" }));
    await waitFor(() => expect(mocks.calculate).toHaveBeenCalledWith(expect.objectContaining({ expectedRevisionVersion: "version-saved", discountValue: 0.1 })));
    expect(mocks.update.mock.invocationCallOrder[0]).toBeLessThan(mocks.calculate.mock.invocationCallOrder[0]);
    expect(screen.getByLabelText("discountValue")).toHaveValue(10);
  });

  it("keeps unapplied discount settings dirty after saving BOQ", async () => {
    mocks.update.mockResolvedValue({ ...estimate.currentRevision, rowVersion: "version-saved" });
    renderDrawer();
    fireEvent.change(screen.getByLabelText("discountType"), { target: { value: "percent" } });
    fireEvent.change(screen.getByLabelText("discountValue"), { target: { value: "10" } });
    fireEvent.change(screen.getByLabelText(/discountReasonCode/), { target: { value: "AGREED" } });
    fireEvent.click(screen.getByRole("button", { name: /saveDraftAction/ }));
    await waitFor(() => expect(mocks.toast.success).toHaveBeenCalledWith("saveDraftSuccess"));
    expect(screen.getByLabelText("discountValue")).toHaveValue(10);
    expect(screen.getByText("workspace.unsaved")).toBeInTheDocument();
    expect(mocks.calculate).not.toHaveBeenCalled();
  });

  it("does not calculate after draft saving fails", async () => {
    mocks.update.mockRejectedValue(new ApiError({ status: 409, code: "ESTIMATE_VERSION_CONFLICT", message: "conflict" }));
    renderDrawer();
    fireEvent.change(await screen.findByRole("textbox", { name: "section name" }), { target: { value: "ใหม่" } });
    fireEvent.click(screen.getByRole("button", { name: "workspace.saveAndCalculate" }));
    await waitFor(() => expect(mocks.toast.error).toHaveBeenCalledWith("versionConflict"));
    expect(mocks.calculate).not.toHaveBeenCalled();
    expect(screen.getByRole("textbox", { name: "section name" })).toHaveValue("ใหม่");
  });

  it("retries calculation after a successful save without resaving or reverting the version", async () => {
    mocks.update.mockResolvedValue({ ...estimate.currentRevision, rowVersion: "version-saved", sections: [{ ...estimate.currentRevision.sections[0], nameTh: "ใหม่" }] });
    mocks.calculate.mockRejectedValueOnce(new ApiError({ status: 503, code: "CALCULATION_UNAVAILABLE", message: "unavailable" }));
    mocks.calculate.mockResolvedValueOnce({ ...estimate.currentRevision, rowVersion: "version-calculated" });
    renderDrawer();
    fireEvent.change(await screen.findByRole("textbox", { name: "section name" }), { target: { value: "ใหม่" } });
    fireEvent.click(screen.getByRole("button", { name: "workspace.saveAndCalculate" }));
    await waitFor(() => expect(screen.getByText("workspace.calculationFailed")).toBeInTheDocument());
    fireEvent.click(screen.getByRole("button", { name: "workspace.saveAndCalculate" }));
    await waitFor(() => expect(mocks.toast.success).toHaveBeenCalledWith("calculateSuccess"));
    expect(mocks.update).toHaveBeenCalledTimes(1);
    expect(mocks.calculate).toHaveBeenCalledTimes(2);
    expect(mocks.calculate.mock.calls[1][0]).toMatchObject({ expectedRevisionVersion: "version-saved" });
  });

  it("validates BOQ before sending requests", async () => {
    const invalid: EstimateDetailResponse = { ...estimate, currentRevision: { ...estimate.currentRevision, sections: [{ ...estimate.currentRevision.sections[0], code: "" }] } };
    renderDrawer(invalid);
    fireEvent.click(screen.getByRole("button", { name: "workspace.saveAndCalculate" }));
    await waitFor(() => expect(screen.getByText("workspace.validationFailed")).toBeInTheDocument());
    expect(mocks.update).not.toHaveBeenCalled();
    expect(mocks.calculate).not.toHaveBeenCalled();
  });

  it("does not silently adopt a refetched version while the user has unsaved BOQ edits", async () => {
    mocks.update.mockRejectedValue(new ApiError({ status: 409, code: "ESTIMATE_VERSION_CONFLICT", message: "conflict" }));
    const view = renderDrawer();
    fireEvent.change(await screen.findByRole("textbox", { name: "section name" }), { target: { value: "ค่าของผู้ใช้" } });
    const remote: EstimateDetailResponse = { ...estimate, currentRevision: { ...estimate.currentRevision, rowVersion: "remote-version", sections: [{ ...estimate.currentRevision.sections[0], nameTh: "ค่าจากผู้ใช้อื่น" }] } };
    view.rerender(<QueryClientProvider client={new QueryClient()}><EstimateWorkspaceDrawer isOpen onClose={vi.fn()} estimate={remote} /></QueryClientProvider>);
    expect(screen.getByRole("textbox", { name: "section name" })).toHaveValue("ค่าของผู้ใช้");
    fireEvent.click(screen.getByRole("button", { name: /saveDraftAction/ }));
    await waitFor(() => expect(mocks.update).toHaveBeenCalledWith(expect.objectContaining({ ifMatch: '"revision-version-1"', payload: expect.objectContaining({ expectedRevisionVersion: "revision-version-1" }) })));
    expect(mocks.toast.error).toHaveBeenCalledWith("versionConflict");
  });

});
