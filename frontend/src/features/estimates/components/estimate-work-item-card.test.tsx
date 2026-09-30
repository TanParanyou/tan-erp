import { FormProvider, useForm } from "react-hook-form";
import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { EstimateWorkItemCard } from "./estimate-work-item-card";
import { estimateWorkspaceSchema, type EstimateWorkspaceFormData } from "../schemas/estimate-workspace-schema";

vi.mock("next-intl", () => ({
  useTranslations: () => (key: string) => key,
}));

vi.mock("../options/estimate-options", () => ({
  useEstimateOptions: () => ({
    options: {
      sellingRuleTypes: [
        { value: "margin", labelKey: "margin", defaultLabelTh: "margin", defaultLabelEn: "margin" },
        { value: "fixed_price", labelKey: "fixed_price", defaultLabelTh: "fixed_price", defaultLabelEn: "fixed_price" },
      ],
      costTypes: [],
      units: [],
      currencies: [],
    },
  }),
}));

vi.mock("@/hooks/useConfirm", () => ({
  useConfirm: () => ({ confirm: vi.fn(), ConfirmDialog: () => null }),
}));

vi.mock("./estimate-cost-component-table", () => ({
  EstimateCostComponentTable: () => <div data-testid="cost-components" />,
}));

vi.mock("@/components/forms/MultiLangInput", () => ({
  MultiLangInput: () => <div data-testid="description-input" />,
}));

const fixedPriceForm: EstimateWorkspaceFormData = {
  discountType: "none",
  discountValue: 0,
  discountReasonCode: "",
  sections: [{
    code: "SEC-01",
    nameTh: "TEST_ONLY",
    nameEn: "TEST_ONLY",
    sortOrder: 1,
    workItems: [{
      code: "WI-01",
      descriptionTh: "TEST_ONLY",
      descriptionEn: "TEST_ONLY",
      quantity: 1,
      unitCode: "unit",
      sellingRuleType: "fixed_price",
      sellingRuleValue: 100,
      sellingRuleReasonCode: "",
      itemId: null,
      overrideReasonCode: "TEST_ONLY",
      overrideReason: "Test fixture work item",
      sortOrder: 1,
      costComponents: [],
    }],
  }],
};

function FixedPriceForm() {
  const methods = useForm<EstimateWorkspaceFormData>({ defaultValues: fixedPriceForm });
  return (
    <FormProvider {...methods}>
      <form>
        <EstimateWorkItemCard
          sectionIndex={0}
          itemIndex={0}
          currency="THB"
          onDuplicateItem={vi.fn()}
          onRemoveItem={vi.fn()}
        />
      </form>
    </FormProvider>
  );
}

describe("EstimateWorkItemCard fixed price override", () => {
  it("shows a required reason field for fixed price rules", () => {
    render(<FixedPriceForm />);

    expect(screen.getByLabelText(/sellingRuleReasonCode/)).toBeRequired();
  });

  it("blocks a fixed price when the reason code is missing", () => {
    const result = estimateWorkspaceSchema.safeParse(fixedPriceForm);

    expect(result.success).toBe(false);
    if (!result.success) {
      expect(result.error.issues.some((issue) => issue.path.join(".") === "sections.0.workItems.0.sellingRuleReasonCode"))
        .toBe(true);
    }
  });

  it("accepts a fixed price when an auditable reason code is supplied", () => {
    const validForm = structuredClone(fixedPriceForm);
    validForm.sections[0].workItems[0].sellingRuleReasonCode = "CUSTOMER_AGREED_PRICE";

    expect(estimateWorkspaceSchema.safeParse(validForm).success).toBe(true);
  });
});
