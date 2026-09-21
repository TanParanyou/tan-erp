import { z } from "zod";

/**
 * Standard ITU-T E.164 and domestic phone number cleaner.
 * Strips whitespace, dashes, dots, and parentheses.
 */
export function cleanPhoneNumber(value: string): string {
  return value.replace(/[\s\-\(\)\.]/g, "");
}

import {
  parsePhoneValue,
  validatePhoneForCountry,
  findCountryByDialCode,
} from "@/components/forms/phone-country-codes";

/**
 * Validates whether a phone number matches country-specific formats:
 * 1. Domestic Thai format: 9 digits for landlines (02, 03x, 04x, 05x, 07x) or 10 digits for mobiles (06, 08, 09)
 * 2. Country-specific international numbers according to selected country code (+66, +65, +1, etc.)
 * 3. ITU-T E.164 fallback for other international dial codes (+ and 7-15 digits)
 */
export function isValidPhoneNumber(value: string): boolean {
  if (!value) return false;
  const cleaned = cleanPhoneNumber(value);
  if (!cleaned) return false;

  // If starts with domestic 0 (assumed Thai domestic)
  if (cleaned.startsWith("0")) {
    return validatePhoneForCountry("+66", cleaned);
  }

  // If international (+...)
  if (cleaned.startsWith("+")) {
    const parsed = parsePhoneValue(cleaned);
    const country = findCountryByDialCode(parsed.dialCode);
    if (country) {
      return validatePhoneForCountry(parsed.dialCode, parsed.nationalNumber, {
        isInternational: true,
      });
    }
    // Fallback E.164
    return /^\+[1-9]\d{6,14}$/.test(cleaned);
  }

  // If un-prefixed digits (assumed Thai national number without 0)
  return validatePhoneForCountry("+66", cleaned);
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
