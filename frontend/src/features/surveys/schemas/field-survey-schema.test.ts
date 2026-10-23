import { describe, it, expect } from "vitest";
import {
  fieldSurveyItemSchema,
  fieldSurveyFormSchema,
  FIELD_SURVEY_PHOTO_TAGS,
  FIELD_SURVEY_MEASUREMENT_TYPES,
  calculateNetAreaSqm,
} from "./field-survey-schema";

describe("field-survey-schema", () => {
  it("validates a box measurement item correctly", () => {
    const validBoxItem = {
      id: "item-1",
      title: "โต๊ะทำงานไม้สัก",
      category: "built_in",
      measurementType: "box",
      boxDimensions: {
        widthMm: 1200,
        depthMm: 600,
        heightMm: 750,
      },
      specSelection: {
        coreSkuId: "sku-teak-plywood",
        coreSkuName: "ไม้อัดสัก 15 มม. เกรด A",
        finishSkuId: "sku-matte-polyurethane",
        finishSkuName: "พ่น PU ด้าน 50%",
        customNotes: "ขอบมน 5 มม. ไม่เอาเสี้ยน",
      },
      photos: [
        {
          id: "photo-1",
          url: "https://example.com/site1.jpg",
          fileName: "site1.jpg",
          tag: "site_location",
          caption: "ตำแหน่งวางโต๊ะริมหน้าต่าง",
        },
      ],
    };

    const result = fieldSurveyItemSchema.safeParse(validBoxItem);
    expect(result.success).toBe(true);
  });

  it("validates an area measurement item with deductions correctly", () => {
    const validAreaItem = {
      id: "item-2",
      title: "ปูพื้นหินอ่อนสีเทาลาย",
      category: "flooring",
      measurementType: "area",
      areaDimensions: {
        grossWidthMm: 4000,
        grossLengthMm: 5000,
        deductions: [
          { label: "เว้าเสาโครงสร้าง", areaSqm: 0.8 },
          { label: "ช่องเปิดบันได", areaSqm: 1.2 },
        ],
      },
      specSelection: {
        finishSkuId: "sku-marble-gray-1",
        finishSkuName: "หินอ่อน Grigio Orobico 60x60",
      },
      photos: [],
    };

    const result = fieldSurveyItemSchema.safeParse(validAreaItem);
    expect(result.success).toBe(true);
  });

  it("validates custom multi-point measurements correctly", () => {
    const validCustomItem = {
      id: "item-3",
      title: "ผ้าม่าน 2 ชั้นพร้อมหลืบม่าน",
      category: "curtain",
      measurementType: "custom",
      customMeasurements: [
        { label: "ความกว้างกรอบหน้าต่าง", valueMm: 2800 },
        { label: "ความสูงจากพื้นถึงฝ้า", valueMm: 2600 },
        { label: "ความลึกหลืบม่านซ่อนไฟ", valueMm: 220, notes: "เว้นสายไฟ LED 12V" },
      ],
      specSelection: {
        coreSkuName: "รางม่านไฟฟ้าระบบ Zigbee",
        finishSkuName: "ผ้าโปร่ง Dim-out + ผ้าทึบกัน UV 100%",
      },
      photos: [
        {
          id: "photo-sketch",
          url: "https://example.com/laser-sketch.jpg",
          fileName: "laser-sketch.jpg",
          tag: "sketch_laser",
          caption: "ระยะวัดจากเครื่องวัดเลเซอร์",
        },
      ],
    };

    const result = fieldSurveyItemSchema.safeParse(validCustomItem);
    expect(result.success).toBe(true);
  });

  it("calculates net area accurately deducting all openings", () => {
    const grossWidthMm = 4000;
    const grossLengthMm = 5000; // 20.00 sqm
    const deductions = [
      { label: "เสา", areaSqm: 0.5 },
      { label: "ช่องเปิด", areaSqm: 1.5 },
    ];

    const netArea = calculateNetAreaSqm(grossWidthMm, grossLengthMm, deductions);
    expect(netArea).toBe(18.0);
  });

  it("fails validation when item title is empty", () => {
    const invalidItem = {
      id: "item-4",
      title: "",
      measurementType: "box",
      boxDimensions: {
        widthMm: 100,
        depthMm: 100,
        heightMm: 100,
      },
    };

    const result = fieldSurveyItemSchema.safeParse(invalidItem);
    expect(result.success).toBe(false);
  });

  it("validates entire field survey form containing multiple items", () => {
    const validForm = {
      opportunityId: "a0000000-0000-4000-a000-000000000001",
      roomName: "ห้องนอนใหญ่ Master Bedroom",
      surveyorNotes: "ลูกค้าต้องการโทน Modern Classic เฟอร์นิเจอร์ทั้งหมดสั่งทำเฉพาะขนาด",
      items: [
        {
          id: "item-1",
          title: "โต๊ะทำงานไม้สัก",
          measurementType: "box",
          boxDimensions: { widthMm: 1200, depthMm: 600, heightMm: 750 },
        },
      ],
    };

    const result = fieldSurveyFormSchema.safeParse(validForm);
    expect(result.success).toBe(true);
  });

  it("exports valid enum lists for photo tags and measurement types", () => {
    expect(FIELD_SURVEY_PHOTO_TAGS).toContain("site_location");
    expect(FIELD_SURVEY_PHOTO_TAGS).toContain("sketch_laser");
    expect(FIELD_SURVEY_PHOTO_TAGS).toContain("material_swatch");

    expect(FIELD_SURVEY_MEASUREMENT_TYPES).toContain("box");
    expect(FIELD_SURVEY_MEASUREMENT_TYPES).toContain("area");
    expect(FIELD_SURVEY_MEASUREMENT_TYPES).toContain("custom");
  });
});
