import React from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import { FormProvider, useForm } from "react-hook-form";
import thMessages from "@/messages/th.json";
import { EstimateCostUnitSelect } from "./estimate-cost-unit-select";
import * as unitOptionsHook from "../hooks/use-estimate-unit-options";
import type { EstimateWorkspaceFormData } from "../schemas/estimate-workspace-schema";
import type { EstimateUnitOptionsResponse } from "@/lib/api/api-client";

function TestWrapper({
  defaultValues,
  children,
}: {
  defaultValues: unknown;
  children: React.ReactNode;
}) {
  const methods = useForm<EstimateWorkspaceFormData>({
    defaultValues: defaultValues as EstimateWorkspaceFormData,
  });

  return (
    <NextIntlClientProvider locale="th" messages={thMessages}>
      <FormProvider {...methods}>
        {children}
        <div data-testid="watched-unit">{methods.watch("sections.0.workItems.0.costComponents.0.unitCode")}</div>
        <div data-testid="watched-cost">{methods.watch("sections.0.workItems.0.costComponents.0.unitCost")}</div>
        <div data-testid="watched-conversion">
          {JSON.stringify(methods.watch("sections.0.workItems.0.costComponents.0.conversion"))}
        </div>
      </FormProvider>
    </NextIntlClientProvider>
  );
}

const mockUnitOptions: EstimateUnitOptionsResponse = {
  baseUnitCode: "M",
  conversionsEnabled: true,
  units: [
    { unitCode: "M", isBase: true, factor: "1", conversionId: null, conversionScope: null },
    { unitCode: "BOX", isBase: false, factor: "12", conversionId: "conv-box-12", conversionScope: "organization" },
  ],
};

function createFormState(overrides: { itemId?: string | null; baseUnitCost?: number | null; unitCode?: string } = {}) {
  return {
    sections: [
      {
        workItems: [
          {
            costComponents: [
              {
                itemId: overrides.itemId !== undefined ? overrides.itemId : "item-wood",
                unitCode: overrides.unitCode ?? "M",
                baseUnitCost: overrides.baseUnitCost !== undefined ? overrides.baseUnitCost : 100,
                unitCost: 100,
                conversion: null,
              },
            ],
          },
        ],
      },
    ],
  };
}

describe("EstimateCostUnitSelect (G-08 Estimate Unit Conversion)", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("renders null if cost line has no itemId", () => {
    vi.spyOn(unitOptionsHook, "useEstimateUnitOptions").mockReturnValue({
      data: mockUnitOptions,
      isLoading: false,
      isError: false,
    } as unknown as ReturnType<typeof unitOptionsHook.useEstimateUnitOptions>);

    const { container } = render(
      <TestWrapper defaultValues={createFormState({ itemId: null })}>
        <EstimateCostUnitSelect sectionIndex={0} itemIndex={0} costIndex={0} branchId="branch-1" />
      </TestWrapper>
    );

    expect(container.querySelector("select")).toBeNull();
  });

  it("renders null if conversions are disabled or item has less than 2 units", () => {
    vi.spyOn(unitOptionsHook, "useEstimateUnitOptions").mockReturnValue({
      data: {
        ...mockUnitOptions,
        conversionsEnabled: false,
      },
      isLoading: false,
      isError: false,
    } as unknown as ReturnType<typeof unitOptionsHook.useEstimateUnitOptions>);

    const { container } = render(
      <TestWrapper defaultValues={createFormState()}>
        <EstimateCostUnitSelect sectionIndex={0} itemIndex={0} costIndex={0} branchId="branch-1" />
      </TestWrapper>
    );

    expect(container.querySelector("select")).toBeNull();
  });

  it("renders options with base unit and conversion formula when conversions are enabled", () => {
    vi.spyOn(unitOptionsHook, "useEstimateUnitOptions").mockReturnValue({
      data: mockUnitOptions,
      isLoading: false,
      isError: false,
    } as unknown as ReturnType<typeof unitOptionsHook.useEstimateUnitOptions>);

    render(
      <TestWrapper defaultValues={createFormState()}>
        <EstimateCostUnitSelect sectionIndex={0} itemIndex={0} costIndex={0} branchId="branch-1" />
      </TestWrapper>
    );

    const select = screen.getByLabelText(thMessages.estimates.unitSelectLabel);
    expect(select).toBeDefined();

    const options = screen.getAllByRole("option");
    expect(options).toHaveLength(2);
    expect(options[0].textContent).toContain(thMessages.estimates.unitOptionBase.replace("{unit}", "M"));
    expect(options[1].textContent).toContain(
      thMessages.estimates.unitOptionConverted.replaceAll("{unit}", "BOX").replace("{factor}", "12").replace("{base}", "M")
    );
  });

  it("updates unitCode, freezes conversion snapshot, and recalculates unitCost on selecting converted unit", () => {
    vi.spyOn(unitOptionsHook, "useEstimateUnitOptions").mockReturnValue({
      data: mockUnitOptions,
      isLoading: false,
      isError: false,
    } as unknown as ReturnType<typeof unitOptionsHook.useEstimateUnitOptions>);

    render(
      <TestWrapper defaultValues={createFormState({ baseUnitCost: 150 })}>
        <EstimateCostUnitSelect sectionIndex={0} itemIndex={0} costIndex={0} branchId="branch-1" />
      </TestWrapper>
    );

    const select = screen.getByLabelText(thMessages.estimates.unitSelectLabel);
    fireEvent.change(select, { target: { value: "BOX" } });

    // unitCode changed to BOX
    expect(screen.getByTestId("watched-unit").textContent).toBe("BOX");
    // unitCost recalculated to 150 * 12 = 1800
    expect(screen.getByTestId("watched-cost").textContent).toBe("1800");

    // conversion snapshot is frozen
    const conversion = JSON.parse(screen.getByTestId("watched-conversion").textContent ?? "null");
    expect(conversion).toEqual({
      conversionId: "conv-box-12",
      scope: "organization",
      fromUnit: "BOX",
      toUnit: "M",
      factor: "12",
    });

    // Switch back to base unit M
    fireEvent.change(select, { target: { value: "M" } });
    expect(screen.getByTestId("watched-unit").textContent).toBe("M");
    expect(screen.getByTestId("watched-cost").textContent).toBe("150");
    const restoredConversion = JSON.parse(screen.getByTestId("watched-conversion").textContent ?? "null");
    expect(restoredConversion).toBeNull();
  });

  it("disables select when baseUnitCost is null", () => {
    vi.spyOn(unitOptionsHook, "useEstimateUnitOptions").mockReturnValue({
      data: mockUnitOptions,
      isLoading: false,
      isError: false,
    } as unknown as ReturnType<typeof unitOptionsHook.useEstimateUnitOptions>);

    render(
      <TestWrapper defaultValues={createFormState({ baseUnitCost: null })}>
        <EstimateCostUnitSelect sectionIndex={0} itemIndex={0} costIndex={0} branchId="branch-1" />
      </TestWrapper>
    );

    const select = screen.getByLabelText(thMessages.estimates.unitSelectLabel);
    expect(select).toHaveProperty("disabled", true);
  });
});
