import React from "react";
import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { PersonalProfileForm } from "./personal-profile-form";
import type { UserDto } from "@/lib/api/api-client";

const mockMutateAsync = vi.fn();
vi.mock("../api/profile-queries", () => ({
  useUpdateMyProfile: () => ({
    mutateAsync: mockMutateAsync,
    isPending: false,
  }),
}));

const mockToast = {
  success: vi.fn(),
  error: vi.fn(),
};

vi.mock("@/hooks/useToast", () => ({
  useToast: () => ({
    toast: mockToast,
  }),
}));

vi.mock("@/hooks/useDeferredFileUpload", () => ({
  useDeferredFileUpload: () => ({
    uploadSingleFile: vi.fn(),
    isUploading: false,
  }),
}));

vi.mock("@/lib/api/use-api-request-context", () => ({
  useApiRequestContext: () => ({
    membershipId: "019a3cf8-96f0-7c9f-b207-93aa818f4a11",
    locale: "th",
    buildOptions: vi.fn().mockResolvedValue({}),
  }),
}));

vi.mock("next/navigation", () => ({
  useRouter: () => ({
    push: vi.fn(),
    replace: vi.fn(),
  }),
}));

describe("PersonalProfileForm", () => {
  const initialUser: UserDto = {
    id: "019a3cf8-96f0-7c9f-b207-93aa818f4a10",
    displayName: "สมชาย ทดสอบ",
    email: "somchai@example.test",
    phoneNumber: "0812345678",
    preferredLocale: "th",
  };

  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("renders existing user data and disabled email field", () => {
    render(<PersonalProfileForm user={initialUser} />);

    const nameInput = screen.getByLabelText(/ชื่อ-นามสกุล/) as HTMLInputElement;
    expect(nameInput.value).toBe("สมชาย ทดสอบ");

    const emailInput = screen.getByLabelText(/อีเมล/) as HTMLInputElement;
    expect(emailInput.value).toBe("somchai@example.test");
    expect(emailInput.disabled).toBe(true);

    const phoneInput = screen.getByLabelText(/เบอร์โทรศัพท์/) as HTMLInputElement;
    expect(phoneInput.value).toBe("0812345678");

    const saveButton = screen.getByRole("button", { name: "บันทึกโปรไฟล์" }) as HTMLButtonElement;
    expect(saveButton.disabled).toBe(true); // not dirty yet
  });

  it("enables save button when modified and calls update mutation", async () => {
    mockMutateAsync.mockResolvedValueOnce({
      ...initialUser,
      displayName: "สมชาย รักสงบ",
    });

    render(<PersonalProfileForm user={initialUser} />);

    const nameInput = screen.getByLabelText(/ชื่อ-นามสกุล/);
    fireEvent.change(nameInput, { target: { value: "สมชาย รักสงบ" } });

    const saveButton = screen.getByRole("button", { name: "บันทึกโปรไฟล์" }) as HTMLButtonElement;
    expect(saveButton.disabled).toBe(false);

    fireEvent.click(saveButton);

    await waitFor(() => {
      expect(mockMutateAsync).toHaveBeenCalledWith({
        displayName: "สมชาย รักสงบ",
        phoneNumber: "0812345678",
        preferredLocale: "th",
        avatarFileId: null,
      });
      expect(mockToast.success).toHaveBeenCalledWith("บันทึกโปรไฟล์เรียบร้อยแล้ว");
    });
  });

  it("shows validation error when display name is cleared", async () => {
    render(<PersonalProfileForm user={initialUser} />);

    const nameInput = screen.getByLabelText(/ชื่อ-นามสกุล/);
    fireEvent.change(nameInput, { target: { value: "   " } });

    const saveButton = screen.getByRole("button", { name: "บันทึกโปรไฟล์" });
    fireEvent.click(saveButton);

    await waitFor(() => {
      expect(screen.getByText("กรุณากรอกชื่อ-นามสกุล")).toBeDefined();
    });

    expect(mockMutateAsync).not.toHaveBeenCalled();
  });
});
