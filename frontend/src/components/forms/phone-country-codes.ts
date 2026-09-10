export interface PhoneCountry {
  code: string;
  dialCode: string;
  nameKey: string;
}

export const DEFAULT_COUNTRY_DIAL_CODE = "+66";

export const PHONE_COUNTRIES: readonly PhoneCountry[] = [
  { code: "TH", dialCode: "+66", nameKey: "thailand" },
  { code: "SG", dialCode: "+65", nameKey: "singapore" },
  { code: "MY", dialCode: "+60", nameKey: "malaysia" },
  { code: "VN", dialCode: "+84", nameKey: "vietnam" },
  { code: "ID", dialCode: "+62", nameKey: "indonesia" },
  { code: "PH", dialCode: "+63", nameKey: "philippines" },
  { code: "LA", dialCode: "+856", nameKey: "laos" },
  { code: "MM", dialCode: "+95", nameKey: "myanmar" },
  { code: "KH", dialCode: "+855", nameKey: "cambodia" },
  { code: "CN", dialCode: "+86", nameKey: "china" },
  { code: "JP", dialCode: "+81", nameKey: "japan" },
  { code: "KR", dialCode: "+82", nameKey: "southKorea" },
  { code: "TW", dialCode: "+886", nameKey: "taiwan" },
  { code: "HK", dialCode: "+852", nameKey: "hongKong" },
  { code: "US", dialCode: "+1", nameKey: "unitedStates" },
  { code: "GB", dialCode: "+44", nameKey: "unitedKingdom" },
  { code: "AU", dialCode: "+61", nameKey: "australia" },
  { code: "DE", dialCode: "+49", nameKey: "germany" },
  { code: "FR", dialCode: "+33", nameKey: "france" },
  { code: "IN", dialCode: "+91", nameKey: "india" },
] as const;

export function findCountryByDialCode(dialCode: string): PhoneCountry | undefined {
  return PHONE_COUNTRIES.find((c) => c.dialCode === dialCode);
}

/**
 * Splits a raw phone number string into dialCode and subscriber/national number.
 */
export function parsePhoneValue(
  value: string | null | undefined,
  defaultDialCode = DEFAULT_COUNTRY_DIAL_CODE,
): { dialCode: string; nationalNumber: string } {
  if (!value) {
    return { dialCode: defaultDialCode, nationalNumber: "" };
  }

  const cleaned = value.trim();
  if (!cleaned) {
    return { dialCode: defaultDialCode, nationalNumber: "" };
  }

  if (cleaned.startsWith("+")) {
    const sortedCountries = [...PHONE_COUNTRIES].sort(
      (a, b) => b.dialCode.length - a.dialCode.length,
    );
    const matched = sortedCountries.find((c) => cleaned.startsWith(c.dialCode));
    if (matched) {
      return {
        dialCode: matched.dialCode,
        nationalNumber: cleaned.slice(matched.dialCode.length).trim(),
      };
    }

    const plusMatch = cleaned.match(/^(\+\d{1,4})(.*)$/);
    if (plusMatch) {
      return {
        dialCode: plusMatch[1],
        nationalNumber: plusMatch[2].trim(),
      };
    }
  }

  // Domestic Thai format (starts with 0) or un-prefixed input defaults to Thailand
  return {
    dialCode: defaultDialCode,
    nationalNumber: cleaned,
  };
}

/**
 * Combines dialCode and subscriber number into a standard phone string.
 */
export function formatCombinedPhone(dialCode: string, nationalNumber: string): string {
  const trimmed = nationalNumber.trim();
  if (!trimmed) {
    return "";
  }

  // If user typed a full international string starting with +
  if (trimmed.startsWith("+")) {
    return trimmed;
  }

  if (dialCode === "+66") {
    // If it starts with domestic '0', preserve it for domestic familiarity (e.g. 0812345678)
    if (trimmed.startsWith("0")) {
      return trimmed;
    }
    // Otherwise prepend +66
    return `+66${trimmed}`;
  }

  // For international countries, strip leading zero if user accidentally typed it
  const sanitized = trimmed.replace(/^0+/, "");
  return `${dialCode}${sanitized}`;
}
