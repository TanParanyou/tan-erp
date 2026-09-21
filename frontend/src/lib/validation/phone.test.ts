import { describe, it, expect } from "vitest";
import { isValidPhoneNumber, createPhoneSchema, cleanPhoneNumber } from "./phone";

describe("phone validation utility", () => {
  describe("cleanPhoneNumber", () => {
    it("strips whitespace, dashes, parentheses and dots", () => {
      expect(cleanPhoneNumber("+66 (0) 81-234.5678")).toBe("+660812345678");
      expect(cleanPhoneNumber("081-234-5678")).toBe("0812345678");
    });
  });

  describe("isValidPhoneNumber", () => {
    it("accepts valid Thai domestic mobile numbers (10 digits)", () => {
      expect(isValidPhoneNumber("0812345678")).toBe(true);
      expect(isValidPhoneNumber("081-234-5678")).toBe(true);
      expect(isValidPhoneNumber("091 234 5678")).toBe(true);
      expect(isValidPhoneNumber("061-234-5678")).toBe(true);
    });

    it("accepts valid Thai domestic landline numbers (9 digits)", () => {
      expect(isValidPhoneNumber("021234567")).toBe(true);
      expect(isValidPhoneNumber("02-123-4567")).toBe(true);
      expect(isValidPhoneNumber("053-123-456")).toBe(true);
    });

    it("rejects incomplete or invalid length Thai phone numbers", () => {
      expect(isValidPhoneNumber("081234")).toBe(false); // only 6 digits
      expect(isValidPhoneNumber("08123456")).toBe(false); // 8 digits (mobile needs 10)
      expect(isValidPhoneNumber("08123456789")).toBe(false); // 11 digits
      expect(isValidPhoneNumber("+6681234")).toBe(false); // incomplete international Thai
      expect(isValidPhoneNumber("+668123456")).toBe(false); // incomplete international Thai
      expect(isValidPhoneNumber("+660812345678")).toBe(false); // invalid leading 0 after +66
      expect(isValidPhoneNumber("81234")).toBe(false); // un-prefixed short
    });

    it("accepts valid international numbers (+CountryCode)", () => {
      expect(isValidPhoneNumber("+66812345678")).toBe(true);
      expect(isValidPhoneNumber("+66 81 234 5678")).toBe(true);
      expect(isValidPhoneNumber("+1 202 555 0125")).toBe(true);
      expect(isValidPhoneNumber("+65 9123 4567")).toBe(true);
      expect(isValidPhoneNumber("+81 90 1234 5678")).toBe(true);
      expect(isValidPhoneNumber("+44 20 7946 0958")).toBe(true);
    });

    it("rejects invalid international phone numbers for specific countries", () => {
      expect(isValidPhoneNumber("+651234")).toBe(false); // Singapore needs 8 digits
      expect(isValidPhoneNumber("+120255")).toBe(false); // US needs 10 digits
    });

    it("rejects invalid phone numbers", () => {
      expect(isValidPhoneNumber("")).toBe(false);
      expect(isValidPhoneNumber("abc-def-ghij")).toBe(false);
      expect(isValidPhoneNumber("12345")).toBe(false);
      expect(isValidPhoneNumber("+012345678")).toBe(false); // Country code cannot start with 0
      expect(isValidPhoneNumber("01234")).toBe(false); // Thai domestic cannot start with 01
    });
  });

  describe("createPhoneSchema", () => {
    const mockT = (key: string) => `trans_${key}`;

    it("validates optional phone schema correctly", () => {
      const schema = createPhoneSchema(mockT);

      expect(schema.safeParse("").success).toBe(true);
      expect(schema.safeParse(undefined).success).toBe(true);
      expect(schema.safeParse("+66 81 234 5678").success).toBe(true);
      expect(schema.safeParse("0812345678").success).toBe(true);
      expect(schema.safeParse("invalid").success).toBe(false);
    });

    it("validates required phone schema correctly", () => {
      const schema = createPhoneSchema(mockT, { required: true });

      expect(schema.safeParse("").success).toBe(false);
      expect(schema.safeParse("+65 9123 4567").success).toBe(true);
    });
  });
});
