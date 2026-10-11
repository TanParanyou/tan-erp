import React from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { ApiError } from "@/lib/api/api-error";
import { OrganizationProfileForm } from "./organization-profile-form";

const mocks = vi.hoisted(() => ({
  permissions: [] as string[],
  update: vi.fn(),
  success: vi.fn(),
  isPending: false,
}));

vi.mock("next/navigation", () => ({ useRouter: () => ({ push: vi.fn() }) }));
vi.mock("@/hooks/useToast", () => ({ useToast: () => ({ toast: { success: mocks.success, error: vi.fn() } }) }));
vi.mock("@/lib/membership/selected-membership-context", () => ({
  useSelectedMembership: () => ({
    selectedMembership: { id: "m1", permissions: mocks.permissions.map((key) => ({ key, scope: "organization" })) },
  }),
}));
vi.mock("../api/organization-admin-queries", () => ({
  useOrganizationProfile: () => ({
    isPending: false,
    isError: false,
    data: {
      id: "o1", name: "บริษัท เดิม", nameEn: null, taxIdentifier: "0105536000003",
      addressTh: "1 ถนนทดสอบ", addressEn: null, phone: null, rowVersion: "v1",
    },
  }),
  useUpdateOrganizationProfile: () => ({ mutateAsync: mocks.update, isPending: mocks.isPending }),
}));

describe("OrganizationProfileForm", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mocks.isPending = false;
    mocks.permissions = ["organizations.read", "organizations.manage"];
    mocks.update.mockResolvedValue({});
  });

  it("shows the loaded values and saves with the row version, sending blank optional fields as null", async () => {
    render(<OrganizationProfileForm />);
    expect(screen.getByLabelText(/ชื่อองค์กร \(ไทย\)/)).toHaveValue("บริษัท เดิม");

    fireEvent.change(screen.getByLabelText(/ชื่อองค์กร \(ไทย\)/), { target: { value: "  บริษัท ใหม่  " } });
    fireEvent.click(screen.getByRole("button", { name: "บันทึกข้อมูลองค์กร" }));

    await waitFor(() => expect(mocks.update).toHaveBeenCalledTimes(1));
    expect(mocks.update).toHaveBeenCalledWith({
      ifMatch: "v1",
      payload: {
        name: "บริษัท ใหม่", nameEn: null, taxIdentifier: "0105536000003",
        addressTh: "1 ถนนทดสอบ", addressEn: null, phone: null,
      },
    });
    await waitFor(() => expect(mocks.success).toHaveBeenCalledWith("บันทึกข้อมูลองค์กรแล้ว"));
  });

  it("blocks a malformed tax identifier before calling the API", async () => {
    render(<OrganizationProfileForm />);

    fireEvent.change(screen.getByLabelText(/เลขประจำตัวผู้เสียภาษี/), { target: { value: "123" } });
    fireEvent.click(screen.getByRole("button", { name: "บันทึกข้อมูลองค์กร" }));

    expect(await screen.findByText("เลขประจำตัวผู้เสียภาษีต้องเป็นตัวเลข 13 หลัก")).toBeInTheDocument();
    expect(mocks.update).not.toHaveBeenCalled();
  });

  it("shows the dedicated message for a server-side tax identifier error", async () => {
    mocks.update.mockRejectedValueOnce(new ApiError({ status: 422, code: "ORGANIZATION_TAX_ID_INVALID", message: "x" }));
    render(<OrganizationProfileForm />);

    fireEvent.click(screen.getByRole("button", { name: "บันทึกข้อมูลองค์กร" }));

    expect(await screen.findByText("เลขประจำตัวผู้เสียภาษีไม่ถูกต้อง")).toBeInTheDocument();
  });

  it("disables the save button while the request is in flight", () => {
    mocks.isPending = true;
    render(<OrganizationProfileForm />);

    expect(screen.getByRole("button", { name: /บันทึกข้อมูลองค์กร/ })).toBeDisabled();
  });

  it("is read-only without organizations.manage", () => {
    mocks.permissions = ["organizations.read"];
    render(<OrganizationProfileForm />);

    expect(screen.getByLabelText(/ชื่อองค์กร \(ไทย\)/)).toBeDisabled();
    expect(screen.queryByRole("button", { name: "บันทึกข้อมูลองค์กร" })).not.toBeInTheDocument();
    expect(screen.getByText("คุณดูข้อมูลได้เท่านั้น ไม่มีสิทธิ์แก้ไข")).toBeInTheDocument();
  });
});
