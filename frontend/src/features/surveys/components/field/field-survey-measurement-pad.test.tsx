import React from "react";
import { describe, it, expect, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { FieldSurveyMeasurementPad } from "./field-survey-measurement-pad";
import type {
  FieldSurveyMeasurementType,
  BoxDimensions,
  AreaDimensions,
  CustomMeasurementPoint,
} from "../../schemas/field-survey-schema";

vi.mock("next-intl", () => ({
  useTranslations: () => (key: string) => {
    const translations: Record<string, string> = {
      measurementType: "รูปแบบการวัดขนาด",
      "measurementTypes.box": "กล่อง 3 มิติ (กว้าง × ลึก × สูง)",
      "measurementTypes.area": "พื้นที่ผิว (ตร.ม. พร้อมหักช่องเปิด)",
      "measurementTypes.custom": "จุดวัดเฉพาะ (หลืบม่าน/ระยะเสา/ระยะฝ้า)",
      "dimensions.title": "มิติและขนาดที่วัดได้จริง",
      "dimensions.width": "ความกว้าง (Width)",
      "dimensions.depth": "ความลึก (Depth)",
      "dimensions.height": "ความสูง (Height)",
      "dimensions.grossWidth": "ความกว้างรวม (Gross W)",
      "dimensions.grossLength": "ความยาวรวม (Gross L)",
      "dimensions.grossArea": "พื้นที่รวม",
      "dimensions.deductions": "รายการหักพื้นที่ช่องเปิด / เสาโครงสร้าง",
      "dimensions.addDeduction": "เพิ่มรายการหักพื้นที่",
      "dimensions.deductionLabel": "ชื่อจุดที่หัก",
      "dimensions.deductionLabelPlaceholder": "เช่น เสาโครงสร้าง, ช่องเปิดบันได, ช่องท่อ",
      "dimensions.deductionArea": "พื้นที่ที่หัก (ตร.ม.)",
      "dimensions.netArea": "พื้นที่สุทธิสำหรับสั่งผลิต/ปูพื้น",
      "dimensions.customPoints": "รายการจุดวัดเฉพาะทาง",
      "dimensions.addCustomPoint": "เพิ่มจุดวัด",
      "dimensions.pointLabel": "ชื่อจุดวัด",
      "dimensions.pointLabelPlaceholder": "เช่น ความกว้างกรอบหน้าต่าง, หลืบม่านลึก, สูงถึงบัว",
      "dimensions.pointValue": "ระยะที่วัดได้ (มม.)",
      "dimensions.pointNotes": "หมายเหตุจุดวัด",
      "dimensions.pointNotesPlaceholder": "เช่น เว้นรางไฟ, ระวังท่อแอร์",
      "dimensions.unitMm": "มม.",
      "dimensions.unitSqm": "ตร.ม.",
    };
    return translations[key] ?? key;
  },
}));

describe("FieldSurveyMeasurementPad Component", () => {
  it("renders box measurement inputs when measurementType is 'box'", () => {
    const boxDimensions: BoxDimensions = { widthMm: 1200, depthMm: 600, heightMm: 750 };
    const onBoxChange = vi.fn();
    const onTypeChange = vi.fn();

    render(
      <FieldSurveyMeasurementPad
        measurementType="box"
        boxDimensions={boxDimensions}
        onTypeChange={onTypeChange}
        onBoxChange={onBoxChange}
      />
    );

    expect(screen.getByDisplayValue("1200")).toBeDefined();
    expect(screen.getByDisplayValue("600")).toBeDefined();
    expect(screen.getByDisplayValue("750")).toBeDefined();
  });

  it("updates box dimensions when user enters new numbers", () => {
    const boxDimensions: BoxDimensions = { widthMm: 1200, depthMm: 600, heightMm: 750 };
    const onBoxChange = vi.fn();

    render(
      <FieldSurveyMeasurementPad
        measurementType="box"
        boxDimensions={boxDimensions}
        onTypeChange={vi.fn()}
        onBoxChange={onBoxChange}
      />
    );

    const widthInput = screen.getByDisplayValue("1200");
    fireEvent.change(widthInput, { target: { value: "1500" } });

    expect(onBoxChange).toHaveBeenCalledWith({
      widthMm: 1500,
      depthMm: 600,
      heightMm: 750,
    });
  });

  it("renders area dimensions with deduction calculations", () => {
    const areaDimensions: AreaDimensions = {
      grossWidthMm: 4000,
      grossLengthMm: 5000,
      deductions: [{ label: "เสา", areaSqm: 1.0 }],
    };
    const onAreaChange = vi.fn();

    render(
      <FieldSurveyMeasurementPad
        measurementType="area"
        areaDimensions={areaDimensions}
        onTypeChange={vi.fn()}
        onAreaChange={onAreaChange}
      />
    );

    expect(screen.getByDisplayValue("4000")).toBeDefined();
    expect(screen.getByDisplayValue("5000")).toBeDefined();
    expect(screen.getByDisplayValue("เสา")).toBeDefined();
    // 20.00 - 1.00 = 19.00 sqm
    expect(screen.getByText("19")).toBeDefined();
  });

  it("adds a new custom measurement point when clicking add", () => {
    const customPoints: CustomMeasurementPoint[] = [
      { label: "ความกว้างกรอบหน้าต่าง", valueMm: 2800 },
    ];
    const onCustomChange = vi.fn();

    render(
      <FieldSurveyMeasurementPad
        measurementType="custom"
        customMeasurements={customPoints}
        onTypeChange={vi.fn()}
        onCustomChange={onCustomChange}
      />
    );

    const addBtn = screen.getByRole("button", { name: /เพิ่มจุดวัด/ });
    fireEvent.click(addBtn);

    expect(onCustomChange).toHaveBeenCalledWith([
      { label: "ความกว้างกรอบหน้าต่าง", valueMm: 2800 },
      { label: "", valueMm: 0 },
    ]);
  });
});
