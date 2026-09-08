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
    selectedMembership: { id: "membership-a" },
  }),
}));

vi.mock("@/lib/api/api-client", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@/lib/api/api-client")>();
  return {
    ...actual,
    apiClient: {
      ...actual.apiClient,
      createCustomer: vi.fn(),
    },
  };
});

const mockedCreate = vi.mocked(apiClient.createCustomer);

function renderEditor(client: QueryClient): void {
  render(
    <QueryClientProvider client={client}>
      <NextIntlClientProvider locale="th" messages={thMessages}>
        <CustomerEditor />
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
});
