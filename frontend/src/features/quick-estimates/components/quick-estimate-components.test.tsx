import React from "react";
import { afterEach, describe, expect, it, vi } from "vitest";
import { render, screen, fireEvent, waitFor, cleanup } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import enMessages from "@/messages/en.json";
import { QuickEstimateWorkspace } from "./quick-estimate-workspace";
import { PricingTemplateDetail } from "./pricing-template-detail";
import * as queries from "../api/quick-estimate-queries";
import type { PricingTemplateResponse, QuickEstimateResponse } from "@/lib/api/api-client";
import {
  QUICK_ESTIMATE_STATUSES,
  SHARE_REASONS,
  TEMPLATE_STATUSES,
  quickEstimateErrorCode,
  quickEstimateStatusVariant,
  shareDecisionVariant,
  templateStatusVariant,
} from "../quick-estimate-status";

const toastMocks = vi.hoisted(() => ({ success: vi.fn(), error: vi.fn() }));
const permissions = vi.hoisted(() => ({ granted: [] as string[] }));

vi.mock("@/hooks/useToast", () => ({
  useToast: () => ({ toast: { success: toastMocks.success, error: toastMocks.error } }),
}));
vi.mock("next/navigation", () => ({
  usePathname: () => "/th/quick-estimates",
  useRouter: () => ({ push: vi.fn(), replace: vi.fn(), back: vi.fn() }),
  useSearchParams: () => new URLSearchParams(),
}));
vi.mock("@/lib/membership/selected-membership-context", () => ({
  useSelectedMembership: () => ({ selectedMembership: { id: "membership-1" } }),
}));
vi.mock("@/lib/permissions/can", () => ({
  can: (_membership: unknown, permission: string) => permissions.granted.includes(permission),
}));

const e = thMessages.quickEstimates.estimates;
const tpl = thMessages.quickEstimates.templates;
const ALL = ["quick-estimates.create", "quick-estimates.read", "quick-estimates.update", "quick-estimates.review", "quick-estimates.share", "quick-estimates.convert", "pricing-templates.manage", "pricing-templates.approve"];

function estimate(status: string, overrides: Partial<QuickEstimateResponse> = {}): QuickEstimateResponse {
  return {
    id: "qe-1",
    number: "QE-2026-0001",
    status,
    template: { id: "t-1", code: "BI", version: 1, name: "บิลท์อิน", status: "active" },
    propertyType: "คอนโด",
    roomOrArea: "ห้องนั่งเล่น",
    gradeCode: "PREMIUM",
    complexityCodes: [],
    addOnCodes: [],
    measurementConfidence: "high",
    customMaterial: false,
    measurements: [{ lineId: "l-1", workSubtype: "ตู้", widthM: 3, heightM: 2.6, depthM: null, quantity: 1 }],
    currentCalculationVersion: 1,
    currentCalculation: {
      version: 1, netAmount: 86120, taxAmount: 0, displayedLower: 75000, displayedUpper: 97000, validUntil: "2026-11-04",
      shareDecision: status === "calculated" ? "shareable" : "pending_review", reasonCodes: status === "calculated" ? [] : ["MEASUREMENT_CONFIDENCE_LOW"],
      inputHash: "a".repeat(64), lines: [{ lineId: "l-1", workSubtype: "ตู้", billableQuantity: 7.8, baseAmount: 62400, adjustedAmount: 81120 }],
    },
    reviews: [], shares: [], conversions: [],
    rowVersion: "00000000-0000-0000-0000-0000000000a1",
    ...overrides,
  } as QuickEstimateResponse;
}

const mutate = {
  saveDraft: vi.fn(), calculate: vi.fn(), submitReview: vi.fn(), decideReview: vi.fn(), share: vi.fn(), convert: vi.fn(), create: vi.fn(),
};

function renderWorkspace(data: QuickEstimateResponse, granted: string[] = ALL) {
  permissions.granted = granted;
  vi.spyOn(queries, "useQuickEstimate").mockReturnValue({ data, isLoading: false, isError: false } as unknown as ReturnType<typeof queries.useQuickEstimate>);
  vi.spyOn(queries, "useEffectiveTemplates").mockReturnValue({
    data: [{ id: "t-1", code: "BI", version: 1, name: "บิลท์อิน", grades: [{ code: "PREMIUM", name: "พรีเมียม" }], complexities: [], addOns: [] }],
    isLoading: false,
  } as unknown as ReturnType<typeof queries.useEffectiveTemplates>);
  vi.spyOn(queries, "useQuickEstimateMutations").mockReturnValue(
    Object.fromEntries(Object.entries(mutate).map(([key, fn]) => [key, { mutateAsync: fn, isPending: false }])) as unknown as ReturnType<typeof queries.useQuickEstimateMutations>,
  );
  render(
    <NextIntlClientProvider locale="th" messages={thMessages}>
      <QuickEstimateWorkspace estimateId="qe-1" />
    </NextIntlClientProvider>,
  );
}

afterEach(() => {
  cleanup();
  vi.restoreAllMocks();
  Object.values(mutate).forEach((fn) => fn.mockReset());
  toastMocks.success.mockReset();
});

describe("QuickEstimateWorkspace", () => {
  it("shows the displayed range, share decision and the indicative-only note", () => {
    renderWorkspace(estimate("calculated"));
    expect(screen.getByText(e.indicativeNote)).toBeInTheDocument();
    expect(screen.getByText(e.shareDecisions.shareable)).toBeInTheDocument();
    expect(screen.getByText(/75,000/)).toBeInTheDocument();
  });

  it("explains why a calculation needs review", () => {
    renderWorkspace(estimate("pending_review"));
    expect(screen.getByText(e.shareReasons.MEASUREMENT_CONFIDENCE_LOW)).toBeInTheDocument();
  });

  it("submits the current calculation version for review", async () => {
    mutate.submitReview.mockResolvedValue(estimate("pending_review"));
    renderWorkspace(estimate("calculated"));
    fireEvent.click(screen.getByRole("button", { name: e.submitReview }));
    await waitFor(() => expect(mutate.submitReview).toHaveBeenCalledWith(expect.objectContaining({ id: "qe-1", sourceVersion: 1 })));
  });

  it("requires a reason code before approving a review", async () => {
    renderWorkspace(estimate("pending_review"));
    fireEvent.click(screen.getByRole("button", { name: e.approve }));
    fireEvent.click(screen.getByRole("button", { name: thMessages.common.actions.confirm }));
    expect(await screen.findByText(e.reasonCodeRequired)).toBeInTheDocument();
    expect(mutate.decideReview).not.toHaveBeenCalled();
  });

  it("hides workflow actions without permission and locks inputs after conversion", () => {
    renderWorkspace(estimate("converted", { conversions: [{ id: "c-1", sourceVersion: 1, officialEstimateId: "est-1" }] }), ["quick-estimates.read"]);
    expect(screen.queryByRole("button", { name: e.convert })).not.toBeInTheDocument();
    expect(screen.queryByRole("button", { name: e.saveAndCalculate })).not.toBeInTheDocument();
    expect(screen.getByText(e.openEstimate)).toBeInTheDocument();
  });

  it("shows a localized message for a known error code", async () => {
    const { ApiError } = await import("@/lib/api/api-error");
    mutate.submitReview.mockRejectedValue(new ApiError({ status: 422, code: "QUICK_ESTIMATE_STALE_VERSION", message: "x" }));
    renderWorkspace(estimate("calculated"));
    fireEvent.click(screen.getByRole("button", { name: e.submitReview }));
    expect(await screen.findByText(thMessages.quickEstimates.errors.QUICK_ESTIMATE_STALE_VERSION)).toBeInTheDocument();
  });
});

describe("PricingTemplateDetail", () => {
  function template(status: string): PricingTemplateResponse {
    return {
      id: "t-1", code: "BI", version: 1, name: "บิลท์อิน", status, workType: "built-in", measurementRule: "area", unitCode: "sqm", taxDisplay: "exclusive",
      referenceRate: 8000, minimumCharge: 20000, baseRangeRate: 0.1, maxRangeRate: 0.3, roundingStep: 1000, validityDays: 30, taxRate: 0.07, directShareLimit: 500000,
      grades: [{ code: "PREMIUM", name: "พรีเมียม", factor: 1.3 }], complexities: [], addOns: [], rowVersion: "00000000-0000-0000-0000-0000000000b1",
    } as PricingTemplateResponse;
  }
  const step = vi.fn();
  const decide = vi.fn();

  function renderTemplate(status: string, granted: string[] = ALL) {
    permissions.granted = granted;
    vi.spyOn(queries, "usePricingTemplate").mockReturnValue({ data: template(status), isLoading: false, isError: false } as unknown as ReturnType<typeof queries.usePricingTemplate>);
    vi.spyOn(queries, "usePricingTemplateMutations").mockReturnValue({
      step: { mutateAsync: step, isPending: false }, decide: { mutateAsync: decide, isPending: false },
      newVersion: { mutateAsync: vi.fn(), isPending: false }, create: { mutateAsync: vi.fn(), isPending: false }, update: { mutateAsync: vi.fn(), isPending: false },
    } as unknown as ReturnType<typeof queries.usePricingTemplateMutations>);
    render(
      <NextIntlClientProvider locale="th" messages={thMessages}>
        <PricingTemplateDetail templateId="t-1" />
      </NextIntlClientProvider>,
    );
  }

  it("offers submit for a draft and sends the row version", async () => {
    step.mockResolvedValue(template("submitted"));
    renderTemplate("draft");
    fireEvent.click(screen.getByRole("button", { name: tpl.submit }));
    await waitFor(() => expect(step).toHaveBeenCalledWith({ id: "t-1", rowVersion: "00000000-0000-0000-0000-0000000000b1", step: "submit" }));
  });

  it("offers approve and return only to approvers of a submitted template", () => {
    renderTemplate("submitted", ["pricing-templates.manage"]);
    expect(screen.queryByRole("button", { name: tpl.approve })).not.toBeInTheDocument();
    cleanup();
    renderTemplate("submitted");
    expect(screen.getByRole("button", { name: tpl.approve })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: tpl.return })).toBeInTheDocument();
  });

  it("warns that a calibration template always needs review", () => {
    renderTemplate("calibration");
    expect(screen.getByText(tpl.calibrationNote)).toBeInTheDocument();
  });
});

describe("quick estimate helpers and messages", () => {
  it("maps statuses to badge variants", () => {
    expect(templateStatusVariant("active")).toBe("success");
    expect(templateStatusVariant("disabled")).toBe("danger");
    expect(quickEstimateStatusVariant("pending_review")).toBe("warning");
    expect(shareDecisionVariant("blocked")).toBe("danger");
    expect(shareDecisionVariant(null)).toBe("neutral");
  });

  it("accepts only known error codes", () => {
    expect(quickEstimateErrorCode("QUICK_ESTIMATE_EXPIRED")).toBe("QUICK_ESTIMATE_EXPIRED");
    expect(quickEstimateErrorCode("SOMETHING_ELSE")).toBeNull();
  });

  it("has Thai and English text for every status, reason and error code", () => {
    for (const messages of [thMessages, enMessages]) {
      const root = messages.quickEstimates;
      for (const status of TEMPLATE_STATUSES) expect(root.templates.statuses[status]).toBeTruthy();
      for (const status of QUICK_ESTIMATE_STATUSES) expect(root.estimates.statuses[status]).toBeTruthy();
      for (const reason of SHARE_REASONS) expect(root.estimates.shareReasons[reason]).toBeTruthy();
      expect(Object.keys(root.errors).length).toBeGreaterThan(20);
    }
    expect(Object.keys(thMessages.quickEstimates.errors)).toEqual(Object.keys(enMessages.quickEstimates.errors));
  });
});
