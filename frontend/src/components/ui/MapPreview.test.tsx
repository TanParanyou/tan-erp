import React from "react";
import { describe, it, expect } from "vitest";
import { render, screen } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import { MapPreview } from "./MapPreview";

const renderWithIntl = (ui: React.ReactElement) => {
  return render(
    <NextIntlClientProvider locale="th" messages={thMessages}>
      {ui}
    </NextIntlClientProvider>
  );
};

describe("MapPreview", () => {
  it("renders null when latitude or longitude is null or undefined", () => {
    const { container: c1 } = renderWithIntl(
      <MapPreview latitude={null} longitude={100.5} />
    );
    expect(c1.firstChild).toBeNull();

    const { container: c2 } = renderWithIntl(
      <MapPreview latitude={13.75} longitude={undefined} />
    );
    expect(c2.firstChild).toBeNull();

    const { container: c3 } = renderWithIntl(
      <MapPreview latitude={null} longitude={null} />
    );
    expect(c3.firstChild).toBeNull();
  });

  it("renders iframe and external link when valid coordinates are given", () => {
    renderWithIntl(<MapPreview latitude={13.8064145} longitude={100.52841} />);

    const iframe = screen.getByTitle("แผนที่ตำแหน่งพิกัด") as HTMLIFrameElement;
    expect(iframe).toBeInTheDocument();
    expect(iframe.src).toContain("13.8064145,100.52841");

    const link = screen.getByRole("link", { name: /เปิดดูบนแผนที่/ });
    expect(link).toHaveAttribute(
      "href",
      "https://www.google.com/maps/search/?api=1&query=13.8064145,100.52841"
    );
  });

  it("hides external link when showExternalLink is false", () => {
    renderWithIntl(
      <MapPreview
        latitude={13.8064145}
        longitude={100.52841}
        showExternalLink={false}
      />
    );

    expect(screen.queryByRole("link", { name: /เปิดดูบนแผนที่/ })).not.toBeInTheDocument();
  });
});
