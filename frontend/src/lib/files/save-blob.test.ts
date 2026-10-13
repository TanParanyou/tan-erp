import { afterEach, describe, expect, it, vi } from "vitest";
import { saveBlob } from "./save-blob";

describe("saveBlob", () => {
  afterEach(() => vi.restoreAllMocks());

  it("clicks a hidden download link with the file name and releases the object URL", () => {
    const createObjectURL = vi.fn(() => "blob:mock-pdf");
    const revokeObjectURL = vi.fn();
    Object.assign(URL, { createObjectURL, revokeObjectURL });
    const click = vi.spyOn(HTMLAnchorElement.prototype, "click").mockImplementation(() => undefined);

    saveBlob(new Blob(["%PDF-"], { type: "application/pdf" }), "QT-2026-0001.pdf");

    expect(createObjectURL).toHaveBeenCalledTimes(1);
    expect(click).toHaveBeenCalledTimes(1);
    expect(revokeObjectURL).toHaveBeenCalledWith("blob:mock-pdf");
    expect(document.querySelector("a[download]")).toBeNull();
  });
});
