/**
 * Helper utilities for Deferred File Upload in ERP forms.
 * Prevents orphan files by staging files locally and only uploading during onSubmit.
 */

export interface DeferredFile {
  id: string;
  file: File;
  previewUrl: string;
  fieldName?: string;
}

export interface FileValidationOptions {
  maxSizeBytes?: number;
  allowedTypes?: string[];
  maxSizeErrorMessage?: string | ((maxMb: number) => string);
  invalidTypeErrorMessage?: string;
}

/**
 * Validates a file against maximum size and allowed MIME types.
 */
export function validateDeferredFile(
  file: File,
  options: FileValidationOptions = {}
): { isValid: boolean; error?: string } {
  const { maxSizeBytes = 10 * 1024 * 1024, allowedTypes, maxSizeErrorMessage, invalidTypeErrorMessage } = options;

  if (file.size > maxSizeBytes) {
    const maxMb = Math.round(maxSizeBytes / (1024 * 1024));
    const error =
      typeof maxSizeErrorMessage === "function"
        ? maxSizeErrorMessage(maxMb)
        : maxSizeErrorMessage || `ขนาดไฟล์เกินกำหนด (สูงสุดไม่เกิน ${maxMb} MB)`;
    return {
      isValid: false,
      error,
    };
  }

  if (allowedTypes && allowedTypes.length > 0) {
    const isAllowed = allowedTypes.some((type) => {
      if (type.endsWith("/*")) {
        const prefix = type.replace("/*", "");
        return file.type.startsWith(prefix);
      }
      return file.type === type;
    });

    if (!isAllowed) {
      return {
        isValid: false,
        error: invalidTypeErrorMessage || "ประเภทไฟล์ไม่ถูกต้อง",
      };
    }
  }

  return { isValid: true };
}

/**
 * Creates a local object URL for previewing the file before upload.
 */
export function createDeferredFile(file: File, fieldName?: string): DeferredFile {
  const previewUrl = URL.createObjectURL(file);
  const id = `${Date.now()}-${Math.random().toString(36).substring(2, 9)}`;

  return {
    id,
    file,
    previewUrl,
    fieldName,
  };
}

/**
 * Revokes an object URL to prevent memory leaks.
 */
export function revokeDeferredFile(deferredFile: DeferredFile | null | undefined): void {
  if (deferredFile?.previewUrl) {
    try {
      URL.revokeObjectURL(deferredFile.previewUrl);
    } catch {
      // Ignore if already revoked
    }
  }
}

/**
 * Uploads all deferred files using the provided uploader function during onSubmit.
 * Returns a map of fieldName or file id to the uploaded storage URL.
 */
export async function uploadDeferredFiles(
  files: DeferredFile[],
  uploader: (file: File) => Promise<string>
): Promise<Record<string, string>> {
  const results: Record<string, string> = {};

  for (const item of files) {
    const uploadedUrl = await uploader(item.file);
    const key = item.fieldName || item.id;
    results[key] = uploadedUrl;
    // Clean up local object URL after successful upload
    revokeDeferredFile(item);
  }

  return results;
}
