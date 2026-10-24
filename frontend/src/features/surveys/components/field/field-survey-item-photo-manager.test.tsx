import React from "react";
import { describe, it, expect, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { FieldSurveyItemPhotoManager } from "./field-survey-item-photo-manager";
import type { FieldSurveyPhoto } from "../../schemas/field-survey-schema";

vi.mock("next-intl", () => ({
  useTranslations: () => (key: string) => {
    const translations: Record<string, string> = {
      title: "ภาพถ่ายและระยะสเก็ตช์",
      subtitle: "แนบภาพถ่ายหน้างานจริง สเก็ตช์ระยะ หรือตัวอย่างวัสดุเพื่อความแม่นยำ",
      uploadBtn: "ถ่ายภาพ / เลือกไฟล์",
      tagSiteLocation: "ภาพตำแหน่งหน้างาน",
      tagSketchLaser: "ภาพสเก็ตช์ / หน้าจอเลเซอร์",
      tagMaterialSwatch: "ภาพตัวอย่างวัสดุ / ลวดลาย",
      tagConstraintDefect: "ภาพอุปสรรค / จุดชำรุด",
      captionPlaceholder: "คำอธิบายภาพ (เช่น ริมหน้าต่างทิศใต้)",
      deletePhoto: "ลบภาพนี้",
      empty: "ยังไม่มีภาพแนบสำหรับรายการนี้",
    };
    return translations[key] ?? key;
  },
}));

vi.mock("../../api/survey-queries", () => ({
  useSurveyReferenceData: () => ({
    data: {
      categories: [],
      photoTags: [
        { code: "site_photo", name: { thai: "ภาพตำแหน่งหน้างาน", english: "Site Location" } },
        { code: "measurement_sketch", name: { thai: "ภาพสเก็ตช์ / หน้าจอเลเซอร์", english: "Sketch / Laser Display" } },
        { code: "material_swatch", name: { thai: "ภาพตัวอย่างวัสดุ / ลวดลาย", english: "Material / Swatch" } },
        { code: "constraint_defect", name: { thai: "ภาพอุปสรรค / จุดชำรุด", english: "Constraint / Defect" } },
      ],
      measurementTypes: [],
    },
    isLoading: false,
  }),
}));

describe("FieldSurveyItemPhotoManager Component", () => {
  it("renders empty state when no photos are provided", () => {
    const photos: FieldSurveyPhoto[] = [];
    const onChange = vi.fn();

    render(<FieldSurveyItemPhotoManager photos={photos} onChange={onChange} />);

    expect(screen.getByText("ภาพถ่ายและระยะสเก็ตช์")).toBeDefined();
    expect(screen.getByText("ยังไม่มีภาพแนบสำหรับรายการนี้")).toBeDefined();
    expect(screen.getByRole("button", { name: /ถ่ายภาพ \/ เลือกไฟล์/ })).toBeDefined();
  });

  it("renders list of attached photos with tag pills and caption inputs", () => {
    const photos: FieldSurveyPhoto[] = [
      {
        id: "p1",
        url: "blob:http://localhost/mock-photo-1",
        fileName: "desk-loc.jpg",
        tag: "site_location",
        caption: "ริมหน้าต่าง",
      },
    ];
    const onChange = vi.fn();

    render(<FieldSurveyItemPhotoManager photos={photos} onChange={onChange} />);

    expect(screen.getByDisplayValue("ริมหน้าต่าง")).toBeDefined();
    expect(screen.getByRole("button", { name: "ภาพตำแหน่งหน้างาน" })).toBeDefined();
  });

  it("updates caption when typing in caption input", () => {
    const photos: FieldSurveyPhoto[] = [
      {
        id: "p1",
        url: "blob:http://localhost/mock-photo-1",
        fileName: "desk-loc.jpg",
        tag: "site_location",
        caption: "ริมหน้าต่าง",
      },
    ];
    const onChange = vi.fn();

    render(<FieldSurveyItemPhotoManager photos={photos} onChange={onChange} />);

    const input = screen.getByDisplayValue("ริมหน้าต่าง");
    fireEvent.change(input, { target: { value: "ริมหน้าต่าง ทิศตะวันออก" } });

    expect(onChange).toHaveBeenCalledWith([
      expect.objectContaining({
        id: "p1",
        caption: "ริมหน้าต่าง ทิศตะวันออก",
      }),
    ]);
  });

  it("changes photo tag when selecting another tag", () => {
    const photos: FieldSurveyPhoto[] = [
      {
        id: "p1",
        url: "blob:http://localhost/mock-photo-1",
        fileName: "desk-loc.jpg",
        tag: "site_location",
      },
    ];
    const onChange = vi.fn();

    render(<FieldSurveyItemPhotoManager photos={photos} onChange={onChange} />);

    const sketchTagBtn = screen.getByRole("button", { name: "ภาพสเก็ตช์ / หน้าจอเลเซอร์" });
    fireEvent.click(sketchTagBtn);

    expect(onChange).toHaveBeenCalledWith([
      expect.objectContaining({
        id: "p1",
        tag: "measurement_sketch",
      }),
    ]);
  });

  it("removes photo when clicking delete button", () => {
    const photos: FieldSurveyPhoto[] = [
      {
        id: "p1",
        url: "blob:http://localhost/mock-photo-1",
        fileName: "desk-loc.jpg",
        tag: "site_location",
      },
    ];
    const onChange = vi.fn();

    render(<FieldSurveyItemPhotoManager photos={photos} onChange={onChange} />);

    const deleteBtn = screen.getByRole("button", { name: "ลบภาพนี้" });
    fireEvent.click(deleteBtn);

    expect(onChange).toHaveBeenCalledWith([]);
  });
});
