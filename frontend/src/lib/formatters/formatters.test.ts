import { describe, it, expect } from "vitest";
import {
  formatDate,
  formatDateRange,
  formatDateTime,
  formatTime,
  formatTimeToHHmm,
  formatTimeRange,
  toCalendarDateTime,
  formatCurrency,
  formatNumber,
  formatRelativeTime,
  formatBytes,
  formatPhoneNumber,
  formatDateTimeWithRelative,
} from "./formatters";

describe("formatters utils", () => {
  describe("formatDate & formatDateRange", () => {
    it("formats dates safely with locale", () => {
      expect(formatDate(null)).toBe("-");
      expect(formatDate(undefined)).toBe("-");
      expect(formatDate("invalid-date")).toBe("-");
      const d = "2026-09-08T00:00:00Z";
      expect(formatDate(d, "th")).toContain("2569");
      expect(formatDate(d, "en")).toContain("2026");
    });

    it("formats date ranges", () => {
      expect(formatDateRange(null, null)).toBe("-");
      expect(formatDateRange("2026-09-01", null, "en")).toContain("2026");
      const range = formatDateRange("2026-09-01", "2026-09-05", "en");
      expect(range).toContain(" - ");
    });
  });

  describe("formatDateTime", () => {
    it("formats date time", () => {
      expect(formatDateTime(null)).toBe("-");
      const dt = formatDateTime("2026-09-08T12:30:00Z", "en");
      expect(dt).toContain("2026");
    });
  });

  describe("formatTimeToHHmm", () => {
    it("converts ISO strings to HH:mm", () => {
      expect(formatTimeToHHmm("2000-01-01T09:00:00Z")).toBe("09:00");
      expect(formatTimeToHHmm("0000-01-01T15:30:00Z")).toBe("15:30");
      expect(formatTimeToHHmm("2026-08-14T07:15:00.000Z")).toBe("07:15");
    });

    it("converts time-only strings to HH:mm", () => {
      expect(formatTimeToHHmm("09:00")).toBe("09:00");
      expect(formatTimeToHHmm("09:00:00")).toBe("09:00");
      expect(formatTimeToHHmm("18:45:30")).toBe("18:45");
    });

    it("handles null, undefined and empty strings", () => {
      expect(formatTimeToHHmm(null)).toBe("");
      expect(formatTimeToHHmm(undefined)).toBe("");
      expect(formatTimeToHHmm("")).toBe("");
    });
  });

  describe("formatTime", () => {
    it("formats ISO timestamps to time string in UTC", () => {
      expect(formatTime("2000-01-01T09:00:00Z", "th")).toBe("09:00");
      expect(formatTime("2000-01-01T18:30:00Z", "en")).toBe("06:30 PM");
    });

    it("formats time-only strings", () => {
      expect(formatTime("09:00:00", "th")).toBe("09:00");
      expect(formatTime("18:30", "th")).toBe("18:30");
    });

    it("returns dash for null or undefined", () => {
      expect(formatTime(null)).toBe("-");
      expect(formatTime(undefined)).toBe("-");
    });
  });

  describe("formatTimeRange", () => {
    it("formats time range correctly from ISO strings", () => {
      expect(
        formatTimeRange("2000-01-01T09:00:00Z", "2000-01-01T10:30:00Z", "th")
      ).toBe("09:00 - 10:30");
    });

    it("handles single time provided", () => {
      expect(formatTimeRange("2000-01-01T09:00:00Z", null, "th")).toBe("09:00");
    });

    it("returns dash if both are null or invalid", () => {
      expect(formatTimeRange(null, null, "th")).toBe("-");
    });
  });

  describe("toCalendarDateTime", () => {
    it("formats calendar date and ISO time safely", () => {
      expect(
        toCalendarDateTime("2026-08-14", "2000-01-01T09:00:00Z")
      ).toBe("20260814T090000");
      expect(
        toCalendarDateTime("2026-08-14", "09:00:00")
      ).toBe("20260814T090000");
      expect(toCalendarDateTime("2026-08-14")).toBe("20260814");
      expect(toCalendarDateTime("")).toBe("");
    });
  });

  describe("formatNumber & formatCurrency", () => {
    it("formats numbers", () => {
      expect(formatNumber(null)).toBe("-");
      expect(formatNumber("invalid")).toBe("-");
      expect(formatNumber(1234567.89)).toBe("1,234,567.89");
    });

    it("formats currency defaults to THB", () => {
      expect(formatCurrency(null)).toBe("-");
      expect(formatCurrency(undefined)).toBe("-");
      expect(formatCurrency("invalid")).toBe("-");

      const thb = formatCurrency(1500, "THB", "th");
      expect(thb).toContain("1,500.00");

      const usd = formatCurrency(50.5, "USD", "en");
      expect(usd).toContain("50.50");
      expect(usd).toContain("$");
    });
  });

  describe("formatRelativeTime & formatDateTimeWithRelative", () => {
    it("formats relative time for recent moments", () => {
      expect(formatRelativeTime(null)).toBe("-");
      const now = new Date();
      expect(formatRelativeTime(now, "th")).toBe("เมื่อสักครู่");
      expect(formatRelativeTime(now, "en")).toBe("just now");

      const fiveMinutesAgo = new Date(Date.now() - 5 * 60 * 1000);
      expect(formatRelativeTime(fiveMinutesAgo, "th")).toBe("5 นาทีที่แล้ว");
      expect(formatRelativeTime(fiveMinutesAgo, "en")).toBe("5 minutes ago");
    });

    it("formats datetime with relative suffix", () => {
      expect(formatDateTimeWithRelative(null)).toBe("-");
      const now = new Date();
      const res = formatDateTimeWithRelative(now, "th");
      expect(res).toContain("เมื่อสักครู่");
    });
  });

  describe("formatBytes & formatPhoneNumber", () => {
    it("formats bytes accurately", () => {
      expect(formatBytes(0)).toBe("0 Bytes");
      expect(formatBytes(null)).toBe("0 Bytes");
      expect(formatBytes(1024)).toBe("1 KB");
      expect(formatBytes(1048576)).toBe("1 MB");
    });

    it("formats phone number", () => {
      expect(formatPhoneNumber(null)).toBe("-");
      expect(formatPhoneNumber("")).toBe("-");
      expect(formatPhoneNumber(" 081-234-5678 ")).toBe("081-234-5678");
    });
  });
});
