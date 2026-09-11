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
      searchAddresses: vi.fn(),
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

async function fillValidSiteForm(): Promise<void> {
  fireEvent.change(screen.getByLabelText(/^ชื่อเรียกสถานที่ตั้ง/), {
    target: { value: "สำนักงานใหญ่" },
  });
  fireEvent.change(screen.getByLabelText(/^ที่อยู่บรรทัดที่ 1/), {
    target: { value: "123 ถ.สุขุมวิท" },
  });

  const mockedSearchAddresses = vi.mocked(apiClient.searchAddresses);
  mockedSearchAddresses.mockResolvedValueOnce({
    items: [
      {
        subdistrictCode: "100101",
        subdistrict: "คลองเตย",
        district: "คลองเตย",
        province: "กรุงเทพมหานคร",
        postalCode: "10110",
        countryCode: "TH",
        latitude: 13.72,
        longitude: 100.58,
        displayText: "คลองเตย » คลองเตย » กรุงเทพมหานคร 10110",
      },
    ],
  });

  const smartSearchInput = screen.getByRole("combobox");
  fireEvent.change(smartSearchInput, { target: { value: "คลองเตย" } });

  const option = await screen.findByRole("option", { name: /คลองเตย/ });
  fireEvent.click(option);
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
      latitude: 13.72,
      longitude: 100.58,
      accessNote: null,
      status: "active",
      createdAtUtc: "2026-09-09T10:00:00Z",
    });

    renderEditor(client, "customer-1");
    await fillValidSiteForm();

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
    await fillValidSiteForm();

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
    await fillValidSiteForm();

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

  it("auto-fills address and coordinates fields when an address is selected from smart search and allows clearing", async () => {
    const mockedSearchAddresses = vi.mocked(apiClient.searchAddresses);
    mockedSearchAddresses.mockResolvedValueOnce({
      items: [
        {
          subdistrictCode: "100101",
          subdistrict: "พระบรมมหาราชวัง",
          district: "พระนคร",
          province: "กรุงเทพมหานคร",
          postalCode: "10200",
          countryCode: "TH",
          latitude: 13.75,
          longitude: 100.49,
          displayText: "พระบรมมหาราชวัง » พระนคร » กรุงเทพมหานคร 10200",
        },
      ],
    });

    renderEditor(client, "customer-1");

    const smartSearchInput = screen.getByRole("combobox");
    fireEvent.change(smartSearchInput, { target: { value: "พระนคร" } });

    await waitFor(() => {
      expect(mockedSearchAddresses).toHaveBeenCalledWith("พระนคร", expect.anything(), 20);
    });

    const option = await screen.findByRole("option", { name: /พระบรมมหาราชวัง/ });
    fireEvent.click(option);

    // After selection, the summary card is rendered
    expect(screen.getByText("พระบรมมหาราชวัง » พระนคร » กรุงเทพมหานคร")).toBeInTheDocument();
    expect(screen.getByText(/10200 • TH/)).toBeInTheDocument();
    expect((screen.getByLabelText(/^ละติจูด/) as HTMLInputElement).value).toBe("13.75");
    expect((screen.getByLabelText(/^ลองจิจูด/) as HTMLInputElement).value).toBe("100.49");

    // Clicking "เปลี่ยนที่อยู่" clears the selection and re-renders the smart search
    const changeBtn = screen.getByRole("button", { name: "เปลี่ยนที่อยู่" });
    fireEvent.click(changeBtn);

    expect(screen.queryByText("พระบรมมหาราชวัง » พระนคร » กรุงเทพมหานคร")).not.toBeInTheDocument();
    expect(screen.getByRole("combobox")).toBeInTheDocument();
  });

  it("appends access note when clicking quick template chips", async () => {
    renderEditor(client, "customer-1");

    const textarea = screen.getByLabelText(/^หมายเหตุการเข้าพื้นที่/) as HTMLTextAreaElement;
    expect(textarea.value).toBe("");

    const chip1 = screen.getByRole("button", { name: /ต้องแลกบัตรก่อนเข้า/ });
    fireEvent.click(chip1);

    expect(textarea.value).toBe("ต้องแลกบัตรก่อนเข้า");

    const chip2 = screen.getByRole("button", { name: /ประตูปิดหลัง 18:00 น./ });
    fireEvent.click(chip2);

    expect(textarea.value).toBe("ต้องแลกบัตรก่อนเข้า, ประตูปิดหลัง 18:00 น.");
  });

  it("populates latitude and longitude on get current location click", async () => {
    const mockGetCurrentPosition = vi.fn((success) => {
      success({
        coords: {
          latitude: 13.7563,
          longitude: 100.5018,
        },
      });
    });

    const originalGeo = navigator.geolocation;
    Object.defineProperty(navigator, "geolocation", {
      value: { getCurrentPosition: mockGetCurrentPosition },
      configurable: true,
      writable: true,
    });

    renderEditor(client, "customer-1");

    const gpsBtn = screen.getByRole("button", { name: /ดึงพิกัดปัจจุบัน \(GPS\)/ });
    fireEvent.click(gpsBtn);

    await waitFor(() => {
      expect((screen.getByLabelText(/^ละติจูด/) as HTMLInputElement).value).toBe("13.7563");
      expect((screen.getByLabelText(/^ลองจิจูด/) as HTMLInputElement).value).toBe("100.5018");
    });

    // Map link should appear
    const mapLink = screen.getByRole("link", { name: /เปิดดูบนแผนที่/ });
    expect(mapLink).toHaveAttribute("href", expect.stringContaining("13.7563,100.5018"));

    Object.defineProperty(navigator, "geolocation", {
      value: originalGeo,
      configurable: true,
      writable: true,
    });
  });

  it("clears pair validation error dynamically when second coordinate is entered", async () => {
    renderEditor(client, "customer-1");

    const latInput = screen.getByLabelText(/^ละติจูด/) as HTMLInputElement;
    const lngInput = screen.getByLabelText(/^ลองจิจูด/) as HTMLInputElement;

    // Fill only latitude -> error should show up
    fireEvent.change(latInput, { target: { value: "13.8057436" } });
    await waitFor(() => {
      expect(
        screen.getByText("หากระบุพิกัด ต้องระบุทั้งละติจูดและลองจิจูดคู่กัน")
      ).toBeInTheDocument();
    });

    // Fill longitude -> error should automatically clear
    fireEvent.change(lngInput, { target: { value: "100.8213577" } });
    await waitFor(() => {
      expect(
        screen.queryByText("หากระบุพิกัด ต้องระบุทั้งละติจูดและลองจิจูดคู่กัน")
      ).not.toBeInTheDocument();
    });
  });
});
