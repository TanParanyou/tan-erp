import { describe, expect, it } from "vitest";
import { createSiteFormSchema } from "./site-form-schema";

const mockT = (key: string) => `trans_${key}`;

describe("createSiteFormSchema", () => {
  const schema = createSiteFormSchema(mockT as any);

  it("validates minimal valid site input", () => {
    const valid = {
      label: "Main HQ",
      addressLine1: "123 Sukhumvit Road",
      subdistrict: "Khlong Toei",
      district: "Khlong Toei",
      province: "Bangkok",
      postalCode: "10110",
      countryCode: "TH",
      latitude: null,
      longitude: null,
      accessNote: "",
    };

    const result = schema.safeParse(valid);
    expect(result.success).toBe(true);
  });

  it("validates valid site input with paired coordinates and accessNote", () => {
    const valid = {
      label: "Site Branch A",
      addressLine1: "456 Phahonyothin Rd",
      subdistrict: "Chatuchak",
      district: "Chatuchak",
      province: "Bangkok",
      postalCode: "10900",
      countryCode: "TH",
      latitude: 13.7563,
      longitude: 100.5018,
      accessNote: "Ring the bell at the front gate",
    };

    const result = schema.safeParse(valid);
    expect(result.success).toBe(true);
  });

  it("fails if required fields are empty", () => {
    const invalid = {
      label: "",
      addressLine1: "",
      subdistrict: "",
      district: "",
      province: "",
      postalCode: "",
      countryCode: "",
    };

    const result = schema.safeParse(invalid);
    expect(result.success).toBe(false);
    if (!result.success) {
      const fieldErrors = result.error.flatten().fieldErrors;
      expect(fieldErrors.label).toBeDefined();
      expect(fieldErrors.addressLine1).toBeDefined();
      expect(fieldErrors.subdistrict).toBeDefined();
      expect(fieldErrors.district).toBeDefined();
      expect(fieldErrors.province).toBeDefined();
      expect(fieldErrors.postalCode).toBeDefined();
    }
  });

  it("fails if countryCode is not two uppercase letters", () => {
    const result = schema.safeParse({
      label: "Site",
      addressLine1: "Line 1",
      subdistrict: "Sub",
      district: "Dist",
      province: "Prov",
      postalCode: "12345",
      countryCode: "th", // lowercase
    });

    expect(result.success).toBe(false);
  });

  it("fails if only latitude is provided without longitude", () => {
    const result = schema.safeParse({
      label: "Site",
      addressLine1: "Line 1",
      subdistrict: "Sub",
      district: "Dist",
      province: "Prov",
      postalCode: "12345",
      countryCode: "TH",
      latitude: 13.7563,
      longitude: null,
    });

    expect(result.success).toBe(false);
  });

  it("fails if only longitude is provided without latitude", () => {
    const result = schema.safeParse({
      label: "Site",
      addressLine1: "Line 1",
      subdistrict: "Sub",
      district: "Dist",
      province: "Prov",
      postalCode: "12345",
      countryCode: "TH",
      latitude: null,
      longitude: 100.5018,
    });

    expect(result.success).toBe(false);
  });

  it("fails if coordinates are out of valid range (-90..90 and -180..180)", () => {
    const resultLat = schema.safeParse({
      label: "Site",
      addressLine1: "Line 1",
      subdistrict: "Sub",
      district: "Dist",
      province: "Prov",
      postalCode: "12345",
      countryCode: "TH",
      latitude: 95.0,
      longitude: 100.0,
    });
    expect(resultLat.success).toBe(false);

    const resultLng = schema.safeParse({
      label: "Site",
      addressLine1: "Line 1",
      subdistrict: "Sub",
      district: "Dist",
      province: "Prov",
      postalCode: "12345",
      countryCode: "TH",
      latitude: 13.0,
      longitude: 185.0,
    });
    expect(resultLng.success).toBe(false);
  });
});
