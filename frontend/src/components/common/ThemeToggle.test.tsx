import { render, screen, fireEvent } from "@testing-library/react";
import { describe, it, expect, beforeEach } from "vitest";
import React from "react";
import { ThemeProvider } from "@/providers/theme-provider";
import { ThemeToggle } from "./ThemeToggle";
import { NextIntlClientProvider } from "next-intl";

const mockMessages = {
  shell: {
    toggleTheme: "เปลี่ยนธีม",
    lightMode: "โหมดสว่าง",
    darkMode: "โหมดมืด",
  },
};

describe("ThemeToggle Component", () => {
  beforeEach(() => {
    localStorage.clear();
    document.documentElement.removeAttribute("data-theme");
  });

  it("renders with light mode by default and allows toggling to dark mode", () => {
    render(
      <NextIntlClientProvider locale="th" messages={mockMessages}>
        <ThemeProvider defaultTheme="light">
          <ThemeToggle />
        </ThemeProvider>
      </NextIntlClientProvider>
    );

    const button = screen.getByRole("button", { name: /เปลี่ยนธีม/i });
    expect(button).toBeDefined();
    expect(document.documentElement.getAttribute("data-theme")).toBe("light");

    fireEvent.click(button);

    expect(document.documentElement.getAttribute("data-theme")).toBe("dark");
  });
});
