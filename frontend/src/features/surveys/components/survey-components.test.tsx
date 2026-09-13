import React from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import { SurveyAppointmentModal } from "./survey-appointment-modal";
import { SurveyCard } from "./survey-card";
import { SurveyWorkspaceDrawer } from "./survey-workspace-drawer";
import * as surveyQueries from "../api/survey-queries";
import * as siteQueries from "@/features/sites/api/site-queries";
import * as userQueries from "@/features/users/api/user-queries";
import type { OpportunityResponse, SiteSurveyResponse } from "@/lib/api/api-client";
import { ToastProvider } from "@/hooks/useToast";

vi.mock("@/lib/auth/auth-session", () => ({
  getAuthToken: vi.fn(async () => "test-token"),
}));

vi.mock("@/lib/membership/selected-membership-context", () => ({
  useSelectedMembership: () => ({
    currentUser: null,
    selectedMembership: { id: "membership-1", branch: { id: "branch-1" } },
  }),
}));

describe("Survey Components", () => {
  let client: QueryClient;

  beforeEach(() => {
    client = new QueryClient({ defaultOptions: { queries: { retry: false } } });
    vi.clearAllMocks();
  });

  const sampleOpportunity: OpportunityResponse = {
    id: "30000000-0000-0000-0000-000000000001",
    code: "OPP-0001",
    title: "โครงการปรับปรุงอาคาร",
    stage: "qualified",
    rowVersion: "00000000-0000-0000-0000-000000000001",
    customer: {
      id: "10000000-0000-0000-0000-000000000001",
      code: "CUS-0001",
      displayNameTh: "ลูกค้า",
    },
    primarySite: {
      id: "20000000-0000-0000-0000-000000000001",
      label: "สำนักงานใหญ่",
      addressLine1: "123 สุขุมวิท",
      subdistrict: "คลองเตย",
      district: "คลองเตย",
      province: "กรุงเทพมหานคร",
      postalCode: "10110",
    },
    branch: { id: "branch-1", name: "สาขา 1" },
  };

  const sampleSurvey: SiteSurveyResponse = {
    id: "40000000-0000-0000-0000-000000000001",
    surveyNumber: "SRV-2026-0001",
    opportunityId: sampleOpportunity.id,
    siteId: sampleOpportunity.primarySite?.id ?? undefined,
    assignedSurveyorId: "user-1",
    status: "scheduled",
    scheduledStartUtc: "2026-09-20T09:00:00Z",
    scheduledEndUtc: "2026-09-20T12:00:00Z",
    currentRevision: {
      id: "50000000-0000-0000-0000-000000000001",
      siteSurveyId: "40000000-0000-0000-0000-000000000001",
      revisionNumber: 1,
      status: "draft",
      readiness: "draft",
      rowVersion: "00000000-0000-0000-0000-000000000001",
      createdAtUtc: "2026-09-13T04:00:00Z",
    },
    rowVersion: "00000000-0000-0000-0000-000000000001",
    createdAtUtc: "2026-09-13T04:00:00Z",
  };

  describe("SurveyCard", () => {
    it("renders survey details, number, status badge, and revision badge", () => {
      render(
        <QueryClientProvider client={client}>
          <NextIntlClientProvider locale="th" messages={thMessages}>
            <ToastProvider>
              <SurveyCard
                survey={sampleSurvey}
                siteLabel="สำนักงานใหญ่ สุขุมวิท"
                surveyorName="สมชาย ช่างสำรวจ"
                opportunityId={sampleOpportunity.id}
                opportunityRowVersion={sampleOpportunity.rowVersion}
              />
            </ToastProvider>
          </NextIntlClientProvider>
        </QueryClientProvider>
      );

      expect(screen.getByText("SRV-2026-0001")).toBeDefined();
      expect(screen.getByText("สำนักงานใหญ่ สุขุมวิท")).toBeDefined();
      expect(screen.getByText("สมชาย ช่างสำรวจ")).toBeDefined();
      expect(screen.getByText("นัดหมายแล้ว")).toBeDefined();
      expect(screen.getByText(/รุ่นที่ 1/)).toBeDefined();
      expect(screen.getByRole("button", { name: /เปิดหน้าต่างสำรวจ/ })).toBeDefined();
    });

    it("opens SurveyWorkspaceDrawer when workspace button is clicked", async () => {
      render(
        <QueryClientProvider client={client}>
          <NextIntlClientProvider locale="th" messages={thMessages}>
            <ToastProvider>
              <SurveyCard
                survey={sampleSurvey}
                siteLabel="สำนักงานใหญ่ สุขุมวิท"
                surveyorName="สมชาย ช่างสำรวจ"
                opportunityId={sampleOpportunity.id}
                opportunityRowVersion={sampleOpportunity.rowVersion}
              />
            </ToastProvider>
          </NextIntlClientProvider>
        </QueryClientProvider>
      );

      const openBtn = screen.getByRole("button", { name: /เปิดหน้าต่างสำรวจ/ });
      fireEvent.click(openBtn);

      expect(await screen.findByText(/บันทึกผลการสำรวจหน้างาน/)).toBeDefined();
    });
  });

  describe("SurveyAppointmentModal", () => {
    it("renders form, selects site and surveyor, and submits create mutation", async () => {
      vi.spyOn(siteQueries, "useCustomerSiteList").mockReturnValue({
        data: {
          items: [
            {
              id: "20000000-0000-0000-0000-000000000001",
              label: "สำนักงานใหญ่ สุขุมวิท",
              customerId: sampleOpportunity.customer?.id ?? undefined,
            },
          ],
          totalCount: 1,
        },
        isLoading: false,
        isError: false,
        error: null,
        refetch: vi.fn(),
      } as unknown as ReturnType<typeof siteQueries.useCustomerSiteList>);

      vi.spyOn(userQueries, "useUserList").mockReturnValue({
        data: {
          items: [
            {
              id: "user-1",
              displayName: "สมชาย ช่างสำรวจ",
              email: "somchai@example.com",
              branchId: "branch-1",
            },
          ],
          totalCount: 1,
        },
        isLoading: false,
        isError: false,
        error: null,
        refetch: vi.fn(),
      } as unknown as ReturnType<typeof userQueries.useUserList>);

      const mutateAsyncMock = vi.fn().mockResolvedValue(sampleSurvey);
      vi.spyOn(surveyQueries, "useCreateSiteSurvey").mockReturnValue({
        mutateAsync: mutateAsyncMock,
        isPending: false,
      } as unknown as ReturnType<typeof surveyQueries.useCreateSiteSurvey>);

      const onClose = vi.fn();
      const onSuccess = vi.fn();

      render(
        <QueryClientProvider client={client}>
          <NextIntlClientProvider locale="th" messages={thMessages}>
            <ToastProvider>
              <SurveyAppointmentModal
                isOpen={true}
                onClose={onClose}
                opportunity={sampleOpportunity}
                onSuccess={onSuccess}
              />
            </ToastProvider>
          </NextIntlClientProvider>
        </QueryClientProvider>
      );

      // Modal title
      expect(screen.getByRole("heading", { name: /นัดหมายสำรวจหน้างาน/ })).toBeDefined();

      // Surveyor input autocomplete
      const surveyorInput = screen.getByPlaceholderText("เลือกพนักงานผู้ทำการสำรวจ...");
      fireEvent.focus(surveyorInput);

      const option = await screen.findByText("สมชาย ช่างสำรวจ");
      fireEvent.mouseDown(option);

      // Submit button
      const submitBtn = screen.getByRole("button", { name: "ยืนยันนัดหมาย" });
      fireEvent.click(submitBtn);

      await waitFor(() => {
        expect(mutateAsyncMock).toHaveBeenCalledTimes(1);
        expect(mutateAsyncMock).toHaveBeenCalledWith(
          expect.objectContaining({
            payload: expect.objectContaining({
              siteId: sampleOpportunity.primarySite?.id,
              assignedSurveyorId: "user-1",
              expectedOpportunityVersion: sampleOpportunity.rowVersion,
            }),
          })
        );
        expect(onSuccess).toHaveBeenCalledTimes(1);
        expect(onClose).toHaveBeenCalledTimes(1);
      });
    });

    it("displays error when end date is before start date", async () => {
      vi.spyOn(siteQueries, "useCustomerSiteList").mockReturnValue({
        data: {
          items: [
            {
              id: "20000000-0000-0000-0000-000000000001",
              label: "สำนักงานใหญ่ สุขุมวิท",
              customerId: sampleOpportunity.customer?.id ?? undefined,
            },
          ],
          totalCount: 1,
        },
        isLoading: false,
        isError: false,
        error: null,
        refetch: vi.fn(),
      } as unknown as ReturnType<typeof siteQueries.useCustomerSiteList>);

      vi.spyOn(userQueries, "useUserList").mockReturnValue({
        data: {
          items: [
            {
              id: "user-1",
              displayName: "สมชาย ช่างสำรวจ",
              email: "somchai@example.com",
              branchId: "branch-1",
            },
          ],
          totalCount: 1,
        },
        isLoading: false,
        isError: false,
        error: null,
        refetch: vi.fn(),
      } as unknown as ReturnType<typeof userQueries.useUserList>);

      const mutateAsyncMock = vi.fn();
      vi.spyOn(surveyQueries, "useCreateSiteSurvey").mockReturnValue({
        mutateAsync: mutateAsyncMock,
        isPending: false,
      } as unknown as ReturnType<typeof surveyQueries.useCreateSiteSurvey>);

      render(
        <QueryClientProvider client={client}>
          <NextIntlClientProvider locale="th" messages={thMessages}>
            <ToastProvider>
              <SurveyAppointmentModal
                isOpen={true}
                onClose={vi.fn()}
                opportunity={sampleOpportunity}
              />
            </ToastProvider>
          </NextIntlClientProvider>
        </QueryClientProvider>
      );

      // Select surveyor
      const surveyorInput = screen.getByPlaceholderText("เลือกพนักงานผู้ทำการสำรวจ...");
      fireEvent.focus(surveyorInput);
      const option = await screen.findByText("สมชาย ช่างสำรวจ");
      fireEvent.mouseDown(option);

      // Fill invalid date range
      const startInput = screen.getByLabelText(/เวลานัดหมายเริ่มต้น/);
      const endInput = screen.getByLabelText(/เวลานัดหมายสิ้นสุด/);
      fireEvent.change(startInput, { target: { value: "2026-09-20T12:00" } });
      fireEvent.change(endInput, { target: { value: "2026-09-20T09:00" } });

      const submitBtn = screen.getByRole("button", { name: "ยืนยันนัดหมาย" });
      fireEvent.click(submitBtn);

      await waitFor(() => {
        expect(screen.getAllByText("เวลาสิ้นสุดต้องอยู่หลังเวลาเริ่มต้น").length).toBeGreaterThanOrEqual(1);
        expect(mutateAsyncMock).not.toHaveBeenCalled();
      });
    });
  });

  describe("SurveyWorkspaceDrawer", () => {
    it("renders empty state, adds area, adds measurement, and saves draft", async () => {
      const updateDraftMock = vi.fn().mockResolvedValue({
        ...sampleSurvey,
      });

      vi.spyOn(surveyQueries, "useUpdateSurveyDraft").mockReturnValue({
        mutateAsync: updateDraftMock,
        isPending: false,
      } as unknown as ReturnType<typeof surveyQueries.useUpdateSurveyDraft>);

      vi.spyOn(surveyQueries, "useMarkSurveyReady").mockReturnValue({
        mutateAsync: vi.fn(),
        isPending: false,
      } as unknown as ReturnType<typeof surveyQueries.useMarkSurveyReady>);

      render(
        <QueryClientProvider client={client}>
          <NextIntlClientProvider locale="th" messages={thMessages}>
            <ToastProvider>
              <SurveyWorkspaceDrawer
                isOpen={true}
                onClose={vi.fn()}
                opportunityId={sampleOpportunity.id ?? ""}
                survey={sampleSurvey}
                currentOpportunityVersion={sampleOpportunity.rowVersion ?? ""}
              />
            </ToastProvider>
          </NextIntlClientProvider>
        </QueryClientProvider>
      );

      // Verify header rendered
      expect(screen.getByText(/บันทึกผลการสำรวจหน้างาน/)).toBeDefined();

      // Enter scope summary
      const scopeTextarea = screen.getByPlaceholderText(/ระบุภาพรวมของงานสำรวจ/);
      fireEvent.change(scopeTextarea, { target: { value: "สำรวจพื้นที่สำหรับงาน built-in ตู้เสื้อผ้า" } });

      // Click add area
      const addAreaBtn = screen.getByRole("button", { name: /เพิ่มพื้นที่สำรวจ/ });
      fireEvent.click(addAreaBtn);

      // Area inputs should appear
      const areaNameInput = screen.getByPlaceholderText("ชื่อพื้นที่");
      fireEvent.change(areaNameInput, { target: { value: "ห้องนอนใหญ่" } });

      // Click add measurement
      const addMeasureBtn = screen.getByRole("button", { name: /เพิ่มระยะวัด/ });
      fireEvent.click(addMeasureBtn);

      // Measurement value input
      const valueInputs = screen.getAllByPlaceholderText("ค่าที่วัดได้");
      fireEvent.change(valueInputs[0], { target: { value: "2500" } });

      // Click Save Draft
      const saveDraftBtn = screen.getByRole("button", { name: "บันทึกฉบับร่าง" });
      fireEvent.click(saveDraftBtn);

      await waitFor(() => {
        expect(updateDraftMock).toHaveBeenCalledTimes(1);
        expect(updateDraftMock).toHaveBeenCalledWith(
          expect.objectContaining({
            payload: expect.objectContaining({
              scopeSummary: "สำรวจพื้นที่สำหรับงาน built-in ตู้เสื้อผ้า",
              areas: expect.arrayContaining([
                expect.objectContaining({
                  name: "ห้องนอนใหญ่",
                  measurements: expect.arrayContaining([
                    expect.objectContaining({
                      value: 2500,
                    }),
                  ]),
                }),
              ]),
            }),
          })
        );
      });
    });

    it("validates readiness rules before opening mark ready confirmation", async () => {
      vi.spyOn(surveyQueries, "useUpdateSurveyDraft").mockReturnValue({
        mutateAsync: vi.fn(),
        isPending: false,
      } as unknown as ReturnType<typeof surveyQueries.useUpdateSurveyDraft>);

      vi.spyOn(surveyQueries, "useMarkSurveyReady").mockReturnValue({
        mutateAsync: vi.fn(),
        isPending: false,
      } as unknown as ReturnType<typeof surveyQueries.useMarkSurveyReady>);

      render(
        <QueryClientProvider client={client}>
          <NextIntlClientProvider locale="th" messages={thMessages}>
            <ToastProvider>
              <SurveyWorkspaceDrawer
                isOpen={true}
                onClose={vi.fn()}
                opportunityId={sampleOpportunity.id ?? ""}
                survey={sampleSurvey}
                currentOpportunityVersion={sampleOpportunity.rowVersion ?? ""}
              />
            </ToastProvider>
          </NextIntlClientProvider>
        </QueryClientProvider>
      );

      // Clear visitedAt to test missing visit validation
      const visitedInput = screen.getByLabelText("วันที่และเวลาเข้าพบจริง");
      fireEvent.change(visitedInput, { target: { value: "" } });

      // Attempt mark ready without visit date
      const markReadyBtn = screen.getByRole("button", { name: "ยืนยันความพร้อม (Mark Ready)" });
      fireEvent.click(markReadyBtn);

      // Expect validation error displayed
      await waitFor(() => {
        expect(screen.getByText("กรุณาระบุวันและเวลาที่เข้าพบจริง")).toBeDefined();
      });

      // Now fill visit date but leave scope summary empty
      fireEvent.change(visitedInput, { target: { value: "2026-09-20T10:00" } });
      fireEvent.click(markReadyBtn);

      await waitFor(() => {
        expect(screen.getByText("กรุณากรอกสรุปขอบเขตงานสำรวจ")).toBeDefined();
      });
    });

    it("converts length measurements to millimeters in real-time and guards unsaved changes", async () => {
      const onClose = vi.fn();

      render(
        <QueryClientProvider client={client}>
          <NextIntlClientProvider locale="th" messages={thMessages}>
            <ToastProvider>
              <SurveyWorkspaceDrawer
                isOpen={true}
                onClose={onClose}
                opportunityId={sampleOpportunity.id ?? ""}
                survey={sampleSurvey}
                currentOpportunityVersion={sampleOpportunity.rowVersion ?? ""}
              />
            </ToastProvider>
          </NextIntlClientProvider>
        </QueryClientProvider>
      );

      // Add area
      const addAreaBtn = screen.getByRole("button", { name: /เพิ่มพื้นที่สำรวจ/ });
      fireEvent.click(addAreaBtn);

      // Add measurement value: 2.5 (meter is default)
      const valueInput = screen.getAllByPlaceholderText("ค่าที่วัดได้")[0];
      fireEvent.change(valueInput, { target: { value: "2.5" } });

      // Expect real-time conversion badge to show "= 2,500 mm"
      await waitFor(() => {
        expect(screen.getAllByText(/= 2,500 mm/).length).toBeGreaterThanOrEqual(1);
      });

      // Attempt to close drawer with unsaved changes
      const cancelBtn = screen.getByRole("button", { name: "ยกเลิก" });
      fireEvent.click(cancelBtn);

      // Unsaved changes confirmation modal should appear
      expect(await screen.findByText("ละทิ้งการเปลี่ยนแปลงหรือไม่?")).toBeDefined();
      expect(onClose).not.toHaveBeenCalled();

      // Click continue editing
      const continueBtn = screen.getByRole("button", { name: "แก้ไขต่อ" });
      fireEvent.click(continueBtn);
      expect(onClose).not.toHaveBeenCalled();

      // Click cancel again and confirm discard
      fireEvent.click(cancelBtn);
      const discardBtn = await screen.findByRole("button", { name: "ละทิ้งข้อมูล" });
      fireEvent.click(discardBtn);
      expect(onClose).toHaveBeenCalledTimes(1);
    });

    it("opens confirmation modal and executes mark ready mutation when valid", async () => {
      const markReadyMock = vi.fn().mockResolvedValue({
        ...sampleSurvey,
        currentRevision: {
          ...sampleSurvey.currentRevision,
          status: "ready",
          snapshotHash: "abc123456789",
        },
      });

      vi.spyOn(surveyQueries, "useUpdateSurveyDraft").mockReturnValue({
        mutateAsync: vi.fn().mockResolvedValue(sampleSurvey.currentRevision),
        isPending: false,
      } as unknown as ReturnType<typeof surveyQueries.useUpdateSurveyDraft>);

      vi.spyOn(surveyQueries, "useMarkSurveyReady").mockReturnValue({
        mutateAsync: markReadyMock,
        isPending: false,
      } as unknown as ReturnType<typeof surveyQueries.useMarkSurveyReady>);

      const onClose = vi.fn();

      render(
        <QueryClientProvider client={client}>
          <NextIntlClientProvider locale="th" messages={thMessages}>
            <ToastProvider>
              <SurveyWorkspaceDrawer
                isOpen={true}
                onClose={onClose}
                opportunityId={sampleOpportunity.id ?? ""}
                survey={sampleSurvey}
                currentOpportunityVersion={sampleOpportunity.rowVersion ?? ""}
              />
            </ToastProvider>
          </NextIntlClientProvider>
        </QueryClientProvider>
      );

      // Fill visitedAt
      const visitedInput = screen.getByLabelText("วันที่และเวลาเข้าพบจริง");
      fireEvent.change(visitedInput, { target: { value: "2026-09-20T10:00" } });

      // Fill scope
      const scopeTextarea = screen.getByPlaceholderText(/ระบุภาพรวมของงานสำรวจ/);
      fireEvent.change(scopeTextarea, { target: { value: "สำรวจพื้นที่เสร็จสมบูรณ์" } });

      // Add area
      const addAreaBtn = screen.getByRole("button", { name: /เพิ่มพื้นที่สำรวจ/ });
      fireEvent.click(addAreaBtn);

      const areaNameInput = screen.getByPlaceholderText("ชื่อพื้นที่");
      fireEvent.change(areaNameInput, { target: { value: "ห้องนอนใหญ่" } });

      // Fill measurement value
      const valueInput = screen.getAllByPlaceholderText("ค่าที่วัดได้")[0];
      fireEvent.change(valueInput, { target: { value: "2500" } });

      // Click Mark Ready
      const markReadyBtn = screen.getByRole("button", { name: "ยืนยันความพร้อม (Mark Ready)" });
      fireEvent.click(markReadyBtn);

      // Confirmation modal should appear
      expect(await screen.findByRole("heading", { name: /ยืนยันความพร้อมเพื่อส่งต่อประเมินราคา/ })).toBeDefined();

      // Click confirm in modal
      const markReadyButtons = screen.getAllByRole("button", { name: "ยืนยันความพร้อม (Mark Ready)" });
      const modalConfirmBtn = markReadyButtons[markReadyButtons.length - 1];
      fireEvent.click(modalConfirmBtn);

      await waitFor(() => {
        expect(markReadyMock).toHaveBeenCalledTimes(1);
        expect(markReadyMock).toHaveBeenCalledWith(
          expect.objectContaining({
            payload: expect.objectContaining({
              expectedRevisionVersion: sampleSurvey.currentRevision?.rowVersion,
              expectedOpportunityVersion: sampleOpportunity.rowVersion,
            }),
          })
        );
        expect(onClose).toHaveBeenCalledTimes(1);
      });
    });

    it("auto-fills area name when standard room preset is selected", async () => {
      render(
        <QueryClientProvider client={client}>
          <NextIntlClientProvider locale="th" messages={thMessages}>
            <ToastProvider>
              <SurveyWorkspaceDrawer
                isOpen={true}
                onClose={vi.fn()}
                opportunityId={sampleOpportunity.id ?? ""}
                survey={sampleSurvey}
                currentOpportunityVersion={sampleOpportunity.rowVersion ?? ""}
              />
            </ToastProvider>
          </NextIntlClientProvider>
        </QueryClientProvider>
      );

      // Add new area
      const addAreaBtn = screen.getByRole("button", { name: /เพิ่มพื้นที่สำรวจ/ });
      fireEvent.click(addAreaBtn);

      // Select preset from dropdown
      const presetSelect = screen.getByLabelText("เลือกห้อง/พื้นที่มาตรฐาน");
      fireEvent.change(presetSelect, { target: { value: "master_bedroom" } });

      // Check that the area name input is auto-filled
      const areaNameInput = screen.getByPlaceholderText("ชื่อพื้นที่") as HTMLInputElement;
      expect(areaNameInput.value).toBe("ห้องนอนใหญ่ (Master Bedroom)");
    });

    it("adds multiple areas at once via batch template modal", async () => {
      render(
        <QueryClientProvider client={client}>
          <NextIntlClientProvider locale="th" messages={thMessages}>
            <ToastProvider>
              <SurveyWorkspaceDrawer
                isOpen={true}
                onClose={vi.fn()}
                opportunityId={sampleOpportunity.id ?? ""}
                survey={sampleSurvey}
                currentOpportunityVersion={sampleOpportunity.rowVersion ?? ""}
              />
            </ToastProvider>
          </NextIntlClientProvider>
        </QueryClientProvider>
      );

      // Open batch template modal
      const batchBtn = screen.getByRole("button", { name: "สร้างจากชุดเทมเพลต" });
      fireEvent.click(batchBtn);

      // Verify modal is open
      expect(await screen.findByRole("heading", { name: /สร้างชุดพื้นที่สำรวจมาตรฐาน/ })).toBeDefined();

      // Click condo studio package (3 rooms: living_room, master_bedroom, kitchen)
      const condoBtn = screen.getByText(/คอนโด Studio/);
      fireEvent.click(condoBtn);

      // Confirm add
      const confirmBtn = screen.getByRole("button", { name: /สร้างพื้นที่ที่เลือก \(3 ห้อง\)/ });
      fireEvent.click(confirmBtn);

      // Check that 3 areas were appended
      await waitFor(() => {
        expect(screen.getByDisplayValue("AREA-01")).toBeDefined();
        expect(screen.getByDisplayValue("AREA-02")).toBeDefined();
        expect(screen.getByDisplayValue("AREA-03")).toBeDefined();
        expect(screen.getByDisplayValue("ห้องนั่งเล่น (Living Room)")).toBeDefined();
        expect(screen.getByDisplayValue("ห้องนอนใหญ่ (Master Bedroom)")).toBeDefined();
        expect(screen.getByDisplayValue("ห้องครัว (Kitchen)")).toBeDefined();
      });
    });

    it("toggles 3D preview visualizer panel on button click", async () => {
      render(
        <QueryClientProvider client={client}>
          <NextIntlClientProvider locale="th" messages={thMessages}>
            <ToastProvider>
              <SurveyWorkspaceDrawer
                isOpen={true}
                onClose={vi.fn()}
                opportunityId={sampleOpportunity.id ?? ""}
                survey={sampleSurvey}
                currentOpportunityVersion={sampleOpportunity.rowVersion ?? ""}
              />
            </ToastProvider>
          </NextIntlClientProvider>
        </QueryClientProvider>
      );

      // Add new area
      const addAreaBtn = screen.getByRole("button", { name: /เพิ่มพื้นที่สำรวจ/ });
      fireEvent.click(addAreaBtn);

      // 3D toggle button should be present
      const toggle3DBtn = screen.getByRole("button", { name: /จำลองแบบ 3D/ });
      expect(toggle3DBtn).toBeDefined();

      // Click to open 3D preview
      fireEvent.click(toggle3DBtn);

      // Visualizer panel should show reset button, zoom slider, and hint
      expect(await screen.findByRole("button", { name: "รีเซ็ตมุมมอง" })).toBeDefined();
      expect(screen.getByLabelText("ซูม (Zoom):")).toBeDefined();
      expect(screen.getByText(/หมุน: ลากนิ้ว\/เมาส์บนภาพ/)).toBeDefined();

      // Click to close 3D preview (toggle button or visualizer close button)
      const closeButtons = screen.getAllByRole("button", { name: /ซ่อน 3D/ });
      fireEvent.click(closeButtons[0]);

      await waitFor(() => {
        expect(screen.queryByRole("button", { name: "รีเซ็ตมุมมอง" })).toBeNull();
      });
    });
  });
});

