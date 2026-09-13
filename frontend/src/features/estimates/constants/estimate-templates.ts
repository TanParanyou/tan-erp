import type { WorkItemFormData } from "../schemas/estimate-workspace-schema";

export interface TemplateCostComponent {
  type: "material" | "labor" | "subcontract" | "equipment" | "overhead" | "other";
  description: string;
  unitCost: number;
}

export interface EstimateTemplateItem {
  id: string;
  titleKey: string;
  itemCode: string;
  itemDescTh: string;
  itemDescEn: string;
  quantity: number;
  unitCode: string;
  sellingRule: "margin" | "markup" | "fixed";
  sellingRuleValue: number;
  costComponents: TemplateCostComponent[];
}

export const ESTIMATE_TEMPLATES: readonly EstimateTemplateItem[] = [
  {
    id: "wardrobe",
    titleKey: "templateCategories.wardrobe",
    itemCode: "WD-01",
    itemDescTh: "ตู้เสื้อผ้า Walk-in Closet บานทึบ-โปร่ง H2400 (โครง HMR 18mm)",
    itemDescEn: "Walk-in Closet Wardrobe H2400 (18mm HMR Core + Blum Hardware)",
    quantity: 1,
    unitCode: "set",
    sellingRule: "margin",
    sellingRuleValue: 35,
    costComponents: [
      {
        type: "material",
        description: "ไม้อัด HMR 18mm ชนิดทนความชื้นพิเศษเกรด E1",
        unitCost: 8500,
      },
      {
        type: "material",
        description: "แผ่นลามิเนต Formica เกรดทนรอยขีดข่วน พร้อมกาวพ่น",
        unitCost: 4200,
      },
      {
        type: "material",
        description: "รางลิ้นชักรับใต้ Blum Tandembox & บานพับ Clip-top Soft-close",
        unitCost: 5800,
      },
      {
        type: "labor",
        description: "ค่าแรงช่างไม้ประกอบโครงตู้และติดตั้งหน้างาน",
        unitCost: 6000,
      },
      {
        type: "subcontract",
        description: "งานทำสีพ่นอุตสาหกรรม PU เคลือบเงา 50%",
        unitCost: 2500,
      },
      {
        type: "material",
        description: "รางไฟอลูมิเนียม LED Warm White พร้อมสวิตช์เซ็นเซอร์บานเปิด",
        unitCost: 1800,
      },
    ],
  },
  {
    id: "kitchen",
    titleKey: "templateCategories.kitchen",
    itemCode: "KT-01",
    itemDescTh: "เคาน์เตอร์ครัวล่าง & เกาะกลาง Island พร้อมท็อปหินควอตซ์",
    itemDescEn: "Kitchen Base Cabinet & Island with Quartz Stone Countertop",
    quantity: 1,
    unitCode: "set",
    sellingRule: "margin",
    sellingRuleValue: 35,
    costComponents: [
      {
        type: "material",
        description: "โครงตู้ไม้อัด HMR 18mm กันชื้นปิดผิวนอก-ในเมลามีน",
        unitCost: 12000,
      },
      {
        type: "material",
        description: "ท็อปหินควอตซ์สังเคราะห์หนา 20mm ทนกรดด่างและรอยขูดขีด",
        unitCost: 18500,
      },
      {
        type: "material",
        description: "ชุดตะแกรงดึงสแตนเลส 304 ใส่เครื่องปรุง & จานชาม Hafele",
        unitCost: 4800,
      },
      {
        type: "material",
        description: "อ่างล้างจานสแตนเลสหลุมเดี่ยวฝังใต้หินพร้อมก๊อกน้ำดัดงอ",
        unitCost: 6500,
      },
      {
        type: "labor",
        description: "ค่าแรงช่างประกอบ ติดตั้งตู้ และวางระบบท่อน้ำดี-น้ำทิ้ง",
        unitCost: 7500,
      },
      {
        type: "subcontract",
        description: "ค่าแรงช่างหินตัด เจาะช่องอ่าง-เตา และเจียรลบมุมบัวหน้างาน",
        unitCost: 3500,
      },
    ],
  },
  {
    id: "wall",
    titleKey: "templateCategories.wall",
    itemCode: "WL-01",
    itemDescTh: "ผนังตกแต่งกรุระแนงไม้สักแท้เซาะร่อง พร้อมหลืบไฟซ่อน LED",
    itemDescEn: "Fluted Teak Accent Wall with Concealed LED Cove Lighting",
    quantity: 1,
    unitCode: "sqm",
    sellingRule: "margin",
    sellingRuleValue: 32,
    costComponents: [
      {
        type: "material",
        description: "โครงเคร่าไม้ยางพาราอบน้ำยากันปลวก + ไม้อัดรองหลัง 9mm",
        unitCost: 3200,
      },
      {
        type: "material",
        description: "ไม้ระแนงไม้สักแท้เกรดคัดลายเซาะร่อง 25x25mm",
        unitCost: 6800,
      },
      {
        type: "material",
        description: "รางไฟ LED Strip 24V 3000K พร้อมเพาเวอร์ซัพพลาย MeanWell",
        unitCost: 2200,
      },
      {
        type: "labor",
        description: "ค่าแรงช่างไม้ติดตั้งโครงสร้างและยิงระแนงจับระดับเลเซอร์",
        unitCost: 4000,
      },
      {
        type: "labor",
        description: "งานย้อมสีไม้สักธรรมชาติและพ่นเคลือบด้านสูตรน้ำไร้กลิ่น",
        unitCost: 2000,
      },
    ],
  },
  {
    id: "tv",
    titleKey: "templateCategories.tv",
    itemCode: "TV-01",
    itemDescTh: "ชั้นวางทีวีแขวนลอยพร้อมตู้โชว์บานกระจกชาทองกรอบอลูมิเนียม Slim",
    itemDescEn: "Floating TV Console & Tinted Glass Display with Slim Aluminum Frame",
    quantity: 1,
    unitCode: "set",
    sellingRule: "margin",
    sellingRuleValue: 35,
    costComponents: [
      {
        type: "material",
        description: "โครงตู้ลอย HMR 18mm กรุลามิเนตลายหินอ่อนตัดขอบดำด้าน",
        unitCost: 7200,
      },
      {
        type: "material",
        description: "หน้าบานกรอบอลูมิเนียม Profile สีดำด้าน + กระจกนิรภัยสีชาทอง 5mm",
        unitCost: 9500,
      },
      {
        type: "material",
        description: "ชุดไฟตู้โชว์ LED Spot & Linear warm light ซ่อนหลังสันกระจก",
        unitCost: 1900,
      },
      {
        type: "labor",
        description: "ค่าแรงประกอบ ติดตั้งเหล็กแขวนรับน้ำหนักตู้ทีวี และเก็บสายไฟ",
        unitCost: 4500,
      },
    ],
  },
  {
    id: "curtain",
    titleKey: "templateCategories.curtain",
    itemCode: "CT-01",
    itemDescTh: "ผ้าม่านลอน 2 ชั้น (ทึบ Blackout 100% + โปร่ง Sheer) พร้อมรางไฟฟ้า",
    itemDescEn: "Double Ripple Fold Curtains (100% Blackout + Sheer) with Motorized Track",
    quantity: 1,
    unitCode: "set",
    sellingRule: "margin",
    sellingRuleValue: 30,
    costComponents: [
      {
        type: "material",
        description: "ผ้าม่านทึบ Blackout เกรดกันแดดและกันความร้อน 100% กว้าง 3.5m",
        unitCost: 5400,
      },
      {
        type: "material",
        description: "ผ้าม่านโปร่งเนื้อละเอียดนำเข้า กรองแสงนุ่มนวล กว้าง 3.5m",
        unitCost: 2800,
      },
      {
        type: "equipment",
        description: "ชุดมอเตอร์ม่านไฟฟ้า Smart Motor (Tuya/Zigbee) พร้อมรางคู่ 3.5m",
        unitCost: 6500,
      },
      {
        type: "labor",
        description: "ค่าแรงช่างเย็บม่าน ติดตั้งรางไฟฟ้า และตั้งค่ารีโมท/แอปพลิเคชัน",
        unitCost: 1800,
      },
    ],
  },
];

export interface QuickCostPreset {
  id: string;
  labelKey: string;
  costType: "material" | "labor" | "subcontract" | "equipment" | "overhead" | "other";
  defaultDesc: string;
  defaultCost: number;
}

export const QUICK_COST_PRESETS: readonly QuickCostPreset[] = [
  {
    id: "hmr",
    labelKey: "quickCosts.hmr",
    costType: "material",
    defaultDesc: "ไม้อัด HMR 18mm กันชื้น",
    defaultCost: 850,
  },
  {
    id: "laminate",
    labelKey: "quickCosts.laminate",
    costType: "material",
    defaultDesc: "ลามิเนต Formica เกรดทนรอย",
    defaultCost: 1400,
  },
  {
    id: "fitting",
    labelKey: "quickCosts.fitting",
    costType: "material",
    defaultDesc: "บานพับถ้วย Blum Soft-close (คู่)",
    defaultCost: 350,
  },
  {
    id: "carpentry",
    labelKey: "quickCosts.carpentry",
    costType: "labor",
    defaultDesc: "ค่าแรงช่างไม้ประกอบติดตั้ง (ตร.ม.)",
    defaultCost: 1500,
  },
  {
    id: "paint",
    labelKey: "quickCosts.paint",
    costType: "subcontract",
    defaultDesc: "ค่าสีพ่นอุตสาหกรรม PU เคลือบเงา",
    defaultCost: 2000,
  },
  {
    id: "electric",
    labelKey: "quickCosts.electric",
    costType: "material",
    defaultDesc: "รางไฟ LED Strip 3000K พร้อมหม้อแปลง",
    defaultCost: 1200,
  },
];

/**
 * Helper to convert an EstimateTemplateItem into a ready-to-insert WorkItemFormData
 */
export function createWorkItemFromTemplate(
  template: EstimateTemplateItem,
  generateId: () => string = () => Math.random().toString(36).substring(2, 9),
  currency: string = "THB"
): WorkItemFormData {
  return {
    id: generateId(),
    code: template.itemCode,
    descriptionTh: template.itemDescTh,
    descriptionEn: template.itemDescEn,
    quantity: template.quantity,
    unitCode: template.unitCode,
    sellingRuleType: template.sellingRule,
    sellingRuleValue: template.sellingRuleValue,
    sortOrder: 1,
    costComponents: template.costComponents.map((c, idx) => ({
      id: generateId(),
      type: c.type,
      description: c.description,
      quantity: 1,
      unitCode: "lot",
      unitCost: c.unitCost,
      currency,
      sortOrder: idx + 1,
    })),
  };
}
