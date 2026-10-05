import { describe, expect, it } from "vitest";
import { dataUrlToPngFile } from "./data-url-to-file";

// 8-byte PNG signature, base64 encoded.
const PNG_DATA_URL = "data:image/png;base64,iVBORw0KGgo=";

// jsdom File has no arrayBuffer(); read through FileReader instead.
function readAsArrayBuffer(file: File): Promise<ArrayBuffer> {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.onload = () => (reader.result instanceof ArrayBuffer ? resolve(reader.result) : reject(new Error("Unexpected result")));
    reader.onerror = () => reject(reader.error);
    reader.readAsArrayBuffer(file);
  });
}

describe("dataUrlToPngFile", () => {
  it("builds a PNG File with the decoded bytes", async () => {
    const file = dataUrlToPngFile(PNG_DATA_URL, "signature.png");

    expect(file.name).toBe("signature.png");
    expect(file.type).toBe("image/png");
    expect(file.size).toBe(8);
    const bytes = new Uint8Array(await readAsArrayBuffer(file));
    expect(Array.from(bytes)).toEqual([0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a]);
  });

  it.each(["", "data:image/jpeg;base64,/9j/4AAQ", "iVBORw0KGgo=", "data:image/png;base64,@@@"])("rejects %s", (value) => {
    expect(() => dataUrlToPngFile(value, "signature.png")).toThrow();
  });
});
