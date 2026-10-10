const PNG_DATA_URL_PREFIX = "data:image/png;base64,";

/** Converts a PNG data URL (as produced by SignaturePad) into a File so it can go through the deferred upload flow. */
export function dataUrlToPngFile(dataUrl: string, filename: string): File {
  if (!dataUrl.startsWith(PNG_DATA_URL_PREFIX)) {
    throw new Error("Expected a PNG data URL.");
  }

  const binary = atob(dataUrl.slice(PNG_DATA_URL_PREFIX.length));
  const bytes = new Uint8Array(binary.length);
  for (let index = 0; index < binary.length; index += 1) {
    bytes[index] = binary.charCodeAt(index);
  }

  return new File([bytes], filename, { type: "image/png" });
}
