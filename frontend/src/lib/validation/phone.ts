import { z } from "zod";

/**
 * Standard ITU-T E.164 and domestic phone number cleaner.
 * Strips whitespace, dashes, dots, and parentheses.
 */
export function cleanPhoneNumber(value: string): string {
  return value.replace(/[\s\-\(\)\.]/g, "");
}

/**
 * Validates whether a phone number matches either:
 * 1. Domestic Thai format: 9-10 digits starting with 0 (e.g. 0812345678, 021234567)
 * 2. International format: E.164 compliant (+ followed by 1-3 digit country code and 4-14 subscriber digits, total 7-15 digits)
 *    e.g. +66812345678, +12025550125, +6591234567, +819012345678
 */
export function isValidPhoneNumber(value: string): boolean {
  if (!value) return false;
  const cleaned = cleanPhoneNumber(value);

  // Domestic Thai format: 0 followed by 8 to 9 digits (total 9-10 digits)
  const domesticThaiRegex = /^0[2-9]\d{7,8}$/;
  if (domesticThaiRegex.test(cleaned)) {
    return true;
  }

  // International E.164: starts with '+' followed by 7 to 15 digits
  const internationalE164Regex = /^\+[1-9]\d{6,14}$/;
  if (internationalE164Regex.test(cleaned)) {
    return true;
  }

  return false;
}

export interface PhoneSchemaOptions {
  required?: boolean;
  requiredMessage?: string;
  invalidMessage?: string;
}

export type PhoneValidationTranslator = (key: "invalidPhone" | "required") => string;

/**
 * Creates a reusable Zod schema for phone number fields.
 * Supports optional or required phone numbers with international and domestic formats.
 */
export function createPhoneSchema(
  t: PhoneValidationTranslator,
  options?: PhoneSchemaOptions,
) {
  const invalidMsg = options?.invalidMessage ?? t("invalidPhone");
  const requiredMsg = options?.requiredMessage ?? t("required");


  if (options?.required) {
    return z
      .string()
      .trim()
      .min(1, requiredMsg)
      .refine((val) => isValidPhoneNumber(val), {
        message: invalidMsg,
      });
  }

  return z
    .string()
    .trim()
    .optional()
    .or(z.literal(""))
    .refine((val) => !val || isValidPhoneNumber(val), {
      message: invalidMsg,
    });
}
