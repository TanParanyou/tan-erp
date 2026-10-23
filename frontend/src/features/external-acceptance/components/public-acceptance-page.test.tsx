import React from "react";
import { describe, expect, it, vi, beforeEach, beforeAll } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import thMessages from "@/messages/th.json";
import enMessages from "@/messages/en.json";
import { PublicAcceptancePage } from "./public-acceptance-page";
import * as publicQueries from "../api/public-acceptance-queries";
import { ApiError } from "@/lib/api/api-error";
import type { PublicAcceptanceViewResponse } from "@/lib/api/api-client";
import { ACCEPTANCE_LINK_STATES, acceptanceLinkErrorCode, acceptanceLinkStateLabel } from "@/features/estimates/quotation-lifecycle-status";

const p = thMessages.externalAcceptance.public;
const mutate = vi.fn();
const requestOtpMutate = vi.fn();
const verifyOtpMutate = vi.fn();

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

function renderPage(
  state: { data?: PublicAcceptanceViewResponse; isPending?: boolean; isError?: boolean },
  mutation = { mutateAsync: mutate, isPending: false, isSuccess: false },
  otpOverrides?: {
    requestOtp?: { mutateAsync: ReturnType<typeof vi.fn>; isPending: boolean };
    verifyOtp?: { mutateAsync: ReturnType<typeof vi.fn>; isPending: boolean };
  }
) {
  const queryClient = new QueryClient({
    defaultOptions: {
      queries: { retry: false },
      mutations: { retry: false },
    },
  });

  vi.spyOn(publicQueries, "usePublicAcceptance").mockReturnValue({ isPending: false, isError: false, ...state } as unknown as ReturnType<typeof publicQueries.usePublicAcceptance>);
  vi.spyOn(publicQueries, "usePublicAcceptMutation").mockReturnValue(mutation as unknown as ReturnType<typeof publicQueries.usePublicAcceptMutation>);
  vi.spyOn(publicQueries, "usePublicPdfDownload").mockReturnValue({ mutateAsync: vi.fn(), isPending: false } as unknown as ReturnType<typeof publicQueries.usePublicPdfDownload>);
  vi.spyOn(publicQueries, "useRequestOtpMutation").mockReturnValue((otpOverrides?.requestOtp ?? { mutateAsync: requestOtpMutate, isPending: false }) as unknown as ReturnType<typeof publicQueries.useRequestOtpMutation>);
  vi.spyOn(publicQueries, "useVerifyOtpMutation").mockReturnValue((otpOverrides?.verifyOtp ?? { mutateAsync: verifyOtpMutate, isPending: false }) as unknown as ReturnType<typeof publicQueries.useVerifyOtpMutation>);

  return render(
    <QueryClientProvider client={queryClient}>
      <NextIntlClientProvider locale="th" messages={thMessages}>
        <PublicAcceptancePage token={"t".repeat(43)} uiLocale="th" />
      </NextIntlClientProvider>
    </QueryClientProvider>
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
    await waitFor(() =>
      expect(mutate).toHaveBeenCalledWith({
        signerName: "คุณสมชาย ใจดี",
        signerRole: "กรรมการ",
        consentAccepted: true,
        consentVersion: "2026-10-v1",
        signatureImage: null,
        documentSha256: null,
        otpVerificationToken: null,
      })
    );
  });

  it("sends the hash of the PDF it showed, and shows that PDF with its hash", async () => {
    mutate.mockResolvedValue({});
    const hash = "c".repeat(64);
    renderPage({ data: view({ pdf: { sha256: hash, byteCount: 1234 } }) });

    expect(screen.getByText(p.pdfHash.replace("{hash}", hash))).toBeDefined();
    fireEvent.change(screen.getByLabelText(new RegExp(p.signerName)), { target: { value: "คุณสมชาย ใจดี" } });
    fireEvent.click(screen.getByRole("checkbox"));
    fireEvent.click(screen.getByRole("button", { name: p.accept }));
    await waitFor(() => expect(mutate).toHaveBeenCalledWith(expect.objectContaining({ documentSha256: hash })));
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

  describe("G-05 OTP Authentication Flow", () => {
    it("handles single-channel OTP request, code verification, and passing token to accept mutation", async () => {
      requestOtpMutate.mockResolvedValue({});
      verifyOtpMutate.mockResolvedValue({ verificationToken: "verified-tok-999" });
      mutate.mockResolvedValue({});

      renderPage({
        data: view({
          otpRequired: true,
          otpChannels: ["sms"],
        }),
      });

      // Initially OTP section is rendered, but not the acceptance inputs
      expect(screen.getByText(p.otpTitle)).toBeDefined();
      expect(screen.queryByLabelText(new RegExp(p.signerName))).toBeNull();

      // Click send OTP code
      const sendButton = screen.getByRole("button", { name: p.otpSend });
      fireEvent.click(sendButton);

      await waitFor(() => {
        expect(requestOtpMutate).toHaveBeenCalledWith("sms");
      });

      // OTP sent message appears along with code input and resend button
      expect(await screen.findByText(p.otpSent)).toBeDefined();
      expect(screen.getByRole("button", { name: p.otpResend })).toBeDefined();

      // Enter 6-digit OTP code and verify
      const codeInput = screen.getByLabelText(p.otpCode);
      fireEvent.change(codeInput, { target: { value: "123456" } });
      const verifyButton = screen.getByRole("button", { name: p.otpVerify });
      fireEvent.click(verifyButton);

      await waitFor(() => {
        expect(verifyOtpMutate).toHaveBeenCalledWith("123456");
      });

      // After verification succeeds, signer form is rendered with verified banner
      expect(await screen.findByText(p.otpVerified)).toBeDefined();
      expect(screen.queryByText(p.otpTitle)).toBeNull();

      // Fill out acceptance form
      fireEvent.change(screen.getByLabelText(new RegExp(p.signerName)), { target: { value: "คุณสมชาย ใจดี" } });
      fireEvent.click(screen.getByRole("checkbox"));
      fireEvent.click(screen.getByRole("button", { name: p.accept }));

      await waitFor(() => {
        expect(mutate).toHaveBeenCalledWith(
          expect.objectContaining({
            signerName: "คุณสมชาย ใจดี",
            otpVerificationToken: "verified-tok-999",
          })
        );
      });
    });

    it("requires selecting a channel when multiple OTP channels are offered", async () => {
      requestOtpMutate.mockResolvedValue({});

      renderPage({
        data: view({
          otpRequired: true,
          otpChannels: ["sms", "email"],
        }),
      });

      // Send button is initially disabled because no channel is selected
      const sendButton = screen.getByRole("button", { name: p.otpSend });
      expect(sendButton).toHaveProperty("disabled", true);

      // Select email channel
      const channelSelect = screen.getByLabelText(p.otpChannel);
      fireEvent.change(channelSelect, { target: { value: "email" } });

      // Button is now enabled
      expect(sendButton).toHaveProperty("disabled", false);
      fireEvent.click(sendButton);

      await waitFor(() => {
        expect(requestOtpMutate).toHaveBeenCalledWith("email");
      });
    });

    it("displays error message when OTP request or verification fails", async () => {
      requestOtpMutate.mockRejectedValue(
        new ApiError({ status: 400, code: "ACCEPTANCE_OTP_UNAVAILABLE", message: "Channel unavailable" })
      );

      renderPage({
        data: view({
          otpRequired: true,
          otpChannels: ["sms"],
        }),
      });

      fireEvent.click(screen.getByRole("button", { name: p.otpSend }));
      expect(await screen.findByText(thMessages.externalAcceptance.errors.ACCEPTANCE_OTP_UNAVAILABLE)).toBeDefined();
    });

    it("clears verification token if submission fails with ACCEPTANCE_OTP_INVALID", async () => {
      requestOtpMutate.mockResolvedValue({});
      verifyOtpMutate.mockResolvedValue({ verificationToken: "stale-tok" });
      mutate.mockRejectedValue(
        new ApiError({ status: 400, code: "ACCEPTANCE_OTP_INVALID", message: "OTP expired" })
      );

      renderPage({
        data: view({
          otpRequired: true,
          otpChannels: ["sms"],
        }),
      });

      fireEvent.click(screen.getByRole("button", { name: p.otpSend }));
      await screen.findByText(p.otpSent);

      fireEvent.change(screen.getByLabelText(p.otpCode), { target: { value: "123456" } });
      fireEvent.click(screen.getByRole("button", { name: p.otpVerify }));

      await screen.findByText(p.otpVerified);

      // Submit form
      fireEvent.change(screen.getByLabelText(new RegExp(p.signerName)), { target: { value: "คุณสมชาย" } });
      fireEvent.click(screen.getByRole("checkbox"));
      fireEvent.click(screen.getByRole("button", { name: p.accept }));

      // Error message is displayed and OTP section re-appears because otpToken was reset to null
      expect(await screen.findByText(thMessages.externalAcceptance.errors.ACCEPTANCE_OTP_INVALID)).toBeDefined();
      expect(await screen.findByText(p.otpTitle)).toBeDefined();
    });
  });
});
