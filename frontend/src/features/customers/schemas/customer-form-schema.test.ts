import { describe, expect, it } from "vitest";
import { createCustomerFormSchema } from "./customer-form-schema";

const t = (key: "required" | "invalidEmail" | "phoneOrEmailRequired"): string =>
  `validation.${key}`;

const schema = createCustomerFormSchema(t);

const validOrganizationWithPhone = {
  customerType: "organization",
  displayNameTh: "บริษัท ตัวอย่าง จำกัด",
  displayNameEn: "",
  preferredLocale: "th",
  primaryContact: {
    name: "คุณตัวอย่าง",
    roleTitle: "",
    phone: "0812345678",
    email: "",
    preferredChannel: "phone",
  },
};

const validPersonWithEmailOnly = {
  customerType: "person",
  displayNameTh: "นาย ตัวอย่าง",
  displayNameEn: "",
  preferredLocale: "th",
  primaryContact: {
    name: "นาย ตัวอย่าง",
    roleTitle: "",
    phone: "",
    email: "example@example.test",
    preferredChannel: "email",
  },
};

const contactWithoutPhoneAndEmail = {
  customerType: "organization",
  displayNameTh: "บริษัท ตัวอย่าง จำกัด",
  displayNameEn: "",
  preferredLocale: "th",
  primaryContact: {
    name: "คุณตัวอย่าง",
    roleTitle: "",
    phone: "",
    email: "",
    preferredChannel: "phone",
  },
};

describe("createCustomerFormSchema", () => {
  it("accepts organization with phone", () => {
    expect(schema.safeParse(validOrganizationWithPhone).success).toBe(true);
  });

  it("accepts person with email only", () => {
    expect(schema.safeParse(validPersonWithEmailOnly).success).toBe(true);
  });

  it("rejects non-canonical customer type", () => {
    expect(
      schema.safeParse({ ...validOrganizationWithPhone, customerType: "corporate" }).success,
    ).toBe(false);
  });

  it("requires phone or email", () => {
    expect(schema.safeParse(contactWithoutPhoneAndEmail).error?.issues[0].message).toBe(
      "validation.phoneOrEmailRequired",
    );
  });

  it("rejects invalid email format", () => {
    const result = schema.safeParse({
      ...validOrganizationWithPhone,
      primaryContact: {
        ...validOrganizationWithPhone.primaryContact,
        phone: "",
        email: "not-an-email",
      },
    });
    expect(result.success).toBe(false);
  });
});
