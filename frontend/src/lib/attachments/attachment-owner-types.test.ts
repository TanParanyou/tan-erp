import { describe, expect, it } from "vitest";
import thMessages from "@/messages/th.json";
import enMessages from "@/messages/en.json";
import {
  ATTACHMENT_ERROR_CODES,
  ATTACHMENT_OWNER_TYPES,
  ATTACHMENT_PURPOSES,
  SIGNATURE_CONSENT_VERSIONS,
  SIGNATURE_PURPOSES,
  attachmentErrorCode,
  isAttachmentPurpose,
} from "./attachment-owner-types";

describe("attachment owner types", () => {
  it("mirrors the backend whitelist for the registered owner types", () => {
    expect(ATTACHMENT_OWNER_TYPES).toEqual(["installation-job"]);
  });

  it("knows the backend purposes and rejects anything else", () => {
    expect(ATTACHMENT_PURPOSES).toEqual(["general", "evidence", "handover", "defect"]);
    expect(isAttachmentPurpose("evidence")).toBe(true);
    expect(isAttachmentPurpose("misc")).toBe(false);
  });

  it("narrows only known error codes", () => {
    expect(attachmentErrorCode("ATTACHMENT_DUPLICATE")).toBe("ATTACHMENT_DUPLICATE");
    expect(attachmentErrorCode("SOMETHING_ELSE")).toBeNull();
    expect(attachmentErrorCode(undefined)).toBeNull();
  });

  it("has a consent version for every signature purpose", () => {
    for (const purpose of SIGNATURE_PURPOSES) {
      expect(SIGNATURE_CONSENT_VERSIONS[purpose]).toMatch(/^[a-z]+-\d{4}-\d{2}-v\d+$/);
    }
  });

  it.each([
    ["th", thMessages],
    ["en", enMessages],
  ])("has %s labels for every purpose, error code and consent version", (_locale, messages) => {
    const ns = messages.attachments;
    for (const purpose of ATTACHMENT_PURPOSES) expect(ns.purposes[purpose]).toBeTruthy();
    for (const code of ATTACHMENT_ERROR_CODES) expect(ns.errors[code]).toBeTruthy();
    for (const purpose of SIGNATURE_PURPOSES) expect(ns.signature.consentVersions[SIGNATURE_CONSENT_VERSIONS[purpose]]).toBeTruthy();
  });
});
