import React from "react";
import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, act } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import { LightboxProvider } from "@/providers/lightbox-provider";
import { CatalogItemDetailDrawer } from "./CatalogItemDetailDrawer";
import type { CatalogItem } from "@/features/estimates/constants/estimate-catalog-items";

const mockItem: CatalogItem = {
  id: "item-001",
  code: "WD-HMR-15",
  name: {
    th: "ไม้ HMR กันชื้น 15 มม.",
    en: "HMR Moisture Resistant Board 15mm",
  },
  aliases: [
    { th: "ไม้เขียว 15มิล", en: "Green Board 15mm" },
  ],
  description: {
    th: "แผ่นใยไม้อัดทนความชื้นสูง สำหรับงานเฟอร์นิเจอร์บิวท์อิน",
    en: "High moisture resistant fiberboard for built-in furniture",
  },
  itemType: "material",
  category: {
    id: "wood",
    name: { th: "ไม้และแผ่นบอร์ด", en: "Wood & Boards" },
  },
  subCategory: {
    id: "sub-hmr",
    name: { th: "แผ่น HMR", en: "HMR Board" },
  },
  brand: {
    id: "brand-vanachai",
    name: { th: "วนชัย", en: "Vanachai" },
  },
  supplier: {
    id: "sup-001",
    code: "SUP-VNC",
    name: { th: "บริษัท วนชัย กรุ๊ป จำกัด", en: "Vanachai Group Co., Ltd." },
  },
  imageUrl: "https://example.com/hmr-15.jpg",
  images: ["https://example.com/hmr-15.jpg", "https://example.com/hmr-15-edge.jpg"],
  status: "active",
  specs: {
    thickness: "15 mm",
    dimensions: "1220 x 2440 mm",
  },
  attributes: {
    density: "730 kg/m3",
  },
  pricing: {
    defaultUnitCost: 560,
    currency: "THB",
    baseUnitCode: "SHEET",
  },
  capabilities: {
    canCost: true,
    canSell: true,
  },
};

function renderDrawer(props: {
  item: CatalogItem | null;
  isOpen: boolean;
  onClose?: () => void;
}) {
  return render(
    <NextIntlClientProvider locale="th" messages={thMessages}>
      <LightboxProvider>
        <CatalogItemDetailDrawer
          item={props.item}
          isOpen={props.isOpen}
          onClose={props.onClose || vi.fn()}
        />
      </LightboxProvider>
    </NextIntlClientProvider>
  );
}

describe("CatalogItemDetailDrawer component", () => {
  beforeEach(() => {
    Object.assign(navigator, {
      clipboard: {
        writeText: vi.fn().mockResolvedValue(undefined),
      },
    });
  });

  it("does not render when isOpen is false or item is null", () => {
    const { rerender } = renderDrawer({ item: null, isOpen: true });
    expect(screen.queryByRole("dialog")).not.toBeInTheDocument();

    rerender(
      <NextIntlClientProvider locale="th" messages={thMessages}>
        <LightboxProvider>
          <CatalogItemDetailDrawer item={mockItem} isOpen={false} onClose={vi.fn()} />
        </LightboxProvider>
      </NextIntlClientProvider>
    );
    expect(screen.queryByRole("dialog")).not.toBeInTheDocument();
  });

  it("renders item details properly when open", () => {
    renderDrawer({ item: mockItem, isOpen: true });

    expect(screen.getByRole("dialog")).toBeInTheDocument();
    expect(screen.getByText("WD-HMR-15")).toBeInTheDocument();
    expect(screen.getByText("ไม้ HMR กันชื้น 15 มม.")).toBeInTheDocument();
    expect(screen.getByText("HMR Moisture Resistant Board 15mm")).toBeInTheDocument();
    expect(screen.getByText("วนชัย")).toBeInTheDocument();
    expect(screen.getByText("ไม้และแผ่นบอร์ด")).toBeInTheDocument();
    expect(screen.getByText("แผ่น HMR")).toBeInTheDocument();
    expect(screen.getByText("SHEET")).toBeInTheDocument();
    expect(screen.getByText("560.00 THB")).toBeInTheDocument();
    expect(screen.getByText("บริษัท วนชัย กรุ๊ป จำกัด")).toBeInTheDocument();
  });

  it("copies item code using central CopyButton", async () => {
    renderDrawer({ item: mockItem, isOpen: true });

    const copyCodeBtn = screen.getByLabelText(thMessages.estimates.catalogDetail.copyCode);
    expect(copyCodeBtn).toBeInTheDocument();

    await act(async () => {
      fireEvent.click(copyCodeBtn);
    });

    expect(navigator.clipboard.writeText).toHaveBeenCalledWith("WD-HMR-15");
  });

  it("calls onClose when close button is clicked", () => {
    const onClose = vi.fn();
    renderDrawer({ item: mockItem, isOpen: true, onClose });

    const closeBtn = screen.getByLabelText("Close drawer");
    fireEvent.click(closeBtn);

    expect(onClose).toHaveBeenCalledTimes(1);
  });
});
