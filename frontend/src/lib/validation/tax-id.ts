import { z } from "zod";

/**
 * Strips whitespace, dashes, and non-digit characters from a Tax ID.
 */
export function cleanTaxId(value: string): string {
  return value.replace(/\D/g, "");
}

/**
 * Formats a 13-digit Thai Tax ID / National ID with standard hyphens:
 * X-XXXX-XXXXX-XX-X
 */
export function formatTaxId(value: string): string {
  const digits = cleanTaxId(value).slice(0, 13);
  if (digits.length === 0) return "";
  if (digits.length <= 1) return digits;
  if (digits.length <= 5) return `${digits.slice(0, 1)}-${digits.slice(1)}`;
  if (digits.length <= 10) return `${digits.slice(0, 1)}-${digits.slice(1, 5)}-${digits.slice(5)}`;
  if (digits.length <= 12) return `${digits.slice(0, 1)}-${digits.slice(1, 5)}-${digits.slice(5, 10)}-${digits.slice(10)}`;
  return `${digits.slice(0, 1)}-${digits.slice(1, 5)}-${digits.slice(5, 10)}-${digits.slice(10, 12)}-${digits.slice(12, 13)}`;
}

/**
 * Validates whether a given string is a valid 13-digit Thai Tax ID.
 * Optionally validates the official Thai Tax ID / National ID checksum (Mod 11).
 * In business practice, test/foreign IDs or standard 13-digit IDs might be used,
 * so checksum validation can be enabled or disabled via options.
 */
export function isValidTaxId(value: string, options?: { checkChecksum?: boolean }): boolean {
  if (!value) return false;
  const digits = cleanTaxId(value);
  if (digits.length !== 13) return false;

  if (options?.checkChecksum) {
    let sum = 0;
    for (let i = 0; i < 12; i++) {
      sum += parseInt(digits[i], 10) * (13 - i);
    }
    const checkDigit = (11 - (sum % 11)) % 10;
    return checkDigit === parseInt(digits[12], 10);
  }

  return true;
}

export interface TaxIdSchemaOptions {
  required?: boolean;
  checkChecksum?: boolean;
  requiredMessage?: string;
  invalidMessage?: string;
}

export type TaxIdValidationTranslator = (key: "invalidTaxId" | "required") => string;

/**
 * Creates a reusable Zod schema for Tax ID fields.
 * Supports optional or required Tax IDs with standard 13-digit validation.
 */
export function createTaxIdSchema(
  t: TaxIdValidationTranslator,
  options?: TaxIdSchemaOptions,
) {
  const invalidMsg = options?.invalidMessage ?? t("invalidTaxId");
  const requiredMsg = options?.requiredMessage ?? t("required");

  if (options?.required) {
    return z
      .string()
      .trim()
      .min(1, requiredMsg)
      .refine((val) => isValidTaxId(val, { checkChecksum: options?.checkChecksum }), {
        message: invalidMsg,
      });
  }

  return z
    .string()
    .trim()
    .optional()
    .or(z.literal(""))
    .refine((val) => !val || isValidTaxId(val, { checkChecksum: options?.checkChecksum }), {
      message: invalidMsg,
    });
}
