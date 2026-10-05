import React from "react";
import { describe, it, expect, vi } from "vitest";
import { render, screen } from "@testing-library/react";
import { ItemAttributesDisplay } from "./ItemAttributesDisplay";

// Mock next-intl hooks
vi.mock("next-intl", () => ({
  useLocale: () => "th",
  useTranslations: () => (key: string) => {
    const messages: Record<string, string> = {
      noAttributes: "ยังไม่มีคุณสมบัติเพิ่มเติม",
      copyAllAttributes: "คัดลอกสเปกทั้งหมด",
      copyAttributeValue: "คัดลอกค่า",
      copiedAttributes: "คัดลอกแล้ว",
    };
    return messages[key] ?? key;
  },
}));

describe("ItemAttributesDisplay component", () => {
  it("renders empty fallback when attributes are null or empty", () => {
    const { rerender } = render(<ItemAttributesDisplay attributes={null} />);
    expect(screen.getByText("ยังไม่มีคุณสมบัติเพิ่มเติม")).toBeInTheDocument();

    rerender(<ItemAttributesDisplay attributes={{}} />);
    expect(screen.getByText("ยังไม่มีคุณสมบัติเพิ่มเติม")).toBeInTheDocument();

    rerender(
      <ItemAttributesDisplay
        attributes={null}
        emptyFallback={<span data-testid="custom-empty">-</span>}
      />
    );
    expect(screen.getByTestId("custom-empty")).toBeInTheDocument();
  });

  it("renders table variant correctly with labels and values", () => {
    render(
      <ItemAttributesDisplay
        attributes={{
          thickness_mm: "18",
          color: "ขาวด้าน",
        }}
        variant="table"
      />
    );

    expect(screen.getByText("ความหนา (มม.)")).toBeInTheDocument();
    expect(screen.getByText("thickness_mm")).toBeInTheDocument();
    expect(screen.getByText("18")).toBeInTheDocument();

    expect(screen.getByText("สี")).toBeInTheDocument();
    expect(screen.getByText("color")).toBeInTheDocument();
    expect(screen.getByText("ขาวด้าน")).toBeInTheDocument();
  });

  it("renders badge variant correctly", () => {
    render(
      <ItemAttributesDisplay
        attributes={[{ key: "material", value: "ไม้อัดยาง" }]}
        variant="badge"
      />
    );

    expect(screen.getByText("วัสดุหลัก:")).toBeInTheDocument();
    expect(screen.getByText("ไม้อัดยาง")).toBeInTheDocument();
  });

  it("renders compact variant correctly", () => {
    render(
      <ItemAttributesDisplay
        attributes={{ thickness_mm: "18" }}
        variant="compact"
      />
    );

    expect(screen.getByText("ความหนา (มม.)")).toBeInTheDocument();
    expect(screen.getByText("18")).toBeInTheDocument();
  });
});
