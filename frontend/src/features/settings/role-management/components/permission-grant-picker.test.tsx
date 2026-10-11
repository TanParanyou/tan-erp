import React from "react";
import { describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen } from "@testing-library/react";
import { PermissionGrantPicker } from "./permission-grant-picker";

const catalog = [
  { key: "customers.read", description: "Read customers", isApprovalPermission: false },
  { key: "estimates.approve", description: "Approve estimates", isApprovalPermission: true },
  { key: "estimates.read", description: null, isApprovalPermission: false },
];

describe("PermissionGrantPicker", () => {
  it("groups by module, toggles a key, flags approval permissions and filters by search", () => {
    const onToggle = vi.fn();
    render(<PermissionGrantPicker catalog={catalog} selected={new Set(["customers.read"])} disabled={false} onToggle={onToggle} />);

    expect(screen.getByLabelText("customers.read")).toBeChecked();
    expect(screen.getByLabelText("estimates.approve")).not.toBeChecked();
    expect(screen.getByText("สิทธิ์อนุมัติ")).toBeInTheDocument();
    expect(screen.getByText("เลือกแล้ว 1 สิทธิ์")).toBeInTheDocument();

    fireEvent.click(screen.getByLabelText("estimates.approve"));
    expect(onToggle).toHaveBeenCalledWith("estimates.approve");

    fireEvent.change(screen.getByPlaceholderText("ค้นหาสิทธิ์"), { target: { value: "zzz" } });
    expect(screen.getByText("ไม่พบสิทธิ์ที่ค้นหา")).toBeInTheDocument();
  });

  it("disables every checkbox when read-only", () => {
    render(<PermissionGrantPicker catalog={catalog} selected={new Set()} disabled onToggle={vi.fn()} />);

    expect(screen.getByLabelText("customers.read")).toBeDisabled();
  });
});
