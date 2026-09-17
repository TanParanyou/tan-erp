import React from "react";
import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import { GalleryLightboxModal, type GalleryItemMetadata } from "./GalleryLightboxModal";
import thMessages from "@/messages/th.json";

function renderWithProviders(ui: React.ReactElement) {
  return render(
    <NextIntlClientProvider locale="th" messages={thMessages}>
      {ui}
    </NextIntlClientProvider>
  );
}

describe("GalleryLightboxModal component", () => {
  const mockItems: GalleryItemMetadata[] = [
    {
      id: "img-1",
      imageUrl: "https://example.com/photo1.webp",
      caption: "มุมหน้างานห้องรับแขก",
      createdByName: "สมชาย ช่างสำรวจ",
      createdAtUtc: "2026-09-17T10:00:00Z",
      stageBadge: <span>สำรวจ</span>,
      canDetach: true,
      onDetach: vi.fn(),
    },
    {
      id: "img-2",
      imageUrl: "https://example.com/photo2.webp",
      caption: "มุมติดตั้งตู้บิวต์อิน",
      createdByName: "สมหญิง ผู้ดูแล",
      createdAtUtc: "2026-09-17T11:00:00Z",
      stageBadge: <span>ประมาณราคา</span>,
      canDetach: false,
    },
  ];

  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("does not render when isOpen is false", () => {
    const { container } = renderWithProviders(
      <GalleryLightboxModal
        isOpen={false}
        onClose={vi.fn()}
        items={mockItems}
        currentIndex={0}
        onIndexChange={vi.fn()}
      />
    );
    expect(container.firstChild).toBeNull();
  });

  it("renders image, counter badge, caption, and metadata when open", () => {
    renderWithProviders(
      <GalleryLightboxModal
        isOpen={true}
        onClose={vi.fn()}
        items={mockItems}
        currentIndex={0}
        onIndexChange={vi.fn()}
      />
    );

    expect(screen.getByText("มุมหน้างานห้องรับแขก")).toBeInTheDocument();
    expect(screen.getByText("สมชาย ช่างสำรวจ")).toBeInTheDocument();
    expect(screen.getByText("1 / 2")).toBeInTheDocument();
    expect(screen.getAllByText("สำรวจ")[0]).toBeInTheDocument();

    const images = screen.getAllByRole("img", { name: "มุมหน้างานห้องรับแขก" });
    expect(images[0]).toHaveAttribute("src", "https://example.com/photo1.webp");
  });

  it("handles next and previous navigation clicks", () => {
    const onIndexChange = vi.fn();

    renderWithProviders(
      <GalleryLightboxModal
        isOpen={true}
        onClose={vi.fn()}
        items={mockItems}
        currentIndex={0}
        onIndexChange={onIndexChange}
      />
    );

    const nextBtn = screen.getByRole("button", { name: /รูปถัดไป/i });
    fireEvent.click(nextBtn);
    expect(onIndexChange).toHaveBeenCalledWith(1);

    const prevBtn = screen.getByRole("button", { name: /รูปก่อนหน้า/i });
    fireEvent.click(prevBtn);
    expect(onIndexChange).toHaveBeenCalledWith(1); // wrap around from index 0 to 1
  });

  it("handles thumbnail click to switch active index", () => {
    const onIndexChange = vi.fn();

    renderWithProviders(
      <GalleryLightboxModal
        isOpen={true}
        onClose={vi.fn()}
        items={mockItems}
        currentIndex={0}
        onIndexChange={onIndexChange}
      />
    );

    const thumbnails = screen.getAllByRole("button", { name: /รูปภาพที่/i });
    expect(thumbnails).toHaveLength(2);

    fireEvent.click(thumbnails[1]);
    expect(onIndexChange).toHaveBeenCalledWith(1);
  });

  it("navigates with keyboard arrow keys and closes on Escape", () => {
    const onClose = vi.fn();
    const onIndexChange = vi.fn();

    renderWithProviders(
      <GalleryLightboxModal
        isOpen={true}
        onClose={onClose}
        items={mockItems}
        currentIndex={0}
        onIndexChange={onIndexChange}
      />
    );

    fireEvent.keyDown(window, { key: "ArrowRight" });
    expect(onIndexChange).toHaveBeenCalledWith(1);

    fireEvent.keyDown(window, { key: "ArrowLeft" });
    expect(onIndexChange).toHaveBeenCalledWith(1); // from 0 wraps to 1

    fireEvent.keyDown(window, { key: "Escape" });
    expect(onClose).toHaveBeenCalled();
  });

  it("handles detach action callback", () => {
    const onDetach = vi.fn();
    const itemsWithDetach = [
      {
        ...mockItems[0],
        onDetach,
        canDetach: true,
      },
    ];

    renderWithProviders(
      <GalleryLightboxModal
        isOpen={true}
        onClose={vi.fn()}
        items={itemsWithDetach}
        currentIndex={0}
        onIndexChange={vi.fn()}
      />
    );

    const detachBtn = screen.getByRole("button", { name: /ลบ/i });
    fireEvent.click(detachBtn);
    expect(onDetach).toHaveBeenCalled();
  });
});
