import { describe, it, expect } from "vitest";
import {
  parsePhoneValue,
  formatCombinedPhone,
  findCountryByDialCode,
  PHONE_COUNTRIES,
  DEFAULT_COUNTRY_DIAL_CODE,
} from "./phone-country-codes";

describe("phone-country-codes", () => {
  describe("findCountryByDialCode", () => {
    it("finds known countries by dial code", () => {
      expect(findCountryByDialCode("+66")?.code).toBe("TH");
      expect(findCountryByDialCode("+65")?.code).toBe("SG");
      expect(findCountryByDialCode("+1")?.code).toBe("US");
      expect(findCountryByDialCode("+81")?.code).toBe("JP");
    });

    it("returns undefined for unknown dial code", () => {
      expect(findCountryByDialCode("+999")).toBeUndefined();
    });
  });

  describe("parsePhoneValue", () => {
    it("handles null, undefined and empty values", () => {
      expect(parsePhoneValue(null)).toEqual({ dialCode: "+66", nationalNumber: "" });
      expect(parsePhoneValue(undefined)).toEqual({ dialCode: "+66", nationalNumber: "" });
      expect(parsePhoneValue("")).toEqual({ dialCode: "+66", nationalNumber: "" });
      expect(parsePhoneValue("   ")).toEqual({ dialCode: "+66", nationalNumber: "" });
    });

    it("parses domestic Thai number starting with 0", () => {
      expect(parsePhoneValue("0812345678")).toEqual({
        dialCode: "+66",
        nationalNumber: "0812345678",
      });
      expect(parsePhoneValue("021234567")).toEqual({
        dialCode: "+66",
        nationalNumber: "021234567",
      });
    });

    it("parses international Thai number (+66)", () => {
      expect(parsePhoneValue("+66812345678")).toEqual({
        dialCode: "+66",
        nationalNumber: "812345678",
      });
    });

    it("parses international numbers for other countries", () => {
      expect(parsePhoneValue("+6591234567")).toEqual({
        dialCode: "+65",
        nationalNumber: "91234567",
      });
      expect(parsePhoneValue("+12025550125")).toEqual({
        dialCode: "+1",
        nationalNumber: "2025550125",
      });
      expect(parsePhoneValue("+8562012345678")).toEqual({
        dialCode: "+856",
        nationalNumber: "2012345678",
      });
    });
  });

  describe("formatCombinedPhone", () => {
    it("returns empty string when nationalNumber is empty", () => {
      expect(formatCombinedPhone("+66", "")).toBe("");
      expect(formatCombinedPhone("+65", "  ")).toBe("");
    });

    it("formats domestic Thai number starting with 0", () => {
      expect(formatCombinedPhone("+66", "0812345678")).toBe("0812345678");
    });

    it("formats Thai number without leading 0", () => {
      expect(formatCombinedPhone("+66", "812345678")).toBe("+66812345678");
    });

    it("formats international number for foreign countries, stripping leading 0 if provided", () => {
      expect(formatCombinedPhone("+65", "91234567")).toBe("+6591234567");
      expect(formatCombinedPhone("+65", "091234567")).toBe("+6591234567");
      expect(formatCombinedPhone("+1", "2025550125")).toBe("+12025550125");
    });

    it("preserves explicit + numbers typed by user", () => {
      expect(formatCombinedPhone("+66", "+6591234567")).toBe("+6591234567");
    });
  });
});
