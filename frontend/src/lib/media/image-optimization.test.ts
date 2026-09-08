import { describe, it, expect } from "vitest";
import { optimizeImageToWebP } from "./image-optimization";

describe("image-optimization utils", () => {
  it("bypasses non-raster or non-image files gracefully", async () => {
    const textFile = new File(["dummy text content"], "sample.txt", { type: "text/plain" });
    const result = await optimizeImageToWebP(textFile);

    expect(result.isOptimized).toBe(false);
    expect(result.savedBytes).toBe(0);
    expect(result.file.name).toBe("sample.txt");
  });

  it("handles pdf or svg files without error", async () => {
    const pdfFile = new File(["%PDF-1.4..."], "doc.pdf", { type: "application/pdf" });
    const result = await optimizeImageToWebP(pdfFile);

    expect(result.isOptimized).toBe(false);
    expect(result.file.name).toBe("doc.pdf");
  });
});
