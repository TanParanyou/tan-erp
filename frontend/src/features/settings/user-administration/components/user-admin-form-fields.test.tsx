import React from "react";
import { describe, expect, it, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { useForm } from "react-hook-form";
import { UserAdminFormFields, type UserAdminFormValues } from "./user-admin-form-fields";

const mocks = vi.hoisted(() => ({
  createDepartment: vi.fn(),
  createJobTitle: vi.fn(),
}));

vi.mock("next-intl", () => ({
  useTranslations: () => (key: string) => key,
  useLocale: () => "th",
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

function TestComponent() {
  const [avatarFile, setAvatarFile] = React.useState<File | null>(null);
  const {
    register,
    control,
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

  return (
    <UserAdminFormFields
      mode="create"
      register={register}
      control={control}
      errors={errors}
      avatarFile={avatarFile}
      onAvatarFileChange={setAvatarFile}
    />
  );
}

describe("UserAdminFormFields", () => {
  it("renders user administration fields", () => {
    render(<TestComponent />);
    expect(screen.getByText("sections.profile")).toBeInTheDocument();
  });
});
