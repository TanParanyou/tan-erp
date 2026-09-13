import React from "react";
import { describe, it, expect } from "vitest";
import { render, screen, fireEvent, act } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import { LightboxProvider, useLightbox } from "./lightbox-provider";

function TestControlsComponent({ images }: { images: string[] }) {
  const {
    openLightbox,
    zoomIn,
    zoomOut,
    rotateCw,
    rotateCcw,
    resetAll,
    scale,
    rotation,
    isZoomed,
    currentImage,
  } = useLightbox();

  return (
    <div>
      <button
        type="button"
        onClick={() => openLightbox({ images, initialIndex: 0, title: "Product Detail" })}
      >
        Open Lightbox
      </button>
      <button type="button" onClick={zoomIn}>
        Trigger Zoom In
      </button>
      <button type="button" onClick={zoomOut}>
        Trigger Zoom Out
      </button>
      <button type="button" onClick={rotateCw}>
        Trigger Rotate CW
      </button>
      <button type="button" onClick={rotateCcw}>
        Trigger Rotate CCW
      </button>
      <button type="button" onClick={resetAll}>
        Trigger Reset All
      </button>
      <span data-testid="scale-val">{scale}</span>
      <span data-testid="rotation-val">{rotation}</span>
      <span data-testid="is-zoomed">{isZoomed ? "yes" : "no"}</span>
      <span data-testid="current-image">{currentImage}</span>
    </div>
  );
}

function renderWithProvider(ui: React.ReactNode) {
  return render(
    <NextIntlClientProvider locale="th" messages={thMessages}>
      <LightboxProvider>{ui}</LightboxProvider>
    </NextIntlClientProvider>
  );
}

describe("LightboxProvider and useLightbox hook", () => {
  it("opens lightbox and exposes current image", () => {
    const images = ["https://example.com/img1.jpg", "https://example.com/img2.jpg"];
    renderWithProvider(<TestControlsComponent images={images} />);

    expect(screen.queryByRole("dialog")).not.toBeInTheDocument();
    expect(screen.getByTestId("current-image")).toHaveTextContent("");

    fireEvent.click(screen.getByText("Open Lightbox"));

    expect(screen.getByRole("dialog")).toBeInTheDocument();
    expect(screen.getByText("Product Detail")).toBeInTheDocument();
    expect(screen.getByText("1 / 2")).toBeInTheDocument();
    expect(screen.getByTestId("current-image")).toHaveTextContent("https://example.com/img1.jpg");
  });

  it("controls zoom level via hook and UI controls", () => {
    const images = ["https://example.com/img1.jpg"];
    renderWithProvider(<TestControlsComponent images={images} />);

    fireEvent.click(screen.getByText("Open Lightbox"));
    expect(screen.getByTestId("scale-val")).toHaveTextContent("1");
    expect(screen.getByTestId("is-zoomed")).toHaveTextContent("no");

    // Zoom in via hook
    act(() => {
      fireEvent.click(screen.getByText("Trigger Zoom In"));
    });
    expect(screen.getByTestId("scale-val")).toHaveTextContent("1.5");
    expect(screen.getByTestId("is-zoomed")).toHaveTextContent("yes");

    // Zoom in via UI button
    const zoomInBtn = screen.getByLabelText(thMessages.common.lightbox.zoomIn);
    act(() => {
      fireEvent.click(zoomInBtn);
    });
    expect(screen.getByTestId("scale-val")).toHaveTextContent("2");

    // Zoom out
    act(() => {
      fireEvent.click(screen.getByText("Trigger Zoom Out"));
    });
    expect(screen.getByTestId("scale-val")).toHaveTextContent("1.5");
  });

  it("rotates image clockwise and counter-clockwise", () => {
    const images = ["https://example.com/img1.jpg"];
    renderWithProvider(<TestControlsComponent images={images} />);

    fireEvent.click(screen.getByText("Open Lightbox"));
    expect(screen.getByTestId("rotation-val")).toHaveTextContent("0");

    // Rotate CW via UI button
    const rotateCwBtn = screen.getByLabelText(thMessages.common.lightbox.rotateCw);
    act(() => {
      fireEvent.click(rotateCwBtn);
    });
    expect(screen.getByTestId("rotation-val")).toHaveTextContent("90");

    // Rotate CCW via UI button
    const rotateCcwBtn = screen.getByLabelText(thMessages.common.lightbox.rotateCcw);
    act(() => {
      fireEvent.click(rotateCcwBtn);
    });
    expect(screen.getByTestId("rotation-val")).toHaveTextContent("0");
  });

  it("supports keyboard shortcuts: zoom, rotate, reset, escape", () => {
    const images = ["https://example.com/img1.jpg"];
    renderWithProvider(<TestControlsComponent images={images} />);

    fireEvent.click(screen.getByText("Open Lightbox"));

    // Keyboard zoom in (+)
    act(() => {
      fireEvent.keyDown(window, { key: "+" });
    });
    expect(screen.getByTestId("scale-val")).toHaveTextContent("1.5");

    // Keyboard rotate (])
    act(() => {
      fireEvent.keyDown(window, { key: "]" });
    });
    expect(screen.getByTestId("rotation-val")).toHaveTextContent("90");

    // Keyboard reset (r)
    act(() => {
      fireEvent.keyDown(window, { key: "r" });
    });
    expect(screen.getByTestId("scale-val")).toHaveTextContent("1");
    expect(screen.getByTestId("rotation-val")).toHaveTextContent("0");

    // Keyboard escape to close
    act(() => {
      fireEvent.keyDown(window, { key: "Escape" });
    });
    expect(screen.queryByRole("dialog")).not.toBeInTheDocument();
  });
});
