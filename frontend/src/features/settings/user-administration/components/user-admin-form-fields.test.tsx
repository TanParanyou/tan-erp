import React from "react";
import { describe, expect, it, vi } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { useForm, FormProvider } from "react-hook-form";
import { UserAdminFormFields, type UserAdminFormData } from "./user-admin-form-fields";

const mocks = vi.hoisted(() => ({
  createDepartment: vi.fn(),
  createJobTitle: vi.fn(),
}));

vi.mock("../api/user-admin-queries", () => ({
  useDepartments: () => ({
    data: [
      { id: "dept-1", code: "ENG", name: "Engineering", isActive: true },
    ],
    isLoading: false,
  }),
  useJobTitles: () => ({
    data: [
      { id: "job-1", code: "DEV", name: "Developer", isActive: true },
    ],
    isLoading: false,
  }),
  useCreateDepartment: () => ({
    mutateAsync: mocks.createDepartment,
    isPending: false,
  }),
  useCreateJobTitle: () => ({
    mutateAsync: mocks.createJobTitle,
    isPending: false,
  }),
}));

function TestWrapper({ children }: { children: React.ReactNode }) {
  const methods = useForm<UserAdminFormData>({
    defaultValues: {
      displayName: "",
      email: "",
      phoneNumber: "",
      employeeCode: "",
      departmentId: "",
      jobTitleId: "",
      notes: "",
      startsAtUtc: "",
      expiresAtUtc: "",
      branchId: "",
      roleIds: [],
    },
  });

  return <FormProvider {...methods}>{children}</FormProvider>;
}

describe("UserAdminFormFields", () => {
  it("renders all extended user administration fields", () => {
    render(
      <TestWrapper>
        <UserAdminFormFields />
      </TestWrapper>
    );

    expect(screen.getByLabelText(/รหัสพนักงาน/)).toBeInTheDocument();
    expect(screen.getByLabelText(/เบอร์โทรศัพท์/)).toBeInTheDocument();
    expect(screen.getByLabelText(/แผนก/)).toBeInTheDocument();
    expect(screen.getByLabelText(/ตำแหน่งงาน/)).toBeInTheDocument();
    expect(screen.getByLabelText(/วันที่เริ่มงาน/)).toBeInTheDocument();
    expect(screen.getByLabelText(/วันที่สิ้นสุดสัญญา/)).toBeInTheDocument();
    expect(screen.getByLabelText(/หมายเหตุ/)).toBeInTheDocument();
  });

  it("allows creating a new department inline via quick-add modal", async () => {
    mocks.createDepartment.mockResolvedValueOnce({
      id: "dept-2",
      code: "ACC",
      name: "Accounting",
      isActive: true,
    });

    render(
      <TestWrapper>
        <UserAdminFormFields />
      </TestWrapper>
    );

    const addDeptButton = screen.getByRole("button", { name: "เพิ่มแผนก" });
    fireEvent.click(addDeptButton);

    expect(screen.getByText("เพิ่มแผนกใหม่")).toBeInTheDocument();

    fireEvent.change(screen.getByLabelText("รหัสแผนก *"), { target: { value: "ACC" } });
    fireEvent.change(screen.getByLabelText("ชื่อแผนก *"), { target: { value: "Accounting" } });

    const submitButton = screen.getByRole("button", { name: "บันทึกข้อมูล" });
    fireEvent.click(submitButton);

    await waitFor(() => {
      expect(mocks.createDepartment).toHaveBeenCalledWith({
        code: "ACC",
        name: "Accounting",
        description: null,
      });
    });
  });
});
