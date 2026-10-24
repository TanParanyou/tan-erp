import React, { useState } from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { useForm } from "react-hook-form";
import { UserAdminFormFields, type UserAdminFormValues } from "./user-admin-form-fields";

const mocks = vi.hoisted(() => ({
  createDepartment: vi.fn(),
  createJobTitle: vi.fn(),
}));

vi.mock("@/lib/membership/selected-membership-context", () => ({
  useSelectedMembership: () => ({
    selectedMembership: {
      id: "membership-1",
      permissions: [{ key: "users.manage", scope: "organization" }],
    },
  }),
}));

vi.mock("@/features/item-master/api/item-master-queries", () => ({
  useOrganizationBranches: () => ({ data: [{ id: "b1", code: "HQ", name: "สาขากรุงเทพ" }] }),
}));

vi.mock("../api/user-admin-queries", () => ({
  useAdminRoles: () => ({
    data: {
      items: [
        { id: "r1", name: "Role 1", assignable: true, requiresApproval: false, permissionKeys: [] },
      ],
    },
    isPending: false,
    isError: false,
  }),
  useDepartments: () => ({
    data: [
      { id: "dept-1", code: "ENG", name: "ฝ่ายวิศวกรรม", isActive: true },
    ],
    isLoading: false,
  }),
  useJobTitles: () => ({
    data: [
      { id: "job-1", code: "DEV", name: "นักพัฒนาซอฟต์แวร์", isActive: true },
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

function FormHarness() {
  const [avatarFile, setAvatarFile] = useState<File | null>(null);
  const {
    register,
    control,
    watch,
    formState: { errors },
  } = useForm<UserAdminFormValues>({
    defaultValues: {
      displayName: "",
      email: "",
      phoneNumber: "",
      employeeCode: "",
      departmentId: "",
      jobTitleId: "",
      notes: "",
      startsAt: "",
      expiresAt: "",
      branchId: "",
      roleIds: [],
    },
  });

  const selectedDepartmentId = watch("departmentId");

  return (
    <UserAdminFormFields
      register={register}
      control={control}
      errors={errors}
      mode="create"
      avatarFile={avatarFile}
      onAvatarFileChange={setAvatarFile}
      selectedDepartmentId={selectedDepartmentId}
    />
  );
}

describe("UserAdminFormFields", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("renders all extended user administration fields", () => {
    render(<FormHarness />);

    expect(screen.getByText("ข้อมูลผู้ใช้")).toBeInTheDocument();
    expect(screen.getByText("แผนกและตำแหน่งงาน")).toBeInTheDocument();
    expect(screen.getByLabelText(/รหัสพนักงาน/)).toBeInTheDocument();
    expect(screen.getByLabelText(/เบอร์โทรศัพท์/)).toBeInTheDocument();
    expect(screen.getByLabelText(/^แผนก$/)).toBeInTheDocument();
    expect(screen.getByLabelText(/^ตำแหน่งงาน$/)).toBeInTheDocument();
    expect(screen.getByLabelText(/เริ่มมีผล/)).toBeInTheDocument();
    expect(screen.getByLabelText(/หมดอายุ/)).toBeInTheDocument();
    expect(screen.getByLabelText(/หมายเหตุ/)).toBeInTheDocument();
  });

  it("allows creating a new department via quick-add drawer", async () => {
    mocks.createDepartment.mockResolvedValueOnce({
      id: "dept-2",
      code: "ACC",
      name: "Accounting",
      isActive: true,
    });

    render(<FormHarness />);

    const addDeptButton = screen.getByRole("button", { name: "เพิ่มแผนกใหม่" });
    fireEvent.click(addDeptButton);

    expect(screen.getByRole("heading", { name: "เพิ่มแผนกใหม่" })).toBeInTheDocument();

    fireEvent.change(screen.getByPlaceholderText("DEP-01"), { target: { value: "ACC" } });
    fireEvent.change(screen.getByPlaceholderText("ระบุชื่อ"), { target: { value: "Accounting" } });

    const saveButton = screen.getByRole("button", { name: "บันทึก" });
    fireEvent.click(saveButton);

    await waitFor(() => {
      expect(mocks.createDepartment).toHaveBeenCalledWith({
        code: "ACC",
        name: "Accounting",
        nameEn: null,
      });
    });
  });
});
