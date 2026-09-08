/**
 * Centralized formatting utilities for tan-erp.
 * Supports th and en locales with defaults tailored to Thai ERP operations.
 */

export function formatDate(
  dateStr: string | Date | null | undefined,
  locale: string = "th"
): string {
  if (!dateStr) return "-";
  const date = new Date(dateStr);
  if (isNaN(date.getTime())) return "-";

  return date.toLocaleDateString(locale === "th" ? "th-TH" : "en-US", {
    year: "numeric",
    month: "short",
    day: "numeric",
  });
}

export function formatDateRange(
  startDateStr: string | Date | null | undefined,
  endDateStr: string | Date | null | undefined,
  locale: string = "th"
): string {
  if (!startDateStr) return "-";
  const start = formatDate(startDateStr, locale);
  const end = endDateStr ? formatDate(endDateStr, locale) : "";

  if (start === end || !end || end === "-") return start;
  return `${start} - ${end}`;
}

export function formatDateTime(
  dateStr: string | Date | null | undefined,
  locale: string = "th"
): string {
  if (!dateStr) return "-";
  const date = new Date(dateStr);
  if (isNaN(date.getTime())) return "-";

  return date.toLocaleString(locale === "th" ? "th-TH" : "en-US", {
    year: "numeric",
    month: "short",
    day: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
}

export function formatTime(
  timeStr: string | Date | null | undefined,
  locale: string = "th"
): string {
  if (!timeStr) return "-";

  if (timeStr instanceof Date) {
    return timeStr.toLocaleTimeString(locale === "th" ? "th-TH" : "en-US", {
      hour: "2-digit",
      minute: "2-digit",
      timeZone: "UTC",
    });
  }

  const date = parseFlexibleDate(timeStr);
  if (!date) return "-";

  const localeTag = locale === "th" ? "th-TH" : "en-US";
  const formatOptions: Intl.DateTimeFormatOptions = {
    hour: "2-digit",
    minute: "2-digit",
    timeZone: "UTC",
  };

  return date.toLocaleTimeString(localeTag, formatOptions);
}

export function formatTimeToHHmm(
  timeStr: string | Date | null | undefined
): string {
  if (!timeStr) return "";
  if (timeStr instanceof Date) {
    const hours = timeStr.getUTCHours().toString().padStart(2, "0");
    const minutes = timeStr.getUTCMinutes().toString().padStart(2, "0");
    return `${hours}:${minutes}`;
  }
  if (typeof timeStr === "string") {
    const trimmed = timeStr.trim();
    if (!trimmed) return "";
    if (trimmed.includes("T")) {
      const timePart = trimmed.split("T")[1].replace("Z", "");
      const [hours = "00", minutes = "00"] = timePart.split(":");
      return `${hours.slice(0, 2).padStart(2, "0")}:${minutes.slice(0, 2).padStart(2, "0")}`;
    }
    if (isTimeOnlyValue(trimmed)) {
      const [hours = "00", minutes = "00"] = trimmed.split(":");
      return `${hours.padStart(2, "0")}:${minutes.padStart(2, "0")}`;
    }
    const parsed = parseFlexibleDate(trimmed);
    if (parsed) {
      const hours = parsed.getUTCHours().toString().padStart(2, "0");
      const minutes = parsed.getUTCMinutes().toString().padStart(2, "0");
      return `${hours}:${minutes}`;
    }
  }
  return "";
}

export function formatTimeRange(
  startTime: string | Date | null | undefined,
  endTime: string | Date | null | undefined,
  locale: string = "th"
): string {
  const start = formatTime(startTime, locale);
  const end = formatTime(endTime, locale);

  if (start === "-" && end === "-") return "-";
  if (start !== "-" && end !== "-" && start !== end) return `${start} - ${end}`;
  return start !== "-" ? start : end;
}

export function toCalendarDateTime(
  date: string,
  time?: string | null
): string {
  if (!date) return "";

  const d = new Date(date);
  if (isNaN(d.getTime())) return "";

  const isoDate = date.includes("T") ? date.split("T")[0] : date;
  const [year, month, day] = isoDate.split("-");

  if (!time) {
    return `${year}${month}${day}`;
  }

  let isoTime = time;
  if (time.includes("T")) {
    isoTime = time.split("T")[1].replace("Z", "");
  }

  const [hour = "00", minute = "00", second = "00"] = isoTime.split(":");
  const cleanSecond = second.split(".")[0];

  return `${year}${month}${day}T${hour}${minute}${cleanSecond}`;
}

export function formatNumber(num: number | string | null | undefined): string {
  if (num === null || num === undefined) return "-";
  const parsed = Number(num);
  if (isNaN(parsed)) return "-";

  return parsed.toLocaleString("th-TH", {
    minimumFractionDigits: 0,
    maximumFractionDigits: 2,
  });
}

export function formatCurrency(
  num: number | string | null | undefined,
  currency: string = "THB",
  locale: string = "th"
): string {
  if (num === null || num === undefined) return "-";
  const parsed = Number(num);
  if (isNaN(parsed)) return "-";

  const intlLocale = locale === "en" ? "en-US" : "th-TH";

  return new Intl.NumberFormat(intlLocale, {
    style: "currency",
    currency: currency,
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }).format(parsed);
}

function parseFlexibleDate(value: string): Date | null {
  if (value.includes("T")) {
    const timePart = value.split("T")[1].replace("Z", "");
    const [hours = "0", minutes = "0", seconds = "0"] = timePart.split(":");
    const parsedHours = Number(hours.slice(0, 2));
    const parsedMinutes = Number(minutes.slice(0, 2));
    const parsedSeconds = Number(seconds.split(".")[0]);
    if (!Number.isNaN(parsedHours) && !Number.isNaN(parsedMinutes)) {
      return new Date(Date.UTC(1970, 0, 1, parsedHours, parsedMinutes, Number.isNaN(parsedSeconds) ? 0 : parsedSeconds));
    }
  }

  if (isTimeOnlyValue(value)) {
    const [hours = "0", minutes = "0", seconds = "0"] = value.split(":");
    const parsedHours = Number(hours);
    const parsedMinutes = Number(minutes);
    const parsedSeconds = Number(seconds);

    if (!Number.isNaN(parsedHours) && !Number.isNaN(parsedMinutes)) {
      return new Date(Date.UTC(1970, 0, 1, parsedHours, parsedMinutes, Number.isNaN(parsedSeconds) ? 0 : parsedSeconds));
    }
  }

  const direct = new Date(value);
  if (!Number.isNaN(direct.getTime())) {
    return direct;
  }

  return null;
}

function isTimeOnlyValue(value: string): boolean {
  return /^\d{2}:\d{2}(:\d{2})?(?:\.\d+)?$/.test(value);
}

/**
 * formatRelativeTime: Formats a date into relative human-readable text.
 */
export function formatRelativeTime(
  dateInput: string | Date | null | undefined,
  locale: string = "th"
): string {
  if (!dateInput) return "-";
  const date = new Date(dateInput);
  if (isNaN(date.getTime())) return "-";

  const now = new Date();
  const diffInSeconds = Math.floor((now.getTime() - date.getTime()) / 1000);

  if (diffInSeconds < 60) {
    return locale === "th" ? "เมื่อสักครู่" : "just now";
  }

  const diffInMinutes = Math.floor(diffInSeconds / 60);
  if (diffInMinutes < 60) {
    return locale === "th"
      ? `${diffInMinutes} นาทีที่แล้ว`
      : `${diffInMinutes} minute${diffInMinutes > 1 ? "s" : ""} ago`;
  }

  const diffInHours = Math.floor(diffInMinutes / 60);
  if (diffInHours < 24) {
    return locale === "th"
      ? `${diffInHours} ชั่วโมงที่แล้ว`
      : `${diffInHours} hour${diffInHours > 1 ? "s" : ""} ago`;
  }

  const diffInDays = Math.floor(diffInHours / 24);
  if (diffInDays === 1) {
    return locale === "th" ? "เมื่อวานนี้" : "yesterday";
  }

  if (diffInDays < 30) {
    return locale === "th"
      ? `${diffInDays} วันที่แล้ว`
      : `${diffInDays} days ago`;
  }

  return formatDate(date, locale);
}

/**
 * formatBytes: Formats a byte number into human-readable size (e.g. "1.5 MB", "500 KB").
 */
export function formatBytes(bytes: number | null | undefined, decimals: number = 2): string {
  if (bytes === null || bytes === undefined || isNaN(bytes) || bytes === 0) return "0 Bytes";

  const k = 1024;
  const dm = decimals < 0 ? 0 : decimals;
  const sizes = ["Bytes", "KB", "MB", "GB", "TB"];

  const i = Math.floor(Math.log(bytes) / Math.log(k));
  const safeIndex = Math.min(i, sizes.length - 1);

  return `${parseFloat((bytes / Math.pow(k, safeIndex)).toFixed(dm))} ${sizes[safeIndex]}`;
}

/**
 * formatPhoneNumber: Cleans and ensures consistent format for telephone numbers.
 */
export function formatPhoneNumber(phone: string | null | undefined): string {
  if (!phone) return "-";
  const trimmed = phone.trim();
  if (!trimmed) return "-";
  return trimmed;
}

/**
 * formatDateTimeWithRelative: Formats a date into local date-time with relative time appended.
 */
export function formatDateTimeWithRelative(
  dateStr: string | Date | null | undefined,
  locale: string = "th"
): string {
  if (!dateStr) return "-";
  const dt = formatDateTime(dateStr, locale);
  if (dt === "-") return "-";
  const rel = formatRelativeTime(dateStr, locale);
  if (rel === "-" || rel === dt) return dt;
  return `${dt} (${rel})`;
}
