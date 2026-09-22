export type ItemCategoryType =
  | "wood"
  | "fitting"
  | "surface"
  | "paint"
  | "electric"
  | "stone"
  | "steel"
  | "tile"
  | "labor_service";

export type CostNatureType =
  | "material"
  | "labor"
  | "subcontract"
  | "equipment"
  | "overhead"
  | "other";

export interface LocalizedString {
  th: string;
  en: string;
}

export interface CatalogSubCategory {
  id: string;
  name: LocalizedString;
}

export interface CatalogCategory {
  id: string;
  name: LocalizedString;
  subCategories?: CatalogSubCategory[];
}

export interface CatalogBrand {
  id: string;
  name: LocalizedString;
}

export interface CatalogSupplier {
  id: string;
  code: string;
  name: LocalizedString;
}

export interface CatalogSpecs {
  thickness?: string;
  grade?: string;
  finish?: string;
  dimensions?: string;
  standard?: string;
  color?: string;
}

export interface CatalogPricing {
  defaultUnitCost: number;
  currency: string;
  baseUnitCode: string;
  costRecordId?: string;
  costRecordVersion?: number;
}

export interface CatalogCapabilities {
  canCost: boolean;
  canSell: boolean;
  canPurchase?: boolean;
  canStock?: boolean;
}

export interface CatalogItem {
  id: string;
  code: string;
  name: LocalizedString;
  aliases?: LocalizedString[];
  description?: LocalizedString;
  itemType: CostNatureType;
  category: CatalogCategory;
  subCategory?: CatalogSubCategory;
  brand: CatalogBrand;
  supplier?: CatalogSupplier;
  imageUrl?: string;
  images?: string[];
  primaryImageFileId?: string;
  status: "active" | "inactive" | "phase_out";
  specs?: CatalogSpecs;
  attributes?: Record<string, string>;
  pricing: CatalogPricing;
  capabilities: CatalogCapabilities;
}

export interface CatalogFilterCriteria {
  search?: string;
  scope?: "all" | "cost" | "sell";
  itemTypes?: string[];
  category?: string;
  subCategory?: string;
  thicknesses?: string[];
  brands?: string[];
  suppliers?: string[];
  status?: string;
  unitCodes?: string[];
  attributes?: Record<string, string>;
}

export const ESTIMATE_CATALOG_ITEMS: readonly CatalogItem[] = [
  // 1. Wood & Boards (ไม้และแผ่นบอร์ด)
  {
    id: "cat-wd-hmr-18",
    code: "MAT-WD-HMR-18",
    name: {
      th: "ไม้อัด HMR 18 มม. ชนิดทนความชื้นพิเศษเกรด E1",
      en: "18mm Moisture Resistant HMR Board (E1)",
    },
    aliases: [
      { th: "ไม้เขียว", en: "Green HMR Board" },
      { th: "ไม้อัดกันชื้น 18 มม.", en: "Moisture Proof MDF 18mm" },
    ],
    description: {
      th: "แผ่นใยไม้อัดทนความชื้นสูง V313 เหมาะสำหรับงานโครงตู้บิวต์อิน ห้องครัว และห้องน้ำ",
      en: "High moisture resistance board (V313) ideal for kitchen & bathroom built-in structures",
    },
    itemType: "material",
    category: {
      id: "wood",
      name: { th: "ไม้และแผ่นบอร์ด", en: "Wood & Boards" },
    },
    subCategory: {
      id: "hmr",
      name: { th: "ไม้ HMR กันชื้น", en: "HMR Boards" },
    },
    brand: {
      id: "vanachai",
      name: { th: "วนชัย", en: "Vanachai" },
    },
    supplier: {
      id: "sup-wood-plus",
      code: "SUP-001",
      name: { th: "บจก. วู๊ดพลัส ดิสทริบิวชั่น", en: "Wood Plus Distribution Co., Ltd." },
    },
    imageUrl: "https://images.unsplash.com/photo-1538688525198-9b88f6f53126?w=600&auto=format&fit=crop&q=80",
    images: [
      "https://images.unsplash.com/photo-1538688525198-9b88f6f53126?w=600&auto=format&fit=crop&q=80",
      "https://images.unsplash.com/photo-1513694203232-719a280e022f?w=600&auto=format&fit=crop&q=80",
      "https://images.unsplash.com/photo-1586023492125-27b2c045efd7?w=600&auto=format&fit=crop&q=80",
    ],
    status: "active",
    specs: {
      thickness: "18mm",
      grade: "E1 Moisture Resistant (V313)",
      dimensions: "1220x2440mm",
      standard: "TIS 876-2547",
    },
    attributes: {
      thickness: "18mm",
      dimensions: "1220x2440mm",
      grade: "E1",
      standard: "TIS",
    },
    pricing: {
      defaultUnitCost: 850,
      currency: "THB",
      baseUnitCode: "sheet",
    },
    capabilities: {
      canCost: true,
      canSell: false,
      canPurchase: true,
      canStock: true,
    },
  },
  {
    id: "cat-wd-hmr-15",
    code: "MAT-WD-HMR-15",
    name: {
      th: "ไม้อัด HMR 15 มม. ชนิดทนความชื้นพิเศษเกรด E1",
      en: "15mm Moisture Resistant HMR Board (E1)",
    },
    aliases: [
      { th: "ไม้เขียว 15 มม.", en: "Green Board 15mm" },
    ],
    itemType: "material",
    category: {
      id: "wood",
      name: { th: "ไม้และแผ่นบอร์ด", en: "Wood & Boards" },
    },
    subCategory: {
      id: "hmr",
      name: { th: "ไม้ HMR กันชื้น", en: "HMR Boards" },
    },
    brand: {
      id: "vanachai",
      name: { th: "วนชัย", en: "Vanachai" },
    },
    supplier: {
      id: "sup-wood-plus",
      code: "SUP-001",
      name: { th: "บจก. วู๊ดพลัส ดิสทริบิวชั่น", en: "Wood Plus Distribution Co., Ltd." },
    },
    status: "active",
    specs: {
      thickness: "15mm",
      grade: "E1 Moisture Resistant",
      dimensions: "1220x2440mm",
      standard: "TIS 876-2547",
    },
    attributes: {
      thickness: "15mm",
      dimensions: "1220x2440mm",
      grade: "E1",
      standard: "TIS",
    },
    pricing: {
      defaultUnitCost: 720,
      currency: "THB",
      baseUnitCode: "sheet",
    },
    capabilities: {
      canCost: true,
      canSell: false,
      canPurchase: true,
    },
  },
  {
    id: "cat-wd-ply-09",
    code: "MAT-WD-PLY-09",
    name: {
      th: "ไม้อัดยางรองหลัง 9 มม. ไส้แน่นเกรดงานเฟอร์นิเจอร์",
      en: "9mm Plywood Backing Board Furniture Grade",
    },
    aliases: [
      { th: "ไม้อัดหลังตู้", en: "Backing Plywood" },
    ],
    itemType: "material",
    category: {
      id: "wood",
      name: { th: "ไม้และแผ่นบอร์ด", en: "Wood & Boards" },
    },
    subCategory: {
      id: "plywood",
      name: { th: "ไม้อัดยาง", en: "Plywood" },
    },
    brand: {
      id: "metro",
      name: { th: "เมโทรพาเนล", en: "Metro" },
    },
    supplier: {
      id: "sup-metro-timber",
      code: "SUP-002",
      name: { th: "บจก. สยามวู้ดพาเนล", en: "Siam Wood Panel Co., Ltd." },
    },
    status: "active",
    specs: {
      thickness: "9mm",
      grade: "Furniture Grade",
      dimensions: "1220x2440mm",
    },
    attributes: {
      thickness: "9mm",
      dimensions: "1220x2440mm",
      grade: "Furniture",
    },
    pricing: {
      defaultUnitCost: 380,
      currency: "THB",
      baseUnitCode: "sheet",
    },
    capabilities: {
      canCost: true,
      canSell: false,
      canPurchase: true,
    },
  },
  {
    id: "cat-wd-mdf-18",
    code: "MAT-WD-MDF-18",
    name: {
      th: "แผ่นไม้ MDF 18 มม. ผิวเรียบเนียนเกรดงานพ่นสี",
      en: "18mm Medium Density Fiberboard (MDF)",
    },
    aliases: [
      { th: "ไม้เอ็มดีเอฟ", en: "MDF Board" },
    ],
    itemType: "material",
    category: {
      id: "wood",
      name: { th: "ไม้และแผ่นบอร์ด", en: "Wood & Boards" },
    },
    subCategory: {
      id: "mdf",
      name: { th: "ไม้ MDF", en: "MDF Boards" },
    },
    brand: {
      id: "metro",
      name: { th: "เมโทรพาเนล", en: "Metro" },
    },
    supplier: {
      id: "sup-metro-timber",
      code: "SUP-002",
      name: { th: "บจก. สยามวู้ดพาเนล", en: "Siam Wood Panel Co., Ltd." },
    },
    status: "active",
    specs: {
      thickness: "18mm",
      grade: "Paint Grade",
      dimensions: "1220x2440mm",
    },
    attributes: {
      thickness: "18mm",
      dimensions: "1220x2440mm",
      grade: "Paint Grade",
    },
    pricing: {
      defaultUnitCost: 650,
      currency: "THB",
      baseUnitCode: "sheet",
    },
    capabilities: {
      canCost: true,
      canSell: false,
      canPurchase: true,
    },
  },

  // 2. Hardware & Fittings (ฟิตติ้งและฮาร์ดแวร์)
  {
    id: "cat-fit-blum-hinge",
    code: "MAT-FIT-BLUM-01",
    name: {
      th: "บานพับถ้วย Blum Clip-top Soft-close 110° (ทับขอบ)",
      en: "Blum Clip-top 110deg Full Overlay Soft-close Hinge",
    },
    aliases: [
      { th: "บานพับบลูม", en: "Blum Hinge" },
      { th: "บานพับซอฟต์โคลส", en: "Soft Close Hinge" },
    ],
    itemType: "material",
    category: {
      id: "fitting",
      name: { th: "ฟิตติ้งและฮาร์ดแวร์", en: "Hardware & Fittings" },
    },
    subCategory: {
      id: "hinge",
      name: { th: "บานพับ", en: "Hinges" },
    },
    brand: {
      id: "blum",
      name: { th: "บลูม", en: "Blum" },
    },
    supplier: {
      id: "sup-modernform",
      code: "SUP-003",
      name: { th: "บมจ. โมเดอร์นฟอร์มกรุ๊ป", en: "Modernform Group Public Co., Ltd." },
    },
    status: "active",
    specs: {
      grade: "Clip-top 110deg Soft-close",
      finish: "Nickel Plated",
    },
    attributes: {
      finish: "Nickel",
      grade: "110deg Soft-close",
    },
    pricing: {
      defaultUnitCost: 350,
      currency: "THB",
      baseUnitCode: "pair",
    },
    capabilities: {
      canCost: true,
      canSell: false,
      canPurchase: true,
    },
  },
  {
    id: "cat-fit-blum-tandem",
    code: "MAT-FIT-BLUM-02",
    name: {
      th: "ชุดรางลิ้นชัก Blum Tandembox Antaro 500 มม. Soft-close",
      en: "Blum Tandembox Antaro Drawer Runner 500mm",
    },
    aliases: [
      { th: "รางลิ้นชักรับใต้บลูม", en: "Blum Undermount Runner" },
      { th: "รางสไลด์ 50 ซม.", en: "Drawer Runner 50cm" },
    ],
    itemType: "material",
    category: {
      id: "fitting",
      name: { th: "ฟิตติ้งและฮาร์ดแวร์", en: "Hardware & Fittings" },
    },
    subCategory: {
      id: "runner",
      name: { th: "รางลิ้นชัก", en: "Drawer Runners" },
    },
    brand: {
      id: "blum",
      name: { th: "บลูม", en: "Blum" },
    },
    supplier: {
      id: "sup-modernform",
      code: "SUP-003",
      name: { th: "บมจ. โมเดอร์นฟอร์มกรุ๊ป", en: "Modernform Group Public Co., Ltd." },
    },
    status: "active",
    specs: {
      dimensions: "500mm",
      grade: "Antaro 30kg Soft-close",
      color: "Silk White",
    },
    attributes: {
      size: "500mm",
      color: "Silk White",
      grade: "30kg Soft-close",
    },
    pricing: {
      defaultUnitCost: 1850,
      currency: "THB",
      baseUnitCode: "set",
    },
    capabilities: {
      canCost: true,
      canSell: false,
      canPurchase: true,
    },
  },
  {
    id: "cat-fit-hafele-basket",
    code: "MAT-FIT-HAF-01",
    name: {
      th: "ชุดตะแกรงดึงสแตนเลส 304 ใส่เครื่องปรุง Hafele กว้าง 200 มม.",
      en: "Hafele 304 Stainless Pull-out Spice Basket 200mm",
    },
    aliases: [
      { th: "ตะแกรงขวดปรุงรส", en: "Hafele Spice Rack" },
    ],
    itemType: "material",
    category: {
      id: "fitting",
      name: { th: "ฟิตติ้งและฮาร์ดแวร์", en: "Hardware & Fittings" },
    },
    subCategory: {
      id: "basket",
      name: { th: "ตะแกรงและอุปกรณ์จัดเก็บ", en: "Storage Baskets" },
    },
    brand: {
      id: "hafele",
      name: { th: "เฮเฟเล่", en: "Hafele" },
    },
    supplier: {
      id: "sup-hafele-th",
      code: "SUP-004",
      name: { th: "บจก. เฮเฟเล่ (ประเทศไทย)", en: "Hafele (Thailand) Co., Ltd." },
    },
    status: "active",
    specs: {
      dimensions: "200mm",
      grade: "Stainless Steel 304",
      finish: "Polished Chrome",
    },
    attributes: {
      size: "200mm",
      finish: "Polished",
      grade: "SUS304",
    },
    pricing: {
      defaultUnitCost: 3200,
      currency: "THB",
      baseUnitCode: "set",
    },
    capabilities: {
      canCost: true,
      canSell: false,
      canPurchase: true,
    },
  },

  // 3. Surfaces & Laminates (วัสดุปิดผิวและลามิเนต)
  {
    id: "cat-sur-formica-matte",
    code: "MAT-SUR-FORM-01",
    name: {
      th: "แผ่นลามิเนต Formica ผิว Matte สีขาว Solid White (0.8 มม.)",
      en: "Formica High-Pressure Laminate Solid White Matte 0.8mm",
    },
    aliases: [
      { th: "ลามิเนตขาวด้าน", en: "White Matte Laminate" },
      { th: "โฟเมก้าขาว", en: "Formica White" },
    ],
    itemType: "material",
    category: {
      id: "surface",
      name: { th: "วัสดุปิดผิวและหิน", en: "Surfaces & Stone" },
    },
    subCategory: {
      id: "laminate",
      name: { th: "แผ่นลามิเนต", en: "HPL Laminates" },
    },
    brand: {
      id: "formica",
      name: { th: "ฟอร์ไมก้า", en: "Formica" },
    },
    supplier: {
      id: "sup-formica-th",
      code: "SUP-005",
      name: { th: "บจก. ฟอร์ไมก้า (ประเทศไทย)", en: "Formica (Thailand) Co., Ltd." },
    },
    status: "active",
    specs: {
      thickness: "0.8mm",
      finish: "Matt",
      color: "White",
      dimensions: "1220x2440mm",
    },
    attributes: {
      thickness: "0.8mm",
      finish: "Matt",
      color: "White",
      size: "1220x2440mm",
    },
    pricing: {
      defaultUnitCost: 1400,
      currency: "THB",
      baseUnitCode: "sheet",
    },
    capabilities: {
      canCost: true,
      canSell: false,
      canPurchase: true,
    },
  },
  {
    id: "cat-sur-formica-wood",
    code: "MAT-SUR-FORM-02",
    name: {
      th: "แผ่นลามิเนต Formica ลายไม้ธรรมชาติ Natural Oak ผิวสัมผัสลายเสี้ยน",
      en: "Formica Natural Oak Woodgrain Texture Laminate",
    },
    aliases: [
      { th: "ลามิเนตลายไม้โอ๊ค", en: "Oak Wood Laminate" },
    ],
    itemType: "material",
    category: {
      id: "surface",
      name: { th: "วัสดุปิดผิวและหิน", en: "Surfaces & Stone" },
    },
    subCategory: {
      id: "laminate",
      name: { th: "แผ่นลามิเนต", en: "HPL Laminates" },
    },
    brand: {
      id: "formica",
      name: { th: "ฟอร์ไมก้า", en: "Formica" },
    },
    supplier: {
      id: "sup-formica-th",
      code: "SUP-005",
      name: { th: "บจก. ฟอร์ไมก้า (ประเทศไทย)", en: "Formica (Thailand) Co., Ltd." },
    },
    status: "active",
    specs: {
      thickness: "0.8mm",
      finish: "Woodgrain Embossed",
      color: "Natural Oak",
      dimensions: "1220x2440mm",
    },
    attributes: {
      thickness: "0.8mm",
      finish: "Woodgrain",
      color: "Oak",
      size: "1220x2440mm",
    },
    pricing: {
      defaultUnitCost: 1850,
      currency: "THB",
      baseUnitCode: "sheet",
    },
    capabilities: {
      canCost: true,
      canSell: false,
      canPurchase: true,
    },
  },

  // 4. Tiles & Stone (กระเบื้องและหิน)
  {
    id: "cat-tile-granite-60",
    code: "MAT-TIL-COTTO-60",
    name: {
      th: "กระเบื้องเกลซพอร์ซเลน COTTO ขนาด 60x60 ซม. สีขาว ผิวด้าน (Matt)",
      en: "COTTO Glazed Porcelain Tile 60x60cm White Matt",
    },
    aliases: [
      { th: "กระเบื้องปูพื้น 60x60 ขาวด้าน", en: "Floor Tile 60x60 White Matt" },
      { th: "แกรนิตโต้ 60x60", en: "Granito 60x60" },
    ],
    itemType: "material",
    category: {
      id: "tile",
      name: { th: "กระเบื้องปูพื้นและผนัง", en: "Tiles" },
    },
    subCategory: {
      id: "porcelain",
      name: { th: "กระเบื้องพอร์ซเลน", en: "Porcelain Tiles" },
    },
    brand: {
      id: "cotto",
      name: { th: "คอตโต้", en: "COTTO" },
    },
    supplier: {
      id: "sup-scg-dist",
      code: "SUP-006",
      name: { th: "บจก. เอสซีจี ดิสทริบิวชั่น", en: "SCG Distribution Co., Ltd." },
    },
    status: "active",
    specs: {
      dimensions: "60x60cm",
      color: "White",
      finish: "Matt",
      thickness: "9mm",
      standard: "TIS 2508-2555",
    },
    attributes: {
      size: "60x60",
      color: "White",
      finish: "Matt",
      thickness: "9mm",
      standard: "TIS",
    },
    pricing: {
      defaultUnitCost: 420,
      currency: "THB",
      baseUnitCode: "sqm",
    },
    capabilities: {
      canCost: true,
      canSell: false,
      canPurchase: true,
    },
  },
  {
    id: "cat-sur-quartz-stone",
    code: "MAT-SUR-QTZ-20",
    name: {
      th: "ท็อปหินควอตซ์สังเคราะห์หนา 20 มม. สีขาว Calacatta ผิวขัดเงา",
      en: "20mm Engineered Quartz Stone Slab Calacatta Polished",
    },
    aliases: [
      { th: "หินเทียมควอตซ์ขาว", en: "White Quartz Top" },
    ],
    itemType: "material",
    category: {
      id: "stone",
      name: { th: "หินควอตซ์และหินสังเคราะห์", en: "Engineered Quartz & Stone" },
    },
    subCategory: {
      id: "quartz",
      name: { th: "หินควอตซ์", en: "Quartz Slabs" },
    },
    brand: {
      id: "vicostone",
      name: { th: "วิโคสโตน", en: "Vicostone" },
    },
    supplier: {
      id: "sup-stone-center",
      code: "SUP-007",
      name: { th: "บจก. สโตนเซ็นเตอร์ อินเตอร์เนชั่นแนล", en: "Stone Center International Co., Ltd." },
    },
    status: "active",
    specs: {
      thickness: "20mm",
      color: "White Calacatta",
      finish: "Polished",
    },
    attributes: {
      thickness: "20mm",
      color: "White",
      finish: "Polished",
    },
    pricing: {
      defaultUnitCost: 6500,
      currency: "THB",
      baseUnitCode: "m",
    },
    capabilities: {
      canCost: true,
      canSell: false,
      canPurchase: true,
    },
  },

  // 5. Structural Steel & Rebar (เหล็กเส้นและโครงสร้าง)
  {
    id: "cat-stl-deformed-12",
    code: "MAT-STL-DB12-SD40",
    name: {
      th: "เหล็กข้ออ้อย DB12 ขนาด 12 มม. ชั้นคุณภาพ SD40 มาตรฐาน มอก. (ยาว 10 ม.)",
      en: "Deformed Steel Bar DB12 SD40 TIS Certified (10m)",
    },
    aliases: [
      { th: "เหล็กข้ออ้อย 12 มิล", en: "Rebar 12mm" },
      { th: "เหล็ก SD40 12mm", en: "SD40 Bar 12mm" },
    ],
    itemType: "material",
    category: {
      id: "steel",
      name: { th: "เหล็กและโครงสร้างโลหะ", en: "Steel & Structural Metals" },
    },
    subCategory: {
      id: "rebar",
      name: { th: "เหล็กข้ออ้อย", en: "Deformed Bars" },
    },
    brand: {
      id: "tata-steel",
      name: { th: "ทาทา สตีล", en: "TATA Steel" },
    },
    supplier: {
      id: "sup-thai-metal",
      code: "SUP-008",
      name: { th: "บจก. ไทยเมทัล เทรดดิ้ง", en: "Thai Metal Trading Co., Ltd." },
    },
    status: "active",
    specs: {
      dimensions: "12mm",
      grade: "SD40",
      standard: "TIS 24-2559",
    },
    attributes: {
      size: "12mm",
      grade: "SD40",
      standard: "TIS",
    },
    pricing: {
      defaultUnitCost: 245,
      currency: "THB",
      baseUnitCode: "pcs",
    },
    capabilities: {
      canCost: true,
      canSell: false,
      canPurchase: true,
    },
  },

  // 6. Electrical & LED Lighting
  {
    id: "cat-elec-led-strip",
    code: "MAT-ELC-LED-24V",
    name: {
      th: "รางไฟอลูมิเนียม LED Strip 24V Warm White 3000K",
      en: "Concealed Aluminum LED Strip 24V 3000K Warm Light",
    },
    aliases: [
      { th: "ไฟริบบิ้น", en: "LED Ribbon Light" },
      { th: "ไฟหลืบ 24V", en: "Cove Light 24V" },
    ],
    itemType: "material",
    category: {
      id: "electric",
      name: { th: "ระบบไฟซ่อนและอุปกรณ์", en: "Concealed LED & Electric" },
    },
    subCategory: {
      id: "led",
      name: { th: "ไฟ LED ซ่อน", en: "LED Strips" },
    },
    brand: {
      id: "meanwell",
      name: { th: "มีนเวลล์", en: "MeanWell" },
    },
    supplier: {
      id: "sup-led-lighting",
      code: "SUP-009",
      name: { th: "บจก. โปรไลท์ติ้ง ซัพพลาย", en: "Pro Lighting Supply Co., Ltd." },
    },
    status: "active",
    specs: {
      grade: "24V 3000K IP20",
      color: "Warm White",
    },
    attributes: {
      color: "Warm White",
      grade: "IP20",
    },
    pricing: {
      defaultUnitCost: 350,
      currency: "THB",
      baseUnitCode: "m",
    },
    capabilities: {
      canCost: true,
      canSell: false,
      canPurchase: true,
    },
  },

  // 7. Labor & Installation Services
  {
    id: "cat-lbr-carpentry",
    code: "LBR-CARP-ASSY",
    name: {
      th: "ค่าแรงช่างไม้ประกอบโครงตู้และติดตั้งหน้างาน",
      en: "Carpentry Assembly & On-site Installation Labor",
    },
    aliases: [
      { th: "ค่าช่างไม้", en: "Carpenter Fee" },
    ],
    itemType: "labor",
    category: {
      id: "labor_service",
      name: { th: "งานแรงงานและบริการ", en: "Labor & Installation Services" },
    },
    brand: {
      id: "internal",
      name: { th: "ทีมช่างในเครือ", en: "Internal Crew" },
    },
    status: "active",
    pricing: {
      defaultUnitCost: 1500,
      currency: "THB",
      baseUnitCode: "sqm",
    },
    capabilities: {
      canCost: true,
      canSell: false,
    },
  },
  {
    id: "cat-sub-pu-spray",
    code: "SUB-PAINT-PU",
    name: {
      th: "งานทำสีพ่นอุตสาหกรรม PU เคลือบกึ่งเงา 50% (ส่งโรงงานทำสี)",
      en: "Industrial PU Spray Paint Finishing Subcontract",
    },
    aliases: [
      { th: "ค่าพ่นสีพียู", en: "PU Spray Cost" },
    ],
    itemType: "subcontract",
    category: {
      id: "labor_service",
      name: { th: "งานแรงงานและบริการ", en: "Labor & Installation Services" },
    },
    brand: {
      id: "factory",
      name: { th: "โรงงานพ่นสีภายนอก", en: "Factory Co." },
    },
    status: "active",
    pricing: {
      defaultUnitCost: 2000,
      currency: "THB",
      baseUnitCode: "sqm",
    },
    capabilities: {
      canCost: true,
      canSell: false,
      canPurchase: true,
    },
  },
];

/**
 * Filters catalog items deterministically based on comprehensive 10-dimension criteria.
 */
export function filterCatalogItems(
  items: readonly CatalogItem[],
  criteria: CatalogFilterCriteria
): CatalogItem[] {
  const query = criteria.search?.trim().toLowerCase() || "";
  const scope = criteria.scope || "all";
  const itemTypes = criteria.itemTypes || [];
  const category = criteria.category && criteria.category !== "all" ? criteria.category : null;
  const subCategory = criteria.subCategory && criteria.subCategory !== "all" ? criteria.subCategory : null;
  const thicknesses = criteria.thicknesses || [];
  const brands = criteria.brands || [];
  const suppliers = criteria.suppliers || [];
  const status = criteria.status || null;
  const unitCodes = criteria.unitCodes || [];
  const attributes = criteria.attributes || {};

  return items.filter((item) => {
    // 1. Scope filter
    if (scope === "cost" && !item.capabilities.canCost) return false;
    if (scope === "sell" && !item.capabilities.canSell) return false;

    // 2. Item Type filter
    if (itemTypes.length > 0 && !itemTypes.includes(item.itemType)) {
      return false;
    }

    // 3. Category & SubCategory filter
    if (category && item.category.id !== category) {
      return false;
    }
    if (subCategory && item.subCategory?.id !== subCategory) {
      return false;
    }

    // 4. Status filter
    if (status && item.status !== status) {
      return false;
    }

    // 5. Unit filter
    if (unitCodes.length > 0 && !unitCodes.includes(item.pricing.baseUnitCode)) {
      return false;
    }

    // 6. Brand filter
    if (brands.length > 0) {
      const matchBrandNameTh = brands.includes(item.brand.name.th);
      const matchBrandNameEn = brands.includes(item.brand.name.en);
      const matchBrandId = brands.includes(item.brand.id);
      if (!matchBrandNameTh && !matchBrandNameEn && !matchBrandId) {
        return false;
      }
    }

    // 7. Supplier filter
    if (suppliers.length > 0) {
      if (!item.supplier) return false;
      const matchSupNameTh = suppliers.includes(item.supplier.name.th);
      const matchSupNameEn = suppliers.includes(item.supplier.name.en);
      const matchSupId = suppliers.includes(item.supplier.id);
      const matchSupCode = suppliers.includes(item.supplier.code);
      if (!matchSupNameTh && !matchSupNameEn && !matchSupId && !matchSupCode) {
        return false;
      }
    }

    // 8. Thickness filter (Legacy / Direct)
    if (thicknesses.length > 0) {
      if (!item.specs?.thickness || !thicknesses.includes(item.specs.thickness)) {
        return false;
      }
    }

    // 9. Dynamic Attributes filter (size, color, finish, grade, standard)
    for (const [attrKey, expectedVal] of Object.entries(attributes)) {
      if (!expectedVal) continue;
      const itemAttrVal = item.attributes?.[attrKey] || (item.specs as Record<string, string | undefined> | undefined)?.[attrKey];
      if (!itemAttrVal || itemAttrVal.toLowerCase() !== expectedVal.toLowerCase()) {
        return false;
      }
    }

    // 10. Comprehensive Tokenized Multi-Keyword Search (Across all 10 dimensions)
    if (query) {
      const tokens = query.split(/\s+/).filter(Boolean);
      const aliasesTh = item.aliases?.map((a) => a.th) || [];
      const aliasesEn = item.aliases?.map((a) => a.en) || [];
      const attrValues = item.attributes ? Object.values(item.attributes) : [];

      const searchableFields = [
        item.code,
        item.name.th,
        item.name.en,
        ...aliasesTh,
        ...aliasesEn,
        item.description?.th || "",
        item.description?.en || "",
        item.category.name.th,
        item.category.name.en,
        item.category.id,
        item.subCategory?.name.th || "",
        item.subCategory?.name.en || "",
        item.subCategory?.id || "",
        item.brand.name.th,
        item.brand.name.en,
        item.brand.id,
        item.supplier?.name.th || "",
        item.supplier?.name.en || "",
        item.supplier?.code || "",
        item.status,
        item.itemType,
        item.pricing.baseUnitCode,
        item.specs?.thickness || "",
        item.specs?.grade || "",
        item.specs?.finish || "",
        item.specs?.dimensions || "",
        item.specs?.standard || "",
        item.specs?.color || "",
        ...attrValues,
      ].map((f) => f.toLowerCase());

      // Every token must match at least one searchable field
      const matchesAllTokens = tokens.every((token) =>
        searchableFields.some((field) => field.includes(token))
      );

      if (!matchesAllTokens) {
        return false;
      }
    }

    return true;
  });
}
