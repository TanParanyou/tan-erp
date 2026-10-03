import React from "react";
import { describe, expect, it, vi, beforeEach, beforeAll } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import enMessages from "@/messages/en.json";
import { PublicAcceptancePage } from "./public-acceptance-page";
import * as publicQueries from "../api/public-acceptance-queries";
import { ApiError } from "@/lib/api/api-error";
import type { PublicAcceptanceViewResponse } from "@/lib/api/api-client";
import { ACCEPTANCE_LINK_STATES, acceptanceLinkErrorCode, acceptanceLinkStateLabel } from "@/features/estimates/quotation-lifecycle-status";

const p = thMessages.externalAcceptance.public;
const mutate = vi.fn();

function view(overrides: Partial<PublicAcceptanceViewResponse> = {}): PublicAcceptanceViewResponse {
  return {
    status: "active",
    expiresAtUtc: "2026-10-20T00:00:00Z",
    signerHint: "คุณสมชาย",
    consentVersion: "2026-10-v1",
    document: {
      number: "QT-2026-0001",
      issuedAtUtc: "2026-10-04T00:00:00Z",
      currency: "THB",
      locale: "th",
      hasIncompleteTranslations: false,
      customer: { displayName: "ลูกค้า" },
      sections: [],
      totals: { subtotal: 100, discountAmount: 0, netBeforeTax: 100, taxAmount: 7, grandTotal: 107 },
    },
    ...overrides,
  } as PublicAcceptanceViewResponse;
}

function renderPage(state: { data?: PublicAcceptanceViewResponse; isPending?: boolean; isError?: boolean }, mutation = { mutateAsync: mutate, isPending: false, isSuccess: false }) {
  vi.spyOn(publicQueries, "usePublicAcceptance").mockReturnValue({ isPending: false, isError: false, ...state } as unknown as ReturnType<typeof publicQueries.usePublicAcceptance>);
  vi.spyOn(publicQueries, "usePublicAcceptMutation").mockReturnValue(mutation as unknown as ReturnType<typeof publicQueries.usePublicAcceptMutation>);
  render(
    <NextIntlClientProvider locale="th" messages={thMessages}>
      <PublicAcceptancePage token={"t".repeat(43)} uiLocale="th" />
    </NextIntlClientProvider>
  );
}

describe("external acceptance helpers and translations", () => {
  it("derive the display state of a link and recognise only known errors", () => {
    expect(acceptanceLinkStateLabel({ status: "accepted" })).toBe("accepted");
    expect(acceptanceLinkStateLabel({ status: "revoked" })).toBe("revoked");
    expect(acceptanceLinkStateLabel({ status: "active", isUsable: true })).toBe("active");
    expect(acceptanceLinkStateLabel({ status: "active", isUsable: false, expiresAtUtc: "2000-01-01T00:00:00Z" })).toBe("expired");
    expect(acceptanceLinkStateLabel({ status: "active", isUsable: false, expiresAtUtc: "2999-01-01T00:00:00Z" })).toBe("unavailable");
    expect(acceptanceLinkErrorCode("ACCEPTANCE_CONSENT_REQUIRED")).toBe("ACCEPTANCE_CONSENT_REQUIRED");
    expect(acceptanceLinkErrorCode("NOPE")).toBeNull();
  });

  it("have Thai and English text for every state and error", () => {
    for (const messages of [thMessages, enMessages]) {
      for (const state of ACCEPTANCE_LINK_STATES) expect(messages.externalAcceptance.links.states[state]).toBeTruthy();
      for (const code of ["ACCEPTANCE_LINK_UNAVAILABLE", "ACCEPTANCE_CONSENT_REQUIRED", "ACCEPTANCE_CONFLICT"] as const) {
        expect(messages.externalAcceptance.errors[code]).toBeTruthy();
      }
    }
  });
});

describe("PublicAcceptancePage", () => {
  // jsdom has no canvas; the signature pad only needs a context object that does nothing.
  beforeAll(() => {
    HTMLCanvasElement.prototype.getContext = vi.fn(() => null) as unknown as HTMLCanvasElement["getContext"];
  });
  beforeEach(() => vi.clearAllMocks());

  it("shows one neutral message for any unusable link", () => {
    renderPage({ isError: true });
    expect(screen.getByText(p.unavailableTitle)).toBeDefined();
    expect(screen.getByText(p.unavailableDetail)).toBeDefined();
  });

  it("requires a name and the consent tick before calling the API", async () => {
    renderPage({ data: view() });
    fireEvent.click(screen.getByRole("button", { name: p.accept }));
    expect(await screen.findByText(p.nameRequired)).toBeDefined();
    fireEvent.change(screen.getByLabelText(new RegExp(p.signerName)), { target: { value: "คุณสมชาย ใจดี" } });
    fireEvent.click(screen.getByRole("button", { name: p.accept }));
    expect(await screen.findByText(p.consentRequired)).toBeDefined();
    expect(mutate).not.toHaveBeenCalled();
  });

  it("submits the signer, consent version and no signature when none is drawn", async () => {
    mutate.mockResolvedValue({});
    renderPage({ data: view() });
    fireEvent.change(screen.getByLabelText(new RegExp(p.signerName)), { target: { value: "คุณสมชาย ใจดี" } });
    fireEvent.change(screen.getByLabelText(new RegExp(p.signerRole)), { target: { value: "กรรมการ" } });
    fireEvent.click(screen.getByRole("checkbox"));
    fireEvent.click(screen.getByRole("button", { name: p.accept }));
    await waitFor(() => expect(mutate).toHaveBeenCalledWith({ signerName: "คุณสมชาย ใจดี", signerRole: "กรรมการ", consentAccepted: true, consentVersion: "2026-10-v1", signatureImage: null }));
  });

  it("shows the translated error instead of the raw message", async () => {
    mutate.mockRejectedValue(new ApiError({ status: 409, code: "ACCEPTANCE_CONFLICT", message: "raw" }));
    renderPage({ data: view() });
    fireEvent.change(screen.getByLabelText(new RegExp(p.signerName)), { target: { value: "คุณสมชาย" } });
    fireEvent.click(screen.getByRole("checkbox"));
    fireEvent.click(screen.getByRole("button", { name: p.accept }));
    expect(await screen.findByText(thMessages.externalAcceptance.errors.ACCEPTANCE_CONFLICT)).toBeDefined();
  });

  it("shows the accepted confirmation instead of the form once accepted", () => {
    renderPage({ data: view({ status: "accepted", acceptedAtUtc: "2026-10-05T00:00:00Z" }) });
    expect(screen.getByText(p.acceptedTitle)).toBeDefined();
    expect(screen.queryByRole("button", { name: p.accept })).toBeNull();
  });
});
