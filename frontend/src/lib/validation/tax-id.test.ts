import { describe, expect, it } from "vitest";
import { cleanTaxId, formatTaxId, isValidTaxId, createTaxIdSchema } from "./tax-id";

describe("tax-id validation and formatting", () => {
  describe("cleanTaxId", () => {
    it("strips dashes and non-digit characters", () => {
      expect(cleanTaxId("0-1055-50000-00-0")).toBe("0105550000000");
      expect(cleanTaxId("0105550000000")).toBe("0105550000000");
      expect(cleanTaxId("abc123def456-789-012-3")).toBe("1234567890123");
    });
  });

  describe("formatTaxId", () => {
    it("formats 13 digits with standard hyphens", () => {
      expect(formatTaxId("0105550000000")).toBe("0-1055-50000-00-0");
      expect(formatTaxId("1234567890123")).toBe("1-2345-67890-12-3");
    });

    it("handles partial input gracefully", () => {
      expect(formatTaxId("0")).toBe("0");
      expect(formatTaxId("0105")).toBe("0-105");
      expect(formatTaxId("010555")).toBe("0-1055-5");
      expect(formatTaxId("")).toBe("");
    });
  });

  describe("isValidTaxId", () => {
    it("accepts valid 13-digit numbers without dashes", () => {
      expect(isValidTaxId("0105550000000")).toBe(true);
    });

    it("accepts valid 13-digit numbers with dashes", () => {
      expect(isValidTaxId("0-1055-50000-00-0")).toBe(true);
    });

    it("rejects invalid lengths", () => {
      expect(isValidTaxId("")).toBe(false);
      expect(isValidTaxId("12345")).toBe(false);
      expect(isValidTaxId("01055500000001")).toBe(false); // 14 digits
      expect(isValidTaxId("010555000000")).toBe(false); // 12 digits
    });

    it("rejects non-numeric characters only", () => {
      expect(isValidTaxId("abcdefghijklm")).toBe(false);
    });

    it("validates checksum correctly when enabled", () => {
      // 1-1001-00123-45-7: 1*13 + 1*12 + 0*11 + 0*10 + 1*9 + 0*8 + 0*7 + 1*6 + 2*5 + 3*4 + 4*3 + 5*2
      // = 13 + 12 + 0 + 0 + 9 + 0 + 0 + 6 + 10 + 12 + 12 + 10 = 84
      // 84 % 11 = 7. 11 - 7 = 4. Checksum should be 4.
      // Let's test with a valid checksum: 1100100123454
      expect(isValidTaxId("1100100123454", { checkChecksum: true })).toBe(true);
      expect(isValidTaxId("1100100123455", { checkChecksum: true })).toBe(false);
    });
  });

  describe("createTaxIdSchema", () => {
    const mockT = (key: "invalidTaxId" | "required") => key;

    it("accepts empty string or undefined when optional", () => {
      const schema = createTaxIdSchema(mockT);
      expect(schema.safeParse("").success).toBe(true);
      expect(schema.safeParse(undefined).success).toBe(true);
      expect(schema.safeParse("0105550000000").success).toBe(true);
    });

    it("rejects invalid 13-digit when provided", () => {
      const schema = createTaxIdSchema(mockT);
      const res = schema.safeParse("123");
      expect(res.success).toBe(false);
      if (!res.success) {
        expect(res.error.issues[0].message).toBe("invalidTaxId");
      }
    });

    it("requires value when required option is true", () => {
      const schema = createTaxIdSchema(mockT, { required: true });
      expect(schema.safeParse("").success).toBe(false);
      expect(schema.safeParse("0105550000000").success).toBe(true);
    });
  });
});
