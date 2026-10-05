import {
  RESERVED_ATTRIBUTE_KEYS,
  CONTROLLED_ATTRIBUTE_REGISTRY,
  type AttributeDefinition,
  type ItemAttributePair,
  type ItemAttributesRecord,
} from "@/types/item-attributes";

/**
 * Converts Record<string, string> or null/undefined to ItemAttributePair[]
 */
export function recordToAttributePairs(
  record?: ItemAttributesRecord | null
): ItemAttributePair[] {
  if (!record || typeof record !== "object") return [];
  return Object.entries(record).map(([key, value]) => ({
    key,
    value: typeof value === "string" ? value : String(value ?? ""),
  }));
}

/**
 * Converts ItemAttributePair[] into clean Record<string, string>
 * Trims whitespace and excludes empty keys.
 */
export function attributePairsToRecord(
  pairs: ItemAttributePair[]
): ItemAttributesRecord {
  const result: ItemAttributesRecord = {};
  if (!Array.isArray(pairs)) return result;

  for (const pair of pairs) {
    const trimmedKey = pair.key?.trim();
    if (trimmedKey) {
      result[trimmedKey] = pair.value?.trim() ?? "";
    }
  }

  return result;
}

export interface AttributeValidationResult {
  isValid: boolean;
  duplicateKeys: string[];
  reservedKeys: string[];
}

/**
 * Validates list of attribute pairs for duplicates and reserved keys
 */
export function validateAttributePairs(
  pairs: ItemAttributePair[]
): AttributeValidationResult {
  const seenKeys = new Set<string>();
  const duplicateKeys: string[] = [];
  const reservedKeys: string[] = [];

  if (!Array.isArray(pairs)) {
    return { isValid: true, duplicateKeys: [], reservedKeys: [] };
  }

  for (const pair of pairs) {
    const rawKey = pair.key?.trim();
    if (!rawKey) continue;

    const lowerKey = rawKey.toLowerCase();

    // Check duplicate
    if (seenKeys.has(lowerKey)) {
      if (!duplicateKeys.includes(rawKey)) {
        duplicateKeys.push(rawKey);
      }
    } else {
      seenKeys.add(lowerKey);
    }

    // Check reserved
    if (
      (RESERVED_ATTRIBUTE_KEYS as readonly string[]).includes(lowerKey) &&
      !reservedKeys.includes(rawKey)
    ) {
      reservedKeys.push(rawKey);
    }
  }

  const isValid = duplicateKeys.length === 0 && reservedKeys.length === 0;
  return { isValid, duplicateKeys, reservedKeys };
}

/**
 * Finds attribute definition from controlled registry
 */
export function getAttributeDefinition(key: string): AttributeDefinition | undefined {
  const normalized = key.trim().toLowerCase();
  return CONTROLLED_ATTRIBUTE_REGISTRY.find(
    (item) => item.key.toLowerCase() === normalized
  );
}

/**
 * Finds attribute definition from dynamic category definitions or fallback to controlled registry
 */
export function getMergedAttributeDefinition(
  key: string,
  categoryDefinitions?: AttributeDefinition[]
): AttributeDefinition | undefined {
  const normalized = key.trim().toLowerCase();
  if (categoryDefinitions && categoryDefinitions.length > 0) {
    const found = categoryDefinitions.find(
      (item) => item.key.toLowerCase() === normalized
    );
    if (found) return found;
  }
  return getAttributeDefinition(key);
}

/**
 * Translates or formats key into human-friendly label
 */
export function getAttributeKeyLabel(
  key: string,
  locale: "th" | "en" = "th",
  categoryDefinitions?: AttributeDefinition[]
): string {
  const found = getMergedAttributeDefinition(key, categoryDefinitions);

  if (found) {
    return locale === "th" ? found.labelTh : found.labelEn;
  }

  return key.replaceAll("_", " ");
}

/**
 * Returns formatted text of all attributes (for copying or printing)
 */
export function formatAttributesToText(
  attributes?: ItemAttributesRecord | ItemAttributePair[] | null,
  locale: "th" | "en" = "th"
): string {
  if (!attributes) return "";

  const pairs: ItemAttributePair[] = Array.isArray(attributes)
    ? attributes
    : recordToAttributePairs(attributes);

  return pairs
    .filter((p) => p.key.trim() && p.value.trim())
    .map((p) => `${getAttributeKeyLabel(p.key, locale)}: ${p.value.trim()}`)
    .join("\n");
}
