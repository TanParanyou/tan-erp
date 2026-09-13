import React from "react";
import { describe, it, expect } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import { LightboxProvider } from "@/providers/lightbox-provider";
import { ImageGallery } from "./ImageGallery";

function renderWithProviders(ui: React.ReactNode) {
  return render(
    <NextIntlClientProvider locale="th" messages={thMessages}>
      <LightboxProvider>{ui}</LightboxProvider>
    </NextIntlClientProvider>
  );
}

describe("ImageGallery component", () => {
  it("renders empty state when no images are provided", () => {
    renderWithProviders(<ImageGallery images={[]} />);

    expect(
      screen.getByText(thMessages.estimates.catalogDetail.noImage)
    ).toBeInTheDocument();
  });

  it("renders single image without thumbnails", () => {
    const images = ["https://example.com/single.jpg"];
    renderWithProviders(<ImageGallery images={images} alt="Single Product" />);

    const mainImg = screen.getByAltText("Single Product");
    expect(mainImg).toBeInTheDocument();
    expect(mainImg).toHaveAttribute("src", "https://example.com/single.jpg");

    // Counter badge and thumbnails shouldn't appear for a single image
    expect(screen.queryByText("1 / 1")).not.toBeInTheDocument();
  });

  it("renders multiple images with counter and allows switching thumbnails", () => {
    const images = [
      "https://example.com/img1.jpg",
      "https://example.com/img2.jpg",
      "https://example.com/img3.jpg",
    ];
    renderWithProviders(<ImageGallery images={images} alt="Multi Product" />);

    expect(screen.getByText("1 / 3")).toBeInTheDocument();

    const thumbnails = screen.getAllByRole("button");
    // Click second thumbnail (thumbnails start after main image clickable button if any)
    const secondThumb = screen.getByLabelText(`${thMessages.estimates.catalogDetail.image} 2`);
    fireEvent.click(secondThumb);

    expect(screen.getByText("2 / 3")).toBeInTheDocument();
  });

  it("triggers lightbox on main image click when enabled", () => {
    const images = ["https://example.com/img1.jpg"];
    renderWithProviders(
      <ImageGallery images={images} alt="Test Product" title="Test Title" enableLightbox={true} />
    );

    const mainImageButton = screen.getByRole("button", {
      name: thMessages.estimates.catalogDetail.viewDetail,
    });
    fireEvent.click(mainImageButton);

    expect(screen.getByRole("dialog")).toBeInTheDocument();
    expect(screen.getByText("Test Title")).toBeInTheDocument();
  });
});
