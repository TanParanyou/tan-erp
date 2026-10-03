import React from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen, waitFor, within } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import messages from "@/messages/th.json";
import { useCostSources, useItemMasterMutations } from "@/features/item-master/api/item-master-queries";
import { CostSourceMaintenance } from "./cost-source-maintenance";

const navigation = vi.hoisted(() => ({ searchParams: new URLSearchParams("status=inactive"), canManage: false }));

vi.mock("next/navigation", () => ({
  usePathname: () => "/th/item-master/cost-sources",
  useRouter: () => ({ push: vi.fn(), replace: vi.fn() }),
  useSearchParams: () => navigation.searchParams,
}));
vi.mock("@/features/item-master/api/item-master-queries", () => ({ useCostSources: vi.fn(), useItemMasterMutations: vi.fn() }));
vi.mock("@/lib/membership/selected-membership-context", () => ({ useSelectedMembership: () => ({ selectedMembership: { permissions: ["cost-sources.read"] } }) }));
vi.mock("@/lib/permissions/can", () => ({ can: (_membership: unknown, permission: string) => permission === "cost-sources.read" || (permission === "cost-sources.manage" && navigation.canManage) }));
vi.mock("@/hooks/useToast", () => ({ useToast: () => ({ toast: { success: vi.fn(), error: vi.fn() } }) }));

function renderPage() {
  return render(<NextIntlClientProvider locale="th" messages={messages}><CostSourceMaintenance /></NextIntlClientProvider>);
}

describe("CostSourceMaintenance", () => {
  beforeEach(() => {
    navigation.canManage = false;
    navigation.searchParams = new URLSearchParams("status=inactive");
    vi.mocked(useCostSources).mockReturnValue({
      data: [
        { id: "active-1", code: "SUPPLIER", name: { thai: "ผู้ขาย", english: "Supplier" }, sourceType: "manual", isActive: true },
        { id: "inactive-1", code: "LEGACY", name: { thai: "แหล่งเก่า", english: "Legacy source" }, sourceType: "manual", isActive: false },
      ], isLoading: false, isError: false, refetch: vi.fn(),
    } as unknown as ReturnType<typeof useCostSources>);
    vi.mocked(useItemMasterMutations).mockReturnValue({
      createCostSource: { mutateAsync: vi.fn(), isPending: false, isError: false },
      updateCostSource: { mutateAsync: vi.fn(), isPending: false, isError: false },
      deactivateCostSource: { mutateAsync: vi.fn(), isPending: false, isError: false },
    } as unknown as ReturnType<typeof useItemMasterMutations>);
  });

  it("applies URL status filters and provides CSV/Excel export controls", () => {
    renderPage();
    expect(screen.getByRole("heading", { name: "แหล่งที่มาต้นทุน" })).toBeInTheDocument();
    expect(screen.getByText("แหล่งเก่า")).toBeInTheDocument();
    expect(screen.queryByText("ผู้ขาย")).toBeNull();
    expect(screen.getByLabelText("สถานะ")).toHaveValue("inactive");
    expect(screen.getByRole("button", { name: "ส่งออกแหล่งที่มาต้นทุน" })).toBeEnabled();
  });
  it("guards unsaved source values and submits through the drawer footer", async () => {
    navigation.canManage = true;
    const create = vi.fn().mockResolvedValue({ code: "SOURCE-1" });
    vi.mocked(useItemMasterMutations).mockReturnValue({
      createCostSource: { mutateAsync: create, isPending: false, isError: false },
      updateCostSource: { mutateAsync: vi.fn(), isPending: false, isError: false },
      deactivateCostSource: { mutateAsync: vi.fn(), isPending: false, isError: false },
    } as unknown as ReturnType<typeof useItemMasterMutations>);
    renderPage();
    fireEvent.click(screen.getByRole("button", { name: "เพิ่มแหล่งที่มา" }));
    const drawer = within(screen.getByRole("dialog"));
    fireEvent.change(drawer.getByPlaceholderText("ชื่อแหล่งที่มา (TH)"), { target: { value: "ผู้ขายไม้" } });
    fireEvent.click(drawer.getByRole("button", { name: "ปิด" }));
    expect(screen.getAllByRole("dialog")).toHaveLength(2);
    fireEvent.click(within(screen.getAllByRole("dialog")[1]).getByRole("button", { name: "ยกเลิก" }));
    expect(drawer.getByPlaceholderText("ชื่อแหล่งที่มา (TH)")).toHaveValue("ผู้ขายไม้");
    fireEvent.click(drawer.getByRole("button", { name: "บันทึกแหล่งที่มา" }));
    await waitFor(() => expect(create).toHaveBeenCalledOnce());
    expect(create).toHaveBeenCalledWith(expect.objectContaining({ payload: { code: null, name: { thai: "ผู้ขายไม้", english: null }, sourceType: "manual" } }));
    await waitFor(() => expect(screen.queryByRole("dialog")).not.toBeInTheDocument());
  });

});
