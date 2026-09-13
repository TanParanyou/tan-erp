export interface RoomPresetOption {
  code: string;
  labelKey: string;
  defaultNameTh: string;
  defaultNameEn: string;
}

export interface PresetPackage {
  id: string;
  labelKey: string;
  rooms: string[]; // array of RoomPresetOption codes
}

export const STANDARD_ROOM_PRESETS: RoomPresetOption[] = [
  { code: "living_room", labelKey: "roomPresets.living_room", defaultNameTh: "ห้องนั่งเล่น (Living Room)", defaultNameEn: "Living Room" },
  { code: "master_bedroom", labelKey: "roomPresets.master_bedroom", defaultNameTh: "ห้องนอนใหญ่ (Master Bedroom)", defaultNameEn: "Master Bedroom" },
  { code: "bedroom_2", labelKey: "roomPresets.bedroom_2", defaultNameTh: "ห้องนอน 2 (Bedroom 2)", defaultNameEn: "Bedroom 2" },
  { code: "bedroom_3", labelKey: "roomPresets.bedroom_3", defaultNameTh: "ห้องนอน 3 (Bedroom 3)", defaultNameEn: "Bedroom 3" },
  { code: "walk_in_closet", labelKey: "roomPresets.walk_in_closet", defaultNameTh: "ห้องแต่งตัว (Walk-in Closet)", defaultNameEn: "Walk-in Closet" },
  { code: "kitchen", labelKey: "roomPresets.kitchen", defaultNameTh: "ห้องครัว (Kitchen)", defaultNameEn: "Kitchen" },
  { code: "pantry", labelKey: "roomPresets.pantry", defaultNameTh: "แพนทรี / เตรียมอาหาร (Pantry)", defaultNameEn: "Pantry" },
  { code: "dining_room", labelKey: "roomPresets.dining_room", defaultNameTh: "ห้องรับประทานอาหาร (Dining Room)", defaultNameEn: "Dining Room" },
  { code: "foyer", labelKey: "roomPresets.foyer", defaultNameTh: "โถงทางเข้า (Foyer)", defaultNameEn: "Foyer" },
  { code: "working_room", labelKey: "roomPresets.working_room", defaultNameTh: "ห้องทำงาน (Home Office)", defaultNameEn: "Home Office" },
  { code: "bathroom", labelKey: "roomPresets.bathroom", defaultNameTh: "ห้องน้ำ (Bathroom)", defaultNameEn: "Bathroom" },
  { code: "balcony", labelKey: "roomPresets.balcony", defaultNameTh: "ระเบียง (Balcony)", defaultNameEn: "Balcony" },
];

export const PRESET_PACKAGES: PresetPackage[] = [
  {
    id: "condo_studio",
    labelKey: "batchTemplateModal.pkg_condo_studio",
    rooms: ["living_room", "master_bedroom", "kitchen"],
  },
  {
    id: "condo_2bed",
    labelKey: "batchTemplateModal.pkg_condo_2bed",
    rooms: ["living_room", "master_bedroom", "bedroom_2", "kitchen"],
  },
  {
    id: "townhome",
    labelKey: "batchTemplateModal.pkg_townhome",
    rooms: ["living_room", "dining_room", "kitchen", "master_bedroom", "walk_in_closet", "bedroom_2"],
  },
  {
    id: "house",
    labelKey: "batchTemplateModal.pkg_house",
    rooms: ["foyer", "living_room", "dining_room", "pantry", "kitchen", "master_bedroom", "walk_in_closet", "bedroom_2", "working_room"],
  },
];

export function useAreaPresets() {
  return {
    presets: STANDARD_ROOM_PRESETS,
    packages: PRESET_PACKAGES,
    getPresetByCode: (code: string) => STANDARD_ROOM_PRESETS.find((p) => p.code === code),
  };
}
