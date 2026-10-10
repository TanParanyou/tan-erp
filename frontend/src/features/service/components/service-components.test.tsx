import React from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import enMessages from "@/messages/en.json";
import { InstallationDetail } from "./installation-detail";
import { ServiceRequestDetail } from "./service-request-detail";
import * as serviceQueries from "../api/service-queries";
import { ApiError } from "@/lib/api/api-error";
import type { InstallationResponse, ServiceRequestResponse } from "@/lib/api/api-client";
import {
  DEFECT_SEVERITIES,
  DEFECT_STATUSES,
  INSTALLATION_STATUSES,
  SERVICE_PRIORITIES,
  SERVICE_REQUEST_STATUSES,
  defectStatusVariant,
  installationStatusVariant,
  serviceErrorCode,
  serviceRequestStatusVariant,
} from "../service-status";

const toastMocks = vi.hoisted(() => ({ success: vi.fn(), error: vi.fn() }));
const permissions = vi.hoisted(() => ({ granted: [] as string[] }));

vi.mock("@/hooks/useToast", () => ({
  useToast: () => ({ toast: { success: toastMocks.success, error: toastMocks.error } }),
}));
vi.mock("next/navigation", () => ({
  usePathname: () => "/th/service",
  useRouter: () => ({ push: vi.fn(), replace: vi.fn(), back: vi.fn() }),
  useSearchParams: () => new URLSearchParams(),
}));
vi.mock("@/lib/membership/selected-membership-context", () => ({
  useSelectedMembership: () => ({ selectedMembership: { id: "membership-1" } }),
}));
vi.mock("@/lib/permissions/can", () => ({
  can: (_membership: unknown, permission: string) => permissions.granted.includes(permission),
}));

vi.mock("@/components/forms/AttachmentList", () => ({
  AttachmentList: (props: { ownerType: string; ownerId: string; canManage: boolean }) => (
    <div data-testid="attachment-list" data-owner-type={props.ownerType} data-owner-id={props.ownerId} data-can-manage={String(props.canManage)} />
  ),
}));
vi.mock("@/components/forms/SignatureCapturePanel", () => ({
  SignatureCapturePanel: (props: { ownerType: string; ownerId: string; purpose: string; canCapture: boolean }) => (
    <div data-testid="signature-panel" data-owner-type={props.ownerType} data-purpose={props.purpose} data-can-capture={String(props.canCapture)} />
  ),
}));

const i = thMessages.service.installations;
const r = thMessages.service.requests;
const ALL = ["installations.manage", "installations.operate", "installations.handover", "service-requests.manage"];

function job(status: string, overrides: Partial<InstallationResponse> = {}): InstallationResponse {
  return {
    id: "job-1",
    number: "INS-2026-0001",
    status,
    project: { id: "p-1", code: "PRJ-1", name: "บ้านคุณสมชาย" },
    scheduledStart: "2026-11-01",
    scheduledEnd: "2026-11-05",
    rowVersion: "00000000-0000-0000-0000-0000000000a1",
    createdBy: { id: "u-1", displayName: "ผู้สร้าง" },
    checklist: [{ id: "c-1", sortOrder: 1, title: "ตรวจวัดพื้นที่", required: true, done: false }],
    defects: [],
    ...overrides,
  } as InstallationResponse;
}

const step = vi.fn();

function renderJob(data: InstallationResponse, granted: string[] = ALL) {
  permissions.granted = granted;
  vi.spyOn(serviceQueries, "useInstallation").mockReturnValue({ data, isLoading: false, isError: false } as unknown as ReturnType<typeof serviceQueries.useInstallation>);
  vi.spyOn(serviceQueries, "useInstallationMutations").mockReturnValue({
    step: { mutateAsync: step, isPending: false },
    create: { mutateAsync: vi.fn(), isPending: false },
  } as unknown as ReturnType<typeof serviceQueries.useInstallationMutations>);
  render(
    <NextIntlClientProvider locale="th" messages={thMessages}>
      <InstallationDetail installationId="job-1" />
    </NextIntlClientProvider>
  );
}

function request(status: string, overrides: Partial<ServiceRequestResponse> = {}): ServiceRequestResponse {
  return {
    id: "req-1",
    number: "SRV-2026-0001",
    title: "ประตูตู้ปิดไม่สนิท",
    description: "ประตูตู้เสื้อผ้า",
    priority: "high",
    status,
    inWarranty: false,
    project: { id: "p-1", code: "PRJ-1", name: "บ้านคุณสมชาย" },
    rowVersion: "00000000-0000-0000-0000-0000000000b1",
    createdBy: { id: "u-1", displayName: "ผู้รับเรื่อง" },
    events: [{ toStatus: "open", actor: { id: "u-1", displayName: "ผู้รับเรื่อง" }, occurredAtUtc: "2026-10-04T00:00:00Z" }],
    ...overrides,
  } as ServiceRequestResponse;
}

const requestStep = vi.fn();

function renderRequest(data: ServiceRequestResponse, granted: string[] = ALL) {
  permissions.granted = granted;
  vi.spyOn(serviceQueries, "useServiceRequest").mockReturnValue({ data, isLoading: false, isError: false } as unknown as ReturnType<typeof serviceQueries.useServiceRequest>);
  vi.spyOn(serviceQueries, "useServiceRequestMutations").mockReturnValue({
    step: { mutateAsync: requestStep, isPending: false },
    create: { mutateAsync: vi.fn(), isPending: false },
  } as unknown as ReturnType<typeof serviceQueries.useServiceRequestMutations>);
  render(
    <NextIntlClientProvider locale="th" messages={thMessages}>
      <ServiceRequestDetail requestId="req-1" />
    </NextIntlClientProvider>
  );
}

describe("service status helpers and translations", () => {
  it("map statuses to badge variants and recognise only known errors", () => {
    expect(installationStatusVariant("handed_over")).toBe("success");
    expect(installationStatusVariant("cancelled")).toBe("danger");
    expect(defectStatusVariant("verified")).toBe("success");
    expect(defectStatusVariant("reopened")).toBe("danger");
    expect(serviceRequestStatusVariant("open")).toBe("warning");
    expect(serviceErrorCode("INSTALLATION_SELF_VERIFICATION")).toBe("INSTALLATION_SELF_VERIFICATION");
    expect(serviceErrorCode("NOPE")).toBeNull();
  });

  it("have Thai and English text for every status, severity, priority and key error", () => {
    for (const messages of [thMessages, enMessages]) {
      for (const status of INSTALLATION_STATUSES) expect(messages.service.installations.statuses[status]).toBeTruthy();
      for (const severity of DEFECT_SEVERITIES) expect(messages.service.installations.severities[severity]).toBeTruthy();
      for (const status of DEFECT_STATUSES) expect(messages.service.installations.defectStatuses[status]).toBeTruthy();
      for (const status of SERVICE_REQUEST_STATUSES) expect(messages.service.requests.statuses[status]).toBeTruthy();
      for (const priority of SERVICE_PRIORITIES) expect(messages.service.requests.priorities[priority]).toBeTruthy();
      for (const code of ["INSTALLATION_CHECKLIST_INCOMPLETE", "INSTALLATION_DEFECTS_OPEN", "INSTALLATION_SELF_VERIFICATION", "SERVICE_INVALID_STATE"] as const) {
        expect(messages.service.errors[code]).toBeTruthy();
      }
      expect(messages.projects.control.errors.PROJECT_OPEN_INSTALLATION).toBeTruthy();
      expect(messages.shell.serviceSubgroup).toBeTruthy();
    }
  });
});

describe("InstallationDetail", () => {
  beforeEach(() => vi.clearAllMocks());

  it("renders the shared attachment list and signature panel for the installation owner", () => {
    renderJob(job("ready_for_handover"), ["installations.operate", "installations.handover"]);

    const list = screen.getByTestId("attachment-list");
    expect(list.dataset.ownerType).toBe("installation-job");
    expect(list.dataset.ownerId).toBe("job-1");
    expect(list.dataset.canManage).toBe("true");

    const signature = screen.getByTestId("signature-panel");
    expect(signature.dataset.ownerType).toBe("installation-job");
    expect(signature.dataset.purpose).toBe("handover");
    expect(signature.dataset.canCapture).toBe("true");
  });

  it("limits attachment and signature controls by status and permission", () => {
    renderJob(job("handed_over"), ["installations.operate"]);
    expect(screen.getByTestId("attachment-list").dataset.canManage).toBe("false");
    expect(screen.getByTestId("signature-panel").dataset.canCapture).toBe("false");
  });

  it("does not allow signing without the handover permission", () => {
    renderJob(job("ready_for_handover"), ["installations.operate"]);
    expect(screen.getByTestId("attachment-list").dataset.canManage).toBe("true");
    expect(screen.getByTestId("signature-panel").dataset.canCapture).toBe("false");
  });

  it("offers start only on a planned job and mark-ready only while in progress", () => {
    renderJob(job("planned"));
    expect(screen.getByRole("button", { name: i.start })).toBeDefined();
    expect(screen.queryByRole("button", { name: i.markReady })).toBeNull();
  });

  it("ticks a checklist item through the API while in progress", async () => {
    step.mockResolvedValue(job("in_progress"));
    renderJob(job("in_progress"));
    fireEvent.click(screen.getByRole("checkbox", { name: "ตรวจวัดพื้นที่" }));
    await waitFor(() => expect(step).toHaveBeenCalledWith({ id: "job-1", rowVersion: "00000000-0000-0000-0000-0000000000a1", step: { kind: "checklist", itemId: "c-1", done: true } }));
  });

  it("shows the translated error when the server refuses to mark ready", async () => {
    step.mockRejectedValue(new ApiError({ status: 422, code: "INSTALLATION_DEFECTS_OPEN", message: "raw" }));
    renderJob(job("in_progress"));
    fireEvent.click(screen.getByRole("button", { name: i.markReady }));
    expect(await screen.findByText(thMessages.service.errors.INSTALLATION_DEFECTS_OPEN)).toBeDefined();
  });

  it("requires the signer and an explicit warranty term before recording an acceptance", async () => {
    step.mockResolvedValue(job("handed_over"));
    renderJob(job("ready_for_handover"));
    fireEvent.click(screen.getByRole("button", { name: i.recordHandover }));
    fireEvent.click(await screen.findByRole("button", { name: i.confirm }));
    expect(await screen.findByText(i.handoverAcceptInvalid)).toBeDefined();
    expect(step).not.toHaveBeenCalled();

    fireEvent.change(screen.getByLabelText(new RegExp("ชื่อผู้รับมอบ")), { target: { value: "คุณสมชาย" } });
    fireEvent.change(screen.getByLabelText(new RegExp("ระยะประกัน")), { target: { value: "12" } });
    fireEvent.click(screen.getByRole("button", { name: i.confirm }));
    await waitFor(() => expect(step).toHaveBeenCalledTimes(1));
    expect(step.mock.calls[0][0].step).toEqual({ kind: "handover", request: { outcome: "accepted", signerName: "คุณสมชาย", note: null, warrantyMonths: 12, handoverDate: null } });
  });

  it("hides handover from users without the handover permission, and verify only on a resolved defect", () => {
    renderJob(
      job("in_progress", { defects: [{ id: "d-1", no: 1, description: "รอย", severity: "minor", status: "resolved", reportedBy: { id: "u", displayName: "x" } } as never] }),
      ["installations.operate"]
    );
    expect(screen.getByRole("button", { name: i.verify })).toBeDefined();
    expect(screen.queryByRole("button", { name: i.recordHandover })).toBeNull();
  });
});

describe("ServiceRequestDetail", () => {
  beforeEach(() => vi.clearAllMocks());

  it("flags a request outside warranty and shows the status history", () => {
    renderRequest(request("open"));
    expect(screen.getByText(r.outOfWarrantyNote)).toBeDefined();
    expect(screen.getByText(r.historyTitle)).toBeDefined();
  });

  it("offers close on a resolved request and requires a note to reopen", async () => {
    requestStep.mockResolvedValue(request("in_progress"));
    renderRequest(request("resolved"));
    expect(screen.getByRole("button", { name: r.close })).toBeDefined();
    fireEvent.click(screen.getByRole("button", { name: r.reopen }));
    fireEvent.click(await screen.findByRole("button", { name: r.confirm }));
    expect(await screen.findByText(r.textRequired)).toBeDefined();
    expect(requestStep).not.toHaveBeenCalled();

    fireEvent.change(screen.getByLabelText(new RegExp(r.reopenReason)), { target: { value: "ปัญหากลับมา" } });
    fireEvent.click(screen.getByRole("button", { name: r.confirm }));
    await waitFor(() => expect(requestStep).toHaveBeenCalledWith({ id: "req-1", rowVersion: "00000000-0000-0000-0000-0000000000b1", step: { kind: "reopen", reason: "ปัญหากลับมา" } }));
  });

  it("hides the actions from users without the manage permission", () => {
    renderRequest(request("open"), []);
    expect(screen.queryByRole("button", { name: r.start })).toBeNull();
  });
});
