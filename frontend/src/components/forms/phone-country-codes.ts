export interface PhoneCountry {
  code: string;
  dialCode: string;
  nameKey: string;
  /** Expected subscriber/national number length without leading zero */
  nationalLengths: readonly number[];
  /** Pattern for national number (digits only, no dialCode, without leading 0 for intl) */
  nationalPattern: RegExp;
  /** Max input length including domestic leading 0 or separators */
  maxInputLength: number;
}

export const DEFAULT_COUNTRY_DIAL_CODE = "+66";

export const PHONE_COUNTRIES: readonly PhoneCountry[] = [
  {
    code: "TH",
    dialCode: "+66",
    nameKey: "thailand",
    nationalLengths: [8, 9], // 9 for mobile (e.g. 812345678), 8 for landline (e.g. 21234567)
    // In domestic format (with 0): 0[689]\d{8} (10 digits) or 0[2-57]\d{7} (9 digits)
    // After +66 (without 0): [689]\d{8} (9 digits) or [2-57]\d{7} (8 digits)
    nationalPattern: /^([689]\d{8}|[2-57]\d{7})$/,
    maxInputLength: 10,
  },
  {
    code: "SG",
    dialCode: "+65",
    nameKey: "singapore",
    nationalLengths: [8],
    nationalPattern: /^[3689]\d{7}$/,
    maxInputLength: 8,
  },
  {
    code: "MY",
    dialCode: "+60",
    nameKey: "malaysia",
    nationalLengths: [8, 9, 10],
    nationalPattern: /^(1\d{8,9}|[3-9]\d{7,8})$/,
    maxInputLength: 11,
  },
  {
    code: "VN",
    dialCode: "+84",
    nameKey: "vietnam",
    nationalLengths: [9, 10],
    nationalPattern: /^[1-9]\d{8,9}$/,
    maxInputLength: 11,
  },
  {
    code: "ID",
    dialCode: "+62",
    nameKey: "indonesia",
    nationalLengths: [8, 9, 10, 11, 12],
    nationalPattern: /^[1-9]\d{7,11}$/,
    maxInputLength: 13,
  },
  {
    code: "PH",
    dialCode: "+63",
    nameKey: "philippines",
    nationalLengths: [10],
    nationalPattern: /^[1-9]\d{9}$/,
    maxInputLength: 11,
  },
  {
    code: "LA",
    dialCode: "+856",
    nameKey: "laos",
    nationalLengths: [8, 9, 10],
    nationalPattern: /^[1-9]\d{7,9}$/,
    maxInputLength: 11,
  },
  {
    code: "MM",
    dialCode: "+95",
    nameKey: "myanmar",
    nationalLengths: [7, 8, 9],
    nationalPattern: /^[1-9]\d{6,8}$/,
    maxInputLength: 10,
  },
  {
    code: "KH",
    dialCode: "+855",
    nameKey: "cambodia",
    nationalLengths: [8, 9],
    nationalPattern: /^[1-9]\d{7,8}$/,
    maxInputLength: 10,
  },
  {
    code: "CN",
    dialCode: "+86",
    nameKey: "china",
    nationalLengths: [11],
    nationalPattern: /^1[3-9]\d{9}$/,
    maxInputLength: 11,
  },
  {
    code: "JP",
    dialCode: "+81",
    nameKey: "japan",
    nationalLengths: [9, 10],
    nationalPattern: /^[1-9]\d{8,9}$/,
    maxInputLength: 11,
  },
  {
    code: "KR",
    dialCode: "+82",
    nameKey: "southKorea",
    nationalLengths: [9, 10],
    nationalPattern: /^[1-9]\d{8,9}$/,
    maxInputLength: 11,
  },
  {
    code: "TW",
    dialCode: "+886",
    nameKey: "taiwan",
    nationalLengths: [9],
    nationalPattern: /^[1-9]\d{8}$/,
    maxInputLength: 10,
  },
  {
    code: "HK",
    dialCode: "+852",
    nameKey: "hongKong",
    nationalLengths: [8],
    nationalPattern: /^[2-9]\d{7}$/,
    maxInputLength: 8,
  },
  {
    code: "US",
    dialCode: "+1",
    nameKey: "unitedStates",
    nationalLengths: [10],
    nationalPattern: /^[2-9]\d{9}$/,
    maxInputLength: 10,
  },
  {
    code: "GB",
    dialCode: "+44",
    nameKey: "unitedKingdom",
    nationalLengths: [10],
    nationalPattern: /^[1-9]\d{9}$/,
    maxInputLength: 11,
  },
  {
    code: "AU",
    dialCode: "+61",
    nameKey: "australia",
    nationalLengths: [9],
    nationalPattern: /^[1-9]\d{8}$/,
    maxInputLength: 10,
  },
  {
    code: "DE",
    dialCode: "+49",
    nameKey: "germany",
    nationalLengths: [10, 11],
    nationalPattern: /^[1-9]\d{9,10}$/,
    maxInputLength: 12,
  },
  {
    code: "FR",
    dialCode: "+33",
    nameKey: "france",
    nationalLengths: [9],
    nationalPattern: /^[1-9]\d{8}$/,
    maxInputLength: 10,
  },
  {
    code: "IN",
    dialCode: "+91",
    nameKey: "india",
    nationalLengths: [10],
    nationalPattern: /^[6-9]\d{9}$/,
    maxInputLength: 10,
  },
] as const;

export function findCountryByDialCode(dialCode: string): PhoneCountry | undefined {
  return PHONE_COUNTRIES.find((c) => c.dialCode === dialCode);
}

/**
 * Validates national subscriber number for a specific country dial code.
 */
export function validatePhoneForCountry(
  dialCode: string,
  nationalNumber: string,
  options?: { isInternational?: boolean },
): boolean {
  const digits = nationalNumber.replace(/\D/g, "");
  if (!digits) return false;

  // Thai domestic handling (leading 0)
  if (dialCode === "+66") {
    // If it is explicitly in international form (+66...), it must NOT have leading 0 (e.g. +660812345678 is invalid)
    if (options?.isInternational && digits.startsWith("0")) {
      return false;
    }

    if (digits.startsWith("0")) {
      // Mobile: 06, 08, 09 (10 digits); Landline: 02, 03, 04, 05, 07 (9 digits)
      const isMobile = /^0[689]\d{8}$/.test(digits);
      const isLandline = /^0[2-57]\d{7}$/.test(digits);
      return isMobile || isLandline;
    }
    // Without leading 0 (e.g. 812345678 or +66812345678)
    return /^([689]\d{8}|[2-57]\d{7})$/.test(digits);
  }

  const country = findCountryByDialCode(dialCode);
  if (country) {
    // International numbers should not have leading zero
    if (options?.isInternational && digits.startsWith("0")) {
      return false;
    }
    const sanitized = digits.replace(/^0+/, "");
    return country.nationalPattern.test(sanitized);
  }

  // Fallback E.164 subscriber length (4 to 14 digits)
  return digits.length >= 4 && digits.length <= 14;
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
