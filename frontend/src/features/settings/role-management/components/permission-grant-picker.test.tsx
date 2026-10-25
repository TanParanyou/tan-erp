import React from "react";
import { describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen } from "@testing-library/react";
import { PermissionGrantPicker } from "./permission-grant-picker";

const catalog = [
  { key: "customers.read", description: "Read customers", module: "crm", isApprovalPermission: false },
  { key: "estimates.approve", description: "Approve estimates", module: "survey_estimating", isDocumentApproval: true, requiresMakerChecker: true, isApprovalPermission: true },
  { key: "estimates.read", description: null, module: "survey_estimating", isApprovalPermission: false },
  { key: "approval-limits.manage", description: "Manage limits", module: "system", isDocumentApproval: false, requiresMakerChecker: true, isApprovalPermission: true },
];

describe("PermissionGrantPicker", () => {
  it("groups by module, toggles a key, flags approval permissions and filters by search", () => {
    const onToggle = vi.fn();
    render(<PermissionGrantPicker catalog={catalog} selected={new Set(["customers.read"])} disabled={false} onToggle={onToggle} />);

    expect(screen.getByLabelText("customers.read")).toBeChecked();
    expect(screen.getByLabelText("estimates.approve")).not.toBeChecked();
    expect(screen.getByText("สิทธิ์อนุมัติเอกสาร • ผูกวงเงินได้")).toBeInTheDocument();
    expect(screen.getByText("ต้องตรวจ 2 ขั้นตอน (Maker-Checker)")).toBeInTheDocument();
    expect(screen.getByText(/สิทธิ์ระดับผู้ดูแลในการเข้าไปกำหนด/)).toBeInTheDocument();
    expect(screen.getByText("เลือกแล้ว 1 สิทธิ์")).toBeInTheDocument();

    fireEvent.click(screen.getByLabelText("estimates.approve"));
    expect(onToggle).toHaveBeenCalledWith("estimates.approve");

    fireEvent.change(screen.getByPlaceholderText("ค้นหาสิทธิ์"), { target: { value: "zzz" } });
    expect(screen.getByText("ไม่พบสิทธิ์ที่ค้นหา")).toBeInTheDocument();
  });

  it("supports batch toggling by group with icon action buttons", () => {
    const onBatchToggle = vi.fn();
    const { rerender } = render(
      <PermissionGrantPicker
        catalog={catalog}
        selected={new Set(["estimates.read"])}
        disabled={false}
        onToggle={vi.fn()}
        onBatchToggle={onBatchToggle}
      />
    );

    // Module order: system (0), crm (1), survey_estimating (2)
    const selectButtons = screen.getAllByRole("button", { name: "เลือกทั้งหมดในกลุ่ม" });
    fireEvent.click(selectButtons[0]);
    expect(onBatchToggle).toHaveBeenCalledWith(["approval-limits.manage"], true);

    // When all items in system group are selected, its button switches to clear
    rerender(
      <PermissionGrantPicker
        catalog={catalog}
        selected={new Set(["approval-limits.manage", "estimates.read"])}
        disabled={false}
        onToggle={vi.fn()}
        onBatchToggle={onBatchToggle}
      />
    );

    const clearButton = screen.getByRole("button", { name: "ล้างการเลือกในกลุ่ม" });
    expect(clearButton).toBeInTheDocument();
    fireEvent.click(clearButton);
    expect(onBatchToggle).toHaveBeenCalledWith(["approval-limits.manage"], false);
  });

  it("disables every checkbox when read-only", () => {
    render(<PermissionGrantPicker catalog={catalog} selected={new Set()} disabled onToggle={vi.fn()} />);

    expect(screen.getByLabelText("customers.read")).toBeDisabled();
  });

  it("supports batch toggling for an entire feature card", () => {
    const onBatchToggle = vi.fn();
    render(
      <PermissionGrantPicker
        catalog={catalog}
        selected={new Set()}
        disabled={false}
        onToggle={vi.fn()}
        onBatchToggle={onBatchToggle}
      />
    );

    // Feature toggle for estimates (has estimates.approve and estimates.read)
    const featureToggle = screen.getByRole("button", {
      name: "เลือกทั้งหมดในหมวด การประเมินราคาทางการ (Official Estimates)",
    });
    expect(featureToggle).toBeInTheDocument();
    fireEvent.click(featureToggle);
    expect(onBatchToggle).toHaveBeenCalledWith(["estimates.approve", "estimates.read"], true);
  });
});
