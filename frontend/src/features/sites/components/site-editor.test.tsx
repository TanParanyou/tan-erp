import React from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import { SiteEditor } from "./site-editor";
import { apiClient } from "@/lib/api/api-client";
import { ToastProvider } from "@/hooks/useToast";
import { ToastContainer } from "@/components/ui/Toast";
import type { SiteResponse } from "@/lib/api/api-client";

const mockPush = vi.fn();

vi.mock("next/navigation", () => ({
  useRouter: () => ({ push: mockPush }),
}));

vi.mock("@/lib/auth/auth-session", () => ({
  getAuthToken: vi.fn(async () => "test-token"),
}));

vi.mock("@/lib/membership/selected-membership-context", () => ({
  useSelectedMembership: () => ({
    selectedMembership: { id: "membership-a" },
  }),
}));

vi.mock("@/lib/api/api-client", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@/lib/api/api-client")>();
  return {
    ...actual,
    apiClient: {
      ...actual.apiClient,
      createSite: vi.fn(),
    },
  };
});

const mockedCreateSite = vi.mocked(apiClient.createSite);

function renderEditor(client: QueryClient, customerId = "customer-1"): void {
  render(
    <QueryClientProvider client={client}>
      <NextIntlClientProvider locale="th" messages={thMessages}>
        <ToastProvider>
          <SiteEditor customerId={customerId} />
          <ToastContainer />
        </ToastProvider>
      </NextIntlClientProvider>
    </QueryClientProvider>
  );
}

function fillValidSiteForm(): void {
  fireEvent.change(screen.getByLabelText(/ชื่อเรียกสถานที่ตั้ง/), {
    target: { value: "สำนักงานใหญ่" },
  });
  fireEvent.change(screen.getByLabelText(/ที่อยู่บรรทัดที่ 1/), {
    target: { value: "123 ถ.สุขุมวิท" },
  });
  fireEvent.change(screen.getByLabelText(/ตำบล \/ แขวง/), {
    target: { value: "คลองเตย" },
  });
  fireEvent.change(screen.getByLabelText(/อำเภอ \/ เขต/), {
    target: { value: "คลองเตย" },
  });
  fireEvent.change(screen.getByLabelText(/จังหวัด/), {
    target: { value: "กรุงเทพมหานคร" },
  });
  fireEvent.change(screen.getByLabelText(/รหัสไปรษณีย์/), {
    target: { value: "10110" },
  });
}

describe("SiteEditor", () => {
  let client: QueryClient;

  beforeEach(() => {
    vi.clearAllMocks();
    client = new QueryClient({ defaultOptions: { queries: { retry: false } } });
    let count = 0;
    vi.spyOn(crypto, "randomUUID").mockImplementation(() => {
      count += 1;
      return `site-key-${count}` as `${string}-${string}-${string}-${string}-${string}`;
    });
  });

  it("submits valid site and navigates back to customer detail on success", async () => {
    mockedCreateSite.mockResolvedValueOnce({
      id: "site-123",
      customerId: "customer-1",
      label: "สำนักงานใหญ่",
      addressLine1: "123 ถ.สุขุมวิท",
      subdistrict: "คลองเตย",
      district: "คลองเตย",
      province: "กรุงเทพมหานคร",
      postalCode: "10110",
      countryCode: "TH",
      latitude: null,
      longitude: null,
      accessNote: null,
      status: "active",
      createdAtUtc: "2026-09-09T10:00:00Z",
    });

    renderEditor(client, "customer-1");
    fillValidSiteForm();

    const submitBtn = screen.getByRole("button", { name: "บันทึกสถานที่ตั้ง" });
    fireEvent.click(submitBtn);

    await waitFor(() => {
      expect(mockedCreateSite).toHaveBeenCalledTimes(1);
    });

    expect(mockedCreateSite).toHaveBeenCalledWith(
      "customer-1",
      expect.objectContaining({
        label: "สำนักงานใหญ่",
        addressLine1: "123 ถ.สุขุมวิท",
        subdistrict: "คลองเตย",
        district: "คลองเตย",
        province: "กรุงเทพมหานคร",
        postalCode: "10110",
        countryCode: "TH",
      }),
      expect.objectContaining({
        token: "test-token",
        membershipId: "membership-a",
        idempotencyKey: "site-key-1",
      })
    );

    await waitFor(() => {
      expect(mockPush).toHaveBeenCalledWith(expect.stringContaining("/customers/customer-1"));
    });
  });

  it("locks submit button while request is in progress to prevent double submit", async () => {
    let resolvePromise: ((value: SiteResponse) => void) | undefined;
    mockedCreateSite.mockImplementation(
      () =>
        new Promise<SiteResponse>((resolve) => {
          resolvePromise = resolve;
        })
    );

    renderEditor(client, "customer-1");
    fillValidSiteForm();

    const submitBtn = screen.getByRole("button", { name: "บันทึกสถานที่ตั้ง" });
    fireEvent.click(submitBtn);

    // Should be locked / disabled during submit
    await waitFor(() => {
      expect(submitBtn).toBeDisabled();
    });

    // Second click should not trigger another call
    fireEvent.click(submitBtn);
    expect(mockedCreateSite).toHaveBeenCalledTimes(1);

    // Resolve request
    resolvePromise!({
      id: "site-123",
      label: "สำนักงานใหญ่",
    });

    await waitFor(() => {
      expect(mockPush).toHaveBeenCalled();
    });
  });

  it("reuses idempotency key on submission retry, rotates key if form changes after failure", async () => {
    mockedCreateSite.mockRejectedValueOnce(new Error("Network glitch 1"));
    mockedCreateSite.mockRejectedValueOnce(new Error("Network glitch 2"));

    renderEditor(client, "customer-1");
    fillValidSiteForm();

    const submitBtn = screen.getByRole("button", { name: "บันทึกสถานที่ตั้ง" });
    fireEvent.click(submitBtn);

    await waitFor(() => {
      expect(mockedCreateSite).toHaveBeenCalledTimes(1);
    });
    expect(mockedCreateSite.mock.calls[0][2]?.idempotencyKey).toBe("site-key-1");

    // Retry without changing any field -> should reuse same idempotency key
    fireEvent.click(submitBtn);
    await waitFor(() => {
      expect(mockedCreateSite).toHaveBeenCalledTimes(2);
    });
    expect(mockedCreateSite.mock.calls[1][2]?.idempotencyKey).toBe("site-key-1");

    // Now change a field
    fireEvent.change(screen.getByLabelText(/ชื่อเรียกสถานที่ตั้ง/), {
      target: { value: "สำนักงานใหญ่ สาขา 2" },
    });

    // Mock success for next submit
    mockedCreateSite.mockResolvedValueOnce({
      id: "site-456",
      label: "สำนักงานใหญ่ สาขา 2",
    });

    fireEvent.click(submitBtn);
    await waitFor(() => {
      expect(mockedCreateSite).toHaveBeenCalledTimes(3);
    });
    // Idempotency key should now be rotated!
    expect(mockedCreateSite.mock.calls[2][2]?.idempotencyKey).toBe("site-key-2");
  });
});
