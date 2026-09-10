import React from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import { CustomerEditor } from "./customer-editor";
import { apiClient } from "@/lib/api/api-client";
import { ApiError } from "@/lib/api/api-error";

const mockPush = vi.fn();

vi.mock("next/navigation", () => ({
  useRouter: () => ({ push: mockPush }),
}));

vi.mock("@/lib/auth/auth-session", () => ({
  getAuthToken: vi.fn(async () => "test-token"),
}));

vi.mock("@/lib/membership/selected-membership-context", () => ({
  useSelectedMembership: () => ({
    selectedMembership: {
      id: "membership-a",
      permissions: [
        { key: "customers.read", scope: "organization" },
        { key: "customers.manage", scope: "organization" },
        { key: "sites.read", scope: "organization" },
        { key: "sites.manage", scope: "organization" },
      ],
    },
  }),
}));

vi.mock("@/lib/api/api-client", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@/lib/api/api-client")>();
  return {
    ...actual,
    apiClient: {
      ...actual.apiClient,
      createCustomer: vi.fn(),
      checkCustomerDuplicates: vi.fn(),
      getCustomer: vi.fn(),
      listCustomerSites: vi.fn(),
    },
  };
});

const mockedCreate = vi.mocked(apiClient.createCustomer);
const mockedCheckDuplicates = vi.mocked(apiClient.checkCustomerDuplicates);
const mockedGetCustomer = vi.mocked(apiClient.getCustomer);
const mockedListCustomerSites = vi.mocked(apiClient.listCustomerSites);

import { ToastProvider } from "@/hooks/useToast";

function renderEditor(client: QueryClient): void {
  render(
    <QueryClientProvider client={client}>
      <NextIntlClientProvider locale="th" messages={thMessages}>
        <ToastProvider>
          <CustomerEditor />
        </ToastProvider>
      </NextIntlClientProvider>
    </QueryClientProvider>,
  );
}

function fillValidForm(container: HTMLElement): void {
  const byId = (id: string): HTMLInputElement => {
    const el = container.querySelector(`#${id}`);
    if (!el) throw new Error(`Missing input #${id}`);
    return el as HTMLInputElement;
  };
  fireEvent.change(byId("displayNameTh"), { target: { value: "บริษัท ตัวอย่าง จำกัด" } });
  fireEvent.change(byId("primaryContactName"), { target: { value: "คุณตัวอย่าง" } });
  fireEvent.change(byId("primaryContactPhone"), { target: { value: "0812345678" } });
}

describe("CustomerEditor create intent", () => {
  let client: QueryClient;

  beforeEach(() => {
    vi.clearAllMocks();
    client = new QueryClient({ defaultOptions: { queries: { retry: false } } });
    let count = 0;
    vi.spyOn(crypto, "randomUUID").mockImplementation(() => {
      count += 1;
      return `key-${count}` as `${string}-${string}-${string}-${string}-${string}`;
    });
  });

  it("sends one POST on valid double click", async () => {
    mockedCreate.mockImplementation(
      () =>
        new Promise((resolve) =>
          setTimeout(
            () =>
              resolve({
                id: "customer-1",
                code: "CUS-0001",
                customerType: "organization",
                displayNameTh: "บริษัท ตัวอย่าง จำกัด",
                status: "draft",
                duplicateCandidates: null,
              }),
            50,
          ),
        ),
    );

    const { container } = render(<div />);
    renderEditor(client);
    const root = container.parentElement ?? document.body;
    void container;
    fillValidForm(document.body as unknown as HTMLElement);

    const saveButton = screen.getByRole("button", { name: "บันทึกข้อมูลลูกค้า" });
    fireEvent.click(saveButton);
    fireEvent.click(saveButton);

    await waitFor(() => expect(mockedCreate).toHaveBeenCalledTimes(1));
    await waitFor(() => expect(mockPush).toHaveBeenCalledTimes(1));
    void root;
  });

  it("reuses key on retry without field change and rotates key after field change", async () => {
    mockedCreate.mockRejectedValueOnce(
      new ApiError({ status: 500, code: "UNKNOWN_ERROR", message: "boom" }),
    );
    mockedCreate.mockRejectedValueOnce(
      new ApiError({ status: 500, code: "UNKNOWN_ERROR", message: "boom-again" }),
    );
    mockedCreate.mockResolvedValueOnce({
      id: "customer-3",
      code: "CUS-0003",
      customerType: "organization",
      displayNameTh: "บริษัท ตัวอย่าง จำกัด แก้ไข",
      status: "draft",
      duplicateCandidates: null,
    });

    renderEditor(client);
    fillValidForm(document.body as unknown as HTMLElement);

    fireEvent.click(screen.getByRole("button", { name: "บันทึกข้อมูลลูกค้า" }));
    await waitFor(() => expect(mockedCreate).toHaveBeenCalledTimes(1));
    expect(mockedCreate.mock.calls[0][1].idempotencyKey).toBe("key-1");
    await waitFor(() => expect(screen.getByText("boom")).toBeDefined());

    // Retry without changing fields reuses key-1
    fireEvent.click(screen.getByRole("button", { name: "บันทึกข้อมูลลูกค้า" }));
    await waitFor(() => expect(mockedCreate).toHaveBeenCalledTimes(2));
    expect(mockedCreate.mock.calls[1][1].idempotencyKey).toBe("key-1");
    await waitFor(() => expect(screen.getByText("boom-again")).toBeDefined());

    // A field change after a failed submission starts a new create intent.
    const nameInput = document.body.querySelector("#displayNameTh") as HTMLInputElement;
    fireEvent.change(nameInput, { target: { value: "บริษัท ตัวอย่าง จำกัด แก้ไข" } });

    fireEvent.click(screen.getByRole("button", { name: "บันทึกข้อมูลลูกค้า" }));
    await waitFor(() => expect(mockedCreate).toHaveBeenCalledTimes(3));
    expect(mockedCreate.mock.calls[2][1].idempotencyKey).toBe("key-2");
    await waitFor(() => expect(mockPush).toHaveBeenCalledTimes(1));
  });

  it("preserves masked duplicate candidate with link instead of navigating", async () => {
    mockedCreate.mockResolvedValue({
      id: "customer-created",
      code: "CUS-0100",
      customerType: "organization",
      displayNameTh: "บริษัท ตัวอย่าง จำกัด",
      status: "draft",
      duplicateCandidates: [
        {
          id: "customer-dup",
          code: "CUS-0009",
          displayNameTh: "บริษัท ตัวอย่าง จำกัด",
          maskedPhone: "081-***-5678",
          maskedEmail: null,
        },
      ],
    });

    renderEditor(client);
    fillValidForm(document.body as unknown as HTMLElement);
    mockPush.mockClear();

    fireEvent.click(screen.getByRole("button", { name: "บันทึกข้อมูลลูกค้า" }));

    await waitFor(() => expect(screen.getByText("081-***-5678")).toBeDefined());
    expect(mockPush).not.toHaveBeenCalled();
    expect(screen.getByRole("link", { name: /ดูข้อมูลลูกค้าที่สร้างแล้ว/ }).getAttribute("href")).toContain(
      "customer-created",
    );
    expect(screen.getByRole("button", { name: "บันทึกข้อมูลลูกค้า" })).toBeDisabled();
    expect(document.body.querySelector("#displayNameTh")).toBeDisabled();
  });

  it("navigates to detail when no duplicate candidate", async () => {
    mockedCreate.mockResolvedValue({
      id: "customer-clean",
      code: "CUS-0101",
      customerType: "organization",
      displayNameTh: "บริษัท ใหม่ จำกัด",
      status: "draft",
      duplicateCandidates: null,
    });

    renderEditor(client);
    fillValidForm(document.body as unknown as HTMLElement);

    fireEvent.click(screen.getByRole("button", { name: "บันทึกข้อมูลลูกค้า" }));

    await waitFor(() =>
      expect(mockPush).toHaveBeenCalledWith(expect.stringContaining("customer-clean")),
    );
  });

  it("submits leadSource and lineId when provided", async () => {
    mockedCreate.mockResolvedValue({
      id: "customer-quick-intake",
      code: "CUS-0102",
      customerType: "organization",
      displayNameTh: "บริษัท ตัวอย่าง จำกัด",
      status: "draft",
      leadSource: "referral",
      primaryContact: {
        name: "คุณตัวอย่าง",
        roleTitle: null,
        phone: "0812345678",
        email: null,
        lineId: "line_lead_99",
        preferredChannel: "phone",
        isMasked: false,
      },
      duplicateCandidates: null,
    });

    renderEditor(client);
    fillValidForm(document.body as unknown as HTMLElement);

    const leadSourceSelect = document.body.querySelector("#leadSource") as HTMLSelectElement;
    fireEvent.change(leadSourceSelect, { target: { value: "referral" } });

    const lineIdInput = document.body.querySelector("#primaryContactLineId") as HTMLInputElement;
    fireEvent.change(lineIdInput, { target: { value: "line_lead_99" } });

    fireEvent.click(screen.getByRole("button", { name: "บันทึกข้อมูลลูกค้า" }));

    await waitFor(() => expect(mockedCreate).toHaveBeenCalledTimes(1));
    const payload = mockedCreate.mock.calls[0][0];
    expect(payload.leadSource).toBe("referral");
    expect(payload.primaryContact?.lineId).toBe("line_lead_99");
  });

  it("renders translated placeholders on all inputs", () => {
    renderEditor(client);

    expect(screen.getByPlaceholderText(thMessages.customers.displayNameThPlaceholder)).toBeDefined();
    expect(screen.getByPlaceholderText(thMessages.customers.displayNameEnPlaceholder)).toBeDefined();
    expect(screen.getByPlaceholderText(thMessages.customers.contactNamePlaceholder)).toBeDefined();
    expect(screen.getByPlaceholderText(thMessages.customers.roleTitlePlaceholder)).toBeDefined();
    expect(screen.getByPlaceholderText(thMessages.customers.phonePlaceholder)).toBeDefined();
    expect(screen.getByPlaceholderText(thMessages.customers.emailPlaceholder)).toBeDefined();
    expect(screen.getByPlaceholderText(thMessages.customers.lineIdPlaceholder)).toBeDefined();
  });

  it("navigates immediately on cancel when form is pristine", () => {
    renderEditor(client);

    const cancelButton = screen.getByRole("button", { name: "ยกเลิก" });
    fireEvent.click(cancelButton);

    expect(mockPush).toHaveBeenCalledWith("/th/customers");
  });

  it("shows confirmation modal on cancel when form is dirty, then navigates on confirm", async () => {
    renderEditor(client);

    const nameInput = document.body.querySelector("#displayNameTh") as HTMLInputElement;
    fireEvent.change(nameInput, { target: { value: "ลูกค้าใหม่" } });

    const cancelButton = screen.getByRole("button", { name: "ยกเลิก" });
    fireEvent.click(cancelButton);

    // Modal should be visible
    expect(screen.getByText(thMessages.common.dialog.confirmCancelTitle)).toBeDefined();
    expect(mockPush).not.toHaveBeenCalled();

    // Confirm navigation
    const confirmButton = screen.getByRole("button", { name: thMessages.common.actions.confirm });
    fireEvent.click(confirmButton);

    expect(mockPush).toHaveBeenCalledWith("/th/customers");
  });

  it("displays live duplicate warning banner when duplicates are detected", async () => {
    mockedCheckDuplicates.mockResolvedValue([
      {
        id: "cust-dup-live",
        code: "CUS-0088",
        displayNameTh: "บริษัท ตัวอย่าง จำกัด",
        maskedPhone: "081-***-5678",
        maskedEmail: null,
      },
    ]);

    renderEditor(client);
    const nameInput = document.body.querySelector("#displayNameTh") as HTMLInputElement;
    fireEvent.change(nameInput, { target: { value: "บริษัท ตัวอย่าง จำกัด" } });

    await waitFor(() => {
      expect(screen.getByText(/ระบบตรวจพบรายชื่อลูกค้าที่อาจซ้ำซ้อนในระบบ/)).toBeDefined();
      expect(screen.getByText("CUS-0088 — บริษัท ตัวอย่าง จำกัด")).toBeDefined();
    });
  });

  it("intercepts submission with DuplicateConfirmationModal when duplicates exist, allows confirm create", async () => {
    mockedCheckDuplicates.mockResolvedValue([
      {
        id: "cust-dup-live",
        code: "CUS-0088",
        displayNameTh: "บริษัท ตัวอย่าง จำกัด",
        maskedPhone: "081-***-5678",
        maskedEmail: null,
      },
    ]);

    mockedCreate.mockResolvedValue({
      id: "cust-created-new",
      code: "CUS-0105",
      customerType: "organization",
      displayNameTh: "บริษัท ตัวอย่าง จำกัด",
      status: "draft",
      duplicateCandidates: null,
    });

    renderEditor(client);
    fillValidForm(document.body as unknown as HTMLElement);

    // Wait for live duplicates to resolve
    await waitFor(() => {
      expect(screen.getByText(/ระบบตรวจพบรายชื่อลูกค้าที่อาจซ้ำซ้อนในระบบ/)).toBeDefined();
    });

    // Click Save
    const saveButton = screen.getByRole("button", { name: "บันทึกข้อมูลลูกค้า" });
    fireEvent.click(saveButton);

    // Modal should appear without calling createCustomer yet
    await waitFor(() => {
      expect(screen.getByText(thMessages.customers.duplicateConfirmTitle)).toBeDefined();
    });
    expect(mockedCreate).not.toHaveBeenCalled();

    // In modal, click "ยืนยันสร้างลูกค้ารายใหม่"
    const confirmNewCustomerBtn = screen.getByRole("button", {
      name: thMessages.customers.confirmCreateNewCustomer,
    });
    fireEvent.click(confirmNewCustomerBtn);

    // Now createCustomer should be called and then navigate to new customer
    await waitFor(() => {
      expect(mockedCreate).toHaveBeenCalledTimes(1);
      expect(mockPush).toHaveBeenCalledWith(expect.stringContaining("cust-created-new"));
    });
  });

  it("opens CustomerQuickViewDrawer which calls getCustomer and listCustomerSites API and displays complete details", async () => {
    mockedCheckDuplicates.mockResolvedValue([
      {
        id: "cust-dup-live-1",
        code: "CUS-0088",
        displayNameTh: "บริษัท ตัวอย่าง จำกัด",
        maskedPhone: "081-***-5678",
        maskedEmail: null,
      },
    ]);

    mockedGetCustomer.mockResolvedValue({
      id: "cust-dup-live-1",
      code: "CUS-0088",
      customerType: "organization",
      displayNameTh: "บริษัท ตัวอย่าง จำกัด",
      displayNameEn: "Sample Company Ltd.",
      preferredLocale: "th",
      status: "active",
      leadSource: "referral",
      leadSourceNote: "จากงานสัมมนา",
      primaryContact: {
        name: "สมศรี มีสุข",
        roleTitle: "ผู้จัดการฝ่ายจัดซื้อ",
        phone: "081-234-5678",
        email: "somsri@example.com",
        lineId: "@somsri_line",
        preferredChannel: "phone",
        isMasked: false,
      },
      duplicateCandidates: null,
      rowVersion: "row-v-1",
      createdAtUtc: "2026-09-01T08:00:00Z",
    });

    mockedListCustomerSites.mockResolvedValue({
      items: [
        {
          id: "site-1",
          customerId: "cust-dup-live-1",
          code: "SITE-001",
          label: "สำนักงานใหญ่ อโศก",
          addressLine1: "123 ถนนสุขุมวิท 21",
          subdistrict: "คลองเตยเหนือ",
          district: "วัฒนา",
          province: "กรุงเทพมหานคร",
          postalCode: "10110",
          countryCode: "TH",
          status: "active",
        },
      ],
    });

    renderEditor(client);
    const nameInput = document.body.querySelector("#displayNameTh") as HTMLInputElement;
    fireEvent.change(nameInput, { target: { value: "บริษัท ตัวอย่าง จำกัด" } });

    // Wait for live duplicate alert
    await waitFor(() => {
      expect(screen.getByText(/ระบบตรวจพบรายชื่อลูกค้าที่อาจซ้ำซ้อนในระบบ/)).toBeDefined();
    });

    // Click "ดูข้อมูล (Drawer)"
    const viewDrawerBtn = screen.getByRole("button", { name: thMessages.customers.viewInDrawer });
    fireEvent.click(viewDrawerBtn);

    // Verify Drawer opens and calls getCustomer & listCustomerSites API
    await waitFor(() => {
      expect(mockedGetCustomer).toHaveBeenCalledWith("cust-dup-live-1", expect.anything());
      expect(mockedListCustomerSites).toHaveBeenCalledWith("cust-dup-live-1", expect.anything());
    });

    // Verify detailed content is displayed inside Drawer
    await waitFor(() => {
      expect(screen.getByText(thMessages.customers.quickViewTitle)).toBeDefined();
      expect(screen.getByText("Sample Company Ltd.")).toBeDefined();
      expect(screen.getByText("สมศรี มีสุข")).toBeDefined();
      expect(screen.getByText("ผู้จัดการฝ่ายจัดซื้อ")).toBeDefined();
      expect(screen.getByText("081-234-5678")).toBeDefined();
      expect(screen.getByText("somsri@example.com")).toBeDefined();
      expect(screen.getByText("@somsri_line")).toBeDefined();
      expect(screen.getByText("จากงานสัมมนา")).toBeDefined();
      expect(screen.getByText("สำนักงานใหญ่ อโศก")).toBeDefined();
      expect(screen.getByText("123 ถนนสุขุมวิท 21")).toBeDefined();
      expect(screen.getByText("กรุงเทพมหานคร")).toBeDefined();
      expect(screen.getByText("10110")).toBeDefined();
    });
  });
});

