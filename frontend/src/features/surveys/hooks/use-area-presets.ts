export interface RoomPresetOption {
  code: string;
  labelKey: string;
}

export interface PresetPackage {
  id: string;
  labelKey: string;
  rooms: string[]; // array of RoomPresetOption codes
}

export const STANDARD_ROOM_PRESETS: RoomPresetOption[] = [
  { code: "living_room", labelKey: "roomPresets.living_room" },
  { code: "master_bedroom", labelKey: "roomPresets.master_bedroom" },
  { code: "bedroom_2", labelKey: "roomPresets.bedroom_2" },
  { code: "bedroom_3", labelKey: "roomPresets.bedroom_3" },
  { code: "walk_in_closet", labelKey: "roomPresets.walk_in_closet" },
  { code: "kitchen", labelKey: "roomPresets.kitchen" },
  { code: "pantry", labelKey: "roomPresets.pantry" },
  { code: "dining_room", labelKey: "roomPresets.dining_room" },
  { code: "foyer", labelKey: "roomPresets.foyer" },
  { code: "working_room", labelKey: "roomPresets.working_room" },
  { code: "bathroom", labelKey: "roomPresets.bathroom" },
  { code: "balcony", labelKey: "roomPresets.balcony" },
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
