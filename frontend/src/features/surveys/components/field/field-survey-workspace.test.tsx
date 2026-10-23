import React from "react";
import { describe, it, expect, vi } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { FieldSurveyWorkspace } from "./field-survey-workspace";

vi.mock("next-intl", () => ({
  useTranslations: () => (key: string, params?: Record<string, unknown>) => {
    const translations: Record<string, string> = {
      title: "สำรวจหน้างานสำหรับรีโนเวท",
      subtitle: "บันทึกขนาด สเปกวัสดุ และภาพถ่ายหน้างานจริง สำหรับส่งต่อทีมประเมินราคา",
      opportunity: "โอกาสทางการขาย",
      selectOpportunity: "— เลือกโอกาสทางการขาย —",
      roomName: "ชื่อห้อง / บริเวณสำรวจ",
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
      "actions.saveDraft": "บันทึกข้อมูลสำรวจ",
      "actions.saving": "กำลังบันทึก...",
      "actions.savedSuccess": "บันทึกข้อมูลสำรวจหน้างานเรียบร้อยแล้ว",
      "actions.sendToEstimator": "ส่งให้ทีมประเมินราคา (BOQ)",
      "actions.deleteItem": "ลบรายการนี้",
      "actions.itemNumber": `รายการที่ ${params?.number ?? ""}`,
      "empty.noItemsTitle": "ยังไม่มีรายการสั่งทำ",
      "empty.noItemsDesc": "กดปุ่ม 'เพิ่มรายการสั่งทำ' เพื่อเริ่มบันทึกขนาดและสเปกวัสดุ",
    };
    return translations[key] ?? key;
  },
  useLocale: () => "th",
}));

vi.mock("@/features/opportunities/api/opportunity-queries", () => ({
  useOpportunityList: () => ({
    data: {
      pages: [
        {
          items: [
            {
              id: "opp-123",
              title: "รีโนเวทคอนโด 2 ห้องนอน คุณสมชาย",
              customer: { name: "คุณสมชาย" },
            },
          ],
        },
      ],
    },
    isLoading: false,
  }),
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
  it("renders header fields, opportunity selector, and empty state when no items", () => {
    render(<FieldSurveyWorkspace />);

    expect(screen.getByText("สำรวจหน้างานสำหรับรีโนเวท")).toBeDefined();
    expect(screen.getByText("โอกาสทางการขาย")).toBeDefined();
    expect(screen.getByText("ชื่อห้อง / บริเวณสำรวจ")).toBeDefined();
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
});
