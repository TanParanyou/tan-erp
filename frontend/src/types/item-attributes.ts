/**
 * Item Attributes Domain Types & Definitions
 * Production-Grade Schema for Architectural & Woodworking ERP
 */

export interface ItemAttributePair {
  key: string;
  value: string;
}

export type ItemAttributesRecord = Record<string, string>;

export type AttributeDataType = "text" | "number" | "select" | "boolean";

export interface AttributeDefinition {
  key: string;
  labelTh: string;
  labelEn: string;
  dataType: AttributeDataType;
  unit?: string;
  isRequired?: boolean;
  defaultValue?: string;
  placeholderTh?: string;
  placeholderEn?: string;
  options?: Array<{ value: string; labelTh: string; labelEn: string }>;
  suggestedCategories?: string[]; // category code prefixes or item types
}

import type { components } from "@/generated/api/tan-erp.v1";

export type CategoryAttributeTemplateResponse = components["schemas"]["CategoryAttributeTemplateResponse"];
export type CategoryAttributeTemplateDto = components["schemas"]["CategoryAttributeTemplateDto"];
export type CategoryAttributeOptionDto = components["schemas"]["CategoryAttributeOptionDto"];

/**
 * Reserved attribute keys blocked by Backend Domain (TanErp.Domain.Items.Item)
 */
export const RESERVED_ATTRIBUTE_KEYS = [
  "price",
  "status",
  "permission",
  "currency",
  "unitcost",
  "cost",
] as const;

export type ReservedAttributeKey = (typeof RESERVED_ATTRIBUTE_KEYS)[number];

/**
 * Controlled Standard Attribute Registry (Production-Grade Specs)
 */
export const CONTROLLED_ATTRIBUTE_REGISTRY: AttributeDefinition[] = [
  {
    key: "thickness_mm",
    labelTh: "ความหนา (มม.)",
    labelEn: "Thickness (mm)",
    dataType: "number",
    unit: "มม. (mm)",
    placeholderTh: "เช่น 18",
    placeholderEn: "e.g. 18",
    suggestedCategories: ["MAT", "material"],
    options: [
      { value: "4", labelTh: "4 มม.", labelEn: "4 mm" },
      { value: "6", labelTh: "6 มม.", labelEn: "6 mm" },
      { value: "9", labelTh: "9 มม.", labelEn: "9 mm" },
      { value: "12", labelTh: "12 มม.", labelEn: "12 mm" },
      { value: "15", labelTh: "15 มม.", labelEn: "15 mm" },
      { value: "18", labelTh: "18 มม.", labelEn: "18 mm" },
      { value: "20", labelTh: "20 มม.", labelEn: "20 mm" },
      { value: "25", labelTh: "25 มม.", labelEn: "25 mm" },
    ],
  },
  {
    key: "width_mm",
    labelTh: "ความกว้าง",
    labelEn: "Width",
    dataType: "number",
    unit: "มม. (mm)",
    placeholderTh: "เช่น 1220",
    placeholderEn: "e.g. 1220",
    suggestedCategories: ["MAT", "material", "product"],
    options: [
      { value: "600", labelTh: "600 มม.", labelEn: "600 mm" },
      { value: "800", labelTh: "800 มม.", labelEn: "800 mm" },
      { value: "1200", labelTh: "1200 มม.", labelEn: "1200 mm" },
      { value: "1220", labelTh: "1220 มม. (มาตรฐาน)", labelEn: "1220 mm (Standard)" },
    ],
  },
  {
    key: "length_mm",
    labelTh: "ความยาว / ความลึก",
    labelEn: "Length / Depth",
    dataType: "number",
    unit: "มม. (mm)",
    placeholderTh: "เช่น 2440",
    placeholderEn: "e.g. 2440",
    suggestedCategories: ["MAT", "material", "product"],
    options: [
      { value: "1200", labelTh: "1200 มม.", labelEn: "1200 mm" },
      { value: "2400", labelTh: "2400 มม.", labelEn: "2400 mm" },
      { value: "2440", labelTh: "2440 มม. (มาตรฐาน)", labelEn: "2440 mm (Standard)" },
      { value: "3000", labelTh: "3000 มม.", labelEn: "3000 mm" },
    ],
  },
  {
    key: "material",
    labelTh: "วัสดุหลัก",
    labelEn: "Primary Material",
    dataType: "select",
    placeholderTh: "เลือกวัสดุหลัก",
    placeholderEn: "Select material",
    suggestedCategories: ["MAT", "material", "product"],
    options: [
      { value: "plywood", labelTh: "ไม้อัดยาง (Plywood)", labelEn: "Plywood" },
      { value: "mdf", labelTh: "ไม้ MDF", labelEn: "MDF" },
      { value: "hmr", labelTh: "ไม้ HMR (ทนชื้นเขียว)", labelEn: "HMR" },
      { value: "particle_board", labelTh: "ปาติเกิลบอร์ด", labelEn: "Particle Board" },
      { value: "solid_wood", labelTh: "ไม้จริง / ไม้ประสาน", labelEn: "Solid Wood" },
      { value: "aluminum", labelTh: "อลูมิเนียม", labelEn: "Aluminum" },
      { value: "stainless_steel", labelTh: "สแตนเลสสตีล", labelEn: "Stainless Steel" },
      { value: "glass", labelTh: "กระจก", labelEn: "Glass" },
      { value: "acrylic", labelTh: "อะคริลิก", labelEn: "Acrylic" },
    ],
  },
  {
    key: "color",
    labelTh: "สี",
    labelEn: "Color",
    dataType: "select",
    placeholderTh: "เลือกโทนสี",
    placeholderEn: "Select color",
    suggestedCategories: ["MAT", "material", "product"],
    options: [
      { value: "white_matte", labelTh: "ขาวด้าน (Matte White)", labelEn: "Matte White" },
      { value: "white_gloss", labelTh: "ขาวเงา (High Gloss White)", labelEn: "Gloss White" },
      { value: "black_matte", labelTh: "ดำด้าน (Matte Black)", labelEn: "Matte Black" },
      { value: "grey_warm", labelTh: "เทาโทนอุ่น (Warm Grey)", labelEn: "Warm Grey" },
      { value: "natural_oak", labelTh: "ลายไม้โอ๊คธรรมชาติ", labelEn: "Natural Oak" },
      { value: "natural_teak", labelTh: "ลายไม้สักธรรมชาติ", labelEn: "Natural Teak" },
      { value: "walnut", labelTh: "ลายไม้วอลนัท", labelEn: "Walnut" },
      { value: "raw_unfinished", labelTh: "สีเนื้อวัสดุดิบ (ไม่ทำสี)", labelEn: "Raw / Unfinished" },
    ],
  },
  {
    key: "finish",
    labelTh: "ผิวสัมผัส / ผิวเคลือบ",
    labelEn: "Finish / Surface",
    dataType: "select",
    placeholderTh: "เลือกลักษณะผิวเคลือบ",
    placeholderEn: "Select finish",
    suggestedCategories: ["MAT", "material", "product"],
    options: [
      { value: "raw", labelTh: "เปลือย / ไม่เคลือบ", labelEn: "Raw / Uncoated" },
      { value: "melamine", labelTh: "เคลือบเมลามีน (Melamine)", labelEn: "Melamine" },
      { value: "hpl_laminate", labelTh: "ปิดผิวลามิเนต (HPL)", labelEn: "HPL Laminate" },
      { value: "veneer", labelTh: "ปิดผิววีเนียร์ไม้จริง", labelEn: "Wood Veneer" },
      { value: "pu_paint", labelTh: "พ่นสีพียู (PU)", labelEn: "PU Paint" },
      { value: "powder_coat", labelTh: "พ่นสีพาวเดอร์โค้ท", labelEn: "Powder Coat" },
      { value: "anodized", labelTh: "ชุบอโนไดซ์ (Anodized)", labelEn: "Anodized" },
    ],
  },
  {
    key: "grade",
    labelTh: "เกรด / มาตรฐานสิ่งแวดล้อม",
    labelEn: "Grade / Environmental Rating",
    dataType: "select",
    placeholderTh: "เลือกเกรดหรือค่าฟอร์มาลดีไฮด์",
    placeholderEn: "Select grade",
    suggestedCategories: ["MAT", "material"],
    options: [
      { value: "e0", labelTh: "E0 (มาตรฐานสิ่งแวดล้อมสูงสุด)", labelEn: "E0" },
      { value: "e1", labelTh: "E1 (มาตรฐานปลอดภัยสากล)", labelEn: "E1" },
      { value: "e2", labelTh: "E2 (ทั่วไป)", labelEn: "E2" },
      { value: "carb_p2", labelTh: "CARB P2 (มาตรฐานส่งออก US)", labelEn: "CARB P2" },
      { value: "grade_a", labelTh: "เกรด A ไสเรียบสวยงาม", labelEn: "Grade A" },
      { value: "grade_b", labelTh: "เกรด B งานโครงสร้างภายใน", labelEn: "Grade B" },
    ],
  },
  {
    key: "surface",
    labelTh: "เท็กซ์เจอร์พื้นผิว",
    labelEn: "Surface Texture",
    dataType: "select",
    placeholderTh: "เลือกเท็กซ์เจอร์",
    placeholderEn: "Select texture",
    suggestedCategories: ["MAT", "material", "product"],
    options: [
      { value: "smooth", labelTh: "ผิวเรียบเนียน (Smooth)", labelEn: "Smooth" },
      { value: "woodgrain", labelTh: "ผิวเสี้ยนไม้ธรรมชาติ (Woodgrain)", labelEn: "Woodgrain" },
      { value: "textured_orange", labelTh: "ผิวส้ม / ผิวเปลือกไม้", labelEn: "Textured" },
      { value: "matte_soft", labelTh: "ผิวสัมผัสนุ่มด้าน (Soft Touch)", labelEn: "Soft Touch Matte" },
    ],
  },
  {
    key: "fixture",
    labelTh: "ตำแหน่งติดตั้ง / สเปกเฟอร์นิเจอร์",
    labelEn: "Fixture Type",
    dataType: "text",
    placeholderTh: "เช่น ตู้ลอย, เคาน์เตอร์, งานผนัง",
    placeholderEn: "e.g. Wall Cabinet, Counter",
    suggestedCategories: ["product", "service", "subcontract"],
  },
  {
    key: "variant",
    labelTh: "รุ่นย่อย / ตัวเลือกเสริม",
    labelEn: "Variant / Model Option",
    dataType: "text",
    placeholderTh: "เช่น แบบมีบานเลื่อน, มีกุญแจ",
    placeholderEn: "e.g. Sliding Door, Keyed",
    suggestedCategories: ["product", "material"],
  },
];
