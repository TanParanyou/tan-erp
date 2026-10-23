import React from "react";
import { describe, it, expect, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { FieldSurveyCatalogPicker } from "./field-survey-catalog-picker";
import type { SpecSelection } from "../../schemas/field-survey-schema";

vi.mock("next-intl", () => ({
  useTranslations: () => (key: string) => {
    const translations: Record<string, string> = {
      title: "สเปกวัสดุและอุปกรณ์",
      subtitle: "เลือก SKU จากระบบ หรือพิมพ์หมายเหตุระบุความต้องการเพิ่มเติม",
      coreMaterial: "วัสดุโครงสร้าง (Core SKU)",
      surfaceFinish: "วัสดุปิดผิว (Finish SKU)",
      hardware: "อุปกรณ์ฟิตติ้ง (Hardware SKU)",
      customNotes: "หมายเหตุสเปกเพิ่มเติม / งานสั่งทำพิเศษ",
      customNotesPlaceholder: "เช่น ขอบโต๊ะลบมน 5 มม. ไม่เห็นเสี้ยนไม้ รางม่านซ่อนไฟ 12V",
      quickSelect: "เลือกสเปกยอดนิยม:",
      noSkuSelected: "ยังไม่ได้ระบุ SKU จากระบบ",
    };
    return translations[key] ?? key;
  },
  useLocale: () => "th",
}));

// Mock ItemAutocomplete to keep unit test isolated
vi.mock("@/features/item-master/components/item-autocomplete", () => ({
  ItemAutocomplete: ({
    label,
    placeholder,
    onChange,
  }: {
    label?: string;
    placeholder?: string;
    onChange: (id: string, item: { id: string; code: string; name: { thai: string } } | null) => void;
  }) => (
    <div data-testid={`mock-autocomplete-${label}`}>
      <span>{label}</span>
      <button
        type="button"
        onClick={() =>
          onChange("sku-mock-123", {
            id: "sku-mock-123",
            code: "MAT-TEAK-01",
            name: { thai: "ไม้สักทองอบแห้ง เกรด A" },
          })
        }
      >
        Select Mock SKU
      </button>
    </div>
  ),
}));

describe("FieldSurveyCatalogPicker Component", () => {
  it("renders catalog picker sections and custom notes textarea", () => {
    const value: SpecSelection = {
      coreSkuName: "ไม้อัดยาง 15 มม.",
      finishSkuName: "พ่นสี PU ด้าน",
      customNotes: "ขอบมน 5 มม.",
    };
    const onChange = vi.fn();

    render(<FieldSurveyCatalogPicker value={value} onChange={onChange} />);

    expect(screen.getByText("สเปกวัสดุและอุปกรณ์")).toBeDefined();
    expect(screen.getByText("วัสดุโครงสร้าง (Core SKU)")).toBeDefined();
    expect(screen.getByText("วัสดุปิดผิว (Finish SKU)")).toBeDefined();
    expect(screen.getByText("อุปกรณ์ฟิตติ้ง (Hardware SKU)")).toBeDefined();
    expect(screen.getByDisplayValue("ขอบมน 5 มม.")).toBeDefined();
  });

  it("calls onChange when typing into custom notes field", () => {
    const value: SpecSelection = {};
    const onChange = vi.fn();

    render(<FieldSurveyCatalogPicker value={value} onChange={onChange} />);

    const textarea = screen.getByPlaceholderText(
      "เช่น ขอบโต๊ะลบมน 5 มม. ไม่เห็นเสี้ยนไม้ รางม่านซ่อนไฟ 12V"
    );
    fireEvent.change(textarea, { target: { value: "ท็อปหินอ่อนมุมตัดเฉียง" } });

    expect(onChange).toHaveBeenCalledWith(
      expect.objectContaining({
        customNotes: "ท็อปหินอ่อนมุมตัดเฉียง",
      })
    );
  });

  it("applies popular quick preset on click", () => {
    const value: SpecSelection = {};
    const onChange = vi.fn();

    render(<FieldSurveyCatalogPicker value={value} onChange={onChange} />);

    // Click quick preset chip
    const presetBtn = screen.getByRole("button", { name: "ไม้อัดยาง 15mm + ปิดผิวลามิเนต" });
    fireEvent.click(presetBtn);

    expect(onChange).toHaveBeenCalledWith(
      expect.objectContaining({
        coreSkuName: "ไม้อัดยาง 15 มม. เกรดเฟอร์นิเจอร์",
        finishSkuName: "แผ่นลามิเนต HPL ผิวสัมผัสด้าน",
      })
    );
  });
});
