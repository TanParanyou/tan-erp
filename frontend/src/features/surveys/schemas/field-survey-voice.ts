export interface ParsedVoiceMeasurement {
  title: string | null;
  widthMm: number | null;
  depthMm: number | null;
  heightMm: number | null;
}

type DimensionKey = "widthMm" | "depthMm" | "heightMm";

const THAI_DIGITS = "๐๑๒๓๔๕๖๗๘๙";

const DIMENSION_WORDS: ReadonlyArray<readonly [DimensionKey, string]> = [
  ["widthMm", "กว้าง|width"],
  ["depthMm", "ลึก|depth"],
  ["heightMm", "สูง|height"],
];

const NUMBER = "(\\d+(?:\\.\\d+)?)";
// Longer units first so "เซนติเมตร" is not read as "เมตร" and "mm" is not read as "m"
const UNIT = "(เซนติเมตร|เซ็นติเมตร|มิลลิเมตร|เมตร|ซม\\.?|เซน|มม\\.?|cm|mm|m|ม\\.?)";

const MILLIMETERS_PER_UNIT: Readonly<Record<string, number>> = {
  เมตร: 1000,
  m: 1000,
  ม: 1000,
  เซนติเมตร: 10,
  เซ็นติเมตร: 10,
  ซม: 10,
  เซน: 10,
  cm: 10,
  มิลลิเมตร: 1,
  มม: 1,
  mm: 1,
};

const DIGIT_WORDS: Readonly<Record<string, number>> = {
  ศูนย์: 0,
  หนึ่ง: 1,
  เอ็ด: 1,
  สอง: 2,
  ยี่: 2,
  สาม: 3,
  สี่: 4,
  ห้า: 5,
  หก: 6,
  เจ็ด: 7,
  แปด: 8,
  เก้า: 9,
};
const MULTIPLIER_WORDS: Readonly<Record<string, number>> = { สิบ: 10, ร้อย: 100, พัน: 1000 };
const NUMBER_WORD = "ศูนย์|หนึ่ง|เอ็ด|สอง|ยี่|สาม|สี่|ห้า|หก|เจ็ด|แปด|เก้า|สิบ|ร้อย|พัน";
const FRACTION_WORD = "ศูนย์|หนึ่ง|สอง|สาม|สี่|ห้า|หก|เจ็ด|แปด|เก้า";
const NUMBER_WORD_RUN = new RegExp(`(?:${NUMBER_WORD})+(?:จุด(?:${FRACTION_WORD})+)?`, "g");

function toMillimeters(value: string, unit: string): number | null {
  const factor = MILLIMETERS_PER_UNIT[unit.replace(/\./g, "").toLowerCase()];
  const millimeters = Number(value) * factor;
  return factor !== undefined && Number.isFinite(millimeters) ? millimeters : null;
}

function normalizeTranscript(text: string): string {
  return text
    .replace(/[๐-๙]/g, (digit) => String(THAI_DIGITS.indexOf(digit)))
    .replace(/(\d),(?=\d)/g, "$1")
    .trim();
}

/** "หนึ่งร้อยยี่สิบ" -> 120, "สิบเอ็ด" -> 11 (integers up to 9,999). */
function thaiWordsToInteger(words: string): number {
  let total = 0;
  let digit: number | null = null;
  for (const match of words.matchAll(new RegExp(NUMBER_WORD, "g"))) {
    const word = match[0];
    if (word in DIGIT_WORDS) {
      digit = DIGIT_WORDS[word];
    } else {
      total += (digit ?? 1) * MULTIPLIER_WORDS[word];
      digit = null;
    }
  }
  return total + (digit ?? 0);
}

/** Replaces spoken Thai numbers such as "สองจุดสี่" with digits ("2.4"). */
function convertThaiNumberWords(text: string): string {
  return text.replace(NUMBER_WORD_RUN, (run) => {
    const [integerPart, fractionPart] = run.split("จุด");
    const fraction = fractionPart
      ? Array.from(fractionPart.matchAll(new RegExp(FRACTION_WORD, "g")), (m) => DIGIT_WORDS[m[0]]).join("")
      : "";
    return ` ${thaiWordsToInteger(integerPart)}${fraction ? `.${fraction}` : ""} `;
  });
}

/**
 * Deterministic parser for spoken Thai/English measurements such as
 * "ตู้เสื้อผ้า กว้าง 2.4 เมตร สูง 2.6 เมตร", "กว้าง สองจุดสี่ เมตร" or "กว้าง 2 เมตร 40 เซนติเมตร".
 * A dimension is only accepted when its number carries an explicit unit: a bare number is never
 * guessed as metres or centimetres, and "2 เมตร 40" (a trailing number without a unit) is rejected
 * as ambiguous rather than silently read as 2 m. Spoken number words are converted only after the
 * first dimension word, so item names such as "ตู้ห้าชั้น" stay untouched.
 */
export function parseVoiceMeasurement(transcript: string): ParsedVoiceMeasurement {
  const original = normalizeTranscript(transcript);
  const result: ParsedVoiceMeasurement = { title: null, widthMm: null, depthMm: null, heightMm: null };

  // The title ends at the first dimension word even when its number has no usable unit
  let firstDimensionIndex = original.length;
  for (const [, words] of DIMENSION_WORDS) {
    const keywordIndex = original.search(new RegExp(`(?:${words})`, "i"));
    if (keywordIndex >= 0) firstDimensionIndex = Math.min(firstDimensionIndex, keywordIndex);
  }

  const head = original.slice(0, firstDimensionIndex);
  const tail = convertThaiNumberWords(original.slice(firstDimensionIndex));

  for (const [key, words] of DIMENSION_WORDS) {
    const pattern = new RegExp(
      `(?:${words})\\s*(?:ประมาณ|ราว|about)?\\s*${NUMBER}\\s*${UNIT}(?:\\s*(?:กับ|และ|and)?\\s*${NUMBER}\\s*${UNIT})?`,
      "i"
    );
    const match = pattern.exec(tail);
    if (!match) continue;

    // "2 เมตร 40": the trailing number has no unit, so the value is ambiguous
    const rest = tail.slice(match.index + match[0].length);
    if (!match[3] && /^\s*(?:กับ|และ|and)?\s*\d/.test(rest)) continue;

    const first = toMillimeters(match[1], match[2]);
    const second = match[3] && match[4] ? toMillimeters(match[3], match[4]) : 0;
    if (first === null || second === null) continue;

    const total = Math.round(first + second);
    if (total <= 0) continue;

    result[key] = total;
  }

  const title = head.replace(/\s*(ขนาด|size)\s*$/i, "").trim();
  result.title = title.length > 0 ? title : null;
  return result;
}

export function hasParsedDimensions(parsed: ParsedVoiceMeasurement): boolean {
  return parsed.widthMm !== null || parsed.depthMm !== null || parsed.heightMm !== null;
}
