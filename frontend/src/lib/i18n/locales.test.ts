import { describe, it, expect } from "vitest";
import thMessages from "@/messages/th.json";
import enMessages from "@/messages/en.json";
import {
  defaultLocale,
  supportedLocales,
  isSupportedLocale,
  getMessages,
} from "./locales";

function getDeepKeys(obj: Record<string, unknown>, prefix = ""): string[] {
  let keys: string[] = [];
  for (const [key, value] of Object.entries(obj)) {
    const fullKey = prefix ? `${prefix}.${key}` : key;
    if (value && typeof value === "object" && !Array.isArray(value)) {
      keys = keys.concat(getDeepKeys(value as Record<string, unknown>, fullKey));
    } else {
      keys.push(fullKey);
    }
  }
  return keys.sort();
}

describe("i18n Locales and Message Parity", () => {
  it("defines defaultLocale as th and supports th and en", () => {
    expect(defaultLocale).toBe("th");
    expect(supportedLocales).toEqual(["th", "en"]);
    expect(isSupportedLocale("th")).toBe(true);
    expect(isSupportedLocale("en")).toBe(true);
    expect(isSupportedLocale("fr")).toBe(false);
  });

  it("returns messages for supported locales", () => {
    expect(getMessages("th")).toBe(thMessages);
    expect(getMessages("en")).toBe(enMessages);
  });

  it("ensures 100% key parity between th.json and en.json", () => {
    const thKeys = getDeepKeys(thMessages);
    const enKeys = getDeepKeys(enMessages);

    expect(thKeys).toEqual(enKeys);
  });

  it("ensures no translation string is empty in either locale", () => {
    const verifyNonEmpty = (obj: Record<string, unknown>, path = "") => {
      for (const [key, value] of Object.entries(obj)) {
        const fullPath = path ? `${path}.${key}` : key;
        if (typeof value === "string") {
          expect(value.trim().length, `Empty string found at ${fullPath}`).toBeGreaterThan(0);
        } else if (value && typeof value === "object") {
          verifyNonEmpty(value as Record<string, unknown>, fullPath);
        }
      }
    };

    verifyNonEmpty(thMessages, "th");
    verifyNonEmpty(enMessages, "en");
  });

  it("contains all required common namespaces", () => {
    const expectedCommonCategories = [
      "actions",
      "status",
      "feedback",
      "dialog",
      "form",
      "validation",
      "table",
      "fields",
      "states",
    ];

    expect(thMessages).toHaveProperty("common");
    expect(enMessages).toHaveProperty("common");

    for (const category of expectedCommonCategories) {
      expect(thMessages.common).toHaveProperty(category);
      expect(enMessages.common).toHaveProperty(category);
    }
  });
});
