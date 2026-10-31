import React from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import type { AuditEventListParams, AuditEventListResponse, AuditEventResponse } from "@/lib/api/api-client";
import { AuditEventList } from "./audit-event-list";
import * as auditQueries from "../api/audit-queries";

const navState = vi.hoisted(() => ({
  search: "",
  push: vi.fn(),
  replace: vi.fn(),
}));

vi.mock("next/navigation", () => ({
  useRouter: () => ({ push: navState.push, replace: navState.replace }),
  usePathname: () => "/th/settings/audit",
  useSearchParams: () => new URLSearchParams(navState.search),
}));

const mockRefetch = vi.fn();

function makeAuditEvent(overrides: Partial<AuditEventResponse> = {}): AuditEventResponse {
  return {
    id: "audit-1",
    occurredAtUtc: "2026-10-20T10:30:00Z",
    action: "create",
    resourceType: "estimate",
    resourceId: "est-101",
    actor: { id: "user-1", name: "สมศรี ผู้จัดการ" },
    branchId: "branch-1",
    traceId: "trace-abc",
    requestId: "req-xyz",
    reason: "สร้างใบประเมินราคาฉบับร่าง",
    changesJson: JSON.stringify({ status: "draft", grandTotal: 15000 }),
    ...overrides,
  };
}

function renderList() {
  return render(
    <NextIntlClientProvider locale="th" messages={thMessages}>
      <AuditEventList />
    </NextIntlClientProvider>
  );
}

describe("AuditEventList (G-21 Audit Log)", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    navState.search = "";
  });

  it("renders audit event list with correctly formatted fields and headers", () => {
    const items = [
      makeAuditEvent({ id: "event-1", action: "create", resourceType: "estimate", resourceId: "EST-001" }),
      makeAuditEvent({ id: "event-2", action: "approve", resourceType: "quotation", resourceId: "QT-002", reason: "อนุมัติเสนอราคา" }),
    ];

    vi.spyOn(auditQueries, "useAuditEvents").mockReturnValue({
      data: {
        items,
        pagination: { page: 1, pageSize: 25, totalPages: 1, totalCount: 2 },
      } as AuditEventListResponse,
      isLoading: false,
      isError: false,
      refetch: mockRefetch,
    } as unknown as ReturnType<typeof auditQueries.useAuditEvents>);

    renderList();

    expect(screen.getByText(thMessages.auditLog.list.title)).toBeDefined();
    expect(screen.getByText(thMessages.auditLog.list.subtitle)).toBeDefined();
    expect(screen.getByText("EST-001")).toBeDefined();
    expect(screen.getByText("QT-002")).toBeDefined();
    expect(screen.getByText("อนุมัติเสนอราคา")).toBeDefined();
  });

  it("opens forensic inspector drawer when inspect button is clicked", () => {
    const item = makeAuditEvent({
      id: "event-inspect",
      action: "update",
      resourceType: "Customer",
      resourceId: "CUST-009",
      traceId: "trace-inspect-123",
      changesJson: JSON.stringify({ name: "บริษัท ทดสอบ จำกัด" }),
    });

    vi.spyOn(auditQueries, "useAuditEvents").mockReturnValue({
      data: {
        items: [item],
        pagination: { page: 1, pageSize: 25, totalPages: 1, totalCount: 1 },
      } as AuditEventListResponse,
      isLoading: false,
      isError: false,
      refetch: mockRefetch,
    } as unknown as ReturnType<typeof auditQueries.useAuditEvents>);

    renderList();

    const inspectBtn = screen.getByRole("button", { name: `${thMessages.auditLog.list.inspect} CUST-009` });
    fireEvent.click(inspectBtn);

    expect(screen.getByText(thMessages.auditLog.inspector.title)).toBeDefined();
    expect(screen.getByText("trace-inspect-123")).toBeDefined();
    expect(screen.getByText(/"name": "บริษัท ทดสอบ จำกัด"/)).toBeDefined();
  });

  it("handles redacted audit information gracefully", () => {
    const redactedItem = makeAuditEvent({
      id: "event-redacted",
      action: "update",
      resourceType: "user",
      resourceId: "usr-999",
      actor: { id: "user-system", name: "[REDACTED]" },
      reason: "[REDACTED: SENSITIVE_INFO]",
      changesJson: JSON.stringify({ passwordHash: "[REDACTED]" }),
    });

    vi.spyOn(auditQueries, "useAuditEvents").mockReturnValue({
      data: {
        items: [redactedItem],
        pagination: { page: 1, pageSize: 25, totalPages: 1, totalCount: 1 },
      } as AuditEventListResponse,
      isLoading: false,
      isError: false,
      refetch: mockRefetch,
    } as unknown as ReturnType<typeof auditQueries.useAuditEvents>);

    renderList();

    expect(screen.getByText("[REDACTED]")).toBeDefined();
    expect(screen.getByText("[REDACTED: SENSITIVE_INFO]")).toBeDefined();
    expect(screen.getByText("usr-999")).toBeDefined();
  });

  it("handles fallback dashes for missing or null event fields", () => {
    const emptyFieldsItem = makeAuditEvent({
      id: "event-empty",
      action: null,
      resourceType: null,
      resourceId: null,
      actor: undefined,
      reason: null,
    });

    vi.spyOn(auditQueries, "useAuditEvents").mockReturnValue({
      data: {
        items: [emptyFieldsItem],
        pagination: { page: 1, pageSize: 25, totalPages: 1, totalCount: 1 },
      } as AuditEventListResponse,
      isLoading: false,
      isError: false,
      refetch: mockRefetch,
    } as unknown as ReturnType<typeof auditQueries.useAuditEvents>);

    renderList();

    const dashes = screen.getAllByText("-");
    expect(dashes.length).toBeGreaterThanOrEqual(4);
  });

  it("passes search and filter parameters to query and updates url", () => {
    let capturedParams: AuditEventListParams | undefined;
    vi.spyOn(auditQueries, "useAuditEvents").mockImplementation((params) => {
      capturedParams = params;
      return {
        data: { items: [], pagination: { page: 1, pageSize: 25, totalPages: 0, totalCount: 0 } },
        isLoading: false,
        isError: false,
        refetch: mockRefetch,
      } as unknown as ReturnType<typeof auditQueries.useAuditEvents>;
    });

    navState.search = "from=2026-10-01&to=2026-10-20&action=create&resourceType=Customer&search=EST-101";
    renderList();

    expect(capturedParams?.from).toBe("2026-10-01T00:00:00Z");
    expect(capturedParams?.to).toBe("2026-10-20T23:59:59Z");
    expect(capturedParams?.action).toBe("create");
    expect(capturedParams?.resourceType).toBe("Customer");
    expect(capturedParams?.resourceId).toBe("EST-101");
  });

  it("removes active filter chips atomically without race conditions", () => {
    vi.spyOn(auditQueries, "useAuditEvents").mockReturnValue({
      data: { items: [], pagination: { page: 1, pageSize: 25, totalPages: 0, totalCount: 0 } },
      isLoading: false,
      isError: false,
      refetch: mockRefetch,
    } as unknown as ReturnType<typeof auditQueries.useAuditEvents>);

    navState.search = "from=2026-10-01&to=2026-10-20&action=create";
    renderList();

    const removeActionChip = screen.getByRole("button", {
      name: `${thMessages.common.filters.removeFilter} ${thMessages.auditLog.list.actionFilter}: create`,
    });
    fireEvent.click(removeActionChip);

    expect(navState.replace).toHaveBeenCalledWith(
      expect.stringContaining("from=2026-10-01"),
      expect.objectContaining({ scroll: false })
    );
    expect(navState.replace).toHaveBeenCalledWith(
      expect.not.stringContaining("action=create"),
      expect.objectContaining({ scroll: false })
    );
  });

  it("shows empty state when there are no audit logs and no filters active", () => {
    vi.spyOn(auditQueries, "useAuditEvents").mockReturnValue({
      data: {
        items: [],
        pagination: { page: 1, pageSize: 25, totalPages: 0, totalCount: 0 },
      } as AuditEventListResponse,
      isLoading: false,
      isError: false,
      refetch: mockRefetch,
    } as unknown as ReturnType<typeof auditQueries.useAuditEvents>);

    renderList();

    expect(screen.getByText(thMessages.auditLog.list.emptyTitle)).toBeDefined();
    expect(screen.getByText(thMessages.auditLog.list.emptyDescription)).toBeDefined();
  });

  it("shows error state and calls refetch when retry button is clicked", () => {
    vi.spyOn(auditQueries, "useAuditEvents").mockReturnValue({
      data: undefined,
      isLoading: false,
      isError: true,
      refetch: mockRefetch,
    } as unknown as ReturnType<typeof auditQueries.useAuditEvents>);

    renderList();

    expect(screen.getByText(thMessages.auditLog.errors.GENERIC)).toBeDefined();
    const retryButton = screen.getByRole("button", { name: thMessages.common.actions.retry });
    fireEvent.click(retryButton);
    expect(mockRefetch).toHaveBeenCalledTimes(1);
  });
});
