import { describe, it, expect } from "vitest";
import {
  formatCurrency,
  formatNumber,
  formatDate,
  formatDateTime,
  formatFileSize,
} from "./formatters";

describe("formatters", () => {
  describe("formatCurrency", () => {
    it("formats THB currency correctly by default", () => {
      const result = formatCurrency(1250);
      expect(result).toBeDefined();
      expect(result).toContain("1,250.00");
    });

    it("handles null or undefined safely", () => {
      expect(formatCurrency(null)).toBe("-");
      expect(formatCurrency(undefined)).toBe("-");
      expect(formatCurrency("")).toBe("-");
      expect(formatCurrency("abc")).toBe("-");
    });

    it("respects decimal places", () => {
      const result = formatCurrency(100.5, "THB", "th", 0);
      expect(result).toBeDefined();
    });
  });

  describe("formatNumber", () => {
    it("formats number with thousand separators", () => {
      const formatted = formatNumber(1234567.89, 2, 2, "en");
      expect(formatted).toBe("1,234,567.89");
    });

    it("handles zero and empty values", () => {
      expect(formatNumber(0)).toBe("0");
      expect(formatNumber(null)).toBe("-");
    });
  });

  describe("formatDate and formatDateTime", () => {
    it("formats valid date string", () => {
      const date = "2026-09-06T12:00:00Z";
      expect(formatDate(date, "en")).toContain("2026");
      expect(formatDateTime(date, "en")).toContain("2026");
    });

    it("returns dash for invalid or null dates", () => {
      expect(formatDate(null)).toBe("-");
      expect(formatDate("invalid-date")).toBe("-");
    });
  });

  describe("formatFileSize", () => {
    it("formats bytes into human readable units", () => {
      expect(formatFileSize(0)).toBe("0 B");
      expect(formatFileSize(1024)).toBe("1 KB");
      expect(formatFileSize(1024 * 1024 * 2.5)).toBe("2.5 MB");
    });

    it("handles negative and null values", () => {
      expect(formatFileSize(null)).toBe("-");
      expect(formatFileSize(-10)).toBe("-");
    });
  });
});
