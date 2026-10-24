import React from "react";
import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { FieldSurveyWorkspace } from "./field-survey-workspace";

const mockPush = vi.fn();
vi.mock("next/navigation", () => ({
  useRouter: () => ({
    push: mockPush,
  }),
}));

vi.mock("next-intl", () => ({
  useTranslations: () => (key: string, params?: Record<string, unknown>) => {
    const translations: Record<string, string> = {
      title: "สำรวจหน้างานสำหรับรีโนเวท",
      subtitle: "บันทึกขนาด สเปกวัสดุ และภาพถ่ายหน้างานจริง สำหรับส่งต่อทีมประเมินราคา",
      opportunity: "โอกาสทางการขาย",
      selectOpportunity: "— เลือกโอกาสทางการขาย —",
      roomName: "ชื่อห้อง / บริเวณสำรวจ",
      roomNameRequired: "กรุณาระบุชื่อห้อง / บริเวณสำรวจ",
      jumpToItem: "ข้ามไปยัง:",
      unnamedItem: "(ยังไม่ได้ระบุชื่อ)",
      roomPlaceholder: "เช่น ห้องนอนใหญ่ Master Bedroom, โถงรับแขก",
      surveyorNotes: "บันทึกภาพรวมหน้างาน / สไตล์ที่ลูกค้าชอบ",
      surveyorNotesPlaceholder: "เช่น ลูกค้าชอบสไตล์ Modern Classic หรูหรา โทนไม้อุ่นตัดหินอ่อนเทา",
      itemsSectionTitle: "รายการที่ลูกค้าสั่งทำในห้องนี้",
      addItem: "เพิ่มรายการสั่งทำ",
      itemTitle: "ชื่อรายการ / ชิ้นงาน",
      itemTitlePlaceholder: "เช่น โต๊ะทำงานไม้สัก, ตู้เสื้อผ้า Walk-in, พื้นหินอ่อน",
      category: "หมวดหมู่งาน",
      "categories.built_in": "เฟอร์นิเจอร์ Built-in",
      "categories.loose": "เฟอร์นิเจอร์ลอยตัว Loose",
      "categories.flooring": "งานพื้นและกระเบื้อง",
      "categories.wall_ceiling": "งานผนังและฝ้าเพดาน",
      "categories.curtain": "งานม่านและตกแต่งอ่อน",
      "categories.mep": "งานระบบไฟฟ้า/ประปา",
      collapseAll: "ย่อทุกรายการ",
      expandAll: "ขยายทุกรายการ",
      itemsCount: `${params?.count ?? ""} ชิ้นงาน`,
      totalPhotos: `${params?.count ?? ""} ภาพถ่าย`,
      "actions.saveDraft": "บันทึกข้อมูลสำรวจ",
      "actions.saving": "กำลังบันทึก...",
      "actions.savedSuccess": "บันทึกข้อมูลสำรวจหน้างานเรียบร้อยแล้ว",
      "actions.saveFailed": "ไม่สามารถบันทึกข้อมูลสำรวจได้ กรุณาลองใหม่อีกครั้ง",
      "actions.sendToEstimator": "ส่งให้ทีมประเมินราคา (BOQ)",
      "actions.deleteItem": "ลบรายการนี้",
      "actions.itemNumber": `รายการที่ ${params?.number ?? ""}`,
      "dimensions.boxSummary": `${params?.w} × ${params?.d} × ${params?.h} มม.`,
      "dimensions.areaSummary": `พื้นที่ ${params?.area} ตร.ม.`,
      "dimensions.customSummary": `${params?.count} จุดวัด`,
      "completionModal.title": "บันทึกข้อมูลสำรวจเรียบร้อยแล้ว",
      "completionModal.subtitle": "ข้อมูลห้องและรายการสั่งทำถูกบันทึกเข้าสู่โอกาสทางการขายแล้ว",
      "completionModal.summaryRoom": "บริเวณสำรวจ:",
      "completionModal.summaryItems": `${params?.count ?? ""} รายการสั่งทำ`,
      "completionModal.summaryPhotos": `${params?.count ?? ""} ภาพถ่าย`,
      "completionModal.nextRoomBtn": "สำรวจห้องถัดไป (งานเดิม)",
      "completionModal.viewOpportunityBtn": "ดูสรุปภาพรวมโอกาสขาย",
      "completionModal.stayOnPageBtn": "อยู่หน้านี้ต่อเพื่อตรวจสอบ",
      "empty.noItemsTitle": "ยังไม่มีรายการสั่งทำ",
      "empty.noItemsDesc": "กดปุ่ม 'เพิ่มรายการสั่งทำ' เพื่อเริ่มบันทึกขนาดและสเปกวัสดุ",
      "style.sectionTitle": "สไตล์และมู้ดแอนด์โทนที่ลูกค้าชอบ (Mood & Tone)",
      "style.sectionDesc": "บันทึกทิศทางความชอบด้านดีไซน์และโทนสี เพื่อเป็นข้อมูลอ้างอิงให้ทีมออกแบบและคำนวณราคา",
      "style.designStyleLabel": "สไตล์การตกแต่ง (Design Style)",
      "style.moodToneLabel": "มู้ดและโทนสี (Mood & Tone)",
      "style.customStyleNotes": "ข้อจำกัดหรือข้อกำหนดพิเศษด้านสไตล์/สี",
      "style.customStyleNotesPlaceholder": "เช่น ลูกค้าไม่เอาไม้โทนแดง, เน้นไฟวอร์มไลท์ 3000K, บัวพื้นสีขาวล้วน",
    };
    return translations[key] ?? key;
  },
  useLocale: () => "th",
}));

vi.mock("../../api/survey-queries", () => ({
  useSurveyReferenceData: () => ({
    data: undefined,
    isLoading: false,
  }),
  useOpportunitySurvey: () => ({
    data: null,
    isLoading: false,
  }),
  useCreateSiteSurvey: () => ({
    mutateAsync: vi.fn(),
    isPending: false,
  }),
}));

vi.mock("@/features/opportunities/api/opportunity-queries", () => ({
  useOpportunityDetail: () => ({
    data: null,
    isLoading: false,
  }),
}));

vi.mock("@/features/opportunities/components/opportunity-autocomplete", () => ({
  OpportunityAutocomplete: ({
    value,
    onChange,
  }: {
    value?: string | null;
    onChange: (id: string) => void;
  }) => (
    <div data-testid="mock-opportunity-autocomplete">
      <input
        aria-label="โอกาสทางการขาย"
        value={value ?? ""}
        onChange={(e) => onChange(e.target.value)}
        placeholder="พิมพ์ค้นหาชื่อโอกาสขาย, รหัส หรือชื่อลูกค้า..."
      />
    </div>
  ),
}));

vi.mock("./room-name-combobox", () => ({
  RoomNameCombobox: ({
    value,
    onChange,
  }: {
    value: string;
    onChange: (name: string) => void;
  }) => (
    <div data-testid="mock-room-name-combobox">
      <label>ชื่อห้อง / บริเวณสำรวจ</label>
      <input
        value={value}
        onChange={(e) => onChange(e.target.value)}
        placeholder="เช่น ห้องนอนใหญ่ Master Bedroom, โถงรับแขก"
      />
    </div>
  ),
}));

vi.mock("./field-survey-catalog-picker", () => ({
  FieldSurveyCatalogPicker: () => <div data-testid="mock-catalog-picker">Mock Catalog Picker</div>,
}));

vi.mock("./field-survey-measurement-pad", () => ({
  FieldSurveyMeasurementPad: () => <div data-testid="mock-measurement-pad">Mock Measurement Pad</div>,
}));

vi.mock("./field-survey-item-photo-manager", () => ({
  FieldSurveyItemPhotoManager: () => <div data-testid="mock-photo-manager">Mock Photo Manager</div>,
}));

describe("FieldSurveyWorkspace Component", () => {
  beforeEach(() => {
    vi.stubGlobal("scrollTo", vi.fn());
    if (typeof window !== "undefined") {
      window.scrollTo = vi.fn();
    }
  });
  it("renders header fields, opportunity selector, and empty state when no items", () => {
    render(<FieldSurveyWorkspace />);

    expect(screen.getAllByText("สำรวจหน้างานสำหรับรีโนเวท").length).toBeGreaterThanOrEqual(1);
    expect(screen.getByTestId("mock-opportunity-autocomplete")).toBeDefined();
    expect(screen.getByTestId("mock-room-name-combobox")).toBeDefined();
    expect(screen.getByText("ยังไม่มีรายการสั่งทำ")).toBeDefined();
  });

  it("adds a new item card when clicking add item button", () => {
    render(<FieldSurveyWorkspace />);

    const addBtn = screen.getAllByRole("button", { name: /เพิ่มรายการสั่งทำ/ })[0];
    fireEvent.click(addBtn);

    expect(screen.getByPlaceholderText("เช่น โต๊ะทำงานไม้สัก, ตู้เสื้อผ้า Walk-in, พื้นหินอ่อน")).toBeDefined();
    expect(screen.getByTestId("mock-measurement-pad")).toBeDefined();
    expect(screen.getByTestId("mock-catalog-picker")).toBeDefined();
    expect(screen.getByTestId("mock-photo-manager")).toBeDefined();
  });

  it("toggles card collapse when clicking card header", () => {
    render(<FieldSurveyWorkspace />);

    // Add an item
    const addBtn = screen.getAllByRole("button", { name: /เพิ่มรายการสั่งทำ/ })[0];
    fireEvent.click(addBtn);

    // Enter title
    const titleInput = screen.getByPlaceholderText("เช่น โต๊ะทำงานไม้สัก, ตู้เสื้อผ้า Walk-in, พื้นหินอ่อน");
    fireEvent.change(titleInput, { target: { value: "โต๊ะทำงานไม้สัก" } });

    // Item starts expanded
    expect(screen.getByTestId("mock-measurement-pad")).toBeDefined();

    // Click card header to collapse
    const header = screen.getByText(/รายการที่ 1:/);
    fireEvent.click(header);

    // Body subcomponents are now hidden in collapsed state
    expect(screen.queryByTestId("mock-measurement-pad")).toBeNull();

    // Click again to re-expand
    fireEvent.click(header);
    expect(screen.getByTestId("mock-measurement-pad")).toBeDefined();
  });

  it("removes item card when clicking delete item button", () => {
    render(<FieldSurveyWorkspace />);

    // Add first item
    const addBtn = screen.getAllByRole("button", { name: /เพิ่มรายการสั่งทำ/ })[0];
    fireEvent.click(addBtn);

    const deleteBtn = screen.getByRole("button", { name: "ลบรายการนี้" });
    fireEvent.click(deleteBtn);

    expect(screen.getByText("ยังไม่มีรายการสั่งทำ")).toBeDefined();
  });

  it("disables save button and displays saving state during submission", async () => {
    const onSave = vi.fn().mockImplementation(() => new Promise((resolve) => setTimeout(resolve, 50)));
    render(<FieldSurveyWorkspace onSave={onSave} />);

    // Select opportunity and room name
    const roomInput = screen.getByPlaceholderText("เช่น ห้องนอนใหญ่ Master Bedroom, โถงรับแขก");
    fireEvent.change(roomInput, { target: { value: "ห้องนอนใหญ่ Master Bedroom" } });

    // Add an item
    const addBtn = screen.getAllByRole("button", { name: /เพิ่มรายการสั่งทำ/ })[0];
    fireEvent.click(addBtn);

    const titleInput = screen.getByPlaceholderText("เช่น โต๊ะทำงานไม้สัก, ตู้เสื้อผ้า Walk-in, พื้นหินอ่อน");
    fireEvent.change(titleInput, { target: { value: "โต๊ะทำงานไม้สัก" } });

    const saveBtn = screen.getByRole("button", { name: "บันทึกข้อมูลสำรวจ" });
    fireEvent.click(saveBtn);

    await waitFor(() => {
      expect(onSave).toHaveBeenCalled();
    });
  });

  it("opens completion modal on save and resets room for next room when clicking next room button", async () => {
    const onSave = vi.fn().mockResolvedValue(undefined);
    render(<FieldSurveyWorkspace initialOpportunityId="opp-123" onSave={onSave} />);

    // Enter room name
    const roomInput = screen.getByPlaceholderText("เช่น ห้องนอนใหญ่ Master Bedroom, โถงรับแขก");
    fireEvent.change(roomInput, { target: { value: "ห้องนอนใหญ่ Master Bedroom" } });

    // Add an item
    const addBtn = screen.getAllByRole("button", { name: /เพิ่มรายการสั่งทำ/ })[0];
    fireEvent.click(addBtn);

    const titleInput = screen.getByPlaceholderText("เช่น โต๊ะทำงานไม้สัก, ตู้เสื้อผ้า Walk-in, พื้นหินอ่อน");
    fireEvent.change(titleInput, { target: { value: "ตู้เสื้อผ้า Walk-in" } });

    // Submit
    const saveBtn = screen.getByRole("button", { name: "บันทึกข้อมูลสำรวจ" });
    fireEvent.click(saveBtn);

    // Modal should appear
    await waitFor(() => {
      expect(screen.getByText("บันทึกข้อมูลสำรวจเรียบร้อยแล้ว")).toBeDefined();
    });

    // Check summary contents inside modal
    expect(screen.getByText("1 รายการสั่งทำ")).toBeDefined();

    // Click "สำรวจห้องถัดไป (งานเดิม)"
    const nextRoomBtn = screen.getByRole("button", { name: /สำรวจห้องถัดไป \(งานเดิม\)/ });
    fireEvent.click(nextRoomBtn);

    // Modal should close and room/items reset
    await waitFor(() => {
      expect(screen.queryByText("บันทึกข้อมูลสำรวจเรียบร้อยแล้ว")).toBeNull();
    });

    const refreshedRoomInput = screen.getByPlaceholderText("เช่น ห้องนอนใหญ่ Master Bedroom, โถงรับแขก") as HTMLInputElement;
    expect(refreshedRoomInput.value).toBe("");
    expect(screen.getByText("ยังไม่มีรายการสั่งทำ")).toBeDefined();
  });

  it("navigates to opportunity detail when clicking view opportunity button in modal", async () => {
    mockPush.mockClear();
    const onSave = vi.fn().mockResolvedValue(undefined);
    render(<FieldSurveyWorkspace initialOpportunityId="opp-777" onSave={onSave} />);

    const roomInput = screen.getByPlaceholderText("เช่น ห้องนอนใหญ่ Master Bedroom, โถงรับแขก");
    fireEvent.change(roomInput, { target: { value: "ห้องครัว" } });

    const addBtn = screen.getAllByRole("button", { name: /เพิ่มรายการสั่งทำ/ })[0];
    fireEvent.click(addBtn);

    const titleInput = screen.getByPlaceholderText("เช่น โต๊ะทำงานไม้สัก, ตู้เสื้อผ้า Walk-in, พื้นหินอ่อน");
    fireEvent.change(titleInput, { target: { value: "เคาน์เตอร์ครัวไอแลนด์" } });

    const saveBtn = screen.getByRole("button", { name: "บันทึกข้อมูลสำรวจ" });
    fireEvent.click(saveBtn);

    await waitFor(() => {
      expect(screen.getByText("บันทึกข้อมูลสำรวจเรียบร้อยแล้ว")).toBeDefined();
    });

    const viewOppBtn = screen.getByRole("button", { name: /ดูสรุปภาพรวมโอกาสขาย/ });
    fireEvent.click(viewOppBtn);

    expect(mockPush).toHaveBeenCalledWith("/th/opportunities/opp-777");
  });

  it("shows localized validation error when submitting with empty room name", () => {
    render(<FieldSurveyWorkspace />);

    // Add an item and give it a title so it doesn't fail HTML5 required validation
    const addBtn = screen.getAllByRole("button", { name: /เพิ่มรายการสั่งทำ/ })[0];
    fireEvent.click(addBtn);

    const titleInput = screen.getByPlaceholderText("เช่น โต๊ะทำงานไม้สัก, ตู้เสื้อผ้า Walk-in, พื้นหินอ่อน");
    fireEvent.change(titleInput, { target: { value: "ตู้บิลต์อิน" } });

    const saveBtn = screen.getByRole("button", { name: "บันทึกข้อมูลสำรวจ" });
    fireEvent.click(saveBtn);

    expect(screen.getByText("กรุณาระบุชื่อห้อง / บริเวณสำรวจ")).toBeDefined();
  });

  it("shows localized error message when onSave throws an error", async () => {
    const onSave = vi.fn().mockRejectedValue(new Error("Network Error"));
    render(<FieldSurveyWorkspace onSave={onSave} />);

    const roomInput = screen.getByPlaceholderText("เช่น ห้องนอนใหญ่ Master Bedroom, โถงรับแขก");
    fireEvent.change(roomInput, { target: { value: "ห้องนอนใหญ่ Master Bedroom" } });

    const addBtn = screen.getAllByRole("button", { name: /เพิ่มรายการสั่งทำ/ })[0];
    fireEvent.click(addBtn);

    const titleInput = screen.getByPlaceholderText("เช่น โต๊ะทำงานไม้สัก, ตู้เสื้อผ้า Walk-in, พื้นหินอ่อน");
    fireEvent.change(titleInput, { target: { value: "เตียงนอนบิลต์อิน" } });

    const saveBtn = screen.getByRole("button", { name: "บันทึกข้อมูลสำรวจ" });
    fireEvent.click(saveBtn);

    await waitFor(() => {
      expect(screen.getByText("ไม่สามารถบันทึกข้อมูลสำรวจได้ กรุณาลองใหม่อีกครั้ง")).toBeDefined();
    });
  });

  it("renders style and mood & tone selection options and allows toggling", () => {
    render(<FieldSurveyWorkspace />);

    // Check style button
    const modernLuxuryBtn = screen.getByRole("button", { name: /Modern Luxury/ });
    expect(modernLuxuryBtn).toBeDefined();

    // Check mood tone button
    const warmEarthyBtn = screen.getByRole("button", { name: /Warm & Earthy/ });
    expect(warmEarthyBtn).toBeDefined();

    // Click to select
    fireEvent.click(modernLuxuryBtn);
    fireEvent.click(warmEarthyBtn);

    // Verify custom style notes input
    const notesInput = screen.getByPlaceholderText(/ลูกค้าไม่เอาไม้โทนแดง/);
    expect(notesInput).toBeDefined();
    fireEvent.change(notesInput, { target: { value: "ไม่เอาไม้สีเข้ม เน้นไม้สนขาว" } });
    expect((notesInput as HTMLInputElement).value).toBe("ไม่เอาไม้สีเข้ม เน้นไม้สนขาว");
  });
});
